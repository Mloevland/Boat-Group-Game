using UnityEngine;
using TMPro;
using System.Collections;

public class DisplayDialogue : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public string subtitleKey;
    public TextMeshProUGUI textComponent;
    public AudioSource audioSource;
    public AudioClip clipToPlay;

    [Header("Fade Settings")]
    public float subtitleFadeDuration = 0.5f;
    public float audioFadeInDuration = 2f;
    public float audioFadeOutDuration = 3f;

    private bool hasPlayed = false;
    private Coroutine fadeTextCoroutine;
    private Coroutine fadeAudioCoroutine;

    void Start()
    {
        if (audioSource != null && clipToPlay != null)
        {
            audioSource.clip = clipToPlay;
            audioSource.playOnAwake = false;
            audioSource.volume = 0f;
        }

        if (textComponent != null)
        {
            SetTextAlpha(0f);
            textComponent.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed && other.CompareTag("Player"))
        {
            PlayOnce();
        }
    }

    void PlayOnce()
    {
        hasPlayed = true;

        if (textComponent != null)
        {
            string subtitleText = LanguageManager.Instance.GetSubtitle(subtitleKey);
            textComponent.text = subtitleText;
            textComponent.enabled = true;

            if (fadeTextCoroutine != null) StopCoroutine(fadeTextCoroutine);
            fadeTextCoroutine = StartCoroutine(FadeTextAlpha(0f, 1f, subtitleFadeDuration));
        }

        if (audioSource != null && clipToPlay != null)
        {
            audioSource.volume = 0f;
            audioSource.Play();

            if (fadeAudioCoroutine != null) StopCoroutine(fadeAudioCoroutine);
            fadeAudioCoroutine = StartCoroutine(FadeAudioVolume(0f, 1f, audioFadeInDuration));

            StartCoroutine(HideAfterAudio());
        }
    }

    IEnumerator HideAfterAudio()
    {
        // Wait until it's time to start fading out
        yield return new WaitForSeconds(clipToPlay.length - audioFadeOutDuration);

        if (fadeTextCoroutine != null) StopCoroutine(fadeTextCoroutine);
        fadeTextCoroutine = StartCoroutine(FadeTextAlpha(1f, 0f, subtitleFadeDuration));

        if (fadeAudioCoroutine != null) StopCoroutine(fadeAudioCoroutine);
        fadeAudioCoroutine = StartCoroutine(FadeAudioVolume(1f, 0f, audioFadeOutDuration));
    }

    IEnumerator FadeTextAlpha(float from, float to, float duration)
    {
        float time = 0f;
        Color c = textComponent.color;

        while (time < duration)
        {
            float t = time / duration;
            c.a = Mathf.Lerp(from, to, t);
            textComponent.color = c;
            time += Time.deltaTime;
            yield return null;
        }

        c.a = to;
        textComponent.color = c;

        if (to == 0f)
            textComponent.enabled = false;
    }

    IEnumerator FadeAudioVolume(float from, float to, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            audioSource.volume = Mathf.Lerp(from, to, t);
            time += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = to;

        if (to == 0f)
            audioSource.Stop();
    }

    void SetTextAlpha(float alpha)
    {
        if (textComponent != null)
        {
            Color c = textComponent.color;
            c.a = alpha;
            textComponent.color = c;
        }
    }
}
