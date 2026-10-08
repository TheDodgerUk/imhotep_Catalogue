using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EnvironmentHelpers
{
    public class PathFollowers : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> m_PathModels = new List<GameObject>();
        [SerializeField]
        private List<Paths> m_Paths = new List<Paths>();

        // Start is called before the first frame update
        void Awake()
        {
            Transform sourceModels = transform.Search("SourceModels");
            for (int i = 0; i < sourceModels.childCount; i++)
            {
                var model = sourceModels.GetChild(i).gameObject;
                model.SetActive(false);
                m_PathModels.Add(model);
            }

            Transform pathRoot = transform.Search("Paths");

            for(int i = 0; i < pathRoot.childCount; i++)
            {
                Transform child = pathRoot.GetChild(i).transform;
                var pathScript = child.AddComponent<Paths>();
                pathScript.Init(m_PathModels);
                m_Paths.Add(pathScript);
            }
        }

        private void Update()
        {
            for(int i = 0; i < m_Paths.Count; i++)
            {
                m_Paths[i].ManualUpdate();
            }
        }
    }
}

namespace EnvironmentHelpers
{
    public class Paths : MonoBehaviour
    {
        [SerializeField]
        private List<Transform> Waypoints = new List<Transform>();
        [SerializeField]
        private List<PathAgent> Agents = new List<PathAgent>();
        [SerializeField]
        private List<GameObject> m_AgentModels = new List<GameObject>();

        [SerializeField]
        private float PathDistance = 0f;

        [SerializeField]
        private float DistanceBetween = 5f;

        [SerializeField]
        private float AgentSpeed = 1.0f;

        [SerializeField]
        private bool Looped = false;

        [SerializeField]
        private bool Random = false;

        [InspectorButton]
        public void Rebuild()
        {
            Build();
        }

        public void ManualUpdate()
        {
            for(int i =0; i < Agents.Count; i++)
            {
                Agents[i].ManualUpdate();
            }
        }

        private void OnAgentReachedWaypoint(PathAgent agent, int iIndex, float overshoot)
        {
            iIndex++;
            int nextIndex = iIndex + 1;
            if (false == Looped)
            {
                iIndex = iIndex.Wrap(Waypoints.Count-1);
                nextIndex = iIndex + 1;
            }
            else
            {
                iIndex = iIndex.Wrap(Waypoints.Count);
                nextIndex = nextIndex.Wrap(Waypoints.Count);
            }

            float distance = Vector3.Distance(Waypoints[iIndex].position, Waypoints[nextIndex].position);
            float normalisedDistance = Mathf.InverseLerp(0f, distance, overshoot);
            agent.SetPosition(iIndex, Waypoints[iIndex], Waypoints[nextIndex], normalisedDistance);
        }

        public void Init(List<GameObject> models)
        {
            m_AgentModels = models;
            Waypoints = transform.GetDirectChildren();

            Build();
        }

        private void ReadData()
        {
            Looped = false;
            Random = false;

            string[] parts = gameObject.name.Split('|');
            for(int i = 0; i < parts.Length; i++)
            {
                //Split by whitespace
                string[] operands = System.Text.RegularExpressions.Regex.Split(parts[i], @"\s+");

                switch(operands[0])
                {
                    case "Speed":
                        AgentSpeed = float.Parse(operands[1]);
                        break;
                    case "Looped":
                        Looped = true;
                        break;
                    case "Distance":
                        DistanceBetween = float.Parse(operands[1]);
                        break;
                    case "Random":
                        Random = true;
                        break;
                }
            }   
        }

        private void Build()
        {
            ReadData();
            PathDistance = CalculatePathLength();

            int agentCount = (int)(PathDistance / DistanceBetween);

            GenerateAgents(agentCount);

            for (int i = 0; i < Agents.Count; i++)
            {
                float offset = 0;
                if(true == Random)
                {
                    offset = Mathf.Max(2f, UnityEngine.Random.Range(-DistanceBetween, DistanceBetween) * 0.4f);
                }

                PlaceAgentAtDistance(Agents[i], (DistanceBetween * i) + offset);
            }
        }

        private void GenerateAgents(int count)
        {
            Agents.ForEach((e) => e.gameObject.DestroyObject());
            Agents.Clear();

            for (int i = 0; i < count; i++)
            {
                var agent = GameObject.Instantiate(m_AgentModels.GetRandom());
                var agentScript = agent.AddComponent<PathAgent>();
                agentScript.Init(AgentSpeed);
                agentScript.OnWaypointCompleted += OnAgentReachedWaypoint;
                agent.SetActive(true);
                Agents.Add(agentScript);
            }
        }

        private void PlaceAgentAtDistance(PathAgent agent, float distance)
        {
            float totalDistance = 0f;
            for (int i = 0; i < Waypoints.Count; i++)
            {
                int iIndex = i + 1;
                if(true == Looped)
                    iIndex = iIndex.Wrap(Waypoints.Count);

                float dist = Vector3.Distance(Waypoints[i].position, Waypoints[iIndex].position);

                //Will we be greater than the distance required
                if ((totalDistance + dist) > distance)
                {
                    float normalisedDistance = Mathf.InverseLerp(totalDistance, totalDistance + dist, distance);
                    //We are between these two waypoints
                    agent.SetPosition(i, Waypoints[i], Waypoints[iIndex], normalisedDistance);
                    return;
                }

                totalDistance += dist;
            }
        }

        private float CalculatePathLength()
        {
            float totalDistance = 0f;
            for (int i = 0; i < Waypoints.Count - 1; i++)
            {
                float distance = Vector3.Distance(Waypoints[i].position, Waypoints[i + 1].position);
                totalDistance += distance;
            }

            if(true == Looped)
                totalDistance += Vector3.Distance(Waypoints[Waypoints.Count-1].position, Waypoints[0].position);

            return totalDistance;
        }

        private void OnDrawGizmos()
        {
            if (null != Waypoints)
            {
                for (int i = 0; i < Waypoints.Count; i++)
                {
                    Gizmos.DrawWireSphere(Waypoints[i].position, 0.05f);
                }
            }
        }
    }    
}

namespace EnvironmentHelpers
{
    public class PathAgent : MonoBehaviour
    {
        public Action<PathAgent, int, float> OnWaypointCompleted;
        public int CurrentIndex;
        public Transform LastWaypoint;
        public Transform CurrentWaypoint;

        Quaternion currentRotation;
        Quaternion TargetRotation;

        private float m_fSpeed = 1f;

        private float distanceBetweenPoints = 0f;
        private float totalDistanceBetweenPoints = 0f;

        public void Init(float speed)
        {
            m_fSpeed = speed;
        }

        public void SetPosition(int iIndex, Transform last, Transform next, float normalisedPosition)
        {            
            totalDistanceBetweenPoints = Vector3.Distance(last.position, next.position);
            distanceBetweenPoints = totalDistanceBetweenPoints * normalisedPosition;
            transform.position = Vector3.Lerp(last.position, next.position, normalisedPosition);

            if (null == CurrentWaypoint)
            {
                transform.LookAt(next);
            }

            CurrentIndex = iIndex;
            LastWaypoint = last;
            CurrentWaypoint = next;

            currentRotation = transform.rotation;
            TargetRotation = Quaternion.LookRotation(next.position - last.position);
        }

        public void ManualUpdate()
        {
            distanceBetweenPoints += m_fSpeed * Time.deltaTime;

            if (distanceBetweenPoints > totalDistanceBetweenPoints)
            {
                //Get next waypoint factoring in overshoot of this waypoint
                OnWaypointCompleted?.Invoke(this, CurrentIndex, distanceBetweenPoints - totalDistanceBetweenPoints);
            }
            else
            {
                //calculate normalised position between points
                float normalisedMovement = Mathf.InverseLerp(0f, totalDistanceBetweenPoints, distanceBetweenPoints);
                transform.position = Vector3.Lerp(LastWaypoint.position, CurrentWaypoint.position, normalisedMovement);
                transform.rotation = Quaternion.Lerp(currentRotation, TargetRotation, normalisedMovement);
            }
        }
    }
}