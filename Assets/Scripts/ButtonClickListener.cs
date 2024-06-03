using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ButtonClickListener : MonoBehaviour
{

    void Start()
    {
        // Find all GameObjects tagged "Button"
        GameObject[] buttonObjects = GameObject.FindGameObjectsWithTag("Button");

        // Loop through each button and add the click event listener
        foreach (GameObject buttonObject in buttonObjects)
        {
            Button button = buttonObject.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(OnButtonClick);
            }
            else
            {
                Debug.LogWarning("GameObject tagged 'Button' does not have a Button component.");
            }
        }

    }

    void OnButtonClick()
    {
        // Play the click sound
        AudioController.Instance.PlaySFX("buttonClick");
    }
}
