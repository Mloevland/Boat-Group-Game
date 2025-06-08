using UnityEngine;
using UnityEngine.UIElements;

public class Scr_Bird : MonoBehaviour
{
    private bool shouldFly = false;
    private float timeBeforeVanish = 1f;
    public LayerMask avoidanceLayer;

    private Vector3 flyToPosition = Vector3.zero;
    private float flyTimer = 0f;

    private float wanderTimer;
    private Vector3 nextWanderSpot;

    
    public bool canWander = true;

    private void Start()
    {
        wanderTimer = Random.Range(1f, 3f);
        nextWanderSpot = transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
    }
    public void FlyAway(Vector3 direction, float delay)
    {
        Vector3 offset = new Vector3 (Random.Range(-2,2),0, Random.Range(-3, 3));
        flyToPosition = transform.position + direction * 8 + offset;
        flyTimer = -delay;
        shouldFly = true;
    }

    public void MovePosition(Vector3 position)
    {
        flyToPosition = position;
        flyTimer = Random.Range(-1.5f,0.5f);
        shouldFly = true;
    }

    public void Update()
    {
        if (shouldFly)
        {
            flyTimer += Time.deltaTime;
            if (flyTimer < 0f)
                return;

            Vector3 flyDirection = (flyToPosition - transform.position).normalized;

            if (Physics.Raycast(transform.position, flyDirection, 5f, avoidanceLayer))
            {
                flyToPosition += Vector3.up * 0.5f;
                flyDirection -= flyDirection * 0.4f;
            }

            transform.position = Vector3.MoveTowards(transform.position, flyToPosition, Time.deltaTime * 10);

            Quaternion desiredRotation = Quaternion.LookRotation(flyDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, Time.deltaTime * 25);

            if (Vector3.Distance(transform.position, flyToPosition) < 0.01f)
            {
                shouldFly = false;
                Destroy(gameObject);
            }
        }
        else
        {
            if (!canWander)
                return;

            wanderTimer -= Time.deltaTime;
            if(wanderTimer <= 0)
            {
                Vector3 wanderDirection = (nextWanderSpot - transform.position).normalized;

                transform.position = Vector3.MoveTowards(transform.position, nextWanderSpot, Time.deltaTime * 3);

                Quaternion desiredRotation = Quaternion.LookRotation(wanderDirection, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * 12);

                if (Vector3.Distance(transform.position, nextWanderSpot) < 0.01f)
                {
                    wanderTimer = Random.Range(2f, 7f);
                    nextWanderSpot = transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                }
            }
        }
    }
}
