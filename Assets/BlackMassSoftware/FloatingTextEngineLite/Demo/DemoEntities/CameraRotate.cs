using UnityEngine;

namespace BlackMassSoftware.FloatingTextEngine.Lite.Demo
{
    public class CameraRotate : MonoBehaviour
    {
        private float cameraRotationSpeed = 5f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Camera.main.transform.RotateAround(new Vector3(-15f, 0f, 5f), Vector3.up, Time.deltaTime * cameraRotationSpeed);
        }
    }

}