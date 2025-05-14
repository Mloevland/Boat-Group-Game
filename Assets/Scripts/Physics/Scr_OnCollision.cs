using UnityEngine;
using UnityEngine.Events;

public class Scr_OnCollision : MonoBehaviour
{
    public string collisionWithTag;
    public UnityEvent onCollisionEvent;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(collisionWithTag))
        {
            onCollisionEvent.Invoke();
        }
    }
}
