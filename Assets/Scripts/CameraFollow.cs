using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera offset")]
    [SerializeField] private Vector3 offset;
    [SerializeField] private float xRotation = 45;
    [SerializeField] private float smoothingTime = 0.05f;

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
        transform.position = Vector3.SmoothDamp(transform.position, bigCube.transform.position + offset, ref velocity, smoothingTime);
        transform.rotation = Quaternion.Euler(xRotation, 0, 0);
    }
}
