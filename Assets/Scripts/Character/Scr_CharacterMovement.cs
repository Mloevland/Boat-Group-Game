using UnityEngine;

public class Scr_CharacterMovement : MonoBehaviour
{
    public Animator ani;
    public float moveSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveCharacter(Vector2 direction)
    {

        ani.SetFloat("Speed", direction.magnitude);
        //Debug.Log(direction);
        transform.position = transform.position + new Vector3(direction.x,0,direction.y).normalized * Time.deltaTime * moveSpeed;
        Quaternion desiredRotation = Quaternion.Euler(0, Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * 8f); 
    }
}
