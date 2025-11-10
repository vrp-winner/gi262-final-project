using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Cat : MonoBehaviour, IInteractable

{
    [Header("Sound")]
    [SerializeField] private AudioClip meowSound;
    [SerializeField] private GameObject interactPrompt;
    private AudioSource audioSource;

        private bool canInteract = true;
    private void Start()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
   
        if (other.CompareTag("Player") && interactPrompt != null)
        {
            interactPrompt.SetActive(true); 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {

        if (other.CompareTag("Player") && interactPrompt != null)
        {
            interactPrompt.SetActive(false); 
        }
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

        public void _Interact()
    {
        if (canInteract && meowSound != null)
        {
           
            audioSource.PlayOneShot(meowSound);

        }
    }
}