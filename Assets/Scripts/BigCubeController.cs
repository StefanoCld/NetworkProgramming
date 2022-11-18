using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigCubeController : MonoBehaviour
{
    [Header("Tags")]
    [SerializeField] private const string smallCubeTag= "SmallCube";
    [SerializeField] private const string bigCubeTag = "BigCube";

    [Header("Rotating Forces")]
    [SerializeField] private float rotatingForce = 30.0f;

    [Header("Floating Forces")]
    [SerializeField] private float floatingSpeed = 0.25f;
    [SerializeField] private float floatingUpForce = 1200.0f;

    [Header("Attraction Forces")]
    [SerializeField] private float attractionForce = 70.0f;
    [SerializeField] private float attractionRadius = 1.5f;
    [SerializeField] private ForceMode attractionForceMode = ForceMode.Acceleration;

    [Header("Boom Forces")]
    [SerializeField] private float boomCapsuleHalfHeight = 2.0f;
    [SerializeField] private float boomForce = 0.75f;
    [SerializeField] private float boomRadius = 3.5f;
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

        rb.AddForce(Vector3.up * (floatingUpForce * UpForceMultiplier));

        // Push little cubes away
        Collider[] attractedColliders = Physics.OverlapCapsule(
            transform.position - Vector3.up * boomCapsuleHalfHeight,
            transform.position + Vector3.up * boomCapsuleHalfHeight, 
            boomRadius);

        foreach (Collider collider in attractedColliders)
        {
            Rigidbody colliderRigidbody = collider.gameObject.GetComponent<Rigidbody>();

            if (colliderRigidbody)
            {
                if(collider.transform.position.y >= this.transform.position.y)
                {
                    Vector3 colliderToMeVersor = (transform.position - collider.transform.position).normalized;
                    colliderRigidbody.AddForce(-colliderToMeVersor * boomForce, boomForceMode);
                }
                else
                {
                    // So that little cubes don't get stuck onto the plane
                    Vector3 myPosProjected = transform.position;
                    myPosProjected.y = collider.transform.position.y;
                    Vector3 MeToColliderVersorProj = (collider.transform.position - myPosProjected).normalized;
                    colliderRigidbody.AddForce(MeToColliderVersorProj * boomForce, boomForceMode);
                }

                SmallCube sc = collider.gameObject.GetComponent<SmallCube>();
                if (sc)
                    sc.Interact();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(smallCubeTag))
        {
            SmallCube sc = collision.gameObject.GetComponent<SmallCube>();
            if (sc)
                sc.Interact();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag(smallCubeTag))
        {
            SmallCube sc = collision.gameObject.GetComponent<SmallCube>();
            if (sc)
                sc.Interact();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, boomRadius);
        Gizmos.DrawWireSphere(transform.position + Vector3.up * boomCapsuleHalfHeight, boomRadius);
        Gizmos.DrawWireSphere(transform.position - Vector3.up * boomCapsuleHalfHeight, boomRadius);
    }
}