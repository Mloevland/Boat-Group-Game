using UnityEngine;
using UnityEngine.InputSystem;
using MA.Events;
using System;

public class Scr_InputManager : MonoBehaviour
{
    public Camera cam;

    public Vector2Event moveEvent;
    private Vector2 moveDir;

    public Vector3 forwardTruth, rightTruth;
    public Vector3 currentForward, currentRight;

    public bool relativeAligned;

    private Vector2 lastFrameInput;

    public Vector2 DebugAlignmentAngle = Vector2.zero;

    private void Start()
    {
        DefineNewRelative(cam.transform);
        AlignRelatives();
    }

    private void Update()
    {
        if (moveDir != Vector2.zero)
        {
            moveEvent.Raise(ConvertToCameraRelative(moveDir));
        }
    }

    private Vector2 ConvertToCameraRelative(Vector2 inputDir)
    {
        float inputDelta = CalculateInputDelta(inputDir);

        if (!relativeAligned)
        {
            
            currentForward = Vector3.Lerp(currentForward, forwardTruth, inputDelta);
            currentRight = Vector3.Lerp(currentRight, rightTruth, inputDelta);

            DebugAlignmentAngle.x = Vector3.Distance(currentForward, forwardTruth);
            DebugAlignmentAngle.y = Vector3.Distance(currentRight, rightTruth);

            if(DebugAlignmentAngle.x < 0.01f && DebugAlignmentAngle.y < 0.01f)
            {
                AlignRelatives();
                relativeAligned = true;
            }
        }

        Vector3 desiredDir = currentForward * inputDir.y + currentRight * inputDir.x;
        Vector2 packedDir = new Vector2(desiredDir.x, desiredDir.z);

        //this is the direction in the world space we want to move:
        return packedDir;
    }

    private float CalculateInputDelta(Vector2 input)
    {

        if(input == Vector2.zero)
        {
            AlignRelatives();
            return 0;
        }

        Vector2 delta = lastFrameInput - input;
        lastFrameInput = input;

        

        return delta.magnitude;
    }

    private void AlignRelatives()
    {
        Debug.Log("Aligned Relatives");
        currentForward = forwardTruth;
        currentRight = rightTruth;
    }

    public void UpdateRelativeObject(GameObject relativeObject)
    {
        DefineNewRelative(relativeObject.transform);
        relativeAligned = false;
    }
    private void DefineNewRelative(Transform truth)
    {
        Vector3 forward = truth.forward;
        Vector3 right = truth.right;

        //project forward and right vectors on the horizontal plane (y = 0)
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        forwardTruth = forward;
        rightTruth = right;

        relativeAligned = false;
    }

    public void GetMove(InputAction.CallbackContext context)
    {
        
        if (context.started)
        {
            moveDir = context.ReadValue<Vector2>();
        }
        if (context.performed)
        {
            moveDir = context.ReadValue<Vector2>();


        }
        if (context.canceled)
        {
            moveDir = Vector2.zero;
            moveEvent.Raise(Vector2.zero);
            AlignRelatives();
        }
    }



}
