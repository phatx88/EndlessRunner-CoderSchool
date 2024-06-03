using System.Collections;
using UnityEngine;

public class FlamethrowerRotation : MonoBehaviour
{
    public float rotationSpeed = 30f; // Speed of rotation
    public float rotationAngle = 45f; // Maximum rotation angle
    public float pauseDuration = 1f; // Pause duration at each end

    private bool rotatingRight = true; // Direction of rotation

    void Start()
    {
        // Start the rotation coroutine
        StartCoroutine(RotateFlamethrower());
    }

    IEnumerator RotateFlamethrower()
    {
        float startYRotation = transform.eulerAngles.y;
        float currentAngle = 0f;

        while (true)
        {
            while (rotatingRight && currentAngle < rotationAngle)
            {
                float rotationStep = rotationSpeed * Time.deltaTime;
                transform.Rotate(0, rotationStep, 0);
                currentAngle += rotationStep;
                yield return null;
            }

            rotatingRight = false;
            yield return new WaitForSeconds(pauseDuration);

            while (!rotatingRight && currentAngle > -rotationAngle)
            {
                float rotationStep = rotationSpeed * Time.deltaTime;
                transform.Rotate(0, -rotationStep, 0);
                currentAngle -= rotationStep;
                yield return null;
            }

            rotatingRight = true;
            yield return new WaitForSeconds(pauseDuration);
        }
    }
}
