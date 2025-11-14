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
    private Transform[] waitPoints;
    private Rigidbody2D rb;

    public void SetPlayerTarget(Transform target)
    {
        playerTarget = target;
    }
    
    public void SetRestPoints(Transform[] points)
    {
        waitPoints = points;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(AttackRoutine());
        Destroy(gameObject, DestroyShoe);
    }

    private IEnumerator AttackRoutine()
    {
        Transform nearestRest = GetNearestRestPoint();
        if (nearestRest != null)
        {
            Vector2 dir = (nearestRest.position - transform.position).normalized;
            rb.linearVelocity = dir * Speed;

            yield return new WaitUntil(() => Vector2.Distance(transform.position, nearestRest.position) < 0.5f);
            rb.linearVelocity = Vector2.zero;
        }
        
        yield return new WaitForSeconds(waitTime);
        if (playerTarget != null)
        {
            Vector2 targetDirection = (playerTarget.position - transform.position).normalized;
            rb.linearVelocity = targetDirection * Speed;
        }
    }
    
    private Transform GetNearestRestPoint()
    {
        if (waitPoints == null || waitPoints.Length == 0)
            return null;

        Transform nearest = waitPoints[0];
        float minDist = Vector2.Distance(transform.position, nearest.position);

        foreach (Transform t in waitPoints)
        {
            float dist = Vector2.Distance(transform.position, t.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = t;
            }
        }

        return nearest;
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