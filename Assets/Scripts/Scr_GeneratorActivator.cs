using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem;

public class Scr_GeneratorActivator : MonoBehaviour
{
    [Header("Energy State")]
    public bool energyTurnedOn = false;

    [Header("Video Player")]
    public VideoPlayer videoPlayer;

    [Header("Mesh Renderer to Enable")]
    public MeshRenderer targetRenderer;

    private void Start()
    {
        // Stop video playback at start
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }

        // Hide the mesh at start
        if (targetRenderer != null)
        {
            targetRenderer.enabled = false;
        }
    }

    private void Update()
    {
        // Gamepad input
        if (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame)
        {
            Debug.Log("Gamepad Square (Button West) pressed.");
            ActivateGenerator();
        }

        // Keyboard input
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("Keyboard E key pressed.");
            ActivateGenerator();
        }

        // When energy is on, ensure visuals and video start
        if (energyTurnedOn)
        {
            if (videoPlayer != null && !videoPlayer.isPlaying)
            {
                videoPlayer.Play();
                Debug.Log("Video started because energyTurnedOn is true.");
            }

            if (targetRenderer != null && !targetRenderer.enabled)
            {
                targetRenderer.enabled = true;
                Debug.Log("MeshRenderer enabled because energyTurnedOn is true.");
            }
        }
    }

    private void ActivateGenerator()
    {
        if (!energyTurnedOn)
        {
            energyTurnedOn = true;
            Debug.Log("Generator activated. Energy is now ON.");
        }
    }
}
