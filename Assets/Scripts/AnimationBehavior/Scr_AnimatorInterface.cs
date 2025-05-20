using UnityEngine;

public class Scr_AnimatorInterface : MonoBehaviour
{
    private Animator ani;
    public string boolReference;
    private void Awake()
    {
        ani = GetComponent<Animator>();
    }

    public void SetBool(bool value)
    {
        ani.SetBool(boolReference, value);
    }
}
