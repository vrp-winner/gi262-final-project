using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

[RequireComponent(typeof(AudioSource))]
public class ButtonSound : MonoBehaviour, IPointerEnterHandler 
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip hoverSound;

    //[Header("Settings")]
    //[Range(0f, 1f)][SerializeField] private float volume = 1f;

    private AudioSource audioSource;
    private Button btn;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        btn = GetComponent<Button>();
    }

    public void PlayClickSound()
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound, 0.2f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (btn != null && !btn.interactable) return;

        if (hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound, 0.2f);
        }
    }
}