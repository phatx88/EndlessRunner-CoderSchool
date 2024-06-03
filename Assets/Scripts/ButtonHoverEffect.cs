using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CanvasGroup startButtonBG; // Reference to the StartButton CanvasGroup
    public CanvasGroup exitButtonBG;  // Reference to the ExitButton CanvasGroup
    public float fadeDuration = 0.5f; // Duration of the fade animation

    private Coroutine fadeInCoroutine;
    private Coroutine fadeOutCoroutine;

    void Start()
    {
        // Ensure the StartButton background is visible by default
        if (startButtonBG != null)
        {
            startButtonBG.alpha = 1;
            startButtonBG.gameObject.SetActive(true);
        }

        // Ensure the ExitButton background is hidden by default
        if (exitButtonBG != null)
        {
            exitButtonBG.alpha = 0;
            exitButtonBG.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer entered: " + eventData.pointerEnter.name);

        // Check if the pointer is over the StartButton
        if (eventData.pointerEnter.transform.IsChildOf(startButtonBG.transform.parent))
        {
            Debug.Log("Hovering over StartButton");
            if (fadeInCoroutine != null) StopCoroutine(fadeInCoroutine);
            if (fadeOutCoroutine != null) StopCoroutine(fadeOutCoroutine);
            fadeInCoroutine = StartCoroutine(FadeIn(startButtonBG));
            fadeOutCoroutine = StartCoroutine(FadeOut(exitButtonBG));
        }
        // Check if the pointer is over the ExitButton
        else if (eventData.pointerEnter.transform.IsChildOf(exitButtonBG.transform.parent))
        {
            Debug.Log("Hovering over ExitButton");
            if (fadeInCoroutine != null) StopCoroutine(fadeInCoroutine);
            if (fadeOutCoroutine != null) StopCoroutine(fadeOutCoroutine);
            fadeInCoroutine = StartCoroutine(FadeIn(exitButtonBG));
            fadeOutCoroutine = StartCoroutine(FadeOut(startButtonBG));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer exited: " + eventData.pointerEnter.name);

        // Reset the background state on exit
        if (eventData.pointerEnter.transform.IsChildOf(startButtonBG.transform.parent))
        {
            Debug.Log("Exiting StartButton");
            if (fadeOutCoroutine != null) StopCoroutine(fadeOutCoroutine);
            fadeOutCoroutine = StartCoroutine(FadeOut(startButtonBG));
        }
        else if (eventData.pointerEnter.transform.IsChildOf(exitButtonBG.transform.parent))
        {
            Debug.Log("Exiting ExitButton");
            if (fadeOutCoroutine != null) StopCoroutine(fadeOutCoroutine);
            fadeOutCoroutine = StartCoroutine(FadeOut(exitButtonBG));
        }
    }

    private IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        canvasGroup.gameObject.SetActive(true);
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1;
    }

    private IEnumerator FadeOut(CanvasGroup canvasGroup)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0;
        canvasGroup.gameObject.SetActive(false);
    }
}
