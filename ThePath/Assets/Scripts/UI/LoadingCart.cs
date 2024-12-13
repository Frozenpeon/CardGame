using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    public class LoadingCart : MonoBehaviour
    {
        public float speed = 1.0f;
        public GameObject nextPose;
        public GameObject startPose;
        public Animator animator => GetComponent<Animator>();
        void Start()
        {
            animator.SetTrigger("Start_Move");
            gameObject.transform.position = startPose.transform.position;
            StartCoroutine(Move());

        }

        private void OnEnable()
        {
            animator.SetTrigger("Start_Move");
            gameObject.transform.position = startPose.transform.position;
            StartCoroutine(Move());
        }
        IEnumerator Move()
        {
            while (true)
            {
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, nextPose.transform.position, speed * Time.deltaTime);

                if (gameObject.transform.position == nextPose.transform.position) gameObject.transform.position = startPose.transform.position;
                yield return null;
            }
        }
    }
}
