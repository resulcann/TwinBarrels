using System;
using System.Collections.Generic;
using Dreamteck.Splines;
using Sirenix.Utilities;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Game
{
    public class Collector : MonoBehaviour
    {
        public GameplayController gameplayController;
        public SplineFollower splineFollower;
        public Barrel barrel, rootBarrel;
        public List<Barrel> barrelList;
        public TextMeshProUGUI scoreText;
        public Transform trailPos, cameraFollowTarget;
        private void Awake()
        {
            barrelList.Add(rootBarrel);
        }
        private void Update()
        {
            if (!gameplayController.IsActive) return;
            
            SetPositions();
            
        }

        private void LateUpdate()
        {
            cameraFollowTarget.localPosition = Vector3.Lerp(cameraFollowTarget.localPosition ,new Vector3(0f, transform.parent.localPosition.y, 1f), .25f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Collectible"))
            {
                var childCount = other.gameObject.transform.childCount;
            
                other.gameObject.GetComponent<Collectible>().SetCollected();
                Destroy(other.gameObject);

                if (childCount > 0)
                {
                    for (var i = 0; i < childCount + 1; i++)
                    {
                        var barrelGo = Instantiate(barrel, rootBarrel.transform.parent, true);
                        barrelGo.GetComponent<Barrel>().collector = GetComponent<Collector>();
                        barrelList.Add(barrelGo);
                    }
                }
                else
                {
                    var barrelGo = Instantiate(barrel, rootBarrel.transform.parent, true);
                    barrelGo.GetComponent<Barrel>().collector = GetComponent<Collector>();
                    barrelList.Add(barrelGo);
                }
            }

            if (other.gameObject.CompareTag("Diamond"))
            {
                Destroy(other.gameObject);
                GameManager.Instance.diamondScore++;
                scoreText.text = GameManager.Instance.diamondScore.ToString();
            }
        
            if (other.gameObject.CompareTag("Enemy"))
            {
                splineFollower.follow = false;
                gameplayController.FinishGameplay(false);
            }

            if (other.gameObject.CompareTag("Stairs"))
            {
                if (barrelList.Count > 1)
                {
                    transform.parent.localPosition = new Vector3(transform.parent.localPosition.x,
                        transform.parent.localPosition.y + 0.5f, transform.parent.localPosition.z);
                }
                else
                {
                    gameplayController.FinishGameplay(false);
                    splineFollower.follow = false;
                }
                
            }

            if (other.gameObject.CompareTag("Finish"))
            {
                gameplayController.FinishGameplay(true);
                splineFollower.follow = false;
            }

            if (other.gameObject.CompareTag("SpeedUp")) splineFollower.followSpeed = 10f;
            
            if (other.gameObject.CompareTag("SpeedDown")) splineFollower.followSpeed = 6f;
                
        }

        private void SetPositions()
        {
            var posY = 0.5f;

            for (var i = 1; i < barrelList.Count; i++)
            {
                rootBarrel.transform.localPosition = new Vector3(rootBarrel.transform.localPosition.x,(barrelList.Count * posY) - posY,
                    rootBarrel.transform.localPosition.z);
                
                barrelList[i].transform.localPosition = new Vector3(rootBarrel.transform.localPosition.x,
                    (i * posY) - posY,
                    rootBarrel.transform.localPosition.z);
            }
        }

    }
}
