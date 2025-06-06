using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;

    private Dictionary<string, string> englishSubtitles = new Dictionary<string, string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize your 4 subtitle lines
        englishSubtitles.Add("radio1_line", "Storm warning. All citizens are re— [dropout] —quested to proceed to the nearest evacuation boat... [static]... Please remain calm and follow instructions. H— [glitch] —elp is on the way.");
        englishSubtitles.Add("radio2", "The pain started, it could be controlled, but it became worse and worse. Your dad was there to calm me down.");
        englishSubtitles.Add("radio3", "When the doctor finally arrived, a wave of relief hit me. But I froze when I heard her name...");
        englishSubtitles.Add("radio4", "Before I knew it, you were there. All my fears and worries evaporated when I heard you cry...");

        Debug.Log("LanguageManager initialized with English subtitles.");
    }

    public string GetSubtitle(string key)
    {
        if (englishSubtitles.ContainsKey(key))
        {
            return englishSubtitles[key];
        }
        else
        {
            Debug.LogWarning("Subtitle not found for key: " + key);
            return "[Missing subtitle]";
        }
    }
}
