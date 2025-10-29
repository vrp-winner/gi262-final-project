using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private int maxHp;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    private int currentHp;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded = true;
    
    public bool canMove = true;

    public bool IsDead => currentHp <= 0;
    
    private PlayerControls controls;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new PlayerControls();
        currentHp = maxHp;

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        controls.Player.Jump.performed += ctx => Jump();
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    private void FixedUpdate() => Move();

    public void Move()
    {
        if (!canMove || IsDead) return;

        Vector2 velocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = velocity;
    }
    
    private void Jump()
    {
        if (!isGrounded || !canMove || IsDead) return;

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;
        
        currentHp -= damage;
        Debug.Log($"Player took {damage} damage. HP left: {currentHp}/{maxHp}");

        if (currentHp <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log("💀 Player died!");
        canMove = false;
        rb.linearVelocity = Vector2.zero;
        gameObject.SetActive(false);
    }
}