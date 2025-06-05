using UnityEngine;
using TMPro;
using System.Collections;

public class DisplayDialogue : MonoBehaviour
{
    public string subtitleKey;
    public TextMeshProUGUI textComponent;
    public AudioSource audioSource;
    public AudioClip clipToPlay;

    public float fadeDuration = 0.5f;

    private bool hasPlayed = false;
    private Coroutine fadeCoroutine;

    void Start()
    {
        if (audioSource != null && clipToPlay != null)
        {
            audioSource.clip = clipToPlay;
        }

        if (textComponent != null)
        {
            SetTextAlpha(0f); // Ensure it's invisible at start
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

            // Start fade in
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeTextAlpha(0f, 1f, fadeDuration));
        }

        if (audioSource != null && clipToPlay != null)
        {
            audioSource.Play();
            StartCoroutine(HideAfterAudio());
        }
    }

    IEnumerator HideAfterAudio()
    {
        yield return new WaitForSeconds(clipToPlay.length);

        // Fade out after audio
        if (textComponent != null)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeTextAlpha(1f, 0f, fadeDuration));
        }
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
