using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [TextArea(2,6)]
    public List<string> lines = new List<string>();

    [Header("Options")]
    public bool triggerOnStart = false;
    public bool triggerOnPlayerEnter = false;
    private bool hasTriggered = false;

    private void Start()
    {
        if (triggerOnStart)
        {
            TriggerDialogue();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggerOnPlayerEnter || hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            //Debug.Log("Player entered Boss trigger — starting dialogue");
            TriggerDialogue();
        }
    }

    public void TriggerDialogue()
    {
        if (DialogueUI.Instance != null)
        {
            //DialogueUI.Instance.StartDialogue(lines);
        }
        else
        {
            //Debug.LogWarning("DialogueTrigger: DialogueUI.Instance is null");
        }
    }
}