using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BannerManager : MonoBehaviour
{
    public CanvasGroup bannerCanvasGroup; // For fading
    public TextMeshProUGUI bannerTextPlayer;  
    public float fadeDuration = 1f; 
    public float displayDuration = 2f; 

    private Coroutine currentCoroutine; 

    void Start()
    {
        bannerCanvasGroup.alpha = 0f; 
    }

    public void ShowBanner(string message)
    {
        // Set the text
        bannerTextPlayer.text = message;

        // Stop any currently running fade coroutines
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // Start a new fade coroutine
        currentCoroutine = StartCoroutine(FadeInAndOut());
    }

    IEnumerator FadeInAndOut()
    {
        // Fade in
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        // Wait for display duration
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));
    }

    IEnumerator Fade(float startAlpha, float endAlpha, float duration)
{
    CanvasGroup bannerCanvasGroup = GameObject.Find("TurnText").GetComponent<CanvasGroup>();
    
    if (bannerCanvasGroup == null)
    {
        Debug.LogError("CanvasGroup not found on TurnText!");
        yield break;
    }

    float elapsedTime = 0f;
    bannerCanvasGroup.alpha = startAlpha;

    while (elapsedTime < duration)
    {
        elapsedTime += Time.deltaTime;
        bannerCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
        yield return null;
    }

    bannerCanvasGroup.alpha = endAlpha;
}
}
