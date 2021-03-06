using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
using DG.Tweening;
using Magiclab.Utility.GenericUtilities;

namespace Game
{
    public class Collector : MonoBehaviour
    {
        public List<GameObject> barrelList;
        [SerializeField] private Transform  cameraFollowTarget;
        [SerializeField] private GameObject barrel, rootBarrel, brokenBarrel, brokenBox, brokenThorn;
        [SerializeField] private Material greenMat;
        [SerializeField] private SplineFollower splineFollower;

        private void Awake()
        {
            barrelList.Add(rootBarrel);
        }

        private void Update()
        {
            if (!GameplayController.Instance.IsActive) return;
            
            splineFollower.follow = GameplayController.Instance.IsActive;
            SetPositions();

        }

        private void LateUpdate()
        {
            cameraFollowTarget.localPosition = Vector3.Lerp(cameraFollowTarget.localPosition,
                new Vector3(0f, transform.parent.localPosition.y, 1f), .25f);
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
                        barrelList.Add(barrelGo);
                    }
                }
                else
                {
                    var barrelGo = Instantiate(barrel, rootBarrel.transform.parent, true);
                    barrelList.Add(barrelGo);
                }
            }

            if (other.gameObject.CompareTag("Diamond"))
            {
                Destroy(other.gameObject);
                GameManager.Instance.diamondScore++;
                var tempScore = (int)(GameManager.Instance.diamondScore);
                GameManager.Instance.scoreText.text = tempScore.ToString();
            }    

            if (other.gameObject.CompareTag("EnemyBox"))
            {
                if (barrelList.Count == 1)
                {
                    FinishFailed();
                }
                else
                {
                    Explosion(other.gameObject.transform, brokenBox);
                }

            }
            if (other.gameObject.CompareTag("EnemyThorn"))
            {
                if (barrelList.Count == 1)
                {
                    FinishFailed();
                }
                else
                {
                    Explosion(other.gameObject.transform, brokenThorn);
                }
            }

            if (other.gameObject.CompareTag("Stairs"))
            {
                if (barrelList.Count > 1)
                { 
                    var lowerBarrel = barrelList[1];
                    lowerBarrel.GetComponentInChildren<Animation>().enabled = false;
                    lowerBarrel.transform.rotation = new Quaternion(0f,0f,0f,0f);
                    lowerBarrel.transform.parent = transform.root;
                    barrelList.Remove(lowerBarrel);
                    other.GetComponent<MeshRenderer>().material = greenMat;

                    transform.parent.localPosition += Vector3.up/2;
                }
                else
                {
                    FinishFailed();
                }
            }

            if (other.gameObject.CompareTag("FinishStairs"))
            {
                if (barrelList.Count > 1)
                { 
                    var lowerBarrel = barrelList[1];
                    AnimationOff(lowerBarrel.transform);
                    lowerBarrel.transform.parent = transform.root;
                    barrelList.Remove(lowerBarrel);
                    transform.parent.localPosition += Vector3.up/2;
                }
                else
                {
                    GameplayController.Instance.FinishGameplay(true);
                    splineFollower.follow = false;
                    AnimationOff(rootBarrel.transform);
                }
            }

            if (other.gameObject.CompareTag("Finish"))
            {
                splineFollower.follow = false;
                GameplayController.Instance.FinishGameplay(true);
                AnimationOff(rootBarrel.transform);
            }

            if (other.gameObject.CompareTag("SpeedUp"))
                splineFollower.followSpeed = Mathf.Lerp(splineFollower.followSpeed, 10, 1f);

            if (other.gameObject.CompareTag("SpeedDown"))
                splineFollower.followSpeed = Mathf.Lerp(splineFollower.followSpeed, 6, 1f);

        }

        private void SetPositions()
        {
            const float posY = 0.5f;
            rootBarrel.transform.localPosition = new Vector3(rootBarrel.transform.localPosition.x,
                (barrelList.Count * posY) - posY,
                rootBarrel.transform.localPosition.z);
            
            if (barrelList.Count <= 1) return;
            for (var i = 1; i < barrelList.Count; i++)
            {
                barrelList[i].transform.localPosition = new Vector3(rootBarrel.transform.localPosition.x,
                    (i * posY) - posY,
                    rootBarrel.transform.localPosition.z);
            }
        }

        private void Explosion(Transform collidedBox, GameObject brokenEnemyPrefab)
        {
            var tempBarrel = barrelList[1];

            var root = transform.root;
            var brokenBarrelObject = Instantiate(brokenBarrel, tempBarrel.transform.position + Vector3.up, Quaternion.identity, root) as GameObject;
            var brokenEnemyObject = Instantiate(brokenEnemyPrefab, collidedBox.position + Vector3.up, brokenEnemyPrefab.transform.rotation, root) as GameObject;
            var allBarrelRigidBodies = brokenBarrelObject.GetComponentsInChildren<Rigidbody>();
            var allEnemyRigidBodies = brokenEnemyObject.GetComponentsInChildren<Rigidbody>();
            
            if (allBarrelRigidBodies.Length > 0)
            {
                foreach (var body in allBarrelRigidBodies)
                {
                    body.AddExplosionForce(300, body.transform.localPosition + Vector3.up, 1);
                }

                barrelList.Remove(barrelList[1]);
                Destroy(tempBarrel);
            }

            if (allEnemyRigidBodies.Length > 0)
            {
                foreach (var body in allEnemyRigidBodies)
                {
                    body.AddExplosionForce(300, body.transform.localPosition + Vector3.up, 1);
                }
                Destroy(collidedBox.gameObject);
            
            }
        }

        private void FinishFailed()
        {
            rootBarrel.GetComponentInChildren<Animation>().enabled = false;
            rootBarrel.transform.rotation = new Quaternion(0f,0f,0f,0f);
            GameplayController.Instance.FinishGameplay(false);
            splineFollower.follow = false;
        }

        private static void AnimationOff(Transform barrel)
        {
            barrel.GetComponentInChildren<Animation>().enabled = false;
            barrel.rotation = new Quaternion(0f,0f,0f,0f);
        }
    }
}
