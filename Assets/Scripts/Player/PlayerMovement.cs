using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private GameInputManager inputManager;

    private Vector2 movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        inputManager =
            FindFirstObjectByType<GameInputManager>();

        if (inputManager == null)
        {
            Debug.LogError(
                "PlayerMovement: GameInputManager не найден.");
        }
    }

    private void Update()
    {
        if (inputManager != null &&
            inputManager.IsPlayerInputBlocked)
        {
            movement = Vector2.zero;
            return;
        }

        movement = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical"));

        movement = movement.normalized;
    }

    private void FixedUpdate()
    {
        if (inputManager != null &&
            inputManager.IsPlayerInputBlocked)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity =
            movement * moveSpeed;
    }
}