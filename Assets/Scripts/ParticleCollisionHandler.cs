using System.Collections.Generic;
using UnityEngine;

public class ParticleCollisionHandler : MonoBehaviour
{
    private ParticleSystem ps;
    private List<ParticleCollisionEvent> collisionEvents;

    PlayerController playerController;
    private bool isCollisionProcessed = false;

    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        // Get the Particle System component
        ps = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        // Check if collision is already processed
        if (isCollisionProcessed) return;

        int numCollisionEvents = ps.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < numCollisionEvents; i++)
        {
            // Debug log to check if any collision is detected
            Debug.Log("Collision detected with: " + other.name);

            // Here you can add your logic for what happens when particles collide
            if (other.CompareTag("Player"))
            {
                Debug.Log("Collision detected with Player!");
                playerController.isDead = true;
                playerController.PlayerDieLogic();
                isCollisionProcessed = true;
                break; // Exit the loop after processing the collision
            }
        }
    }
}
