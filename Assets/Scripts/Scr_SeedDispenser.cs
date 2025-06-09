using UnityEngine;
using System.Collections;

public class Scr_SeedDispenser : MonoBehaviour
{
    [Header("Seeds")]
    public GameObject seedPrefab;
    public Transform dropPoint;
    public Transform groundTarget;
    public int seedCount = 10;
    public float spreadForce = 2f;
    public float dropInterval = 0.05f;

    [Header("Bird Arrival")]
    public float birdArrivalDelay = 0.08f;
    public float arcHeight = 2.5f;
    public float landingSpreadRadius = 1.0f;
    public float birdFlightSpeed = 5f;

    [Header("Optional")]
    public Collider cubeColliderToDisable;

    public void DropSeeds()
    {
        // Disable cube collider when called
        if (cubeColliderToDisable != null)
        {
            cubeColliderToDisable.enabled = false;
            Debug.Log("Cube collider disabled!");
        }

        StartCoroutine(StreamSeedsThenCallBirds());
    }

    private IEnumerator StreamSeedsThenCallBirds()
    {
        // Drop seeds one at a time
        for (int i = 0; i < seedCount; i++)
        {
            Vector3 offset = Random.insideUnitSphere * 0.05f;
            GameObject seed = Instantiate(seedPrefab, dropPoint.position + offset, Quaternion.identity);

            if (seed.TryGetComponent(out Rigidbody rb))
            {
                Vector3 force = new Vector3(
                    Random.Range(-1f, 1f),
                    1f,
                    Random.Range(-1f, 1f)
                ).normalized;

                rb.AddForce(force * spreadForce, ForceMode.Impulse);
            }

            yield return new WaitForSeconds(dropInterval);
        }

        yield return new WaitForSeconds(0.4f);

        // Send birds
        Scr_SeedBirdTarget[] allFeeders = FindObjectsOfType<Scr_SeedBirdTarget>();
        foreach (var feeder in allFeeders)
        {
            Vector2 circle = Random.insideUnitCircle * landingSpreadRadius;
            Vector3 end = groundTarget.position + new Vector3(circle.x, 0f, circle.y);
            Vector3 start = feeder.transform.position;

            StartCoroutine(FlyInArc(feeder, start, end, arcHeight, birdFlightSpeed));

            yield return new WaitForSeconds(birdArrivalDelay);
        }
    }

    private IEnumerator FlyInArc(Scr_SeedBirdTarget bird, Vector3 start, Vector3 end, float height, float speed)
    {
        float distance = Vector3.Distance(start, end);
        float duration = distance / speed;
        float time = 0f;

        bird.SetLandingLock();

        while (time < duration)
        {
            float t = time / duration;

            Vector3 currentPos = Vector3.Lerp(start, end, t);
            currentPos.y += height * Mathf.Sin(Mathf.PI * t);

            Vector3 previousPos = bird.transform.position;
            bird.transform.position = currentPos;

            Vector3 dir = currentPos - previousPos;
            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion lookRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
                bird.transform.rotation = Quaternion.Lerp(bird.transform.rotation, lookRot, Time.deltaTime * 10f);
            }

            time += Time.deltaTime;
            yield return null;
        }

        bird.transform.position = end;
    }
}
