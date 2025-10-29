using System.Collections;
using UnityEngine;

public class BossFightManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private Boss boss;

    private int currentWave = 0;
    private bool isFighting = false;

    private void Start()
    {
        DialogueUI.Instance.OnDialogueFinished += OnDialogueCompleteFromUI;
    }
    
    private void OnDialogueCompleteFromUI()
    {
        DialogueUI.Instance.OnDialogueFinished -= OnDialogueCompleteFromUI;
        
        boss.HideBoss();
        StartFight(); 
    }

    public void StartFight()
    {
        if (isFighting) return;
        isFighting = true;
        currentWave = 0;

        Debug.Log("Fight started!");
        StartNextWave();
    }

    public void StartNextWave()
    {
        currentWave++;
        StartCoroutine(HandleWave(currentWave));
    }

    private IEnumerator HandleWave(int waveNumber)
    {
        Debug.Log($"Wave {waveNumber} started!");

        yield return StartCoroutine(boss.SpawnWave(waveNumber));

        yield return new WaitUntil(() =>
        {
            bool noGhosts = FindObjectsByType<Ghost>(FindObjectsSortMode.None).Length == 0;
            bool playerDead = (player == null || player.IsDead);
            return noGhosts || playerDead;
        });

        if (player == null || player.IsDead)
        {
            Debug.Log("❌ Player is dead during wave.");
            EndGame(false);
            yield break;
        }

        Debug.Log($"Wave {waveNumber} finished!");
        if (waveNumber >= 3)
        {
            EndGame(true);
        }
        else
        {
            yield return new WaitForSeconds(1f);
            StartNextWave();
        }
    }

    public void OnPlayerHit()
    {
        if (player == null) return;

        player.TakeDamage(1);

        if (player == null || player.IsDead)
        {
            EndGame(false);
        }
    }

    public void EndGame(bool isWin)
    {
        isFighting = false;
        boss.StopSpawning();
        Debug.Log(isWin ? "You Win!" : "You Lose!");
    }
}