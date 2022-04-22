using System.Collections.Generic;
using Game;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public List<Level> Levels;
    public int currentLevelIndex = 1;
    

    private void Awake() 
    {
        Instance = this;
    }

    private void Start() 
    {
        SpawnCurrentLevel();
        
    }

    private void SpawnCurrentLevel()
    {
        Levels[currentLevelIndex].CreateLevel();
    }

    public void NextLevel()
    {
        Levels[currentLevelIndex].DestroyLevel();
        Levels[currentLevelIndex + 1].CreateLevel();
    }
    public void RetryLevel()
    {
        Levels[currentLevelIndex].DestroyLevel();
        Levels[currentLevelIndex].CreateLevel();
    }
    
}