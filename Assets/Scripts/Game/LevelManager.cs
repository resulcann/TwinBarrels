using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }
        public List<Level> Levels;
        public int currentLevelIndex = 0;
        private bool _isRandom;

        private void Awake() 
        {
            Instance = this;
            _isRandom = false;
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
            if (_isRandom == false)
            {
                Levels[currentLevelIndex].DestroyLevel();
                currentLevelIndex++;
                Levels[currentLevelIndex].CreateLevel();
                if (currentLevelIndex == 4) _isRandom = true;
            }
            else
            {
                var randomValue = Random.Range(0, 5);
                Levels[currentLevelIndex].DestroyLevel();
                while (randomValue == currentLevelIndex) randomValue = Random.Range(0, 5);
                currentLevelIndex = randomValue;
                Levels[currentLevelIndex].CreateLevel();
            }
        
        }
        public void RetryLevel()
        {
            Levels[currentLevelIndex].DestroyLevel();
            Levels[currentLevelIndex].CreateLevel();
        }
    
    }
}