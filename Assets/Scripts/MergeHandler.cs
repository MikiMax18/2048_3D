using UnityEngine;

public class MergeHandler : MonoBehaviour
{
    private bool hasMerged = false;
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
        if (hasMerged || LogicScript.Instance.IsGameOver)
            return;

        if (!collision.gameObject.TryGetComponent(out MergeHandler other) || other == this || other.hasMerged)
            return;

        if (!collision.gameObject.TryGetComponent(out NumberCube otherCube))
            return;

        int currentValue = numberCube.value;

        if (currentValue != otherCube.value)
            return;

        float impulse = collision.relativeVelocity.magnitude * rb.mass;
        if (impulse < mergeImpulseThreshold)
            return;

        hasMerged = true;
        other.hasMerged = true;

        if (mergeAudio != null && audioSource != null)
            audioSource.PlayOneShot(mergeAudio);

        Vector3 mergePosition = (transform.position + other.transform.position) / 2f;

        Destroy(other.gameObject);
        transform.position = mergePosition;

        numberCube.SetValue(currentValue * 2);

        if (numberCube.value >= LogicScript.Instance.winCubeValue)
        {
            LogicScript.Instance.ShowGameOver();
        }

        GameEvents.CubeMerged(currentValue);
        rb.AddForce(Vector3.up * 3f, ForceMode.Impulse);

        Invoke(nameof(ResetMergeFlag), 0.1f);
    }

    private void ResetMergeFlag()
    {
        hasMerged = false;
    }
}
