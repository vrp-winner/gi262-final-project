using UnityEngine;
using System.Collections;


[RequireComponent(typeof(SpriteRenderer))]
public class BlinkingIndicator : MonoBehaviour
{
    [Header("Blink Settings")]
    [SerializeField] private float blinkInterval = 0.2f;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip blinkSound;
    [Range(0f, 1f)][SerializeField] private float blickVolum = 1f;


    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;


    public void StartBlinking(float totalDuration)
    {
       spriteRenderer = GetComponent<SpriteRenderer>();
       audioSource = GetComponent<AudioSource>();

        StartCoroutine(BlinkRoutine(totalDuration));
    }

    private IEnumerator BlinkRoutine(float duration)
    {
        float timer = 0f;
        bool isVisible = true;

        while (timer < duration)
        {

            spriteRenderer.enabled = isVisible;

            if (isVisible && blinkSound != null)
            {
                audioSource.PlayOneShot(blinkSound);
            }

            isVisible = !isVisible; 

            yield return new WaitForSeconds(blinkInterval);

            timer += blinkInterval;
        }

        Destroy(gameObject);
    }
}