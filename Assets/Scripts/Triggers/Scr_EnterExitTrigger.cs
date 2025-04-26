using UnityEngine;
using UnityEngine.Events;

public class Scr_EnterExitTrigger : MonoBehaviour
{
    public string triggerTag = "Player";
    public UnityEvent onEnter;
    public UnityEvent onExit;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            onEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            onExit.Invoke();
        }
    }
}
