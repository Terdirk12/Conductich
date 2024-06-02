using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class raycastThrower : MonoBehaviour
{

    private LineRenderer lineRenderer;
    private AudioSource audioSource;
    public Color hitColor = Color.red;
    public Transform controllerTransform; // Reference to the controller's transform
    RaycastHit2D hit;
    private bool hitting;
    private int tick, secondTickLol;
    private XRBaseController xr;
    // Reference to the AudioDistortionFilter component
    private AudioDistortionFilter distortionFilter;
    public float distortionResetValue = 0.7f;
    private float distortionActualValue;

    private void Start()
    {
        distortionActualValue = distortionResetValue;
        lineRenderer = FindAnyObjectByType<LineRenderer>();
        xr = FindAnyObjectByType<XRBaseController>();

        audioSource = GameObject.FindGameObjectWithTag("Important").GetComponent<AudioSource>();

        // Add the AudioDistortionFilter component to the AudioSource GameObject
        distortionFilter = audioSource.gameObject.AddComponent<AudioDistortionFilter>();

        // Configure the parameters of the distortion effect
        distortionFilter.distortionLevel = distortionActualValue; // Adjust the level of distortion (0.0 to 1.0)

    }

    void Update()
    {
        // Cast a ray from the controller's position in the up direction (2D)
        Vector2 origin = controllerTransform.position;
        Vector2 direction = controllerTransform.up;
        distortionFilter.distortionLevel = distortionActualValue;

        // Perform a 2D raycast
        hit = Physics2D.Raycast(origin, direction);

        if (hit.collider != null)
        {
            // Check if the collider of the hit object belongs to the LineRenderer
            if (hit.collider.gameObject == lineRenderer.gameObject)
            {
                hitting = true;
                // Change the color of the LineRenderer when hit
                lineRenderer.material.color = Color.green;

            }
        }
        else
        {
            hitting = false;
            ActivateHaptic();
            lineRenderer.material.color = Color.red;
        }
    }
    void ActivateHaptic()
    {
        if (xr != null) xr.SendHapticImpulse(1f, 0.1f);
        else Debug.LogError("log error: XR == null: " + xr);
    }

    private void FixedUpdate()
    {
        if (!hitting) { tick++; } else tick--;
        if (tick > 60) tick = 60;
        if (tick < 0) tick = 0;
        if (tick >= 60)
        {
            secondTickLol++;
            ApplyAudioDistortion();
        }
        else
        {
            distortionActualValue = distortionResetValue;
            ResetAudioDistortion();
        }

        if (secondTickLol >= 120)
        {
            distortionActualValue += 0.1f;
            secondTickLol = 0;
        }
    }

    private void ApplyAudioDistortion()
    {
        distortionFilter.enabled = true;
    }

    private void ResetAudioDistortion()
    {
        distortionFilter.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(controllerTransform.position, controllerTransform.up);
    }
}
