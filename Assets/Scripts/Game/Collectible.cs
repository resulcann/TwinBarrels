using UnityEngine;

namespace Game
{
    public class Collectible : MonoBehaviour
    {
        private bool _isCollected;

        private void Start()
        {
            _isCollected = false;
        }
    

        public bool GetIsCollected()
        {
            return _isCollected;
        }

        public void SetCollected()
        {
            _isCollected = true;
            transform.tag = "Collected";
        }
    
    }
}
