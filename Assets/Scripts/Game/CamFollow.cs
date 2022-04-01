using UnityEngine;

namespace Game
{
    public class CamFollow : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset;
    

        private void LateUpdate()
        {
            transform.position = target.position + offset;

        }
    
    }
}
