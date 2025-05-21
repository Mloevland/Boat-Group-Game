using MA.Events;
using UnityEngine;

public class Scr_LimitPlayerMovementTrigger : MonoBehaviour
{

    public Vector2Event limitMovementEvent;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Vector3 rightDir = transform.right;
            rightDir.y = 0;
            rightDir.Normalize();
            limitMovementEvent.Raise(new Vector2(rightDir.x, rightDir.z));
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * transform.InverseTransformPoint(other.transform.position).z);
        if(other.CompareTag("Player"))
        {
            other.GetComponent<Rigidbody>().AddForce(-transform.forward * transform.InverseTransformPoint(other.transform.position).z * 10, ForceMode.Acceleration);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            limitMovementEvent.Raise(Vector2.one);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.0f, 0.75f, 0.2f, 1f);
        Gizmos.DrawLine(transform.position - transform.right * 0.3f, transform.position + transform.right * 0.3f);
        Gizmos.DrawLine(transform.position - transform.right * 0.3f, transform.position - transform.right * 0.15f + transform.up * 0.15f);
        Gizmos.DrawLine(transform.position - transform.right * 0.3f, transform.position - transform.right * 0.15f - transform.up * 0.15f);
        Gizmos.DrawLine(transform.position + transform.right * 0.3f, transform.position + transform.right * 0.15f + transform.up * 0.15f);
        Gizmos.DrawLine(transform.position + transform.right * 0.3f, transform.position + transform.right * 0.15f - transform.up * 0.15f);

        // Convert the local coordinate values into world
        // coordinates for the matrix transformation.
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = new Color(0.0f, 0.75f, 0.2f, 0.25f);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
        Gizmos.color = new Color(0.0f, 0.75f, 0.3f, 0.6f);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        


    }
#endif
}
