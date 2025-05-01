using UnityEngine;

public class Scr_CharacterMovement : MonoBehaviour
{
    public Animator ani;
    public Rigidbody rb;
    public Transform positionReferencePoint;
    public float moveSpeed;
    public LayerMask groundLayer;

    [Header("Movement Settings")]
    public float acceleration = 10f;
    public float deccelleration = 10f;
    public float velPower = 2f;
    public float jumpForce = 4f;

    private float jumpResetBuffer = 0.2f;
    private float jumpResetTimer = 0;
    private bool bufferJumpReset = false;

    private bool overrideMovement;

    //Movement Variables
    private bool isGrounded = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        CheckGrounded();

        if (!isGrounded)
        {
            RaycastHit hit;
            if(Physics.Raycast(positionReferencePoint.position,transform.forward, out hit, 0.6f, groundLayer))
            {
                PlayOverrideAnimation("Climb");
            }
            else
            {
                Debug.DrawLine(positionReferencePoint.position, positionReferencePoint.position + transform.forward*0.6f, Color.red);
            }
            
        }

    }

    public void PlayOverrideAnimation(string animationName)
    {

    }

    private void CheckGrounded()
    {
        RaycastHit hit;
        if (Physics.SphereCast(positionReferencePoint.position,0.8f, Vector3.down, out hit, 0.22f, groundLayer))
        {
            Debug.DrawLine(transform.position, hit.point, Color.green);
            if (isGrounded)
                return;
            if (bufferJumpReset)
                return;

            SetGrounded(true);
        }
        else
        {
            SetGrounded(false);
        }
    }

    private void Update()
    {
        HandleJumpBuffers();
    }

    private void HandleJumpBuffers()
    {
        if (bufferJumpReset)
        {
            jumpResetTimer -= Time.deltaTime;
            if (jumpResetTimer <= 0)
            {
                bufferJumpReset = false;
            }
        }
    }

    private void SetGrounded(bool value)
    {
        isGrounded = value;
        ani.SetBool("isGrounded", value);
    }

    public void MoveCharacter(Vector2 direction)
    {
        direction = Vector2.ClampMagnitude(direction, 1);

        if (isGrounded)
        {

            ani.SetFloat("Speed", direction.magnitude);
            Vector2 movement = CalculateMovementForce(direction, moveSpeed);
            rb.AddForce(new Vector3(movement.x, 0, movement.y) * Time.deltaTime);
            //transform.position = transform.position + new Vector3(direction.x,0,direction.y) * Time.deltaTime * moveSpeed;
            if (direction != Vector2.zero)
            {
                Quaternion desiredRotation = Quaternion.Euler(0, Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg, 0);
                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * 8f);
            }

            CheckForObstacles();
        }
        else
        {
            rb.AddForce(new Vector3(direction.x, 0, direction.y) * Time.deltaTime * 5, ForceMode.VelocityChange);
        }


        
    }

    private void CheckForObstacles()
    {

    }

    public void Jump(bool value)
    {
        if (!isGrounded)
            return;

        if(!value)
            return;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        ani.SetTrigger("Jump");
        jumpResetTimer = jumpResetBuffer;
        bufferJumpReset = true;
        SetGrounded(false);
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
