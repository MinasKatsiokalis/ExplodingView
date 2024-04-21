using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MK.ExplodingView.Samples
{   
    /// <summary>
    /// Camera controler for the sample scene.
    /// It enables the camera to rotate around the target object.
    /// </summary>
    public class CameraControler : MonoBehaviour
    {
        [Tooltip("Target for the camera to rotate around.")]
        public Transform target;
        [Tooltip("Speed of the camera rotation.")]
        public float rotateSpeed = 5.0f;
        [Tooltip("Smooth factor for camera rotation.")]
        public float smoothTime = 0.01f; 

        private float mouseX, mouseY;
        private float velX = 0.0f;
        private float velY = 0.0f;

        void Start()
        {
            if (target == null)
                Debug.LogError("CameraControler: No target set for the camera to rotate around.");
        }
        void FixedUpdate()
        {
            if (target == null)
                return;

            if (Input.GetMouseButton(0))
            {
                float targetX = mouseX + Input.GetAxis("Mouse X") * rotateSpeed;
                float targetY = mouseY - Input.GetAxis("Mouse Y") * rotateSpeed;
                mouseY = Mathf.Clamp(targetY, -35, 60);

                mouseX = Mathf.SmoothDamp(mouseX, targetX, ref velX, smoothTime);
                mouseY = Mathf.SmoothDamp(mouseY, targetY, ref velY, smoothTime);

                transform.position = target.position + Quaternion.Euler(mouseY, mouseX, 0) * new Vector3(0, 0, -1) * (target.position - transform.position).magnitude;
                transform.LookAt(target);
            }
        }
    }
}

