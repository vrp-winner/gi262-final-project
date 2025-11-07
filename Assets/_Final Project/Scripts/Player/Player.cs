using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private int maxHp;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint; 
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;      

    [Header("Jump Physics (Hollow Knight Feel)")]
    [SerializeField] private float fallGravityMultiplier = 2.5f;  
    [SerializeField] private float JumpMultiplier = 0.5f;

    [Header("Game Settings")]
    [SerializeField] private bool isInstantKillMode = true;
    private float baseGravityScale;
    private bool isFacingRight = false;

    private int currentHp;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer; 
    private Vector2 moveInput;
    private bool isGrounded;

    public bool canMove = true;
    public bool IsDead => currentHp <= 0;

    private PlayerControls controls;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        controls = new PlayerControls();
        currentHp = maxHp;

        baseGravityScale = rb.gravityScale; 

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

       
        controls.Player.Jump.performed += ctx => Jump();
        controls.Player.Jump.canceled += ctx => HandleVariableJump();
       
    }


    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

   
    private void Update()
    {
        CheckGrounded();
        Flip();
    }

    private void FixedUpdate()
    {
        Move();
        HandleGravity();
    }

    public void Move()
    {
        if (!canMove || IsDead) return;

        
        Vector2 velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        rb.velocity = velocity;
    }

    private void Jump()
    {
        if (!isGrounded || !canMove || IsDead) return;

       
        rb.velocity = new Vector2(rb.velocity.x, 0f);

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        
    }

    
    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }

   
    private void HandleGravity()
    {
        
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = baseGravityScale * fallGravityMultiplier;
        }
        else 
        {
            rb.gravityScale = baseGravityScale;
        }
    }

    private void HandleVariableJump()
    {
        if (rb.velocity.y > 0)
        {
            
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * JumpMultiplier);
        }
    }

    
    private void Flip()
    {
       
        if ((isFacingRight && moveInput.x < 0) || (!isFacingRight && moveInput.x > 0))
        {
            isFacingRight = !isFacingRight;
            //spriteRenderer.flipX = !isFacingRight;
            spriteRenderer.flipX = isFacingRight;


        }
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        if (isInstantKillMode)
        {
            currentHp = 0;
            Debug.Log("Player was hit in Instant Kill Mode!");
        }
        else
        {
            currentHp -= damage;
            Debug.Log($"Player took {damage} damage. HP left: {currentHp}/{maxHp}");
        }

        if (currentHp <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log("💀 Player died!");
        canMove = false;
        rb.velocity = Vector2.zero;
        gameObject.SetActive(false);
    }
}