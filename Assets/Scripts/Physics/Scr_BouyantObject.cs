using UnityEngine;


[RequireComponent (typeof(Rigidbody))]
public class Scr_BouyantObject : MonoBehaviour
{
    public float underWaterDrag = 5f;
    public float underWaterAngularDrag = 2f;
    private float airDrag = 1;
    private float airAngularDrag = 0.05f;

    public float buoyancy = 15f;

    //public float waterHeight = 0f;

    Rigidbody rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        airDrag = rb.linearDamping;
        airAngularDrag = rb.angularDamping;
    }
    public void CalculateBouyancy(float waterHeight)
    {
        float difference = transform.position.y - (transform.lossyScale.y*0.5f) - waterHeight;

        if(difference < 0 )
        {
            rb.AddForceAtPosition(Vector3.up * (buoyancy*buoyancy) * Mathf.Abs(difference), transform.position, ForceMode.Force);
        }
    }

    public void IsInWater(bool inWater)
    {
        if (inWater)
        {
            rb.linearDamping = underWaterDrag;
            rb.angularDamping = underWaterAngularDrag;
        }
        else
        {
            rb.linearDamping = airDrag;
            rb.angularDamping = airAngularDrag;
        }
    }
}
