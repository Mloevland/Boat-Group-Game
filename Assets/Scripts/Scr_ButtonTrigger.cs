using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class Scr_ButtonTrigger : MonoBehaviour
{
    [Header("Animator Targets")]
    public Animator animatorA;
    public Animator animatorB;

    [Header("Trigger Names")]
    public string triggerA = "StartWatering";
    public string triggerB = "StartNozzle";

    private Rigidbody rb;

    private void Awake()
    {
        // Lock down Rigidbody if present
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    private void Update()
    {
        // Gamepad input
        if (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame)
        {
            Debug.Log("Gamepad Square (Button West) pressed.");
            TriggerAnimations();
        }

        // Keyboard input
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("Keyboard E key pressed.");
            TriggerAnimations();
        }
    }

    private void TriggerAnimations()
    {
        if (animatorA != null)
        {
            animatorA.SetTrigger(triggerA);
        }

        if (animatorB != null)
        {
            animatorB.SetTrigger(triggerB);
        }

        Debug.Log("Button triggered! Animators activated.");
    }
}
