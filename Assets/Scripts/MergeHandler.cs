using UnityEngine;

public class MergeHandler : MonoBehaviour
{
    private bool hasMerged = false; // Prevents multiple merges at once
    public float mergeImpulseThreshold = 0.3f; 
    public AudioClip mergeAudio;

    private AudioSource audioSource;
    private Rigidbody rb;
    private NumberCube numberCube;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>(); 
        numberCube = GetComponent<NumberCube>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Stop if already merged or game is over
        if (hasMerged || LogicScript.Instance.IsGameOver)
            return;

        // Check if the other object has MergeHandler and isn't the same or already merged
        if (!collision.gameObject.TryGetComponent(out MergeHandler other) || other == this || other.hasMerged)
            return;

        // Check if the other object has NumberCube script
        if (!collision.gameObject.TryGetComponent(out NumberCube otherCube))
            return;

        int currentValue = numberCube.value;

        // Only merge if values are equal
        if (currentValue != otherCube.value)
            return;

        // Calculate impact force
        float impulse = collision.relativeVelocity.magnitude * rb.mass;
        if (impulse < mergeImpulseThreshold)
            return;

        // Mark both cubes as merged
        hasMerged = true;
        other.hasMerged = true;

        // Play merge sound
        if (mergeAudio != null && audioSource != null)
            audioSource.PlayOneShot(mergeAudio);

        // Merge position is the average between two cubes
        Vector3 mergePosition = (transform.position + other.transform.position) / 2f;

        Destroy(other.gameObject); 
        transform.position = mergePosition; // Move to merge center

        // Update value to doubled
        numberCube.SetValue(currentValue * 2);

        // Check win condition
        if (numberCube.value >= LogicScript.Instance.winCubeValue)
        {
            GameEvents.GameOver(true);
        }

        // Fire cube merge event for score system (UI)
        GameEvents.CubeMerged(currentValue);

        rb.AddForce(Vector3.up * 3f, ForceMode.Impulse);

        // Reset flag slightly after collision
        Invoke(nameof(ResetMergeFlag), 0.1f);
    }

    private void ResetMergeFlag()
    {
        hasMerged = false;
    }
}
