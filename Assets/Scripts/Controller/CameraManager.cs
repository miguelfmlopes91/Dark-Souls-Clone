using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Controller
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private bool lockOn;
        [SerializeField] private float followSpeed = 9f;
        [SerializeField] private float mouseSpeed = 2f;
        [SerializeField] private float controllerSpeed = 7f;
        
        private float turnSmoothing = .1f;
        private float smoothX;
        private float smoothY;
        private float smoothXVelocity;
        private float smoothYVelocity;
        [SerializeField]private float minAngle = -35f;
        [SerializeField]private float maxAngle = 35f;
        [SerializeField] private float lookAngle;
        [SerializeField] private float tiltAngle;
        
        private Transform target;
        private Transform pivot;
        private Transform camTrans;

        public static CameraManager Instance { get; private set; }


        public void Init(Transform t)
        {
            target = t;
            camTrans = Camera.main.transform;
            pivot = camTrans.parent;
        }
        
        public void Tick(float d)
        {
            float h = Input.GetAxis("Mouse X");
            float v = Input.GetAxis("Mouse Y");
            
            float c_h = Input.GetAxis("RightAxis X");
            float c_v = Input.GetAxis("RightAxis Y");

            float targetSpeed = mouseSpeed;
            if (c_h != 0 || c_v != 0)
            {
                h = c_h;
                v = c_v;
                targetSpeed = controllerSpeed;
            }
            
            FollowTarget(d);
            HandleRotation(d, v, h, targetSpeed);
        }

        private void FollowTarget(float d)
        {
            float speed = d * followSpeed;
            Vector3 targetPostion = Vector3.Lerp(transform.position, target.position, speed);
            transform.position = targetPostion;
        }

        private void HandleRotation(float d, float v, float h, float targetSpeed)
        {
            if (turnSmoothing > 0)
            {
                smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothXVelocity, turnSmoothing);
                smoothY = Mathf.SmoothDamp(smoothY, v, ref smoothYVelocity, turnSmoothing);
            }
            else
            {
                smoothX = h;
                smoothY = v;
            }

            if (lockOn)
            {
                
            }

            lookAngle += smoothX * targetSpeed;
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);

            tiltAngle -= smoothY * targetSpeed;
            tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);
        }
        
        private void Awake()
        {
            Instance = this;//TODO: proper singleton
        }
        
    } 
}