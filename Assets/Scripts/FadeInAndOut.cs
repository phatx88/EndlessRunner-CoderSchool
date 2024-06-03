using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FadeInAndOut : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1f;
    public float visibleDuration = 3f;

    private void Start()
    {
        // Ensure the canvas group starts with alpha 0
        canvasGroup.alpha = 0f;
        // Start the fade-in, wait, fade-out sequence
        FadeSequence();
    }

    private void FadeSequence()
    {
        // Fade in to alpha 1
        canvasGroup.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            // Wait for visibleDuration seconds
            DOVirtual.DelayedCall(visibleDuration, () =>
            {
                // Fade out to alpha 0
                canvasGroup.DOFade(0f, fadeDuration);
            });
        });
    }
}
