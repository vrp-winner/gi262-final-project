using UnityEngine;
using System.Collections;

public class ShoeAttack : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private float Speed = 15f;
    [SerializeField] private float DestroyShoe = 4f;
    [SerializeField] private int damage = 1;
    private Transform playerTarget;
    private Rigidbody2D rb;

    public void SetPlayerTarget(Transform target)
    {
        playerTarget = target;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(AttackRoutine());
        Destroy(gameObject, DestroyShoe);
    }

    private IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(waitTime);
        if (playerTarget != null)
        {
            Vector2 targetDirection = (playerTarget.position - transform.position).normalized;
            rb.velocity = targetDirection * Speed;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Hit");
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }
}


    
