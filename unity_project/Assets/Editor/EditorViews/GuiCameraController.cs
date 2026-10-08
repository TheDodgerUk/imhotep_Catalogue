using System.Collections.Generic;
using UnityEngine;

namespace EditorTools.Views
{
    [System.Serializable]
    public class GuiCameraController
    {
        private const float OrbitSpeed = 0.5f;
        private const float ZoomSpeed = 0.025f;
        private const float PanSpeed = 0.0025f;

        [SerializeField]
        private Transform m_Transform;
        [SerializeField]
        private Camera m_Camera;

        private float m_Dolly = 0;

        public void Create(Camera camera)
        {
            m_Transform = camera.transform;
            m_Camera = camera;

            m_Transform.position = new Vector3(-1, 1, 1);
            m_Transform.LookAt(Vector3.zero, Vector3.up);
            m_Dolly = m_Transform.position.magnitude;
        }

        public void FocusCamera(List<GameObject> objects) => FrameObjects(objects);

        public void OnGUI(Rect position, List<GameObject> objects)
        {
            DrawCameraControls(position);

            var e = Event.current;
            var currentPos = e.mousePosition;
            switch (e.type)
            {
                case EventType.MouseDrag:
                    OnMouseDrag(e);
                    break;
                case EventType.KeyDown:
                    break;
                case EventType.KeyUp:
                    break;
                case EventType.ScrollWheel:
                    OnMouseWheel(e);
                    break;
                case EventType.ValidateCommand:
                    if (e.commandName == "FrameSelected")
                    {
                        FrameObjects(objects);
                    }
                    break;
                default: break;
            }
        }

        private void DrawCameraControls(Rect position)
        {
            if (GUI.Button(GetBtnRect(position, -1, 0), "↓") == true)
            {
                AlignToDirection(-m_Transform.forward, m_Transform.right, Vector3.up, Vector3.forward);
            }
            if (GUI.Button(GetBtnRect(position, -1, 2), "↑") == true)
            {
                AlignToDirection(-m_Transform.forward, -m_Transform.right, Vector3.up, Vector3.forward);
            }
            if (GUI.Button(GetBtnRect(position, -2, 1), "→") == true)
            {
                var direction = -m_Transform.forward.With(y: 0).normalized;
                AlignToDirection(direction, Vector3.up, Vector3.right, Vector3.forward);
            }
            if (GUI.Button(GetBtnRect(position, 0, 1), "←") == true)
            {
                var direction = -m_Transform.forward.With(y: 0).normalized;
                AlignToDirection(direction, -Vector3.up, Vector3.right, Vector3.forward);
            }

            bool isOrthographic = m_Camera.orthographic;
            if (GUI.Button(GetBtnRect(position, -1, 1), isOrthographic ? "O" : "P") == true)
            {
                m_Camera.orthographic = !isOrthographic;
            }
        }

        private void AlignToDirection(Vector3 current, Vector3 normal, params Vector3[] directions)
        {
            if (current.sqrMagnitude == 0)
            {
                current = Vector3.forward;
            }
            var pivot = GetPivot();
            var target = GetClosest(current, normal, directions);
            m_Transform.position = pivot + target * m_Dolly;
            m_Transform.LookAt(pivot, Vector3.up);
        }

        private Vector3 GetClosest(Vector3 current, Vector3 normal, params Vector3[] directions)
        {
            var pivot = GetPivot();
            var targetDirections = new List<Vector3>();
            foreach (var direction in directions)
            {
                targetDirections.Add(direction);
                targetDirections.Add(-direction);
            }

            var targetDirection = Vector3.zero;
            float lowestPositiveAngle = float.MaxValue;
            foreach (var direction in targetDirections)
            {
                float angle = current.AngleSigned(direction, normal);
                if (angle > 0.001 && angle < lowestPositiveAngle)
                {
                    lowestPositiveAngle = angle;
                    targetDirection = direction;
                }
                Debug.DrawLine(pivot, pivot + direction, Color.red);
            }
            Debug.DrawLine(pivot, pivot + targetDirection, Color.cyan);
            Debug.DrawLine(pivot, pivot + current, Color.yellow);

            UnityEditor.SceneView.RepaintAll();
            return targetDirection;
        }

        private Rect GetBtnRect(Rect r, int x, int y)
        {
            const float s = 32;
            return new Rect(r.xMax + s * (x - 1), r.yMin + s * y, s, s);
        }

        private void OnMouseDrag(Event e)
        {
            var delta = e.delta;
            if (e.alt == true)
            {
                if (e.button == 0)
                {
                    RotateAroundPivot(delta);
                }
                if (e.button == 1)
                {
                    Zoom(delta);
                }
            }
            if (e.button == 2)
            {
                float speed = PanSpeed * m_Dolly;
                m_Transform.Translate(-delta.x * speed, 0, 0);
                m_Transform.Translate(0, delta.y * speed, 0);
            }
        }

        private void OnMouseWheel(Event e)
        {
            Zoom(-e.delta * 2);
        }

        private void RotateAroundPivot(Vector2 delta)
        {
            var pivot = GetPivot();
            m_Transform.RotateAround(pivot, m_Transform.right, delta.y * OrbitSpeed);
            m_Transform.RotateAround(pivot, Vector3.up, delta.x * OrbitSpeed);
            m_Transform.LookAt(pivot, m_Transform.up);
        }

        private void Zoom(Vector2 delta)
        {
            var pivot = GetPivot();
            float speed = m_Dolly * ZoomSpeed;
            float zoom = (delta.x + delta.y).Clamp(-1, 1);
            m_Dolly = (m_Dolly - (zoom * speed)).Max(0.01f);
            m_Transform.position = pivot - m_Transform.forward * m_Dolly;
            m_Camera.orthographicSize = m_Dolly * 0.5f;
        }

        private void FrameObjects(List<GameObject> objects)
        {
            var bounds = new Bounds();
            foreach (var obj in objects)
            {
                foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            Vector3 max = bounds.size;

            // get the radius of a sphere circumscribing the bounds
            float radius = (max.magnitude * 0.5f).Max(0.1f);

            // get the horizontal FOV, since it may be the limiting of the two FOVs to properly encapsulate the objects
            float horizontalFOV = 2f * Mathf.Atan(Mathf.Tan(m_Camera.fieldOfView * Mathf.Deg2Rad * 0.5f) * m_Camera.aspect) * Mathf.Rad2Deg;

            // use the smaller FOV as it limits what would get cut off by the frustum        
            float fov = Mathf.Min(m_Camera.fieldOfView, horizontalFOV);
            float distance = radius / (Mathf.Sin(fov * Mathf.Deg2Rad * 0.5f));

            m_Dolly = distance;
            m_Transform.position = bounds.center - m_Transform.forward * m_Dolly;
            m_Transform.LookAt(bounds.center);
            m_Camera.orthographicSize = radius;
        }

        private Vector3 GetPivot() => m_Transform.position + m_Transform.forward * m_Dolly;
    }
}
