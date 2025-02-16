using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCredits : MonoBehaviour
{
    public GameObject mainMenu;   // Assign the main menu panel in the Inspector
    public GameObject creditsPanel; // Assign the credits panel in the Inspector

    // Function to show credits
    public void ShowCreditsPanel()
    {
        mainMenu.SetActive(false);      // Disable the main menu
        creditsPanel.SetActive(true);   // Show the credits panel
    }

    // Function to hide credits and return to the main menu
    public void HideCreditsPanel()
    {
        creditsPanel.SetActive(false);  // Hide the credits panel
        mainMenu.SetActive(true);       // Enable the main menu
    }
}
