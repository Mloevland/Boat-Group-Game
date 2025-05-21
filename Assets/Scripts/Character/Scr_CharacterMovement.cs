using System.Collections.Generic;
using UnityEngine;

public class Scr_CharacterMovement : MonoBehaviour
{
    public Animator ani;
    public Rigidbody rb;
    public Transform positionReferencePoint;
    public float moveSpeed;
    public LayerMask groundLayer;
    public LayerMask climbableLayer;
    public LayerMask interactableLayer;

    [Header("Movement Settings")]
    public float acceleration = 10f;
    public float deccelleration = 10f;
    public float velPower = 2f;
    public float jumpForce = 4f;

    public float raycastScale = 1;

    private float jumpResetBuffer = 0.2f;
    private float jumpResetTimer = 0;
    private bool bufferJumpReset = false;

    private bool overrideMovement;
    private Vector3 matchStartPosition;
    private Vector3 matchPosition;
    private Quaternion matchRotation;
    private float normalisedStartTime;
    private float normalisedEndTime;
    private Rigidbody climbingRigidbody;

    private List<Vector2> limitMovementList;
    private Vector2 limitMovementDirection;
    

    //Movement Variables
    private bool isGrounded = true;
    private bool isSliding = false;
    private Vector3 groundNormal;
    private Vector2 movementInput;
    Vector3[] raycastOffsets = { Vector3.zero, Vector3.right, Vector3.forward, -Vector3.right, -Vector3.forward };


    //Sliding
    private Vector3 slidingVelocity;

    private SpringJoint draggingObject;
    private Transform draggingTransform;
    private bool isDragging = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        limitMovementList = new List<Vector2>();
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

            if (rb.linearVelocity.magnitude > 0.8f)
            {
                Quaternion desiredRotation = Quaternion.Euler(0, Mathf.Atan2(rb.linearVelocity.x, rb.linearVelocity.z) * Mathf.Rad2Deg, 0);
                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * 8f);
            }

            if (slidingVelocity.magnitude < 0.01f)
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

            rb.AddForce(Vector3.down * 3f);

            RaycastHit hit;
            if(Physics.Raycast(positionReferencePoint.position, new Vector3(movementInput.x,0,movementInput.y), out hit, 0.6f * transform.localScale.y * raycastScale, climbableLayer))
            {
                RaycastHit hit2;

                if(Physics.Raycast(hit.point + (Vector3.up*2 - hit.normal*0.3f) * transform.localScale.y * raycastScale, Vector3.down, out hit2, 2f * transform.localScale.y * raycastScale, groundLayer))
                {
                    PlayOverrideAnimation("Climb",transform.position, new Vector3(hit.point.x,hit2.point.y,hit.point.z),
                        Quaternion.LookRotation(new Vector3(hit.point.x, transform.position.y, hit.point.z) - transform.position, Vector3.up), 0.05f, 0.6f);
                    if(climbingRigidbody = hit.collider.GetComponent<Rigidbody>())
                    {
                        climbingRigidbody.isKinematic = true;
                    }

                    Debug.DrawLine(hit.point + Vector3.up * 2 - hit.normal * 0.3f, hit2.point, Color.blue, 5);
                }
                else
                {
                    Debug.DrawLine(hit.point + Vector3.up*2 - hit.normal * 0.5f, hit.point + Vector3.up*2 - hit.normal * 0.5f + Vector3.down * 2f, Color.red);
                }
               
            }
            else
            {
                Debug.DrawLine(positionReferencePoint.position, positionReferencePoint.position + new Vector3(movementInput.x, 0, movementInput.y) * 0.6f, Color.red);
            }
            
        }
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(positionReferencePoint.position, new Vector3(movementInput.x, 0, movementInput.y), out hit, 0.6f * transform.localScale.y * raycastScale, groundLayer))
            {

            }

            DoMovement();
            HandleRotation();
        }

    }

    [ContextMenu("Test Animation")]
    public void TestOverrideAni()
    {
        PlayOverrideAnimation("Climb",transform.position, transform.position + Vector3.up * 5,Quaternion.identity, 0.0f, 0.5f);
    }

    public void PlayOverrideAnimation(string animationName,Vector3 startPos, Vector3 matchPos,Quaternion rotation, float normalizedStartTime, float normalizedEndTime)
    {
        ani.applyRootMotion = true;
        rb.isKinematic = true;

        matchPosition = matchPos;
        matchStartPosition = startPos;
        matchRotation = rotation;
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
        transform.rotation = Quaternion.Lerp(transform.rotation, matchRotation, Time.deltaTime * 12);
    }

    public void StopOverrideAnimation()
    {
        ani.applyRootMotion = false;
        overrideMovement = false;
        rb.isKinematic = false;

        if (climbingRigidbody)
        {
            climbingRigidbody.isKinematic = false;
            climbingRigidbody = null;
        }
    }

    public void CheckForInteraction(bool interact)
    {
        if (interact)
        {
            Collider[] hitInteractables = Physics.OverlapSphere(positionReferencePoint.position, 0.75f, interactableLayer);
            if (hitInteractables.Length != 0)
            {

                draggingTransform = hitInteractables[0].transform;

                RaycastHit hit;
                Physics.Raycast(positionReferencePoint.position, hitInteractables[0].transform.position - positionReferencePoint.position, out hit, 1.05f, interactableLayer);

                draggingObject = hitInteractables[0].gameObject.AddComponent<SpringJoint>();
                draggingObject.anchor = hitInteractables[0].transform.InverseTransformPoint(hit.point);
                draggingObject.connectedBody = rb;
                draggingObject.minDistance = 0.05f;
                draggingObject.maxDistance = 0.05f;
                draggingObject.spring = 100f;
                draggingObject.damper = 1;
                draggingObject.enableCollision = true;

                isDragging = true;
                ani.SetLayerWeight(2, 1f);
            }
        }
        else
        {
            if(draggingObject == null)
                return;

            draggingTransform = null;
            Destroy(draggingObject);
            isDragging = false;
            ani.SetLayerWeight(2, 0f);
        }
        
    }

    private void CheckGrounded()
    {
        RaycastHit hit;
        groundNormal = Vector3.zero;
        bool groundDetected = false;

        for (int i = 0; i < raycastOffsets.Length; i++)
        {
            if (Physics.Raycast(positionReferencePoint.position + (raycastOffsets[i]*0.5f) * transform.localScale.y * 0.33f, Vector3.down, out hit, 1.07f * transform.localScale.y * raycastScale, groundLayer))
            {
                Debug.DrawLine(positionReferencePoint.position + (raycastOffsets[i]*0.5f) * transform.localScale.y * 0.33f, hit.point, Color.green);
                groundNormal += hit.normal;
                groundDetected = true; 
            }
            else
            {
                Debug.DrawLine(positionReferencePoint.position + (raycastOffsets[i]*0.5f) * transform.localScale.y * 0.33f, positionReferencePoint.position + (raycastOffsets[i]*0.5f) * transform.localScale.y + Vector3.down * 1.05f * transform.localScale.y * raycastScale, Color.red);
            }
        }
        groundNormal.Normalize();

        if (groundDetected)
        {

            //Debug.Log(Vector3.Dot(groundNormal, Vector3.up));
            if(Vector3.Dot(groundNormal, Vector3.up) < 0.8f)
            {
                
                slidingVelocity = Vector3.ProjectOnPlane(groundNormal, Vector3.up) * 0.5f;
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

    public void OverrideMovementLimit(Vector2 limit)
    {
        Debug.Log(limit);
        //limitMovementDirection = limit;
    }

    public void AddMovementLimit(Vector2 limit)
    {
        limitMovementList.Add(limit);
        CalculateMovementLimit();
    }

    public void RemoveMovementLimit(Vector2 limit)
    {
        if(limitMovementList.Contains(limit))
            limitMovementList.Remove(limit);

        CalculateMovementLimit();
    }

    private void CalculateMovementLimit()
    {
        if (limitMovementList.Count == 0)
        {
            limitMovementDirection = Vector2.zero;
            return;
        }
        Vector2 tempDirection = Vector2.zero;
        float tempMagnitude = 0f;
        for (int i = 0; i < limitMovementList.Count; i++)
        {
            tempDirection += limitMovementList[i];
            tempMagnitude += limitMovementList[i].magnitude;
        }
        tempDirection.Normalize();
        tempMagnitude /= limitMovementList.Count;
        limitMovementDirection = tempDirection * tempMagnitude;
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

        if(limitMovementDirection != Vector2.zero)
        {
            Vector2 limitNorm = limitMovementDirection.normalized;
            float limitMag = limitMovementDirection.magnitude * -1 +1;
            

            Vector3 overrideForward = new Vector2(limitNorm.y, -limitNorm.x);

            
            Vector2 rightMovement = limitNorm * Vector2.Dot(direction, limitNorm);
            Vector2 forwardMovement = overrideForward * Vector2.Dot(direction, overrideForward);

            float playerCompliance = Mathf.Abs(Vector2.Dot(direction, rightMovement)) * -1+1;
            //Debug.Log(playerCompliance);

            movementInput = (rightMovement + (forwardMovement*playerCompliance)*limitMag);

            Debug.Log(limitMag);
            if (limitMag > 0.05f)
            {
                movementInput.Normalize();
                movementInput *= direction.magnitude;

            }
            else if(playerCompliance > 0.8f)
            {
                movementInput = Vector2.zero;
            }

            
        }



    }

    public void RespawnPlayer(GameObject respawnPoint)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.position = respawnPoint.transform.position;
        rb.transform.position = respawnPoint.transform.position;
        rb.transform.rotation = respawnPoint.transform.rotation;
        rb.isKinematic = false;
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

            //if(limitMovementDirection != Vector2.one)
                //movementInput *= Mathf.Abs(Vector2.Dot(movementInput, limitMovementDirection));

            Vector2 movement = CalculateMovementForce(movementInput, moveSpeed);
            
            rb.AddForce(Vector3.ProjectOnPlane(new Vector3(movement.x, 0, movement.y), groundNormal));

            if(movementInput.magnitude == 0)
            {
                if(Vector3.Dot(groundNormal, Vector3.up) < 0.95f)
                    rb.linearVelocity = new Vector3(0, rb.linearVelocity.y * 0.8f, 0);
            }

            if (isDragging)
            {
                ani.SetFloat("DragDir", Vector3.Dot((draggingTransform.position - positionReferencePoint.position).normalized, new Vector3(movementInput.x, 0, movementInput.y).normalized));
            }

            CheckForObstacles();
        }
        else
        {
            rb.AddForce(new Vector3(movementInput.x, 0, movementInput.y));

        }
    }

    private void HandleRotation()
    {
        if (isGrounded)
        {
            if (isDragging)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(
                    new Vector3(draggingTransform.position.x, positionReferencePoint.position.y, draggingTransform.position.z) - positionReferencePoint.position, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * 8f);
                return;
            }

            if (movementInput != Vector2.zero)
            {
                Quaternion desiredRotation = Quaternion.Euler(0, Mathf.Atan2(movementInput.x, movementInput.y) * Mathf.Rad2Deg, 0);
                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * 8f);
            }
        }
        else
        {


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

        ani.SetTrigger("Jump");
        ApplyJumpForce();
        
    }

    public void ApplyJumpForce()
    {
        rb.AddForce(Vector3.up * jumpForce + new Vector3(movementInput.x, 0, movementInput.y) * 1, ForceMode.Impulse);

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

    public void OnCollisionEnter(Collision collision)
    {
        Rigidbody collisionBody;
        if(collisionBody = collision.gameObject.GetComponent<Rigidbody>())
        {
            Debug.Log("Collision of force: " + collision.gameObject.GetComponent<Rigidbody>().linearVelocity.magnitude * collision.gameObject.GetComponent<Rigidbody>().mass);
        }
        


    }

}
