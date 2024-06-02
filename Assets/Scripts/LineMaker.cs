using UnityEngine;

public class LineMaker : MonoBehaviour
{
    public GameObject objectPrefab; // The prefab of the object you want to instantiate
    public AudioClip clipper;
    public float lineThickness = 0.1f; // Thickness of the line
    public int samplesASecond = 2; // Number of samples per second
    public float objectSpacing = 0.5f; // Spacing between instantiated objects
    private LineRenderer lineRenderer; // Reference to the LineRenderer component
    private AudioSource audioSource; // Reference to the AudioSource component
    private EdgeCollider2D edgeCollider; // Reference to the EdgeCollider2D component
    private float moveSpeed = 1.25f; // Speed of movement in objects per second;
    private Material lineMaterial; // Material for the LineRenderer
    public bool shouldMakeLine;

    void Awake()
    {
        float[] peakVolumes = GetPeakVolumesPerSecond(clipper);

        moveSpeed = samplesASecond / 2;
        moveSpeed *= objectSpacing;
        // Loop through each element of the peakVolumes array
        for (int i = 0; i < peakVolumes.Length; i++)
        {
            // Calculate position along the x-axis based on index and spacing
            float xPos = i * objectSpacing;

            // Calculate height of the object based on the peak volume of the corresponding second
            float height = peakVolumes[i] * 10; // You may need to adjust this multiplier for appropriate scaling

            // Instantiate the object at the calculated position with the calculated height
            GameObject obj = Instantiate(objectPrefab, new Vector3(xPos, height / 2, 0), Quaternion.identity);
            // Set the position of the object to the calculated height
            obj.transform.position = new Vector3(obj.transform.position.x, height / 2, obj.transform.position.z);

            // Set the parent of the instantiated object to the parent game object
            obj.transform.parent = this.transform;
        }

        if (shouldMakeLine)
        {

            // Add LineRenderer component to the parent game object
            lineRenderer = this.gameObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = peakVolumes.Length;
            lineRenderer.startWidth = lineThickness;
            lineRenderer.endWidth = lineThickness;

            // Create a material with the desired color
            lineMaterial = new Material(Shader.Find("Standard"));
            lineRenderer.material = lineMaterial; // Assign the material to the LineRenderer 

            // Add EdgeCollider2D component to the parent game object
            edgeCollider = this.gameObject.AddComponent<EdgeCollider2D>();

            // Set EdgeCollider2D thickness to match LineRenderer thickness
            edgeCollider.edgeRadius = lineThickness * 0.5f;

            Vector2[] colliderPoints = new Vector2[lineRenderer.positionCount];

            // Set the points of the line to match the positions of the instantiated objects
            for (int i = 0; i < peakVolumes.Length; i++)
            {
                Vector3 newPosition = this.gameObject.transform.GetChild(i).position;
                lineRenderer.SetPosition(i, newPosition);
                colliderPoints[i] = newPosition;
            }

            // Set the points of the EdgeCollider2D to match the LineRenderer positions
            edgeCollider.points = colliderPoints;
        }

        // Add AudioSource component to play the audio clip
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clipper;
        audioSource.Play();
    }

    void Update()
    {
        // Move all objects to the left at the specified speed
        float moveDistance = moveSpeed * Time.deltaTime;
        this.gameObject.transform.Translate(Vector3.left * moveDistance, Space.World);

        if (shouldMakeLine)
        {
            // Update the LineRenderer positions and EdgeCollider2D points to match the moved objects
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                lineRenderer.SetPosition(i, this.gameObject.transform.GetChild(i).position);
            }
        }

        // Stop updating if audio playback is finished
        if (!audioSource.isPlaying)
        {
            enabled = false;
        }
    }

    // Call this method with your audio clip to get the peak volumes per second
    public float[] GetPeakVolumesPerSecond(AudioClip clip)
    {
        var sampleData = new float[clip.samples * clip.channels];
        clip.GetData(sampleData, 0);

        int samplesPerSecond = clip.frequency * clip.channels;
        samplesPerSecond /= samplesASecond;
        int totalSeconds = clip.samples / samplesPerSecond;
        var peakVolumes = new float[totalSeconds];

        for (int i = 0; i < totalSeconds; i++)
        {
            float maxVolume = 0f;
            for (int j = 0; j < samplesPerSecond; j++)
            {
                int sampleIndex = i * samplesPerSecond + j;
                if (sampleIndex < sampleData.Length)
                {
                    float volume = Mathf.Abs(sampleData[sampleIndex]);
                    if (volume > maxVolume)
                    {
                        maxVolume = volume;
                    }
                }
            }
            peakVolumes[i] = maxVolume;
        }

        return peakVolumes;
    }

}