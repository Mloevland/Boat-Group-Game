using UnityEngine;

public class Scr_DropBoat : MonoBehaviour
{
    public GameObject boatObject;
    private FixedJoint jointToBreak;

    void Start()
    {
        jointToBreak = boatObject.GetComponent<FixedJoint>();
    }

    public void DropBoat()
    {
        if (jointToBreak != null)
        {
            Destroy(jointToBreak);
        }

        Rigidbody rb = boatObject.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
    }
}
