using UnityEngine;
using UnityEngine.InputSystem;
using MA.Events;

public class Scr_InputManager : MonoBehaviour
{
    public Camera cam;

    public Vector2Event moveEvent;
    private Vector2 moveDir;

    private void Update()
    {
        if (moveDir != Vector2.zero)
        {
            moveEvent.Raise(ConvertToCameraRelative(moveDir));
        }
    }

    private Vector2 ConvertToCameraRelative(Vector2 inputDir)
    {
        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        //project forward and right vectors on the horizontal plane (y = 0)
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 desiredDir = forward * inputDir.y + right * inputDir.x;
        Vector2 packedDir = new Vector2(desiredDir.x, desiredDir.z);

        //this is the direction in the world space we want to move:
        return packedDir;
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


        }
    }



}
