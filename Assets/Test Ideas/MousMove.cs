using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class MousMove : MonoBehaviour
    {
        private Vector3 lastFramePosition;
        private Camera camera;
        [SerializeField] private float speed;
        [SerializeField] private float rotationSpeed;

        private BoostScript wordl;

        private void Start()
        {
            camera = Camera.main;
            wordl = GetComponent<BoostScript>();
        }

        void Update()
        {
            Vector3 currentFramePosition = Vector3.zero;

            if (Input.touchCount > 0)
                currentFramePosition = camera.ScreenToViewportPoint(Input.touches[0].position);

            //Vector3 currentFramePosition = camera.ScreenToViewportPoint(Input.mousePosition);
            if (Input.mousePosition != null)
                currentFramePosition = camera.ScreenToViewportPoint(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    Transform objectHit = hit.transform;

                    Debug.Log(objectHit.transform.localPosition);

                    //int posX = (int)objectHit.position.x;
                    //int posY = (int)objectHit.position.y;
                    //int posZ = (int)objectHit.position.z;

                    Box myBox = wordl.world.GetBoxAt(objectHit.localPosition);

                    //if(myBox!=null)

                    if (myBox.touchCount == 0)
                    {
                        objectHit.GetComponent<MeshRenderer>().material.color = Color.red;
                    }
                    if (myBox.touchCount >= 1)
                    {
                        Destroy(objectHit.gameObject);
                    }
                    myBox.touchCount++;

                    ////Destroy(positionMatrix[(int)delecte.x, (int)delecte.y, (int)delecte.z].myObject);

                    //// Do something with the object that was hit by the raycast.
                }
            }
            else if (Input.GetKey(KeyCode.M))
            {

                float rotX = Input.GetAxis("Mouse X") * rotationSpeed * Mathf.Deg2Rad;
                float rotY = Input.GetAxis("Mouse Y") * rotationSpeed * Mathf.Deg2Rad;

                transform.Rotate(Vector3.up, -rotX);
                transform.Rotate(Vector3.right, rotY);
            }

            if (Input.touchCount == 3)
            {
                Vector3 diff = lastFramePosition - currentFramePosition;
                camera.transform.Translate(diff * speed * Time.deltaTime);
            }
            if (Input.touchCount == 2)
            {
                float rotX = Input.GetAxis("Mouse X") * rotationSpeed * Mathf.Deg2Rad;
                float rotY = Input.GetAxis("Mouse Y") * rotationSpeed * Mathf.Deg2Rad;

                transform.Rotate(Vector3.up, -rotX);
                transform.Rotate(Vector3.right, rotY);
            }

            if (Input.GetMouseButton(1))
            {
                Vector3 diff = lastFramePosition - currentFramePosition;
                camera.transform.Translate(diff * speed * Time.deltaTime);
            }

            if (Input.touchCount > 0)
                lastFramePosition = camera.ScreenToViewportPoint(Input.touches[0].position);

            if (Input.mousePosition != null)
                lastFramePosition = camera.ScreenToViewportPoint(Input.mousePosition);
        }
    }
}