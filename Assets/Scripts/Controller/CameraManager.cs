using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Controller
{
    public class CameraManager : MonoBehaviour
    {
        public bool LockOn { get; set; }
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
        
        public Transform target;
        public Transform lockOnTarget;
        
        [HideInInspector] public Transform pivot;
        [HideInInspector] public Transform camTrans;

        public static CameraManager Instance { get; private set; }


        public void Init(Transform t)
        {
            target = t;
            camTrans = Camera.main.transform;
            pivot = camTrans.parent;
        }
        
        public void FixedTick(float d)
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

            tiltAngle -= smoothY * targetSpeed;
            tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);
            
            
            if (LockOn && lockOnTarget != null)
            {
                //directions towards lockOn target
                Vector3 targetDir = lockOnTarget.position - transform.position;
                targetDir.Normalize();
                //targetDir.y = 0;
                if (targetDir == Vector3.zero)
                    targetDir = transform.forward;

                Quaternion targetRotation = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, d * 9f);
                lookAngle = transform.eulerAngles.y;
                return;
            }
            lookAngle += smoothX * targetSpeed;
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);
        }
        
        private void Awake()
        {
            Instance = this;//TODO: proper singleton
        }
        
    } 
}