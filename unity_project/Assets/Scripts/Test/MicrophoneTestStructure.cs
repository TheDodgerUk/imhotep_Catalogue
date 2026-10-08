using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneTestStructure : MonoBehaviour
{
    public Vector3 startPosition;
    public Vector3 endPosition;

    public Vector3 direction = Vector3.up;
    public Transform m_MovingPart = null;
    public float m_fMinHeight = 0.8f;
    public float m_fMaxHeight = 1.35f;

    public Transform m_Target;
    public Transform m_Pivot;
    public Transform m_Hinge;
    public Transform m_ClipPoint;

    [SerializeField]
    private Vector3 RootToPivot;
	// Use this for initialization
	void Start ()
    {
        RootToPivot = m_Pivot.position - transform.position;
    }
	
	// Update is called once per frame
	void Update ()
    {
        startPosition = m_Target.position;
        endPosition.x = startPosition.x;
        endPosition.z = startPosition.z;
        //Set root position
        transform.position = endPosition;
        

        Vector3 verticalOffset = Vector3.zero;
        float dist = (startPosition.y - endPosition.y) - m_fMinHeight;
        dist = Mathf.Clamp(dist, 0f, m_fMaxHeight - m_fMinHeight);
        verticalOffset = direction * dist;

        //Calculate angle difference
        m_Pivot.localPosition = RootToPivot + verticalOffset;

        //Sort pivot
        Vector3 pivotForward = m_Target.forward;
        pivotForward.y = 0f;
        Quaternion pivotRotation = Quaternion.LookRotation(pivotForward, transform.up);
        Debug.DrawRay(m_Pivot.position, transform.up, Color.red);
        Debug.DrawRay(m_Pivot.position, m_Target.forward, Color.yellow);
        m_Pivot.localRotation = pivotRotation;

        //Sort Angle
        Vector3 refAngle = Vector3.zero;
        refAngle.y = m_Pivot.rotation.eulerAngles.y;
        float angle = Vector3.SignedAngle(m_Pivot.forward, m_Target.forward, m_Pivot.right);
        m_Hinge.localRotation = Quaternion.AngleAxis(angle, Vector3.right);
        
        //Calculate offset in position relative to clip
        Vector3 pivotToClipOffset = m_ClipPoint.position - m_Pivot.position;
        
        //extract horizontal movement only to apply to base position
        Vector3 horizontalMovement = Vector3.zero;
        horizontalMovement.x = pivotToClipOffset.x;
        horizontalMovement.z = pivotToClipOffset.z;

        //extract height adjustment
        pivotToClipOffset.x = 0f;
        pivotToClipOffset.z = 0f;
        float finalHeight = transform.InverseTransformPoint(m_Target.position).y;
        finalHeight = Mathf.Clamp(finalHeight, RootToPivot.y, RootToPivot.y + (m_fMaxHeight - m_fMinHeight));
        Vector3 verticalMovement = Vector3.up * finalHeight;
        m_Pivot.localPosition = verticalMovement - pivotToClipOffset;
        transform.position = endPosition - horizontalMovement;

        if (null != m_MovingPart)
        {
            m_MovingPart.localPosition = m_Pivot.localPosition - RootToPivot;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
      //  Gizmos.DrawWireSphere(startPosition, 0.05f);
        Gizmos.color = Color.green;
       // Gizmos.DrawWireSphere(endPosition, 0.05f);
        Gizmos.DrawRay(m_Target.position, m_Target.forward);
    }
}
