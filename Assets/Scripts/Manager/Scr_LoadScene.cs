using UnityEngine;
using UnityEngine.SceneManagement;

public class Scr_LoadScene : MonoBehaviour
{
    public string sceneName;
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
