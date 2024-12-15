using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphManager : MonoBehaviour
{
    [SerializeField] private AnimalManager animalManager; // Reference to the AnimalManager script
    [SerializeField] private List<Button> animalButtons;  // List of buttons representing animals on the graph
    [SerializeField] private TMP_Text feedbackText;       // Feedback text to display messages

    private Dictionary<string, Button> animalButtonMap = new Dictionary<string, Button>();
    Dictionary<string, List<string>> foodChain = new Dictionary<string, List<string>>()
    {
        { "coyote", new List<string> { "deer", "rat", "snake" } },
        { "eagle", new List<string> { "snake", "rat", "trout" } },
        { "bear", new List<string> { "trout", "bee", "bush", "rat" } },
        { "snake", new List<string> { "rat" } },
        { "trout", new List<string> {  } },
        { "bush", new List<string> {  } },
        { "rat", new List<string> { "bush" } },
        { "deer", new List<string>{"bush"}},
        { "bee", new List<string>{"bush"}},
        
    };

    private Button selectedPredator = null;  // Currently selected predator button
    private Color selectedColor = new Color(131f / 255f, 254f / 255f, 94f / 255f, 1f); // Predator highlight color
    private Color defaultColor = Color.white;

    private void Start()
    {
        // Initialize the button map based on GameObject names
        foreach (Button button in animalButtons)
        {
            string buttonName = button.gameObject.name.ToLower();
            Debug.Log("Creating dictionary entry for: " + buttonName);
            animalButtonMap[buttonName] = button;

            // Add click listeners to the buttons
            button.onClick.AddListener(() => OnAnimalButtonClick(button));
        }
        ShowDefaultText();
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

    private void OnAnimalButtonClick(Button clickedButton)
    {
        string animalName = clickedButton.gameObject.name.ToLower();
        Animal clickedAnimal = animalManager.GetAnimalByName(animalName);

        // Case 1: Undiscovered animal
        if (clickedAnimal == null || !clickedAnimal.isDiscovered)
        {
            feedbackText.text = $"This animal is still undiscovered!";
            if (selectedPredator != null)
            {
                DeselectButton(selectedPredator);
                selectedPredator = null;
            }

            return;
        }

        // Case 2: Click the same button again to deselect
        if (clickedButton == selectedPredator)
        {
            DeselectButton(clickedButton);
            feedbackText.text = "You have deselected the predator. Select a discovered animal as Predator.";
            selectedPredator = null;
            return;
        }

        // Case 3: Select a predator if none is selected
        if (selectedPredator == null)
        {
            SelectPredator(clickedButton, clickedAnimal);
            return;
        }

        // Case 4: Check if a valid food chain exists
        CheckFoodChain(clickedButton, clickedAnimal);
    }

    private void SelectPredator(Button predatorButton, Animal predatorAnimal)
    {
        selectedPredator = predatorButton;
        selectedPredator.image.color = selectedColor;
        feedbackText.text = $"You have selected {predatorAnimal.animalName} as your Predator. Now select the prey to complete the food chain.";
    }

    private void ShowDefaultText()
    {
        feedbackText.text = "Select a discovered animal as Predator";
    }

    private void DeselectButton(Button button)
    {
        button.image.color = defaultColor;
    }

    private void CheckFoodChain(Button preyButton, Animal preyAnimal)
    {
        string predatorName = selectedPredator.gameObject.name.ToLower();
        string preyName = preyButton.gameObject.name.ToLower();

        

        if (foodChain.ContainsKey(predatorName) && foodChain[predatorName].Contains(preyName))
        {
            feedbackText.text = $"Success! {predatorName} depends on {preyName} in the food chain.";
            // Optionally mark connection visually here
            DeselectButton(selectedPredator);
            selectedPredator = null;
            
        }
        else
        {
            feedbackText.text = $"Invalid food chain! {predatorName} does not depend on {preyName}.";
            // Reset both buttons
            DeselectButton(selectedPredator);
            DeselectButton(preyButton);
            selectedPredator = null;
        }
    }
}
