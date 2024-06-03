using UnityEngine;

public class OceanMovement : MonoBehaviour
{
    public float waveSpeed = 1.0f;
    public float waveHeight = 0.5f;
    public float waveFrequency = 1.0f;
    public float moveSpeed = 10f;
    public Vector3 moveDirection = Vector3.forward;
    public float followSpeed = 8f; // Speed at which the ocean follows the player

    Transform player; // Reference to the player character

    private MeshFilter meshFilter;
    private Vector3[] baseVertices;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            baseVertices = meshFilter.mesh.vertices;
        }
    }

    void Update()
    {
        if (meshFilter != null)
        {
            Vector3[] vertices = new Vector3[baseVertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vertex = baseVertices[i];
                vertex.y += Mathf.Sin(Time.time * waveSpeed + vertex.x * waveFrequency) * waveHeight;
                vertices[i] = vertex;
            }

            Mesh mesh = meshFilter.mesh;
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
        }

        // Smoothly follow the player character
        if (player != null)
        {
            Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}
