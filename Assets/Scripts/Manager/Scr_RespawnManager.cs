using UnityEngine;
using MA.Events;

public class Scr_RespawnManager : MonoBehaviour
{
    public GameObjectEvent respawnPlayer;
    private GameObject respawnCheckpoint;

    public float respawnDelay = 1f;

    private bool shouldRespawn;
    private float respawnTimer;

    private void Update()
    {
        if(!shouldRespawn)
            return;

        respawnTimer += Time.deltaTime;
        if(respawnTimer >= respawnDelay)
        {
            shouldRespawn = false;
            respawnTimer = 0f;
            if(respawnCheckpoint != null)
            {
                respawnPlayer.Raise(respawnCheckpoint);
            }
        }
        
    }

    public void SetCheckpoint(GameObject gameObject)
    {
        respawnCheckpoint = gameObject;
    }

    public void RespawnPlayer()
    {
        shouldRespawn = true;
        //respawnPlayer.Raise(respawnCheckpoint);
    }
}
