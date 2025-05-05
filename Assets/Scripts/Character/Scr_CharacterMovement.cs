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
    private Vector3 matchStartPosition;
    private Vector3 matchPosition;
    private Quaternion matchRotation;
    private float normalisedStartTime;
    private float normalisedEndTime;

    //Movement Variables
    private bool isGrounded = true;
    private bool isSliding = false;
    private Vector2 movementInput;

    //Sliding
    private Vector3 slidingVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (overrideMovement)
        {
            //MatchTarget();

            return;
        }


        CheckGrounded();

        if (isSliding)
        {
            rb.AddForce(slidingVelocity * 10f, ForceMode.Acceleration);

            if(slidingVelocity.magnitude < 0.01f)
            {
                
                Debug.Log("Slide velocity below threshold " + slidingVelocity.magnitude);
                SetSliding(false);
            }
            else
            {
                return;

            }

        }

        if (!isGrounded)
        {
            RaycastHit hit;
            if(Physics.Raycast(positionReferencePoint.position, transform.forward, out hit, 0.6f * transform.localScale.y, groundLayer))
            {
                RaycastHit hit2;

                if(Physics.Raycast(hit.point + (Vector3.up*2 - hit.normal*0.3f) * transform.localScale.y, Vector3.down, out hit2, 2f * transform.localScale.y, groundLayer))
                {
                    PlayOverrideAnimation("Climb",transform.position, new Vector3(hit.point.x,hit2.point.y,hit.point.z), 0.05f, 0.6f);
                    Debug.DrawLine(hit.point + Vector3.up * 2 - hit.normal * 0.3f, hit2.point, Color.blue, 5);
                }
                else
                {
                    Debug.DrawLine(hit.point + Vector3.up*2 - hit.normal * 0.5f, hit.point + Vector3.up*2 - hit.normal * 0.5f + Vector3.down * 2f, Color.red);
                }
               
            }
            else
            {
                Debug.DrawLine(positionReferencePoint.position, positionReferencePoint.position + transform.forward*0.6f, Color.red);
            }
            
        }
        else
        {
            DoMovement();
        }

    }

    [ContextMenu("Test Animation")]
    public void TestOverrideAni()
    {
        PlayOverrideAnimation("Climb",transform.position, transform.position + Vector3.up * 5, 0.0f, 0.5f);
    }

    public void PlayOverrideAnimation(string animationName,Vector3 startPos, Vector3 matchPos, float normalizedStartTime, float normalizedEndTime)
    {
        ani.applyRootMotion = true;
        rb.isKinematic = true;

        matchPosition = matchPos;
        matchStartPosition = startPos;
        matchRotation = Quaternion.identity;
        normalisedStartTime = normalizedStartTime;
        normalisedEndTime = normalizedEndTime;

        ani.SetTrigger(animationName);
        overrideMovement = true;
    }

    public void MatchTarget()
    {
        if (ani.isMatchingTarget)
            return;

        float normalizeTime = Mathf.Clamp01(ani.GetCurrentAnimatorStateInfo(2).normalizedTime);

        if(normalizeTime < normalisedStartTime)
            return;
        if (normalizeTime > normalisedEndTime)
            return;

        float remappedTime = Remap(normalizeTime, normalisedStartTime, normalisedEndTime, 0f, 1f);

        Debug.Log("Matching: " + normalizeTime + " and " + remappedTime);

        //ani.MatchTarget(matchPosition, matchRotation, target, new MatchTargetWeightMask(Vector3.one,0), normalisedStartTime, normalisedEndTime);

        transform.position = Vector3.Lerp(transform.position,Vector3.Lerp(matchStartPosition, matchPosition, remappedTime),Time.deltaTime * 12);
    }

    public void StopOverrideAnimation()
    {
        ani.applyRootMotion = false;
        overrideMovement = false;
        rb.isKinematic = false;

    }

    private void CheckGrounded()
    {
        RaycastHit hit;
        if (Physics.SphereCast(positionReferencePoint.position,0.8f * transform.localScale.y, Vector3.down, out hit, 0.22f * transform.localScale.y, groundLayer))
        {
            Debug.DrawLine(transform.position, hit.point, Color.green);

            Debug.Log(Vector3.Dot(hit.normal, Vector3.up));
            if(Vector3.Dot(hit.normal, Vector3.up) < 0.8f)
            {
                
                slidingVelocity = Vector3.ProjectOnPlane(hit.normal, Vector3.up) * 0.5f;
                Debug.Log("Start Slide with velocity " + slidingVelocity + " and a magnitude of " + slidingVelocity.magnitude);
                SetSliding(true);
            }
            else
            {
                slidingVelocity *= 0.5f;
                //SetSliding(false);
            }

            if (isGrounded)
                return;
            if (bufferJumpReset)
                return;

            SetGrounded(true);
        }
        else
        {
            SetGrounded(false);
            if (isSliding)
            {

            }
                //SetSliding(false);
        }
    }

    private void Update()
    {
        HandleJumpBuffers();

        if (overrideMovement)
        {
            MatchTarget();

            return;
        }
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

    private void SetSliding(bool value)
    {
        isSliding = value;
        if (!value)
        {
            slidingVelocity = Vector3.zero;
        }
        ani.SetBool("isSliding", value);
    }

    public void MoveCharacter(Vector2 direction)
    {

        movementInput = Vector2.ClampMagnitude(direction, 1); 
    }

    private void DoMovement()
    {
        if (overrideMovement)
            return;

        if (isSliding)
            return;



        if (isGrounded)
        {

            ani.SetFloat("Speed", movementInput.magnitude);
            Vector2 movement = CalculateMovementForce(movementInput, moveSpeed);
            rb.AddForce(new Vector3(movement.x, 0, movement.y));
            //transform.position = transform.position + new Vector3(direction.x,0,direction.y) * Time.deltaTime * moveSpeed;
            if (movementInput != Vector2.zero)
            {
                Quaternion desiredRotation = Quaternion.Euler(0, Mathf.Atan2(movementInput.x, movementInput.y) * Mathf.Rad2Deg, 0);
                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * 8f);
            }

            CheckForObstacles();
        }
        else
        {
            rb.AddForce(new Vector3(movementInput.x, 0, movementInput.y));
            if (rb.linearVelocity.magnitude > 0.8f)
            {
                Quaternion desiredRotation = Quaternion.Euler(0, Mathf.Atan2(rb.linearVelocity.x, rb.linearVelocity.z) * Mathf.Rad2Deg, 0);
                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * 8f);
            }

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

    public float Remap( float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }


}
