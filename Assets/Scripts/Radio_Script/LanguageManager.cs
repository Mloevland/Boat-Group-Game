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
        englishSubtitles.Add("radio2_line", "Proce— [audio crackle] —ed immediately to the nearest evacuation boat. Follow the routes and... [static] ...stay in o— [dropout] —pen areas. Evacu— [distortion] —ation is underway.");
        englishSubtitles.Add("radio3_line", "Evacuation is... [long dropout] ...no longer possible. For... [crackle and noise] ...your own safety: Ta— [glitch] —ke shelter in so— [choppy] —lid buildings and m—... [distorted] —ove away from low-lying areas.");
        englishSubtitles.Add("radio4_line", "Evac—... [static] —uation has en— [signal break] —ded. The boats... [skipping audio] ...have departed. If you st— [dropout] —ill hear this, you must... [glitch] ...seek higher ground and fin— [whine] —d shelter. Try to s— [pause] —end an SOS... with light. If we see... [quiet dropout] ...we’ll come.");

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
