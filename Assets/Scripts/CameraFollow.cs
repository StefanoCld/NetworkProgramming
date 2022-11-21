using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera offset")]
    [SerializeField] private Vector3 offset;
    [SerializeField] private float xRotation = 45.0f;

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
        transform.SetPositionAndRotation(
            new Vector3(bigCube.transform.position.x + offset.x, offset.y, bigCube.transform.position.z + offset.z), 
            Quaternion.Euler(xRotation, 0, 0)
        );
    }
}
