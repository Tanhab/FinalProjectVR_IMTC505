using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GraphManager : MonoBehaviour
{
    [SerializeField] private AnimalManager animalManager; // Reference to the AnimalManager script
    [SerializeField] private TMP_Text feedbackText;       // Feedback text to display messages
    [SerializeField] private List<Button> animalButtons;  // List of buttons representing animals on the graph
    [SerializeField] private List<GameObject> foodChainLines;
    [SerializeField] private Slider progressSlider;       // Reference to the Slider UI component
    [SerializeField] private TMP_Text sliderText;         // Reference to the TMP_Text displaying progress
    [SerializeField] private GameObject foodChainFeedbackCanvas;
    [SerializeField] private TMP_Text feedbackFoodChainText;
    [SerializeField] private Button feedbackCanvasCloseButton;
    [SerializeField] private GameObject borderBlockCanvas; 
    private int foodChainCompletedCount = 0;              // Counter for successful food chains
    private int totalFoodChains = 14;                     // Total number of food chains
    
    
    private Dictionary<string, GameObject> arrowMap = new Dictionary<string, GameObject>();
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

    private Dictionary<string, string> remarksText = new Dictionary<string, string>()
    {
        {
            "eagle_snake",
            "The bald eagle, with its sharp talons and keen eyesight, occasionally preys on rattlesnakes. By controlling snake populations, the eagle ensures that the balance in the food web is maintained, while the rattlesnake offers the eagle a unique meal that provides important nutrients. "
        },
        {
            "eagle_mouse",
            "The rattlesnake, a stealthy predator, is highly skilled at ambushing small prey like mice. Using its venomous bite to immobilize its target, the snake helps regulate rodent populations, which in turn protects plant life from overgrazing. "
        },
        {
            "eagle_trout",
            "The bald eagle relies on the river’s fish, particularly trout, as a primary food source. With its sharp vision, the eagle spots its prey from high above, swooping down to catch fish, which is a critical part of the eagle's diet and its role in regulating fish populations in the ecosystem. "
        },
        {
            "snake_rat",
            "The rattlesnake, a stealthy predator, is highly skilled at ambushing small prey like mice. Using its venomous bite to immobilize its target, the snake helps regulate rodent populations, which in turn protects plant life from overgrazing. "
        },
        {
            "bear_trout",
            "The black bear, a skilled fisher, wades into rivers and streams to catch trout. This seasonal feast provides the bear with essential protein and fat, while the bear's leftovers enrich the forest floor, nourishing plants and insects. "
        },
        {
            "bear_bee",
            "Drawn to the sweet rewards of a wild honeybee's hive, the black bear is an occasional raider. By breaking open hives, the bear spreads honey and wax, which can benefit other creatures, though it disrupts the bee colony's hard work. "
        },
        {
            "bear_bush",
            "The black bear has a sweet tooth for lowbush blueberries, which ripen during the summer and fall. These berries provide a crucial source of energy and nutrients, especially as the bear prepares for hibernation. By consuming large quantities of blueberries, the bear also aids in seed dispersal, as seeds pass through its digestive system and are deposited across the forest floor, helping new bushes grow and sustain the ecosystem. "
        },
        {
            "bear_rat",
            "The opportunistic black bear may occasionally encounter the western harvest mouse while foraging. Though not a primary food source, the mouse provides a small but valuable protein boost, especially during times when other food sources are scarce. The bear’s digging and foraging activities, in turn, disturb the soil, creating opportunities for seed dispersal and plant growth, which benefit the harvest mouse’s habitat. "
        },
        {
            "coyote_deer",
            "The coyote, a cunning predator, often preys on young or weakened deer. By targeting these individuals, the coyote plays a critical role in maintaining a healthy deer population and preventing overgrazing of vegetation. "
        },
        {
            "coyote_rat",
            "The coyote often hunts mice, which are an easy and plentiful food source. This predation helps keep rodent populations in check, reducing competition for seeds and grasses among other animals "
        },
        {
            "coyote_snake",
            "Though uncommon, the coyote occasionally preys on rattlesnakes, particularly if the snake is small or injured. This opportunistic behavior shows the coyote's adaptability and its role as a regulator in the ecosystem."
        },
        {
            "bee_bush",
            "The wild honeybee dances among the blossoms of a bush, collecting nectar and pollen. In doing so, the bee pollinates the bush's flowers, enabling the production of seeds or fruits, which sustain other creatures in the ecosystem."
        },
        {
            "rat_bush",
            "The mouse is both a helper and a consumer of the blueberry bush. It eats the berries for nourishment, scattering seeds in its droppings, which helps propagate new bushes across the forest. "
        },
        {
            "deer_bush",
            "The deer feed on the foliage and fruits of the blueberry bush, helping to disperse the seeds through their droppings. This benefits the blueberry bush by promoting its spread across the forest floor."
        }
    }; 

    private Button selectedPredator = null;  // Currently selected predator button
    private Color selectedColor = new Color(131f / 255f, 254f / 255f, 94f / 255f, 1f); // Predator highlight color
    private Color defaultColor = Color.white;

    private void Start()
    {
        foreach (GameObject line in foodChainLines)
        {
            string arrowName = line.name.ToLower(); // Use the name (e.g., "coyote_deer")
            arrowMap[arrowName] = line;

            // Ensure all lines are initially inactive
            line.SetActive(false);
        }
        // Initialize the button map based on GameObject names
        foreach (Button button in animalButtons)
        {
            string buttonName = button.gameObject.name.ToLower();
            Debug.Log("Creating dictionary entry for: " + buttonName);
            animalButtonMap[buttonName] = button;

            // Add click listeners to the buttons
            button.onClick.AddListener(() => OnAnimalButtonClick(button));
        }

        feedbackCanvasCloseButton.onClick.AddListener(CloseFoodChainRemarks);
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

            // Activate the corresponding arrow (predator_prey)
            string arrowKey = $"{predatorName}_{preyName}";
            if (arrowMap.ContainsKey(arrowKey) && !arrowMap[arrowKey].activeSelf)
            {
                arrowMap[arrowKey].SetActive(true);
                Debug.Log($"Activated arrow: {arrowKey}");

                // Update progress only if this connection was not already counted
                foodChainCompletedCount++;
                UpdateProgress();
            }
            
            OpenFoodChainRemarks(arrowKey);

            // Reset selections
            DeselectButton(selectedPredator);
            selectedPredator = null;
        }
        else
        {
            feedbackText.text = $"Invalid food chain! {predatorName} does not depend on {preyName}.";

            // Reset buttons
            DeselectButton(selectedPredator);
            DeselectButton(preyButton);
            selectedPredator = null;
        }
    }

    private void UpdateProgress()
    {
        // Update slider value and text
        progressSlider.value = foodChainCompletedCount;
        sliderText.text = $"Food Chain completed - {foodChainCompletedCount}/{totalFoodChains}";

        // Check if all food chains are completed
        if (foodChainCompletedCount >= totalFoodChains)
        {
            feedbackText.text = "All food chains are restored! Loading the final scene...";
            LoadEndScene();
        }
    }

    private void LoadEndScene()
    {
        SceneManager.LoadScene("EndScene"); // Replace "EndScene" with your actual end scene name
    }

    private void OpenFoodChainRemarks(string pred_prey )
    {
        if (remarksText.ContainsKey(pred_prey))
        {
            borderBlockCanvas.gameObject.SetActive(true);
            foodChainFeedbackCanvas.gameObject.SetActive(true);
            feedbackFoodChainText.text = remarksText[pred_prey];
        }
    }

    private void CloseFoodChainRemarks()
    {
        borderBlockCanvas.gameObject.SetActive(false);
        foodChainFeedbackCanvas.gameObject.SetActive(false);
        feedbackFoodChainText.text = "";
        ShowDefaultText();
    }

}
