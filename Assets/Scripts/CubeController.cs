using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class CubeController : MonoBehaviour
{
    public float dragSpeed = 12f;        
    public float launchForce = 14f;      // Force when launching the cube
    public float lerpSpeed = 0.35f;      // Smooth movement factor
    private const float minX = -0.85f;  
    private const float maxX = 1.52f;  

    private Rigidbody rb;
    private bool isDragging = false;
    public bool isActiveCube = true; // Is this cube the one under player control
    private Vector2 touchStartPos;

    private float startXOffset = 0f; // Offset between finger and cube position

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
        rb.isKinematic = true;                  // Disable physics at start
        rb.constraints = RigidbodyConstraints.FreezeRotationX; 
    }

    private void Update()
    {
        // Don't allow control if not active or game is over
        if (!isActiveCube || logic == null || logic.IsGameOver)
            return;

        // Touch input for mobile
        if (Touchscreen.current != null)
        {
            HandleTouchInput();
        }
#if UNITY_EDITOR
        // Mouse input for testing in editor
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
            // Store offset between touch and cube X position
            startXOffset = (currentTouchPos.x / Screen.width) - transform.position.x;
        }

        if (touch.press.isPressed && isDragging)
        {
            // Convert touch X to world X between minX and maxX
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
            // Calculate horizontal drag from mouse delta
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
        spawner.OnCubeLaunched();             // Tell spawner to wait and spawn new cube
        rb.isKinematic = false;               
        rb.AddForce(Vector3.forward * launchForce, ForceMode.Impulse); 
        Invoke(nameof(DestroyComponent), 0.6f); 
    }

    private void DestroyComponent()
    {
        rb.constraints = RigidbodyConstraints.None; 
        Destroy(this);                              // Remove this controller script
    }
}
