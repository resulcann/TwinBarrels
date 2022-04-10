using UnityEngine;

namespace Game
{
    public class CamFollow : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset;


        private void LateUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, target.position + offset, 0.25f);
            transform.LookAt(target);

        }
    }
}
