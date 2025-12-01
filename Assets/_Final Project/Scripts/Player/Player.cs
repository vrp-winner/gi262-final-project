using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private int maxHp;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    [Header("Interaction")]
    [SerializeField] private float interactRadius = 1.5f; 
    [SerializeField] private LayerMask interactableLayer; 

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

    [Header("UI")]
    //[SerializeField] private Slider healthBarSlider;
    [SerializeField] private Slider timerBarSlider;

    [Header("Audio Settings")] 
    [SerializeField] private AudioClip[] jumpSound;
    [Range(0f, 1f)][SerializeField] private float jumpVolume = 1.0f;

    [SerializeField] private AudioClip hurtSound;
    [Range(0f, 1f)][SerializeField] private float hurtVolume = 1.0f;

    [SerializeField] private AudioClip deadSound;
    [Range(0f, 1f)][SerializeField] private float deadVolume = 1.0f;

    [SerializeField] private AudioClip[] walkSound;
    [Range(0f, 1f)][SerializeField] private float walkVolume = 1.0f;
    [SerializeField] private float stepRate = 0.3f;
    private float nextStepTime = 0f;
    
    private AudioSource audioSource;

    [Header("Anim")]
    private PlayerControls controls;
    [SerializeField] private Animator anim;
    [SerializeField] private float deathDuration = 1.5f;
    public bool canMove = true;
    public bool IsDead => currentHp <= 0;

    private int currentHp;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        controls = new PlayerControls();
        currentHp = maxHp;
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        //if (healthBarSlider != null)
        //{
        //    healthBarSlider.maxValue = maxHp;
        //    healthBarSlider.value = currentHp;
        //}

        if (HealthPointUI.Instance != null) 
        {
            HealthPointUI.Instance.SetupHP(maxHp); 
        }

        baseGravityScale = rb.gravityScale;

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        controls.Player.Interact.performed += ctx => OnInteract();

        controls.Player.Jump.performed += ctx => Jump();
        controls.Player.Jump.canceled += ctx => HandleVariableJump();

    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();


    private void Update()
    {
        CheckGrounded();
        Animation();
        Flip();
        Footsteps();
    }
    
    private void FixedUpdate()
    {
        Move();
        HandleGravity();
    }
    
    private void OnDrawGizmos() //Gizmos เหมือนเอาไว้วาดภาพจำลองให้เห็นภาพ เห็นเส้น เห็นวง ประมาณนั้น (มั้ง) แบบอันนี้ก็ให้มันทำวงกลม
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }

    private void OnInteract()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRadius, interactableLayer);
        
        if (hit != null)
        {
            IInteractable interactableObject = hit.GetComponent<IInteractable>();
            
            if (interactableObject != null)
            {
                interactableObject._Interact(); 
            }
        }
    }

    public void Move()
    {
        if (!canMove || IsDead) return;


        Vector2 velocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = velocity;
    }

    private void Jump()
    {
        if (!isGrounded || !canMove || IsDead) return;


        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        if (jumpSound.Length > 0)
        {
            int randomIndex = Random.Range(0, jumpSound.Length);
            PlaySound(jumpSound[randomIndex], jumpVolume);
        }
    }

    private void HandleVariableJump()
    {
        if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * JumpMultiplier);
        }
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }

    private void HandleGravity()
    {
        if (rb.linearVelocity.y < 0)
            rb.gravityScale = baseGravityScale * fallGravityMultiplier;
        
        else
            rb.gravityScale = baseGravityScale;
        
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
        {
            PlaySound(hurtSound, hurtVolume);
            
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

            if (HealthPointUI.Instance != null) 
                HealthPointUI.Instance.UpdateHealth(currentHp); 
            
            if (currentHp <= 0)
                Die();
        }
    }

    private void Animation()
    {
        anim.SetBool("isGrounded", isGrounded);
        
        bool isWalking = moveInput.x != 0f;

        if (isGrounded)
            anim.SetBool("isWalking", isWalking);
        
        else
            anim.SetBool("isWalking", false);
    }

    private void Footsteps()
    {
        if (isGrounded && moveInput.x != 0 && !IsDead && canMove)
        {
            if (Time.time >= nextStepTime)
            {
                if (audioSource != null && walkSound.Length > 0)
                {
                    int randomIndex = Random.Range(0, walkSound.Length);
                    AudioClip clipToPlay = walkSound[randomIndex];

                    if (clipToPlay != null)
                        audioSource.PlayOneShot(clipToPlay, walkVolume);
                }
                
                nextStepTime = Time.time + stepRate;
            }
        }
    }

    private void Die()
    {
        if (!canMove) return;
        
        canMove = false;
        rb.linearVelocity = Vector2.zero;
        
        PlaySound(deadSound, deadVolume);

        if (anim != null)
            anim.SetTrigger("die");     
        
        StartCoroutine(DeathSequence());

        //GameManager.Instance.ShowGameOverScreen(); 
        //spriteRenderer.enabled = false; 
    }

    private IEnumerator DeathSequence()
    {
       yield return new WaitForSeconds(deathDuration);
       GameManager.Instance.ShowGameOverScreen();

    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip, volume); 
    }
}