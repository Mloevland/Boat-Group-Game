using UnityEngine;

public class Scr_CameraManager : MonoBehaviour
{
    private GameObject currentCamera;
    public RenderTexture criticalObjectTexture;

    public Camera mainCam;
    public Camera objectCam;

    private void Awake()
    {
        criticalObjectTexture.height = Mathf.RoundToInt(Screen.height * 0.5f);
        criticalObjectTexture.width = Mathf.RoundToInt(Screen.width * 0.5f);

    }

    public void UpdateCamera(GameObject camera)
    {
        

        if(currentCamera != null )
            currentCamera.SetActive(false);

        camera.SetActive(true);

        currentCamera = camera;

       
 
    }

    private void FixedUpdate()
    {
        objectCam.fieldOfView = mainCam.fieldOfView;
        objectCam.nearClipPlane = mainCam.nearClipPlane;
    }
}
