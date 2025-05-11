using MA.Events;
using UnityEngine;
using UnityEngine.Events;

public class Scr_LeverInteractable : MonoBehaviour
{
    public HingeJoint hingeJoint;
    private bool isLeverActive = false;
    private Vector2 angleLimits;
    public float angleLeniency = 5f;

    public UnityEvent onLeverActivated;
    public UnityEvent onLeverDeactivated;

    private void Start()
    {
        angleLimits = new Vector2(hingeJoint.limits.min, hingeJoint.limits.max);
    }

    private void FixedUpdate()
    {
        if (isLeverActive)
        {
            if (hingeJoint.angle <= angleLimits.x + angleLeniency)
            {
                isLeverActive = false;
                onLeverDeactivated.Invoke();
            }
        }
        else
        {
            if (hingeJoint.angle >= angleLimits.y - angleLeniency)
            {
                isLeverActive=true;
                onLeverActivated.Invoke();
            }
            
        }

    }
}
