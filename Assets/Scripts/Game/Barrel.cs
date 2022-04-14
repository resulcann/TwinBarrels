using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public class Barrel : MonoBehaviour
    { // sol collector ve sol trail konumu yanlış ve kamera yükselmesini hallet!
        public Collector collector;
        public Material greenMat;
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
    }
}
