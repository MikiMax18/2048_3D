using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class CubeController : MonoBehaviour
{
    public float dragSpeed = 1f;
    public float launchForce = 1f;
    public float lerpSpeed = 0.2f;
    private const float minX = -0.85f;
    private const float maxX = 1.52f;

    private Rigidbody rb;
    private bool isDragging = false;
    public bool isActiveCube = true;
    private Vector2 touchStartPos;

    private float startXOffset = 0f;

    private CubeSpawner spawner;
    private LogicScript logic;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        spawner = FindAnyObjectByType<CubeSpawner>();
        logic = LogicScript.Instance;
    }

    private void Start()
    {
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX;
    }

    private void Update()
    {
        if (!isActiveCube || logic == null || logic.IsGameOver)
            return;

        if (Touchscreen.current != null)
        {
            HandleTouchInput();
        }
#if UNITY_EDITOR
        else if (Mouse.current != null)
        {
            HandleMouseInput();
        }
#endif
    }

    private void HandleTouchInput()
    {
        var touch = Touchscreen.current.primaryTouch;
        Vector2 currentTouchPos = touch.position.ReadValue();

        if (touch.press.wasPressedThisFrame)
        {
            if (EventSystem.current.IsPointerOverGameObject(touch.touchId.ReadValue())) return;

            isDragging = true;
            touchStartPos = currentTouchPos;
            startXOffset = (currentTouchPos.x / Screen.width) - transform.position.x;
        }

        if (touch.press.isPressed && isDragging)
        {
            float normalizedTouchX = Mathf.Clamp01(currentTouchPos.x / Screen.width);
            float targetX = Mathf.Lerp(minX, maxX, normalizedTouchX) - startXOffset;
            targetX = Mathf.Clamp(targetX, minX, maxX);

            Vector3 targetPos = new Vector3(targetX, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed);
        }

        if (touch.press.wasReleasedThisFrame && isDragging)
        {
            Launch();
        }
    }

    private void HandleMouseInput()
    {
        var mouse = Mouse.current;
        Vector2 currentMousePos = mouse.position.ReadValue();

        if (mouse.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            isDragging = true;
            touchStartPos = currentMousePos;
        }

        if (mouse.leftButton.isPressed && isDragging)
        {
            Vector2 delta = currentMousePos - touchStartPos;
            float deltaX = delta.x / Screen.width * dragSpeed;

            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x + deltaX, minX, maxX);
            transform.position = Vector3.Lerp(transform.position, pos, lerpSpeed);

            touchStartPos = currentMousePos;
        }

        if (mouse.leftButton.wasReleasedThisFrame && isDragging)
        {
            Launch();
        }
    }

    private void Launch()
    {
        isDragging = false;
        isActiveCube = false;
        spawner.OnCubeLaunched();
        rb.isKinematic = false;
        rb.AddForce(Vector3.forward * launchForce, ForceMode.Impulse);
        Invoke(nameof(DestroyComponent), 0.6f);
    }

    private void DestroyComponent()
    {
        rb.constraints = RigidbodyConstraints.None;
        Destroy(this);
    }
}
