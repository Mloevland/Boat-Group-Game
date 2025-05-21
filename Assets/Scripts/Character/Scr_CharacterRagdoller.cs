using UnityEngine;

public class Scr_CharacterRagdoller : MonoBehaviour
{
    public Transform[] ragdollBones;
    private Scr_CharacterMovement movementScript;
    private Rigidbody mainRb;
    private Animator ani;
    private Collider mainCol;

    private void Awake()
    {
        mainRb = GetComponent<Rigidbody>();
        movementScript = GetComponent<Scr_CharacterMovement>();
        ani = GetComponent<Animator>();
        mainCol = GetComponent<Collider>();
    }

    [ContextMenu("Ragdoll Character")]
    public void RagdollCharacter()
    {

        movementScript.enabled = false;
        ani.enabled = false;
        mainCol.enabled = false;

        for (int i = 0; i < ragdollBones.Length; i++)
        {
            Rigidbody tempRb = ragdollBones[i].GetComponent<Rigidbody>();
            tempRb.isKinematic = false;
            tempRb.linearVelocity = mainRb.linearVelocity;
            ragdollBones[i].GetComponent<Collider>().enabled = true;
        }
        mainRb.isKinematic = true;
    }
}
