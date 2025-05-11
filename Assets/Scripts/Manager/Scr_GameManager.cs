using UnityEngine;

public class Scr_GameManager : MonoBehaviour
{

    public int targetFrameRate = 60;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = targetFrameRate;
    }

   
}
