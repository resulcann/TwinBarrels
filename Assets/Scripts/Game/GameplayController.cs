using System;
using MoreMountains.NiceVibrations;
using UnityEngine;

namespace Game
{
    public class GameplayController : MonoBehaviour
    {
        public static GameplayController Instance { get; private set; }
        public bool IsActive { get; set; }

        public Action<bool> OnGameplayFinished;

        private void Awake()
        {
            Instance = this;
        }

        public void StartGameplay()
        {
            IsActive = true;
        }

        public void RetryGameplay()
        {
            IsActive = true;
        }

        public void FinishGameplay(bool success)
        {
            IsActive = false;

            var hapticType = success ? HapticTypes.Success : HapticTypes.Failure;

            MMVibrationManager.Haptic(hapticType);
            OnGameplayFinished?.Invoke(success);
        }

        private void Update()
        {
            if (!IsActive)
                return;

            if (Input.GetKeyDown(KeyCode.U))
            {
                FinishGameplay(true);
            }
            else if (Input.GetKeyDown(KeyCode.J))
            {
                FinishGameplay(false);
            }
        }
    }
}
