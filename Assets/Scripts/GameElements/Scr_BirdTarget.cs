using UnityEngine;

public class Scr_BirdTarget : MonoBehaviour
{
    public bool ignorePlayer = true;
    private Scr_Bird bird;

    private void Awake()
    {
        bird = GetComponent<Scr_Bird>();
    }

    public void FlyToSeed(Vector3 seedPos)
    {
        if (bird != null)
        {
            // Keep target slightly above seed to avoid destruction
            Vector3 safeTarget = seedPos + Vector3.up * 0.15f;

            bird.FlyToFood(safeTarget);
            bird.canWander = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ignorePlayer)
        {
            // Override flee logic
        }
    }
}
