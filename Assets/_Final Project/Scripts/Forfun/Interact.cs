using UnityEngine;

public class Interact : MonoBehaviour, IInteractable
{
    [Header("Dialogue")]
    [SerializeField] private DialogueTree dialogueTreeToShow;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    public void _Interact()
    {

        if (dialogueTreeToShow != null && DialogueUI.Instance != null)
        {
            DialogueUI.Instance.StartDialogue(null);
        }


        if (animator != null)
        {
            animator.SetTrigger("DoTheThing");
        }
    }
    [SerializeField] private GameObject interactPrompt;

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
}
