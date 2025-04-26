using UnityEngine;

public class Scr_CameraManager : MonoBehaviour
{
    private GameObject currentCamera;
    public void UpdateCamera(GameObject camera)
    {
        camera.SetActive(true);

        if(currentCamera != null )
            currentCamera.SetActive(false);

        currentCamera = camera;
    }
}
