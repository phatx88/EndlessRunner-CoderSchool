using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TextEffect : MonoBehaviour
{
    public float duration = 3f;
    public float pauseDuration = 2f;
    public float radius = 500f;
    private Transform logoPlacement;

    void Start()
    {
        logoPlacement = GameObject.Find("LogoPlacement").transform;

        // Create the sequence
        Sequence sequence = DOTween.Sequence();

        // Move to 1000x
        sequence.Append(transform.DOMoveX(logoPlacement.position.x, duration).SetEase(Ease.InOutSine));
        sequence.AppendInterval(pauseDuration);

        // Move to 3000x
        sequence.Append(transform.DOMoveX(3000, duration).SetEase(Ease.InOutSine));
        //sequence.AppendInterval(pauseDuration);

        // Define the circular path to move back to -1000x
        Vector3[] path = new Vector3[4];
        path[0] = new Vector3(2000, transform.position.y + radius, transform.position.z);
        path[1] = new Vector3(1000, transform.position.y + 2 * radius, transform.position.z);
        path[2] = new Vector3(0, transform.position.y + radius, transform.position.z);
        path[3] = new Vector3(-1000, transform.position.y, transform.position.z);

        // Add circular movement back to -1000x
        sequence.Append(transform.DOPath(path, 4f, PathType.CatmullRom).SetEase(Ease.Linear));

        // Loop the sequence
        sequence.SetLoops(-1, LoopType.Restart);

        // Start the sequence
        sequence.Play();
    }
}
