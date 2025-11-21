using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;


public class EndingManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject creditsPanel;    
    [SerializeField] private RectTransform creditsText;

    [Header("Transition")]
    [SerializeField] private CanvasGroup fadePanel;   
    [SerializeField] private float fadeDuration = 1.5f; 

    [Header("Settings")]
    [SerializeField] private float scrollSpeed = 50f;    
    [SerializeField] private float stopPosY = 1500f;     
    [SerializeField] private string mainMenuName = "MainMenu";

    private bool startScrolling = false;

    private void Start()
    {
        Time.timeScale = 1f;
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
      
        if (fadePanel != null)
        {
            fadePanel.alpha = 1f; 
            StartCoroutine(FadeInRoutine());
        }

        if (DialogueUI.Instance != null)
        {
            DialogueUI.Instance.StartDialogue(OnDialogueFinished);
        }
        else
        {
            OnDialogueFinished();
        }
    }

    private void Update()
    {
        if (startScrolling && creditsText != null)
        {
            creditsText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

            if (creditsText.anchoredPosition.y > stopPosY)
            {
                GoToMainMenu();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && startScrolling)
        {
            GoToMainMenu();
        }
    }

    private void OnDialogueFinished()
    {
        Debug.Log("Dialogue Finished. Starting Credits...");
        creditsPanel.SetActive(true);
        startScrolling = true;       
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuName);
    }

    private IEnumerator FadeInRoutine()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (fadePanel != null)
            {
                fadePanel.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            }
            yield return null;
        }
        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;
        }
    }
}
