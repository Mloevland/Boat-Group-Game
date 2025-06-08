using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using TMPro;

public class Scr_LoadScene : MonoBehaviour
{
    public AssetReference sceneKey;
    public AsyncOperationHandle<SceneInstance> handle;

    private bool sceneLoaded = false;
    private bool updateSlider = false;

    public TMP_Text progressSlider;

    private void Start()
    {
        LoadSceneAsync(sceneKey);
        sceneLoaded = true;
    }

    public void LoadScene()
    {
        if (sceneLoaded)
        {
            Debug.LogWarning("Scene is already loaded.");
            return;
        }

        //LoadSceneAsync(sceneKey);
        sceneLoaded = true;
    }

    private void Update()
    {
        if (updateSlider)
        {
            Debug.Log(handle.GetDownloadStatus().Percent);
            progressSlider.text = "Game Load Progress: " + handle.GetDownloadStatus().Percent*100 + "%";
        }
    }

    private async void LoadSceneAsync(AssetReference scene)
    {
        handle = Addressables.LoadSceneAsync(scene, new LoadSceneParameters(LoadSceneMode.Additive), true, 100);
        updateSlider = true;
        await handle.Task;
        updateSlider = false;
        progressSlider.gameObject.SetActive(false);
    }
}
