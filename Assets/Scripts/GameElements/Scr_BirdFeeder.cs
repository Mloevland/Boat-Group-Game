using UnityEngine;
using System.Collections.Generic;

public class Scr_BirdFeeder : MonoBehaviour
{
    public float detectionRadius = 20f;
    public LayerMask groundLayer;
    public LayerMask birdVolumeLayer;
    public bool hasSpilled = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasSpilled && IsGround(collision))
        {
            hasSpilled = true;
            Vector3 foodPosition = transform.position;

            AttractBirdsTo(foodPosition);
        }
    }

    private bool IsGround(Collision collision)
    {
        return ((1 << collision.gameObject.layer) & groundLayer) != 0;
    }

    private void AttractBirdsTo(Vector3 position)
    {
        Collider[] volumes = Physics.OverlapSphere(position, detectionRadius, birdVolumeLayer);

        foreach (var volume in volumes)
        {
            Scr_BirdVolume birdVolume = volume.GetComponent<Scr_BirdVolume>();
            if (birdVolume != null)
            {
                foreach (var bird in birdVolume.birdsInVolume)
                {
                    if (bird != null)
                    {
                        bird.FlyToFood(position + Random.insideUnitSphere * 1.5f);
                    }
                }
            }
        }
    }
}
