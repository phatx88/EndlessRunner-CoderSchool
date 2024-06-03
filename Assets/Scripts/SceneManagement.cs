using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneManagement : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI bestDistanceText;
    [SerializeField] public TextMeshProUGUI maxCoinsText;
    [SerializeField] private TMP_SpriteAsset coinSpriteAsset; // Reference to the sprite asset
    [SerializeField] public Animator transition;
    [SerializeField] public Animator hideMenuUi;

    [SerializeField] public float transtionTime = 1f;

    private PlayerController playerController;

    void Start()
    {
        // Initialize text with stored high scores or default values
        bestDistanceText.text = "Best Running Distance Score: " + PlayerPrefs.GetInt("highScoreD", 0).ToString() + "M";

        // Get the max coins value and append the coin sprite
        int maxCoins = PlayerPrefs.GetInt("highScoreC", 0);
        maxCoinsText.text = "Max. Coin Collected: " + maxCoins.ToString() + " <sprite name=GoldCoin>";

        // Ensure the sprite asset is assigned to the TextMeshProUGUI component
        maxCoinsText.spriteAsset = coinSpriteAsset;

        // Find the Player GameObject and get the PlayerController component
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }
    }

    void Update()
    {
        if (playerController != null)
        {
            // Update Message when Player is Dead
            if (playerController.isDead)
            {
                // Initialize text with stored high scores or default values
                bestDistanceText.text = "Best Running Distance Score: " + PlayerPrefs.GetInt("highScoreD", 0).ToString() + "M";

                // Get the max coins value and append the coin sprite
                int maxCoins = PlayerPrefs.GetInt("highScoreC", 0);
                maxCoinsText.text = "Max. Coin Collected: " + maxCoins.ToString() + " <sprite name=GoldCoin>";
            }
        }
    }

    public void ToGame()
    {
        SceneManager.LoadScene("Level_01");
        //StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1 ));
    }

    public void ToMenu()
    {
        SceneManager.LoadScene("Menu");
        //StartCoroutine(LoadLevel(0));
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if the next scene index is within the build settings range
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
            //StartCoroutine(LoadLevel(nextSceneIndex));
        }
        else
        {
            Debug.Log("No more levels to load!");
        }
    }

    IEnumerator LoadLevel(int SceneIndex)
    {
        if(transition != null) { transition.SetTrigger("End"); }
           

        if(hideMenuUi != null) { hideMenuUi.SetTrigger("HideMenuUi"); }
            

        yield return new WaitForSeconds(transtionTime);

        SceneManager.LoadSceneAsync(SceneIndex);

        if (transition != null) { transition.SetTrigger("Start"); }
            
    }

}
