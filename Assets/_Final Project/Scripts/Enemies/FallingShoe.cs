using UnityEngine;
using System.Collections;

public class FallingShoe : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int damageToDeal = 1;      
    [SerializeField] private float DestroyShoe = 5f;

    [Header("Fall Logic")]
    [SerializeField] private float waitBeforeFalling = 1f; 
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
            StartCoroutine(FallRoutine());
            Destroy(gameObject, DestroyShoe);
        }
    }
    private IEnumerator FallRoutine()
    {
        yield return new WaitForSeconds(waitBeforeFalling);

        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damageToDeal);
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
