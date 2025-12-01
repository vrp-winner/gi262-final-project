using UnityEngine;
using System.Collections;

public class ShoeAttack : MonoBehaviour
{
    [Header("Settings")]
    //[SerializeField] private float waitTime = 1f;
    [SerializeField] private float Speed = 15f;
    [SerializeField] private float DestroyShoe = 4f;
    [SerializeField] private float floatSpeed = 5f;
    [SerializeField] private int damage = 1;

    [Header("Audio")]
    [SerializeField] private AudioClip throwSound;
    [Range(0f, 1f)][SerializeField] private float throwVolume = 1f;


    private Transform playerTarget;
    //private Transform[] waitPoints;
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private bool isLaunched = false;

    public void SetPlayerTarget(Transform target)
    {
        playerTarget = target;
    }
    
    //public void SetRestPoints(Transform[] points)
    //{
    //    waitPoints = points;
    //}

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //StartCoroutine(AttackRoutine());
        //Destroy(gameObject, DestroyShoe);
        audioSource = GetComponent<AudioSource>();
    }

    public void MoveToWaitPoint(Transform waitPoint)
    {
        StartCoroutine(GoToWaitPointRoutine(waitPoint));
    }


    private IEnumerator GoToWaitPointRoutine(Transform targetPoint)
    {
        if (targetPoint != null)
        {
            while (Vector2.Distance(transform.position, targetPoint.position) > 0.5f && !isLaunched)
            {
                Vector2 dir = (targetPoint.position - transform.position).normalized;
                rb.linearVelocity = dir * floatSpeed;
                yield return null;
            }

            if (!isLaunched)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    public void LaunchAtPlayer()
    {
        isLaunched = true;
        if (audioSource != null && throwSound != null)
        {
            audioSource.PlayOneShot(throwSound, throwVolume);
        }

        if (playerTarget != null)
        {
            Vector2 targetDirection = (playerTarget.position - transform.position).normalized;
            rb.linearVelocity = targetDirection * Speed;

            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        Destroy(gameObject, DestroyShoe);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Hit");
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}