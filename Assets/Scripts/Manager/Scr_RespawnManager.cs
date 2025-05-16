using UnityEngine;
using MA.Events;

public class Scr_RespawnManager : MonoBehaviour
{
    public GameObjectEvent respawnPlayer;
    private GameObject respawnCheckpoint;

    public void SetCheckpoint(GameObject gameObject)
    {
        respawnCheckpoint = gameObject;
    }

    public void RespawnPlayer()
    {
        respawnPlayer.Raise(respawnCheckpoint);
    }
}
