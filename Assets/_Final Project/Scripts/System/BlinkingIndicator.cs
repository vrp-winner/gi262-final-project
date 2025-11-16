using UnityEngine;
using System.Collections;


[RequireComponent(typeof(SpriteRenderer))]
public class BlinkingIndicator : MonoBehaviour
{
    [Header("Blink Settings")]
    [SerializeField] private float blinkInterval = 0.2f; 

    private SpriteRenderer spriteRenderer;

    
    public void StartBlinking(float totalDuration)
    {
       spriteRenderer = GetComponent<SpriteRenderer>();

       StartCoroutine(BlinkRoutine(totalDuration));
    }

    private IEnumerator BlinkRoutine(float duration)
    {
        float timer = 0f;
        bool isVisible = true;

        while (timer < duration)
        {
            
            spriteRenderer.enabled = isVisible;
            isVisible = !isVisible; 

            yield return new WaitForSeconds(blinkInterval);

            timer += blinkInterval;
        }

        Destroy(gameObject);
    }
}