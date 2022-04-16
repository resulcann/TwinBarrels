using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    
    public class Barrel : MonoBehaviour
    {
        public Collector collector;
        public Material greenMat;
        public GameObject brokenRock, brokenRockEffect;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Stairs"))
            {
                transform.localPosition += Vector3.down / 2;
                collector.barrelList.Remove(this);
                transform.parent = null;
                AnimationOff();
                other.gameObject.GetComponent<MeshRenderer>().material = greenMat;

            }
        }

        private void AnimationOff()
        {
            GetComponentInChildren<Animation>().enabled = false;
            transform.rotation = new Quaternion(0f,0f,0f,0f);
        }
        
        private IEnumerator BoxExplosions()
        {
            yield return new WaitForSeconds(1.5f);
            Instantiate(brokenRockEffect, transform.position,Quaternion.identity);
            GameObject brokenRockObject = Instantiate(brokenRock, transform.position,Quaternion.identity) as GameObject;
            Rigidbody[] allRigidBodies = brokenRockObject.GetComponentsInChildren<Rigidbody>();
            if (allRigidBodies.Length > 0)
            {
                foreach (var body in allRigidBodies)
                {
                    body.AddExplosionForce(500, transform.position,1);
                }
            }
            Destroy(this.gameObject);
        }
        
    }
}
