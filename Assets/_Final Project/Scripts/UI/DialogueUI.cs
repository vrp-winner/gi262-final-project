using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    public DialogueTree tree;
    public DialogueNode currentNode;

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;


    [Header("Manual Buttons")]
    [SerializeField] private GameObject choiceButton1;
    [SerializeField] private GameObject choiceButton2; 

    private Button button1;
    private TextMeshProUGUI button1_Text;
    private Button button2;
    private TextMeshProUGUI button2_Text;

    private Action onDialogueFinishedCallback;
    private List<string> currentChoiceKeys;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); }
        else { Instance = this; }

        LoadConversation();

        if (choiceButton1 != null)
        {
            button1 = choiceButton1.GetComponent<Button>();
            button1_Text = choiceButton1.GetComponentInChildren<TextMeshProUGUI>();
        }
        if (choiceButton2 != null)
        {
            button2 = choiceButton2.GetComponent<Button>();
            button2_Text = choiceButton2.GetComponentInChildren<TextMeshProUGUI>();
        }

        dialoguePanel.SetActive(false);
    }

    private void LoadConversation()
    {
        tree = new DialogueTree();
        DialogueNode start = new DialogueNode("...", "Boss");
        DialogueNode playerReply = new DialogueNode("...", "Player");
        DialogueNode bossFinal = new DialogueNode("...", "Boss");
        tree.rootNode = start;
        start.AddChoice("... (ตอบแบบที่ 1) ...", playerReply);
        start.AddChoice("... (ตอบแบบที่ 2) ...", bossFinal);
        playerReply.AddChoice("... (ตอบกลับ) ...", bossFinal);
    }

    public void StartDialogue(Action onDialogueFinished)
    {
        this.onDialogueFinishedCallback = onDialogueFinished;
        currentNode = tree.rootNode;

        dialoguePanel.SetActive(true);
        ShowNode(currentNode);
    }

    private void ShowNode(DialogueNode node)
    {
        currentNode = node;
        speakerNameText.text = node.speakerName;
        dialogueText.text = node.dialogueText;

        button1.onClick.RemoveAllListeners();
        button2.onClick.RemoveAllListeners();

        choiceButton1.SetActive(false);
        choiceButton2.SetActive(false);

        currentChoiceKeys = new List<string>(node.nexts.Keys);

        if (currentChoiceKeys.Count > 0)
        {
            choiceButton1.SetActive(true); 
            button1_Text.text = currentChoiceKeys[0];
            button1.onClick.AddListener(() =>
            { 
                SelectChoice(0); 
            });
        }

        
        if (currentChoiceKeys.Count > 1)
        {
            choiceButton2.SetActive(true);
            button2_Text.text = currentChoiceKeys[1]; 
            button2.onClick.AddListener(() => { 
                SelectChoice(1); 
            });
        }
    }

    
    public void SelectChoice(int index)
    {
        if (index < 0 || index >= currentChoiceKeys.Count) return;
        string choiceKey = currentChoiceKeys[index];
        DialogueNode nextNode = currentNode.nexts[choiceKey];
        if (nextNode.nexts.Count == 0)
        {
            ShowNode(nextNode);
            EndDialogue();
        }
        else
        {
            ShowNode(nextNode);
        }
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        if (onDialogueFinishedCallback != null)
        {
            onDialogueFinishedCallback.Invoke();
            onDialogueFinishedCallback = null;
        }
    }
}