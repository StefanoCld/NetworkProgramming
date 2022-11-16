using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigCubeController : MonoBehaviour
{
    [Header("Rotating Forces")]
    [SerializeField] private float rotatingForce = 30.0f;

    [Header("Floating Forces")]
    [SerializeField] private float floatingSpeed = 0.5f;
    [SerializeField] private float floatingUpForce = 1200.0f;
    [SerializeField] private float maxFloatingUpForceMultiplierValue = 100.0f;
    [Header("Attraction Forces")]

    [SerializeField] private float attractionForce = 70.0f;
    [SerializeField] private float attractionRadius = 1.5f;
    [SerializeField] private ForceMode attractionForceMode = ForceMode.Acceleration;

    [Header("Boom Forces")]
    [SerializeField] private float boomForce = 0.0075f;
    [SerializeField] private float boomRadius = 2;
    [SerializeField] private ForceMode boomForceMode = ForceMode.Impulse;

    // Private Stuff
    private Rigidbody rb;

    private float horizontalAxis;
    private float verticalAxis;
    private bool isAttracting;
    private bool isFloating;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        horizontalAxis = Input.GetAxis("Horizontal");
        verticalAxis = Input.GetAxis("Vertical");

        isAttracting = Input.GetButton("Fire1");
        isFloating = Input.GetButton("Fire2");
    }

    void FixedUpdate()
    {
        if(rb)
        {
            if (!isFloating)
            {
                Vector3 rotationVector = new Vector3(verticalAxis, 0, -horizontalAxis);
                rotationVector *= rotatingForce;
                rb.AddTorque(rotationVector, ForceMode.VelocityChange);
            }
            else
            {
                Vector3 movingVector = new Vector3(horizontalAxis, 0, verticalAxis);
                rb.AddForce(movingVector * floatingSpeed, ForceMode.VelocityChange);
            }
        }

        if (isAttracting)
        {
            Attract();
            isFloating = false;
        }

        if (isFloating)
        {
            Float();
            isAttracting = false;
        }
    }

    void Attract()
    {
        Collider[] attractedColliders = Physics.OverlapSphere(transform.position, attractionRadius);

        foreach(Collider collider in attractedColliders)
        {
            Rigidbody colliderRigidbody = collider.gameObject.GetComponent<Rigidbody>();

            if (colliderRigidbody)
            {
                Vector3 colliderToMeVersor = (transform.position - collider.transform.position).normalized;

                colliderRigidbody.AddForce(colliderToMeVersor * attractionForce, attractionForceMode);
            }
        }
    }

    void Float()
    {
        // Push BigCube upwards
        float UpForceMultiplier = 1 / (transform.position.y);
        //UpForceMultiplier = Mathf.Clamp(UpForceMultiplier, 0, maxFloatingUpForceMultiplierValue);
        rb.AddForce(Vector3.up * (floatingUpForce * UpForceMultiplier));

        // Push little cubes away
        Collider[] attractedColliders = Physics.OverlapSphere(transform.position, boomRadius);

        foreach (Collider collider in attractedColliders)
        {
            Rigidbody colliderRigidbody = collider.gameObject.GetComponent<Rigidbody>();

            if (colliderRigidbody)
            {
                Vector3 colliderToMeVersor = (transform.position - collider.transform.position).normalized;

                colliderRigidbody.AddForce(-colliderToMeVersor * boomForce, boomForceMode);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attractionRadius);
    }
}