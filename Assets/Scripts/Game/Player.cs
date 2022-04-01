using Magiclab.Utility.GenericUtilities;
using UnityEngine;

namespace Game
{
    public class Player : MonoBehaviour
    {
        [Header("Movement")] 
        [SerializeField] private float dragMultiplier = 2f;
        [SerializeField] private Vector2 leftBarrelBounds;
        [SerializeField] private Vector2 rightBarrelBounds;

        private float _mouseStartPos;
        private float _mouseCurrentPos;
        private float _playerCurrentPos;
        private float _leftBarrelCurrentPos;
        private float _rightBarrelCurrentPos;

        public Transform leftBarrel;
        public Transform rightBarrel;


        private void Start()
        {
            _leftBarrelCurrentPos = transform.localPosition.x;
            _rightBarrelCurrentPos = transform.localPosition.x;
            _mouseStartPos = Input.mousePosition.x;
            _mouseStartPos /= Screen.width;
        }


        private void Update()
        {
            HandLeMovement();
        }

        private void HandLeMovement()
        {
        if (Input.GetMouseButtonDown(0))
        {
            _leftBarrelCurrentPos = transform.localPosition.x;
            _rightBarrelCurrentPos = transform.localPosition.x;
            _mouseStartPos = Input.mousePosition.x;
            _mouseStartPos /= Screen.width;
        
        }
        //
        //     if (!Input.GetMouseButton(0)) return;
        //
        //     _mouseCurrentPos = Input.mousePosition.x;
        //     _mouseCurrentPos /= Screen.width;
        //     var targetPos = _playerCurrentPos + (_mouseCurrentPos - _mouseStartPos) * dragMultiplier;
        //     targetPos = Mathf.Clamp(targetPos, bounds.x, bounds.y);
        //
        //     var transform1 = transform;
        //     var localPosition = transform1.localPosition;
        //     localPosition = new Vector3(targetPos, localPosition.y, localPosition.z);
        //     transform1.localPosition = localPosition;

        if (InputHandler.GetMouseButton())
        {
            _mouseCurrentPos = InputHandler.GetMousePosition().x;
            _mouseCurrentPos /= Screen.width;
            
            var leftBarrelTargetPos = _leftBarrelCurrentPos + (_mouseCurrentPos - _mouseStartPos) * dragMultiplier;
            var leftBarrelTransform = leftBarrel.transform;
            var leftBarrelLocalPos = leftBarrel.localPosition;
            
            var rightBarrelTargetPos = _rightBarrelCurrentPos + ( - _mouseCurrentPos + _mouseStartPos) * dragMultiplier;
            var rightBarrelTransform = rightBarrel.transform;
            var rightBarrelLocalPos = rightBarrel.localPosition;
            
            leftBarrelTargetPos = Mathf.Clamp(leftBarrelTargetPos, leftBarrelBounds.x, leftBarrelBounds.y);
            rightBarrelTargetPos = Mathf.Clamp(rightBarrelTargetPos, rightBarrelBounds.x, rightBarrelBounds.y);
            
            leftBarrelLocalPos = new Vector3(leftBarrelTargetPos, leftBarrelLocalPos.y, leftBarrelLocalPos.z);
            leftBarrelTransform.localPosition = leftBarrelLocalPos;
            
            rightBarrelLocalPos = new Vector3(rightBarrelTargetPos, rightBarrelLocalPos.y, rightBarrelLocalPos.z);
            rightBarrelTransform.localPosition = rightBarrelLocalPos;

            
            
            

            
        }
        
            
        }
    }
}