using UnityEngine;

public class Scr_CharacterMovement : MonoBehaviour
{

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
        Debug.Log(direction);
        transform.position = transform.position + new Vector3(direction.x,0,direction.y) * Time.deltaTime * moveSpeed;
    }
}
