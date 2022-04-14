using UnityEngine;

namespace Game
{
    public class CamFollow : MonoBehaviour
    {
        public Transform target;

        private void LateUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, target.position, 0.25f);
        }
    }
}
