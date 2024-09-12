using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class LevelSystem : MonoBehaviour
{
    public int currentLevel = 1;
    public int currentExperience = 0;
    public int experienceToNextLevel = 100;
    public Slider experienceBar;

    public void AddExperience(int amount)
    {
        currentExperience += amount;
        experienceBar.value = (float)currentExperience / experienceToNextLevel;

        if (currentExperience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExperience -= experienceToNextLevel;
        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.5f); // Augmentation exponentielle de l'XP nécessaire
        UnityEngine.Debug.Log("Level Up! Current Level: " + currentLevel);
    }
}
