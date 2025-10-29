using UnityEngine;

public class Ghost : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float lifetime = 5f;

    private Vector2 moveDirection;
    private bool isLaunched = false;

    public void Init(Transform playerTarget)
    {
        moveDirection = (playerTarget.position - transform.position).normalized;
        isLaunched = true;

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (!isLaunched) return;

        transform.Translate(moveDirection * (speed * Time.deltaTime), Space.World);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player p = collision.gameObject.GetComponent<Player>();
            if (p != null && !p.IsDead)
            {
                Debug.Log("Ghost hit Player!");
                p.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}