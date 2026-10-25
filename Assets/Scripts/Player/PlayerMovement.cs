using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    private DialogueManager dialogueManager;
    private DayManager dayManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        dialogueManager = FindFirstObjectByType<DialogueManager>();
        dayManager = FindFirstObjectByType<DayManager>();
    }

    private void Update()
    {
        if (IsMovementBlocked())
        {
            movement = Vector2.zero;
            return;
        }

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        movement = movement.normalized;
    }

    private void FixedUpdate()
    {
        if (IsMovementBlocked())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = movement * moveSpeed;
    }

    private bool IsMovementBlocked()
    {
        if (dialogueManager != null && dialogueManager.IsDialogueActive)
            return true;

        if (dayManager != null &&
            dayManager.CurrentState == WorkDayState.Summary)
            return true;

        return false;
    }
}