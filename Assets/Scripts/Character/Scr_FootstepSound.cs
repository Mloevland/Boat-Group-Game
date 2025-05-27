using UnityEngine;

public class Scr_FootstepSound : MonoBehaviour
{
    public Transform[] footstepPositions;

    public AudioSource footstepPlayer;

    public FootstepSource[] footstepSources;

    public LayerMask groundLayer;

    public void PlayFootstep(int foot)
    {
        

        if (Physics.Raycast(footstepPositions[foot].position,Vector3.down, out RaycastHit hit, 1f, groundLayer))
        {
            bool hitKnownMaterial = false;
            for (int i = 0; i < footstepSources.Length; i++)
            {
                if (hit.collider.gameObject.CompareTag(footstepSources[i].material))
                {
                    Debug.Log("Found matching material for footstep: " + hit.collider.gameObject.tag);
                    AudioClip clip = footstepSources[i].footstepSounds[Random.Range(0, footstepSources[i].footstepSounds.Length)];
                    footstepPlayer.PlayOneShot(clip);
                    hitKnownMaterial = true;
                    break;
                }
                Debug.Log("Didn't find matching material for footstep: " + hit.collider.gameObject.tag);
            }
            if (!hitKnownMaterial)
            {
                AudioClip clip = footstepSources[0].footstepSounds[Random.Range(0, footstepSources[0].footstepSounds.Length)];
                footstepPlayer.PlayOneShot(clip);
            }

        }
        else
        {
            Debug.LogWarning("No ground detected for footstep at position: " + footstepPositions[foot].position);
        }
    }
}

[System.Serializable]
public class FootstepSource
{
    public string material;
    public AudioClip[] footstepSounds;
}
