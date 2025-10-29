using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Boss Settings")]
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private bool isSpawning = false;

    public IEnumerator SpawnWave(int waveNumber)
    {
        if (isSpawning) yield break;

        Player player = FindFirstObjectByType<Player>();
        if (player == null || player.IsDead)
        {
            Debug.LogWarning($"SpawnWave cancelled at start — Player not found or dead (wave {waveNumber}).");
            yield break;
        }

        isSpawning = true;
        int ghostCount = waveNumber + 2;

        Debug.Log($"Spawning wave {waveNumber}...");

        for (int i = 0; i < ghostCount; i++)
        {
            if (player == null || !player.gameObject.activeSelf || player.IsDead)
            {
                Debug.LogWarning($"SpawnWave stopped early — Player is dead (wave {waveNumber}, after {i} ghosts).");
                isSpawning = false;
                yield break;
            }

            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject ghost = Instantiate(ghostPrefab, point.position, Quaternion.identity);

            // ✅ ผูกเป้าหมายกับ player
            Ghost ghostScript = ghost.GetComponent<Ghost>();
            if (ghostScript != null)
            {
                ghostScript.Init(player.transform);
            }
            else
            {
                Debug.LogWarning("Spawned ghost prefab missing Ghost script!");
            }

            yield return new WaitForSeconds(1f);
        }

        yield return null;
        Debug.Log($"Wave {waveNumber} finished spawning!");
        isSpawning = false;
    }

    public void StopSpawning()
    {
        StopAllCoroutines();
        isSpawning = false;
    }

    public void HideBoss()
    {
        gameObject.SetActive(false);
    }
}
