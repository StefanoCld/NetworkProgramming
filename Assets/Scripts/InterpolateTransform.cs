using UnityEngine;

public class InterpolateTransform : MonoBehaviour
{
    [SerializeField] private float positionInterpolationFactor = 50f;
    [SerializeField] private float rotationInterpolationFactor = 50f;

    public bool IsMasterClient
    {
        get; set;
    }

    public Vector3 lastPackagePosition;
    public Quaternion lastPackageRotation;

    void Start()
    {
        lastPackagePosition = transform.position;
        lastPackageRotation = transform.rotation;
    }

    void Update()
    {
        if (!IsMasterClient)
        {
            transform.position = Vector3.Lerp(transform.position, lastPackagePosition, Time.deltaTime * positionInterpolationFactor);
            transform.rotation = Quaternion.Slerp(transform.rotation, lastPackageRotation, Time.deltaTime * rotationInterpolationFactor);
        }
    }
}
