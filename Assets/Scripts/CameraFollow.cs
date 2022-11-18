using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera offset")]
    [SerializeField] private Vector3 offset;
    [SerializeField] private float xRotation = 45;
    [SerializeField] private float smoothingTime = 0.25f;

    private GameObject bigCube;
    private Rigidbody rbBigCube;

    void Start()
    {
        BigCubeController bc = FindObjectOfType<BigCubeController>();

        if (bc)
        {
            bigCube = bc.gameObject;
            rbBigCube = bc.gameObject.GetComponent<Rigidbody>();
        }
    }

    private void LateUpdate()
    {
        Vector3 velocity = Vector3.zero;
        Vector3 fixedYOffset = new Vector3((bigCube.transform.position + offset).x, 5, (bigCube.transform.position + offset).z);
        transform.position = Vector3.SmoothDamp(transform.position, fixedYOffset, ref velocity, smoothingTime);
        transform.rotation = Quaternion.Euler(xRotation, 0, 0);
    }
}
