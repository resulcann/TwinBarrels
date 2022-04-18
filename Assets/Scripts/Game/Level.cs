using Cinemachine;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName ="New Level", menuName ="Levels", order = 1)]
    public class Level : ScriptableObject
    {
        public GameObject levelPrefab;
        public int levelIndex;
        private GameObject _spawnedLevelPrefab;
        [SerializeField] private Transform player;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        public void CreateLevel()
        {
            _spawnedLevelPrefab = Instantiate(levelPrefab) as GameObject;
            virtualCamera.Follow = player;
            virtualCamera.LookAt = player;
        }

        public void DestroyLevel()
        {
            DestroyImmediate(_spawnedLevelPrefab);
        }
    }
}
