using UnityEngine;

public class Scr_SeedBirdTarget : MonoBehaviour
{
    public bool ignorePlayer = true;
    public LayerMask avoidanceLayer;

    private bool shouldFly = false;
    private Vector3 flyToPosition;
    private float flyTimer = 0f;

    public bool canWander = false;
    private float wanderTimer;
    private Vector3 nextWanderSpot;

    private bool isBeingControlledExternally = false;

    private void Start()
    {
        if (canWander)
        {
            wanderTimer = Random.Range(1f, 3f);
            nextWanderSpot = transform.position + GetSmallWanderOffset();
        }
    }

    public void FlyToSeed(Vector3 seedPos)
    {
        if (isBeingControlledExternally)
            return;

        flyToPosition = seedPos;
        flyTimer = Random.Range(-0.3f, 0.1f);
        shouldFly = true;
        canWander = false;
    }

    public void SetLandingLock()
    {
        isBeingControlledExternally = true;
        shouldFly = false;
    }

    private void Update()
    {
        if (isBeingControlledExternally)
            return;

        if (shouldFly)
        {
            flyTimer += Time.deltaTime;
            if (flyTimer < 0f)
                return;

            Vector3 direction = (flyToPosition - transform.position).normalized;
            Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z);

            if (Physics.Raycast(transform.position, flatDirection, 5f, avoidanceLayer))
            {
                flyToPosition += Vector3.up * 0.5f;
                flatDirection -= flatDirection * 0.4f;
            }

            transform.position = Vector3.MoveTowards(transform.position, flyToPosition, Time.deltaTime * 10f);

            RotateTowards(flatDirection);

            if (Vector3.Distance(transform.position, flyToPosition) < 0.05f)
            {
                shouldFly = false;
            }
        }
        else if (canWander)
        {
            wanderTimer -= Time.deltaTime;
            if (wanderTimer <= 0f)
            {
                Vector3 wanderDir = (nextWanderSpot - transform.position).normalized;
                Vector3 flatWanderDir = new Vector3(wanderDir.x, 0f, wanderDir.z);

                transform.position = Vector3.MoveTowards(transform.position, nextWanderSpot, Time.deltaTime * 3f);

                RotateTowards(flatWanderDir);

                if (Vector3.Distance(transform.position, nextWanderSpot) < 0.01f)
                {
                    wanderTimer = Random.Range(2f, 5f);
                    nextWanderSpot = transform.position + GetSmallWanderOffset();
                }
            }
        }
    }
    private void RotateTowards(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.001f)
            return;

        // Get angle from +X in the XZ plane
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

        // Lock X to -90, Z to 0, rotate only around Y
        transform.rotation = Quaternion.Euler(-90f, -angle, 0f);
    }




    private Vector3 GetSmallWanderOffset()
    {
        float radius = 0.3f;
        return new Vector3(Random.Range(-radius, radius), 0f, Random.Range(-radius, radius));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ignorePlayer)
        {
            // Do nothing (prevent flee)
        }
    }
}
