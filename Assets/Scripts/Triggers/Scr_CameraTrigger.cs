using MA.Events;
using UnityEngine;

public class Scr_CameraTrigger : MonoBehaviour
{
    public string triggerTag = "Player"; // Tag to check for
    public GameObject myCamera;
    public GameObjectEvent cameraEvent;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            cameraEvent.Raise(myCamera);
            Debug.Log("Camera Triggered");
        }
    }
}
