using UnityEngine;

namespace Game
{
    public class LootCalculator : MonoBehaviour
    {
        private float _counter = 1;
        private GameManager _gameManager;
        public Collector collector;

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("FinishStairs")) return;
            if (collector.barrelList.Count > 1) _counter += 0.25f;
            if (collector.barrelList.Count != 1) return;
            _gameManager.diamondScore *= _counter;
            var tempScore = (int)(_gameManager.diamondScore);
            _gameManager.scoreText.text = tempScore.ToString();
            
        }
    }
}
