using System.Collections.Generic;
using Dreamteck.Splines;
using TMPro;
using UnityEngine;

namespace Game
{
    public class Collector : MonoBehaviour
    {
        public GameplayController gameplayController;
        public SplineFollower splineFollower;
        public GameObject barrel;
        public Transform rootBarrel;
        public List<GameObject> barrelList;
        public TextMeshProUGUI scoreText;
        private void Awake()
        {
            barrelList.Add(rootBarrel.gameObject);
        }

        private void Update()
        {
            splineFollower.follow = gameplayController.IsActive;
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
                        var barrelGo = Instantiate(barrel, rootBarrel.parent, true) as GameObject;
                        barrelList.Add(barrelGo);
                    }
                }
                else
                {
                    var barrelGo = Instantiate(barrel, rootBarrel.parent, true) as GameObject;
                    barrelList.Add(barrelGo);
                }
            
                SetPositions();
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
        }

        private void SetPositions() //pozisyonlandırma doğru çalışmıyor ayarla..
        {
            var posY = 0.5f;
        
            for (var i = 1; i < barrelList.Count; i++)
            {
                rootBarrel.transform.localPosition = new Vector3(rootBarrel.transform.localPosition.x, (barrelList.Count * posY) - posY,
                    rootBarrel.transform.localPosition.z);
            
                barrelList[i].transform.localPosition = new Vector3(rootBarrel.transform.localPosition.x,
                    (i * posY) - posY,
                    rootBarrel.transform.localPosition.z);
            }
        }

    }
}
