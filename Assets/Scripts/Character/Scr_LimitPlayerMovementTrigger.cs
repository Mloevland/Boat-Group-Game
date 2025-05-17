
using UnityEngine;

public class Scr_LimitPlayerMovementTrigger : MonoBehaviour
{
    [Range(0,1)]
    public float strenght = 1;
    public float centeringForce = 10;
    //public Vector2Event limitMovementEvent;

    private Vector2 myLimit;

    [Range(-90,90)]
    public float rotateDirection = 0;

    private void Start()
    {
        Vector3 rightDir = Quaternion.AngleAxis(rotateDirection, Vector3.up) * transform.right;
        //Debug.Log(rightDir);
        rightDir.y = 0;
        rightDir.Normalize();
        //Debug.Log(rightDir);
        rightDir *= strenght;
        //Debug.Log(rightDir);

        myLimit = new Vector2(rightDir.x, rightDir.z);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            /*
            Vector3 rightDir = transform.right;
            Debug.Log(rightDir);
            rightDir.y = 0;
            rightDir.Normalize();
            Debug.Log(rightDir);
            rightDir *= strenght;
            Debug.Log(rightDir);
            */
            
            other.GetComponent<Scr_CharacterMovement>().AddMovementLimit(myLimit);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * transform.InverseTransformPoint(other.transform.position).z);
        if(other.CompareTag("Player"))
        {
            other.GetComponent<Rigidbody>().AddForce(-transform.forward * transform.InverseTransformPoint(other.transform.position).z * centeringForce, ForceMode.Acceleration);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<Scr_CharacterMovement>().RemoveMovementLimit(myLimit);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {

        Vector3 rightDir = Quaternion.AngleAxis(rotateDirection, Vector3.up) * transform.right;


        Gizmos.color = new Color(0.4f - strenght * 0.4f, 0.75f, 0f + strenght * 0.25f, 1f);
        Gizmos.DrawLine(transform.position - rightDir * 0.3f, transform.position + rightDir * 0.3f);
        Gizmos.DrawLine(transform.position - rightDir * 0.3f, transform.position - rightDir * 0.15f + transform.up * 0.15f);
        Gizmos.DrawLine(transform.position - rightDir * 0.3f, transform.position - rightDir * 0.15f - transform.up * 0.15f);
        Gizmos.DrawLine(transform.position + rightDir * 0.3f, transform.position + rightDir * 0.15f + transform.up * 0.15f);
        Gizmos.DrawLine(transform.position + rightDir * 0.3f, transform.position + rightDir * 0.15f - transform.up * 0.15f);

        // Convert the local coordinate values into world
        // coordinates for the matrix transformation.
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = new Color(0.4f - strenght * 0.4f, 0.75f, 0f + strenght * 0.25f, 0.25f);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
        Gizmos.color = new Color(0.4f - strenght * 0.4f, 0.75f, 0f + strenght * 0.35f, 0.6f);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        


    }
#endif
}
