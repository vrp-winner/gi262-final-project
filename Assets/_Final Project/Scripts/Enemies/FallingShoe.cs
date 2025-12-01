using UnityEngine;
using System.Collections;

public class FallingShoe : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int damageToDeal = 1;      
    [SerializeField] private float DestroyShoe = 5f;

    [Header("Fall Logic")]
    //[SerializeField] private float waitBeforeFalling = 1f; 
    private Rigidbody2D rb;
    
    [Header("VFX")]
    [SerializeField] private GameObject ImpactEffect;
    [SerializeField] private AudioClip fallSound;
    [Range(0f, 1f)][SerializeField] private float fallVolume = 1.0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
            //StartCoroutine(FallRoutine());
            Destroy(gameObject, DestroyShoe);
        }
    }

    public void Drop()
    {
        if (rb != null)
        {
            rb.isKinematic = false; 
        }
        Destroy(gameObject, DestroyShoe);
    }
    //private IEnumerator FallRoutine()
    //{
    //    yield return new WaitForSeconds(waitBeforeFalling);

    //    if (rb != null)
    //    {
    //        rb.isKinematic = false;
    //    }
    //}
    private void OnCollisionEnter2D(Collision2D collision)

    {
        Vector2 impactPoint = collision.contacts[0].point;
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damageToDeal);
            }
            SpawnImpactEffect(impactPoint);
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
            SpawnImpactEffect(impactPoint);

        }
        if (fallSound != null)
        {
            AudioSource.PlayClipAtPoint(fallSound, transform.position);
        }

    }
    private void SpawnImpactEffect(Vector2 position)
    {
       
        if (ImpactEffect != null)
        {

            Instantiate(ImpactEffect, position, Quaternion.identity);
        }
    }
}