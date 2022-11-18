using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera offset")]
    [SerializeField] private Vector3 offset;
    [SerializeField] private float xRotation = 45.0f;
    //[SerializeField] private float smoothingTime = 0.25f;

    private GameObject bigCube;

    void Start()
    {
        BigCubeController bc = FindObjectOfType<BigCubeController>();

        if (bc)
        {
            bigCube = bc.gameObject;
        }
    }

    private void LateUpdate()
    {
        //Vector3 velocity = Vector3.zero;
        //Vector3 fixedYOffset = new Vector3(bigCube.transform.position.x + offset.x, offset.y, bigCube.transform.position.z + offset.z);
        //transform.position = Vector3.SmoothDamp(transform.position, fixedYOffset, ref velocity, smoothingTime);
        transform.SetPositionAndRotation(
            new Vector3(bigCube.transform.position.x + offset.x, offset.y, bigCube.transform.position.z + offset.z), 
            Quaternion.Euler(xRotation, 0, 0)
        );
    }
}
