#region Original script with refactor

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // Enumeration for player direction
    enum enDirection { North, East, West };

    [Header("VFX")]
    [SerializeField] private ParticleSystem VFXDie;
    [SerializeField] private ParticleSystem collectCoinVFX;
    [SerializeField] private ParticleSystem powerUpVFX;
    [SerializeField] private ParticleSystem dropGoldVFX;
    [SerializeField] private ParticleSystem playerTrailVFX;
    [SerializeField] private ParticleSystem invulnerabilityVFX;
    [SerializeField] private ParticleSystem heartLifeVFX;

    [Header("UI Elements")]
    [SerializeField] private GameObject GUIMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject respawnButton;
    [SerializeField] private GameObject nextLevelButton;
    [SerializeField] private CanvasGroup instructionUICanvasGroup;
    [SerializeField] private TextMeshProUGUI distanceScoreText;
    [SerializeField] private TextMeshProUGUI coinScoreText;
    [SerializeField] private TextMeshProUGUI heartLifeText;
    [SerializeField] private TextMeshProUGUI timeRemainText;

    [Header("Player Settings")]
    [SerializeField] private List<GameObject> gameObjects;
    [SerializeField] public int heartLife = 3;
    [SerializeField] public float playerMaxSpeed = 10.0f;
    [SerializeField] public float invulMaxDuration = 12.0f;
    [SerializeField] public float timeRemain = 60f;
    [SerializeField] public float translationFactor = 10.0f; // Smoothens the turning of direction
    //[SerializeField] public float translationFactorMobile = 5.0f; //Smoothens the turning of direction for Mobile
    [SerializeField] private float playerSpeed;
    [SerializeField] private float gValue = 20.0f; // Gravity value
    [SerializeField] private float jumpForce = 1.5f;
    [SerializeField] private float verticalVelocity = 0f; // Additional variable for vertical velocity
    [SerializeField] public bool isDead = false;
    //[SerializeField] public bool mobileEnabled = true;

    Vector3 playerVector; // Player movement vector
    Vector3 previousPosition;
    Vector3 lastPosition;

    enDirection playerDirection = enDirection.North; // Default direction (North)
    enDirection playerNextDirection = enDirection.North; // Next direction for the player

    CharacterController characterController; // Components and player properties
    Animator anim;
    BridgeSpawner bridgeSpawner;

    int coinCollected = 0;
    int coinCollectedBest;
    int distanceRun = 0;
    int distanceRunBest;
    
    float timer = 0;
    float distance = 0;
    
    bool canTurnRight = false;
    bool canTurnLeft = false;
    bool isMoving = false;
    bool isInvulnerable = false;
    bool instructionUIToggle = true;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f; // Resume the game

        playerSpeed = playerMaxSpeed; // Initialize player speed

        characterController = GetComponent<CharacterController>(); // Get CharacterController component
        anim = GetComponent<Animator>(); // Get Animator component
        bridgeSpawner = GameObject.Find("BridgeManager").GetComponent<BridgeSpawner>();

        InitializeUIElements(); // Initialize all UI elements
       
        playerVector = Vector3.forward * playerSpeed * Time.deltaTime; // Initialize player movement vector
        playerDirection = enDirection.North; // Set initial direction to North
        playerNextDirection = enDirection.North; // Ensure next direction is also North
        
        AudioController.Instance.PlayMusic("TikiDrum"); // Play BackGround music
        lastPosition = transform.position; // Initialize lastPosition

        //Hide GameOverPanel & NextLevel Button
        gameOverMenu.SetActive(false);
        nextLevelButton.SetActive(false);
        GUIMenu.SetActive(false);

        //Setting value for Best Coin and Distance
        distanceRunBest = PlayerPrefs.GetInt("highscoreD");
        coinCollectedBest = PlayerPrefs.GetInt("highscoreC");


    }

    // Update is called once per frame
    void Update()
    {
        PlayerLogic(); // Call the logic function each frame

        UpdateUI(); // Update UI elements with current values
        
        InstructionUIToggle();

        //Debug.Log(Input.acceleration.x);
    }

    void PlayerLogic()
    {
        if (isDead)
            return;

        if (!characterController.enabled)
        {
            characterController.enabled = true;
        }

        isMoving = false; // Reset isMoving flag

        //timer += Time.deltaTime;
        //playerSpeed += 0.05f * Time.deltaTime; // Gradually increase player speed over time

        #region Horizontal Direction 
        // Handles player direction change based on key inputs 'E' and 'Q'
        if (Input.GetKeyDown(KeyCode.E)) // When 'E' key is pressed
        {
            switch (playerDirection)
            {
                case enDirection.North:
                    playerNextDirection = enDirection.East;
                    transform.rotation = Quaternion.Euler(0, 90, 0);  // Rotate player to face East
                    break;
                case enDirection.West:
                    playerNextDirection = enDirection.North;
                    transform.rotation = Quaternion.Euler(0, 0, 0);  // Rotate player to face North
                    break;
            }

            AudioController.Instance.PlaySFX("turn");
        }
        else if (Input.GetKeyDown(KeyCode.Q)) // When 'Q' key is pressed
        {
            switch (playerDirection)
            {
                case enDirection.North:
                    playerNextDirection = enDirection.West;
                    transform.rotation = Quaternion.Euler(0, -90, 0);  // Rotate player to face West
                    break;
                case enDirection.East:
                    playerNextDirection = enDirection.North;
                    transform.rotation = Quaternion.Euler(0, 0, 0);  // Rotate player to face North
                    break;
            }

            AudioController.Instance.PlaySFX("turn");

        }

        playerDirection = playerNextDirection; // Update player direction

        // Update player movement vector based on direction
        if (playerDirection == enDirection.North)
        {
            playerVector = Vector3.forward * playerSpeed * Time.deltaTime;
        }
        else if (playerDirection == enDirection.East)
        {
            playerVector = Vector3.right * playerSpeed * Time.deltaTime;
        }
        else if (playerDirection == enDirection.West)
        {
            playerVector = Vector3.left * playerSpeed * Time.deltaTime;
        }
        #endregion

        // Set horizontal movement based on direction and input
        switch (playerDirection)
        {
            case enDirection.North:
                playerVector.x = Input.GetAxisRaw("Horizontal") * translationFactor * Time.deltaTime; 
                break;
            case enDirection.East:
                playerVector.z = -Input.GetAxisRaw("Horizontal") * translationFactor * Time.deltaTime; 
                break;
            case enDirection.West:
                playerVector.z = Input.GetAxisRaw("Horizontal") * translationFactor * Time.deltaTime;
                break;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            DoSliding(); // Trigger sliding when the down arrow key is pressed

        }

        #region Adding Vertical Velocity to vertical movement logic 

        // Vertical movement logic
        if (characterController.isGrounded)
        {
            if (verticalVelocity < 0)
            {
                verticalVelocity = -0.2f; // Ensure player stays grounded
            }

            // Jumping logic
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AudioController.Instance.PlaySFX("gruntJump", 0.4f);
                anim.SetTrigger("isJumping"); // Trigger jump animation
                verticalVelocity = Mathf.Sqrt(jumpForce * gValue); // Calculate jump force
            }
        }
        else
        {
            verticalVelocity -= gValue * Time.deltaTime; // Apply gravity if not grounded
        }

        // Apply vertical velocity to player vector
        playerVector.y = verticalVelocity * Time.deltaTime;
        #endregion


        //Death Flag On if Fall into bottom
        if (transform.position.y < -1f)
        {
            isDead = true;
            AudioController.Instance.PlaySFX("scream", 2f);
            anim.SetTrigger("isTripping");
            PlayerDieLogic();
            SaveScore();
        }

        previousPosition = transform.position; // Store previous position before moving

        characterController.Move(playerVector); // Move the character controller

        lastPosition = transform.position; // Store previous position after moving

        // Calculate distance based on position change
        if (Vector3.Distance(previousPosition, transform.position) > 0.01f) // Check if player has moved
        {
            isMoving = true; // Set isMoving to true if player has moved
        }

        if (isMoving)
        {
            timer += Time.deltaTime; // Update timer only if player is moving

        }

        if (playerSpeed > playerMaxSpeed)
        {
            playerSpeed -= Time.deltaTime * 2;
        }


        if (playerSpeed <= playerMaxSpeed)
        {
            playerTrailVFX.Pause();
        }

        //Calculate remaining Time
        if (timeRemain > 0)
        {
            timeRemain -= Time.deltaTime;
        }

        //Debug.Log(playerSpeed);
        //Calculate Distrance Travelled
        distance = playerSpeed * timer;
        distanceRun = (int)distance;
    }

    void DoSliding()
    {
        // Adjust CharacterController for sliding
        characterController.height = 1f;
        characterController.center = new Vector3(0, 0.5f, 0);
        characterController.radius = 0;
        if (playerSpeed <= playerMaxSpeed + playerMaxSpeed / 4)
        {
            playerSpeed += playerSpeed / 3; //increase speed by a third for short time
        }
        StartCoroutine(ReEnableCC()); // Reset CharacterController after sliding
        AudioController.Instance.PlaySFX("slide");
        anim.SetTrigger("isSliding"); // Trigger sliding animation
    }

    IEnumerator ReEnableCC()
    {
        yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds

        // Reset CharacterController dimensions
        characterController.height = 2f;
        characterController.center = new Vector3(0, 1f, 0);
        characterController.radius = 0.2f;

    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "LCorner")
        {
            canTurnLeft = true;
        }
        else if (hit.gameObject.tag == "RCorner")
        {
            canTurnRight = true;
        }
        else
        {
            canTurnLeft = false;
            canTurnRight = false;
           
        }


        //Death Flag On if Hit Obstacle
        if (hit.gameObject.tag == "Obstacle" && !isInvulnerable)
        {
            isDead = true;
            AudioController.Instance.PlaySFX("splat", 0.4f);
            anim.SetTrigger("isTripping");
            PlayerDieLogic();
            SaveScore();
        }

    }

    private void OnGUI()
    {

        if (isDead)
        {
            GUIMenu.SetActive(true);
            if (heartLife <= 0)
            {
                respawnButton.SetActive(false);
                gameOverMenu.SetActive(true);
            }
        }
        if (timeRemain <= 0)
        {
            Time.timeScale = 0f; // Pause the game
            GUIMenu.SetActive(true);
            respawnButton.SetActive(false);
            nextLevelButton.SetActive(true);
        }
    }

    public void DeathEvent()
    {
        characterController.enabled = false; // Disable the character controller
        isDead = true; // Set isDead to true to pause player logic

        // Reset player position and rotation
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(0, 0, 0);

        // Reset player direction and speed
        playerDirection = enDirection.North;
        playerNextDirection = enDirection.North;
        playerSpeed = playerMaxSpeed;

        // Reset any residual velocities or movement vectors
        verticalVelocity = 0f;
        playerVector = Vector3.forward * playerSpeed * Time.deltaTime;

        // Clean the scene through the BridgeSpawner
        bridgeSpawner.CleanTheScene();

        //Reset Timer & score
        timer = 0;
        coinCollected = 0;

        // Trigger the spawn animation
        anim.SetTrigger("isSpawned");

        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].SetActive(true);

        }
        StartCoroutine(ReEnableCharacterController()); // Re-enable the character controller after a slight delay
    }

    IEnumerator ReEnableCharacterController()
    {
        yield return new WaitForSeconds(0.1f); // Wait for a short duration to ensure segments are initialized

        // Ensure position is reset correctly before re-enabling
        transform.position = Vector3.zero;
        playerVector = Vector3.forward * playerSpeed * Time.deltaTime; // Reset movement vector

        //Reset Death TextMenu
        GUIMenu.SetActive(false);

        characterController.enabled = true; // Re-enable the character controller
        isDead = false; // Mark the player as not dead
    }

    void FootStepEventA()
    {
        AudioController.Instance.PlaySFX("footStep", 0.8f);
    }
    void FootStepEventB()
    {
        AudioController.Instance.PlaySFX("footStep", 0.8f);
    }
    void JumpLandEvent()
    {
        AudioController.Instance.PlaySFX("gruntJumpLand", 2f);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Coin")
        {
            collectCoinVFX.Play();
            AudioController.Instance.PlaySFX("coin");
            coinCollected += 1;
        }

        //Power Up if Hit PU Item
        if (col.gameObject.tag == "PU_Speed")
        {
            powerUpVFX.Play();
            playerTrailVFX.Play();
            AudioController.Instance.PlaySFX("powerUp", 2f);
            col.gameObject.GetComponentInChildren<ParticleSystem>().Play();
            playerSpeed *= 2;
        }

        // Power Up if Hit Invulnerability Item
        if (col.gameObject.tag == "PU_Invul") // Add this condition
        {
            StartCoroutine(InvulnerabilityPowerUp());
        }

        if (col.gameObject.tag == "Bomb")
        {
            dropGoldVFX.Play();
            AudioController.Instance.PlaySFX("explosion", 0.4f);
            col.gameObject.GetComponentInChildren<ParticleSystem>().Play();
            coinCollected -= 10;
            if (coinCollected <= 0)  //No coin left will take life instead
            {
                isDead = true;
                PlayerDieLogic();
                SaveScore();
            }
        }
        if (col.gameObject.tag == "Trap") //Collision with trap will not cause destroy object
        {
            return;
        }

        Destroy(col.gameObject);

        

    }

    IEnumerator InvulnerabilityPowerUp()
    {
        isInvulnerable = true;
        invulnerabilityVFX.Play(); // Play invulnerability VFX
        AudioController.Instance.PlaySFX("powerUp_invul");

        // Allow the player to move through obstacles and gaps
        characterController.detectCollisions = false; // Disable collision detection

        float timer = 0f;

        while (timer < invulMaxDuration)
        {
            CollectNearbyCoins(); // Check and collect nearby coins
            timer += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        invulnerabilityVFX.Stop(); // Stop invulnerability VFX
        characterController.detectCollisions = true; // Re-enable collision detection
        isInvulnerable = false; // Reset invulnerability flag
    }

    //Allow Player to collect coin even during Invulnerable period
    void CollectNearbyCoins()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1f); // Adjust the radius as needed
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.tag == "Coin")
            {
                collectCoinVFX.Play();
                AudioController.Instance.PlaySFX("coin");
                coinCollected += 1;
                Destroy(hitCollider.gameObject);
            }

            if (hitCollider.tag == "PU_Speed")
            {
                powerUpVFX.Play();
                AudioController.Instance.PlaySFX("powerUp", 2f);
                playerTrailVFX.Play();
                hitCollider.gameObject.GetComponentInChildren<ParticleSystem>().Play();
                playerSpeed *= 2;
                Destroy(hitCollider.gameObject);
            }

        }
    }

    void SaveScore()
    {
        if (coinCollected > coinCollectedBest)
        {
            coinCollectedBest = coinCollected;
            PlayerPrefs.SetInt("highScoreC", coinCollectedBest);
            PlayerPrefs.Save();
        }

        if (distanceRun > distanceRunBest)
        {
            distanceRunBest = distanceRun;
            PlayerPrefs.SetInt("highScoreD", distanceRunBest);
            PlayerPrefs.Save();
        }
    }

    public void PlayerDieLogic()
    {
        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].SetActive(false);

        }
        AudioController.Instance.PlaySFX("explosion_poof");
        VFXDie.Play();

        //Reduce Player Life on UI
        heartLife -= 1;
        heartLifeVFX.Play(); //VFX on UI
    }

    void InstructionUIToggle()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            instructionUIToggle = !instructionUIToggle;
            if (instructionUIToggle)
            {
                instructionUICanvasGroup.alpha = 1;
            }
            else
            {
                instructionUICanvasGroup.alpha = 0;
            }
        }

    }

    void InitializeUIElements()
    {
        GUIMenu = GameObject.Find("GUIMenu");
        gameOverMenu = GameObject.Find("GameOver");
        respawnButton = GameObject.Find("RespawnButton");
        nextLevelButton = GameObject.Find("NextLevelButton");
        instructionUICanvasGroup = GameObject.Find("InstructionUI").GetComponent<CanvasGroup>();
        distanceScoreText = GameObject.Find("DistanceScoreText").GetComponent<TextMeshProUGUI>();
        coinScoreText = GameObject.Find("CoinScoreText").GetComponent<TextMeshProUGUI>();
        heartLifeText = GameObject.Find("HeartLifeText").GetComponent<TextMeshProUGUI>();
        timeRemainText = GameObject.Find("TimeRemainText").GetComponent<TextMeshProUGUI>();
        heartLifeVFX = GameObject.Find("HeartPoof").GetComponent<ParticleSystem>();
    }

    void UpdateUI()
    {
        distanceScoreText.text = distanceRun.ToString();
        coinScoreText.text = "x" + coinCollected.ToString();
        heartLifeText.text = "x" + heartLife.ToString();
        timeRemainText.text = "x" + (int)timeRemain;
    }
}

#endregion

