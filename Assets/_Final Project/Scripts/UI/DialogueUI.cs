using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    [Header("UI refs")]
    public GameObject panel;
    public TMP_Text dialogueText;
    public Button nextButton;
    public Button skipButton;

    [Header("Settings")]
    public float typeSpeed = 0.01f;

    private List<string> lines = new List<string>();
    private int index = 0;
    private Coroutine typingCoroutine;

    public event Action OnDialogueFinished;
    private Action onDialogueFinishedCallback;


    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
        panel.SetActive(false);

        nextButton.onClick.AddListener(OnNextClicked);
        skipButton.onClick.AddListener(OnSkipClicked);
    }

    public void StartDialogue(List<string> lines, Action onDialogueFinished)
    {
        if (lines == null || lines.Count == 0)
        {
            Debug.LogWarning("DialogueUI: empty dialogueLines");
            OnDialogueFinished?.Invoke();
            return;
        }

        this.lines = new List<string>(lines);
        index = 0;
        panel.SetActive(true);
        ShowLine(index);
        
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            player.canMove = false;
            player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
        this.onDialogueFinishedCallback = onDialogueFinished;

    }

    private void ShowLine(int i)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(lines[i]));
    }

    private IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        typingCoroutine = null;
    }

    private void OnNextClicked()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = lines[index];
            typingCoroutine = null;
            return;
        }

        index++;
        if (index >= lines.Count)
        {
            EndDialogue();
        }
        else
        {
            ShowLine(index);
        }
    }

    private void OnSkipClicked()
    {
        EndDialogue();
    }

    private void EndDialogue()
    {
        panel.SetActive(false);
        
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            player.canMove = true;

            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        if (onDialogueFinishedCallback != null)
        {
            onDialogueFinishedCallback.Invoke(); // สั่ง "เริ่มสู้"
            onDialogueFinishedCallback = null; // เคลียร์ทิ้ง
        }

        OnDialogueFinished?.Invoke();
    }
}
