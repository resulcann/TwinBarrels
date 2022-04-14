using Dreamteck.Splines;
using Magiclab.Utility.GenericUtilities;
using UnityEngine;

namespace Game
{
    public class Player : MonoBehaviour
    {
        [Header("Movement")] 
        [SerializeField] private float _dragMultiplier = 5f;
        [SerializeField] private Vector2 _leftBarrelBounds, _rightBarrelBounds;

        private float _mouseStartPos, _mouseCurrentPos, _playerCurrentPos, _leftBarrelCurrentPos, _rightBarrelCurrentPos;
        public Transform leftBarrel, rightBarrel;
        public GameplayController gameplayController;
        private SplineFollower _splineFollower;
        

        void Awake()
        {
            _splineFollower = GetComponentInParent<SplineFollower>();
        }
        private void Start()
        {
            _leftBarrelCurrentPos = transform.localPosition.x;
            _rightBarrelCurrentPos = transform.localPosition.x;
            _mouseStartPos = Input.mousePosition.x;
            _mouseStartPos /= Screen.width;
            
        }

        private void Update()
        {
            _splineFollower.follow = gameplayController.IsActive;
            if (gameplayController.IsActive == false) return;
            
            HandLeMovement();
        }

        private void HandLeMovement()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _leftBarrelCurrentPos = leftBarrel.localPosition.x;
                _rightBarrelCurrentPos = rightBarrel.localPosition.x;
                _mouseStartPos = Input.mousePosition.x;
                _mouseStartPos /= Screen.width;
            
            }

            if (InputHandler.GetMouseButton())
            {
                _mouseCurrentPos = InputHandler.GetMousePosition().x;
                _mouseCurrentPos /= Screen.width;
                
                var leftBarrelTargetPos = _leftBarrelCurrentPos + (_mouseCurrentPos - _mouseStartPos) * _dragMultiplier;
                var leftBarrelTransform = leftBarrel.transform;
                var leftBarrelLocalPos = leftBarrel.localPosition;
                
                var rightBarrelTargetPos = _rightBarrelCurrentPos + ( - _mouseCurrentPos + _mouseStartPos) * _dragMultiplier;
                var rightBarrelTransform = rightBarrel.transform;
                var rightBarrelLocalPos = rightBarrel.localPosition;
                
                leftBarrelTargetPos = Mathf.Clamp(leftBarrelTargetPos, _leftBarrelBounds.x, _leftBarrelBounds.y);
                rightBarrelTargetPos = Mathf.Clamp(rightBarrelTargetPos, _rightBarrelBounds.x, _rightBarrelBounds.y);
                
                leftBarrelLocalPos = new Vector3(leftBarrelTargetPos, leftBarrelLocalPos.y, leftBarrelLocalPos.z);
                leftBarrelTransform.localPosition = leftBarrelLocalPos;
                
                rightBarrelLocalPos = new Vector3(rightBarrelTargetPos, rightBarrelLocalPos.y, rightBarrelLocalPos.z);
                rightBarrelTransform.localPosition = rightBarrelLocalPos;

                
            }
        
            
        }

        public void SpeedUp()
        {
            _splineFollower.followSpeed = 8f;
        }
        public void SpeedDown()
        {
            _splineFollower.followSpeed = 5f;
        }
    }
}