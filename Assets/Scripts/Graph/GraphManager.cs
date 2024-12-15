using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphManager : MonoBehaviour
{
    [SerializeField] private AnimalManager animalManager; // Reference to the AnimalManager script
    [SerializeField] private List<Button> animalButtons;  // List of buttons representing animals on the graph

    // Map button names (GameObject names) to their corresponding buttons
    private Dictionary<string, Button> animalButtonMap = new Dictionary<string, Button>();

    private void Start()
    {
        // Initialize the button map based on GameObject names
        foreach (Button button in animalButtons)
        {
            string buttonName = button.gameObject.name; // Use the GameObject name as the key
            
            buttonName = buttonName.ToLower();
            Debug.Log("creating dictionary for" + buttonName);
            animalButtonMap[buttonName] = button;

            // Ensure the button text is initially "Unknown"
            //button.GetComponentInChildren<Text>().text = "Unknown";
        }
    }

    public void UpdateGraphButtonText(string animalName)
    {
        Debug.Log($"Updating button text for discovered animal: {animalName}");

        // Find the animal using AnimalManager
        Animal discoveredAnimal = animalManager.GetAnimalByName(animalName);

        if (discoveredAnimal != null && discoveredAnimal.isDiscovered)
        {
            // Check if the button exists for the animal
            if (animalButtonMap.TryGetValue(animalName, out Button button))
            {
                // Update the button text and make any visual changes
                button.GetComponentInChildren<TMP_Text>().text = discoveredAnimal.animalName;
                
                // Optionally update the button's color or sprite
                //Color color = new Color(131f, 254f, 94f, 255f);
                button.image.color = Color.white; // Mark discovered animals in green
                
                Debug.Log($"Button text updated for animal: {animalName}");
            }
            else
            {
                Debug.LogWarning($"No button found for animal: {animalName}");
            }
        }
        else
        {
            Debug.LogWarning($"Animal {animalName} is either not found or not discovered yet.");
        }
    }
}
