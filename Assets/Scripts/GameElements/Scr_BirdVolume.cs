using UnityEngine;

public class Scr_BirdVolume : MonoBehaviour
{
    public Scr_Bird[] birdsInVolume;
    public bool canScareAwayBirds = true;
    public LayerMask groundLayer;

    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < birdsInVolume.Length; i++)
        {
            float delay = Vector3.Distance(other.transform.position, birdsInVolume[i].transform.position) * 0.1f - Random.Range(0.10f, 0.20f);
            birdsInVolume[i].FlyAway(Vector3.up + (transform.position - (other.transform.position-Vector3.up)) * 0.5f, delay);
        }
    }

    public void MoveVolume(Vector3 position)
    {
        float volumeRadius = GetComponent<SphereCollider>().radius;

        for (int i = 0; i < birdsInVolume.Length; i++)
        {

            Vector3 offset = new Vector3(Random.Range(-1f,1f),0, Random.Range(-1f, 1f));
            offset = Vector3.ClampMagnitude(offset, 1);
            offset *= Random.Range(1f,volumeRadius);

            RaycastHit hit;
            if(Physics.Raycast(position + offset + Vector3.up, Vector3.down,out hit, 3f, groundLayer))
            {
                Debug.DrawLine(position + offset + Vector3.up, hit.point, Color.green, 3f);
                birdsInVolume[i].MovePosition(hit.point);
            }
            else
            {
                Debug.DrawLine(position + offset + Vector3.up, position + offset + Vector3.up - Vector3.up * 3f, Color.red, 3f);
                //birdsInVolume[i].MovePosition(position + offset);
            }

        }
    }

    [ContextMenu("Fly Away All Birds")]
    public void TestFlyAway()
    {
        for (int i = 0; i < birdsInVolume.Length; i++)
        {
            birdsInVolume[i].FlyAway(Vector3.up, 0);
        }
    }

    [ContextMenu("Move Volume")]
    public void TestMoveVolume()
    {
        MoveVolume(transform.position);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {

        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = new Color(0.75f, 0.75f, 0.75f, 0.25f);
        Gizmos.DrawSphere(Vector3.zero, GetComponent<SphereCollider>().radius);
        Gizmos.color = new Color(0.1f, 0.1f, 0.1f, 0.6f);
        Gizmos.DrawWireSphere(Vector3.zero, GetComponent<SphereCollider>().radius);
    }
#endif
}
