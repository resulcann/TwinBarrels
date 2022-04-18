using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public List<Level> Levels;
    public int currentLevelIndex = 0;

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
        if(currentLevelIndex == 2)
        {
            currentLevelIndex = Random.Range(0,2);
        }
        else{
            currentLevelIndex += 1;
        }
        
        Levels[currentLevelIndex].CreateLevel();
        
    }
    public void RetryLevel()
    {
        Levels[currentLevelIndex].DestroyLevel();
        Levels[currentLevelIndex].CreateLevel();
        
    }
    
}