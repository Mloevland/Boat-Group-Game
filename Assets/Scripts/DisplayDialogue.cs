using UnityEngine;
using TMPro;

public class DisplayDialogue : MonoBehaviour
{
    public string subtitleKey;                        // Unique subtitle key (e.g., "radio1")
    public TextMeshProUGUI textComponent;             // UI text object
    public AudioSource audioSource;                   // Audio source component
    public AudioClip clipToPlay;                      // Dialogue audio clip

    private bool playerInRange = false;

    void Start()
    {
        if (audioSource != null && clipToPlay != null)
        {
            audioSource.clip = clipToPlay;
        }

        // Optional: disable text until needed
        if (textComponent != null)
        {
            textComponent.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            UpdateText();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (textComponent != null)
                textComponent.enabled = false;
        }
    }

    public void UpdateText()
    {
        if (playerInRange)
        {
            if (textComponent == null)
            {
                Debug.LogWarning("Text component not assigned on: " + gameObject.name);
                return;
            }

            string subtitleText = LanguageManager.Instance.GetSubtitle(subtitleKey);
            textComponent.text = subtitleText;
            textComponent.enabled = true;

            PlayAudio();
        }
    }

    void PlayAudio()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
