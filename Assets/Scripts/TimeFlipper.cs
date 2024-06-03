using System.Collections;
using UnityEngine;

public class TimeFlipper : MonoBehaviour
{
    private bool isRotating = true;
    private float totalRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RotateAndPause());
    }

    IEnumerator RotateAndPause()
    {
        while (true)
        {
            if (isRotating)
            {
                if (totalRotation < 180f)
                {
                    transform.Rotate(0, 0, 1);
                    totalRotation += 1f;
                }
                else
                {
                    isRotating = false;
                    totalRotation = 0f;
                    yield return new WaitForSeconds(1f);
                }
            }
            else
            {
                isRotating = true;
            }
            yield return null; // Wait for the next frame
        }
    }
}
