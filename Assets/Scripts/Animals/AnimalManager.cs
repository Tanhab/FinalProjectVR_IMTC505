using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Animal> animals = new List<Animal>();


    public Animal GetAnimalByName(string name)
    {
        foreach (Animal animal in animals)
        {
            if (animal.animalName.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return animal;
            }
        }
        return null; 
    }

    public void UpdateDiscoveredAnimal(string name)
    {
        // Attempt to find the animal by name in the list
        Animal animal = animals.Find(a => a.animalName.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (animal != null)
        {
            // Update the isDiscovered status
            animal.isDiscovered = true;
            Debug.Log($"Animal {name} marked as discovered.");
        }
        else
        {
            Debug.LogWarning($"Animal {name} not found in the list.");
        }
    }
}
