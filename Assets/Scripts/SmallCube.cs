using System.Collections;
using UnityEngine;

public class SmallCube : MonoBehaviour
{
    [Header("Tags")]
    [SerializeField] private const string smallCubeTag = "SmallCube";
    [SerializeField] private const string bigCubeTag = "BigCube";

    [Header("Cube colors")]
    [SerializeField] private Color normalColor;
    [SerializeField] private Color interactColor;

    [Header("Color Fading parameters")]
    [SerializeField] private float waitToFadeTime = 1.0f;
    [SerializeField] private float fadingTime = 0.25f;

    // Private stuff
    private Material material;

    private float timerCache;
    public bool isInteracting;

    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        material.color = normalColor;
    }

    public void Interact()
    {
        if (!isInteracting)
        {
            StartCoroutine(InteractingCoroutine());
            isInteracting = true;
        }
        else
        {
            StopAllCoroutines();
            isInteracting = true;
            StartCoroutine(InteractingCoroutine());
        }
    }

    private IEnumerator InteractingCoroutine()
    {
        material.color = interactColor;

        yield return new WaitForSeconds(waitToFadeTime);

        isInteracting = false;
        timerCache = 0;

        while(timerCache < fadingTime)
        {
            Color interpolatedColor = Color.Lerp(interactColor, normalColor, timerCache / fadingTime);
            material.color = interpolatedColor;
            timerCache += Time.deltaTime;
            yield return null;
        }

        material.color = normalColor;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(smallCubeTag))
        {
            Interact();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag(smallCubeTag))
        {
            Interact();
        }
    }
}
