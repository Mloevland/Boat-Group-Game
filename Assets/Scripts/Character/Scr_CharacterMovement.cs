using UnityEngine;

public class Scr_CharacterMovement : MonoBehaviour
{
    public Animator ani;
    public Rigidbody rb;
    public float moveSpeed;

    [Header("Movement Settings")]
    public float acceleration = 10f;
    public float deccelleration = 10f;
    public float velPower = 2f;
    public float jumpForce = 4f;

    //Movement Variables
    private bool isGrounded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 1.05f))
        {
            isGrounded = true;
        }
        else
        {
            //isGrounded = false;
        }
    }

    public void MoveCharacter(Vector2 direction)
    {
        direction = Vector2.ClampMagnitude(direction, 1);
        ani.SetFloat("Speed", direction.magnitude);
        Vector2 movement = CalculateMovementForce(direction, moveSpeed);
        rb.AddForce(new Vector3(movement.x, 0, movement.y) * Time.deltaTime);
        //transform.position = transform.position + new Vector3(direction.x,0,direction.y) * Time.deltaTime * moveSpeed;
        if(direction != Vector2.zero)
        {
            Quaternion desiredRotation = Quaternion.Euler(0, Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * 8f);
        }
       
    }

    public void Jump(bool value)
    {
        if (!isGrounded)
            return;

        if(!value)
            return;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        ani.SetTrigger("Jump");
        isGrounded = true;
    }

    private Vector2 CalculateMovementForce(Vector2 playerInput, float speedMultiplier)
    {
        Vector2 targetSpeed = playerInput * speedMultiplier;
        Vector2 currentSpeed = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z);
        Vector2 accelDif = targetSpeed - currentSpeed;
        float accelRateX = (Mathf.Abs(targetSpeed.x) > 0.01f) ? acceleration : deccelleration;
        float accelRateY = (Mathf.Abs(targetSpeed.y) > 0.01f) ? acceleration : deccelleration;
        Vector2 movement;
        movement.x = Mathf.Pow(Mathf.Abs(accelDif.x) * accelRateX, velPower) * Mathf.Sign(accelDif.x);
        movement.y = Mathf.Pow(Mathf.Abs(accelDif.y) * accelRateY, velPower) * Mathf.Sign(accelDif.y);
        return movement;
    }
}
