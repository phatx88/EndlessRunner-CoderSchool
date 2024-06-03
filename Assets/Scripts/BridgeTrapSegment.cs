using UnityEngine;

public class BridgeTrapSegment : MonoBehaviour
{
    public GameObject trap; // Assign the "trap spike" object in the inspector
    public float moveUpDistance = 1f; // Distance to move the spike up
    public float moveDuration = 1f; // Duration of the move

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool playerDetected = false;

    void Start()
    {
        // Store the initial position of the trap spike
        initialPosition = trap.transform.position;
        // Calculate the target position
        targetPosition = initialPosition + new Vector3(0, moveUpDistance, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the bridge segment
        if (other.CompareTag("Player") && !playerDetected)
        {
            playerDetected = true;
            // Start the coroutine to move the trap spike
            StartCoroutine(MoveTrapSpike());
        }
    }

    System.Collections.IEnumerator MoveTrapSpike()
    {
        float elapsedTime = 0f;
        Vector3 startPosition = trap.transform.position;

        while (elapsedTime < moveDuration)
        {
            trap.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the trap spike reaches the target position
        trap.transform.position = targetPosition;
    }
}
