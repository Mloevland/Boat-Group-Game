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

    void OnDrawGizmos()
    {


        // Convert the local coordinate values into world
        // coordinates for the matrix transformation.
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = new Color(0.75f, 0.3f, 0.0f, 0.25f);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
        Gizmos.color = new Color(0.75f, 0.1f, 0.0f, 0.6f);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
