using UnityEngine;

public class Scr_PlayerCheckpoint : MonoBehaviour
{
    [HideInInspector]
    public Mesh playerModel;

#if UNITY_EDITOR
    void OnDrawGizmos()
    {

        // Convert the local coordinate values into world
        // coordinates for the matrix transformation.
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = new Color(0.0f, 0.2f, 0.75f, 0.25f);
        Gizmos.DrawCube(Vector3.up * 0.5f, Vector3.one);
        Gizmos.color = new Color(0.0f, 0.3f, 0.75f, 0.6f);
        Gizmos.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
        Gizmos.color = new Color(1f, 1f, 1f, 0.6f);
        Gizmos.DrawMesh(playerModel, 0, Vector3.up * 0.5f, transform.rotation);

    }
#endif
}
