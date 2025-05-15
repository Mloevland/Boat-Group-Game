using MA.Events;
using UnityEngine;

public class Scr_PlayerDeathTrigger : MonoBehaviour
{
    public VoidEvent deathEvent;
    public float timeTillDeath = 2f;

    private float deathTimer;

    private bool playerInTrigger;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInTrigger = true;
            deathTimer = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }

    private void Update()
    {
        if (!playerInTrigger)
            return;

        deathTimer += Time.deltaTime;

        if (deathTimer >= timeTillDeath)
        {
            deathEvent.Raise();
            playerInTrigger = false;
        }
    }
}
