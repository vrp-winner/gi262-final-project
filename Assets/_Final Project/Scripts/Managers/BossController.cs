using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class BossController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Animator anim;

    [Header("Attack 1 (Shoe)")]
    [SerializeField] private GameObject shoePrefab;
    [SerializeField] private Transform[] shoeSpawnPoints;
    [SerializeField] private Transform[] shoeWaitingPoints;
    [SerializeField] private float Coodown = 3f;
    [SerializeField] private float animDuration = 1.0f;

    [Header("Attack 2 (FallingShoe)")]
    [SerializeField] private GameObject fallingShoePrefab;
    [SerializeField] private Transform[] fallingShoeSpawnPoints;

    [Header("Attack 3 (ShoeDrops)")]
    [SerializeField] private GameObject Indicator;
    [SerializeField] private GameObject ShoeDropPrefab;
    [SerializeField] private Transform DropSpawnPoint_Left;
    [SerializeField] private Transform DropSpawnPoint_Right;
    [SerializeField] private float DropSpawnWidth = 10f;
    [SerializeField] private float DropWarningDuration = 2f;
    [SerializeField] private float attack3Duration = 8f;
    [SerializeField] private float DropRate = 0.2f;

    [Header("Phase Timings (in Seconds)")]
    [SerializeField] private float phase2StartTime = 60f;
    [SerializeField] private float phase3StartTime = 120f;
    [SerializeField] private float FightEndTime = 240f;

    [Header("UI")]
    [SerializeField] private Slider timerBarSlider;
    [SerializeField] private GameObject bossFightUIPanel;

    [Header("Transition")]
    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip shoeAttackSpawnSound;
    [Range(0f, 1f)][SerializeField] private float shoeAttackSpawnVolum = 1f;

    [SerializeField] private AudioClip fallingShoeSpawnSound;
    [Range(0f, 1f)][SerializeField] private float fallingShoeSpawnVolum = 1f;

    [SerializeField] private AudioClip shoeDropSound;
    [Range(0f, 1f)][SerializeField] private float shoeDropVolum = 1f;
   
    [Header("BGM Settings")]
    [SerializeField] private AudioSource bgmAudioSource; 
    [SerializeField] private float normalSpeed = 1f;     
    [SerializeField] private float phase2Speed = 1.2f;   
    [SerializeField] private float phase3Speed = 1.5f;

    private AudioSource audioSource;
    private int lastAttackIndex = -1;
    private bool isFighting = false;
    private float fightTimer = 0f;

    private List<ShoeAttack> currentShoes = new List<ShoeAttack>();
    private List<FallingShoe> currentFallingShoes = new List<FallingShoe>();

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (isFighting)
        {
            fightTimer += Time.deltaTime;

            if (timerBarSlider != null)
            {
                timerBarSlider.value = Mathf.Clamp(fightTimer, 0f, FightEndTime);
            }
            HandleBGMSpeed();
        }
    }

    public void StartBossFight()
    {
        if (isFighting) return;

        if (bossFightUIPanel != null) bossFightUIPanel.SetActive(true);

        if (bgmAudioSource != null)
        {
            bgmAudioSource.Play();
        }

        isFighting = true;
        fightTimer = 0f;
        lastAttackIndex = -1;
        Debug.Log("BOSS FIGHT HAS BEGUN!");

        if (timerBarSlider != null)
        {
            timerBarSlider.maxValue = FightEndTime;
            timerBarSlider.value = 0;
        }
        StartCoroutine(AttackLoop());
    }

    private IEnumerator AttackLoop()
    {
        while (isFighting)
        {
            if (fightTimer >= FightEndTime)
            {
                if (timerBarSlider != null) timerBarSlider.value = FightEndTime;
                EndBossFight();
                yield break;
            }

            int nextAttack = ChooseNextAttack();
            lastAttackIndex = nextAttack;

            if (nextAttack == 3) yield return StartCoroutine(Attack3_ShoeDrops());
            else
            {
                if (nextAttack == 1) yield return StartCoroutine(Attack1_Shoe_Sequence());
                if (nextAttack == 2) yield return StartCoroutine(Attack2_FallingShoe());
                float currentCooldown = Coodown;

                if (fightTimer >= phase3StartTime)
                {
                    currentCooldown = Coodown * 0.5f;
                }
                else if (fightTimer >= phase2StartTime)
                {
                    currentCooldown = Coodown * 0.8f;
                }
                yield return new WaitForSeconds(currentCooldown);
            }
        }
    }

    private IEnumerator Attack1_Shoe_Sequence()
    {
        if (anim != null) anim.SetTrigger("Att_1");
        else
        {
            SpawnShoeEvent();
            yield return new WaitForSeconds(0.5f);
            ThrowShoeEvent();
        }
        yield return new WaitForSeconds(animDuration);
    }

    public void SpawnShoeEvent()
    {
        currentShoes.Clear();
        PlaySound(shoeAttackSpawnSound);

        int shoeCount = 1;
        if (fightTimer >= phase3StartTime) shoeCount = 3;
        else if (fightTimer >= phase2StartTime) shoeCount = 2;

        for (int i = 0; i < shoeCount; i++)
        {
            Transform randomSpawnPoint = shoeSpawnPoints[Random.Range(0, shoeSpawnPoints.Length)];
            GameObject shoeInstance = Instantiate(shoePrefab, randomSpawnPoint.position, Quaternion.identity);
            ShoeAttack shoeScript = shoeInstance.GetComponent<ShoeAttack>();

            shoeScript.SetPlayerTarget(playerTransform);

            if (shoeWaitingPoints.Length > i)
            {
                shoeScript.MoveToWaitPoint(shoeWaitingPoints[i]);
            }
            currentShoes.Add(shoeScript);
        }
    }

    public void ThrowShoeEvent()
    {
        foreach (ShoeAttack shoe in currentShoes)
        {
            if (shoe != null) shoe.LaunchAtPlayer();
        }
        currentShoes.Clear();
    }

    private int ChooseNextAttack()
    {
        int maxAttackID;

        if (fightTimer >= phase3StartTime) maxAttackID = 3;
        else if (fightTimer >= phase2StartTime) maxAttackID = 2;
        else maxAttackID = 1;

        if (maxAttackID == 1) return 1;

        int newAttackIndex;
        do { newAttackIndex = Random.Range(1, maxAttackID + 1); }
        while (newAttackIndex == lastAttackIndex);

        return newAttackIndex;
    }

    private void EndBossFight()
    {
        isFighting = false;
        Debug.Log("BOSS FIGHT ENDED!");
        StopAllCoroutines();

        ClearAllAttacks();
        if (bgmAudioSource != null)
        {
            bgmAudioSource.Stop();
        }

        StartCoroutine(LoadSceneSequence());
    }

    private void ClearAllAttacks()
    {
        var shoes = FindObjectsOfType<ShoeAttack>();
        foreach (var s in shoes) { if (s != null) Destroy(s.gameObject); }

        var fallingShoes = FindObjectsOfType<FallingShoe>();
        foreach (var f in fallingShoes) { if (f != null) Destroy(f.gameObject); }

        var indicators = FindObjectsOfType<BlinkingIndicator>();
        foreach (var i in indicators) { if (i != null) Destroy(i.gameObject); }
    }

    private IEnumerator LoadSceneSequence()
    {
        if (fadePanel != null) fadePanel.blocksRaycasts = true;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            if (fadePanel != null) fadePanel.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        if (fadePanel != null) fadePanel.alpha = 1f;
        Time.timeScale = 0f;
        SceneManager.LoadScene("EndingScene");
    }

    private IEnumerator Attack2_FallingShoe()
    {
        if (anim != null) anim.SetTrigger("Att_2");
        else
        {
            SpawnFallingShoeEvent();
            yield return new WaitForSeconds(0.5f);
            DropFallingShoeEvent();
        }
        yield return new WaitForSeconds(animDuration);
    }

    public void SpawnFallingShoeEvent()
    {
        Debug.Log(">>> เสก <<<");

        currentFallingShoes.Clear();
        PlaySound(fallingShoeSpawnSound);

        if (fallingShoePrefab != null && fallingShoeSpawnPoints.Length > 0)
        {
            foreach (Transform spawnPoint in fallingShoeSpawnPoints)
            {
                GameObject shoeObj = Instantiate(fallingShoePrefab, spawnPoint.position, Quaternion.identity);
                FallingShoe shoeScript = shoeObj.GetComponent<FallingShoe>();

                if (shoeScript != null)
                {
                    currentFallingShoes.Add(shoeScript);
                }
            }
        }
    }

    public void DropFallingShoeEvent()
    {
        Debug.Log(">>> สั่งร่วงแล้วจ้า! <<<");
        foreach (FallingShoe shoe in currentFallingShoes)
        {
            if (shoe != null) shoe.Drop();
        }
        currentFallingShoes.Clear();
    }

    private IEnumerator Attack3_ShoeDrops()
    {
        if (anim != null) anim.SetTrigger("Att_3");

        int sideIndex = Random.Range(0, 2);
        Transform spawnArea = (sideIndex == 0) ? DropSpawnPoint_Left : DropSpawnPoint_Right;

        GameObject indicator = Instantiate(Indicator, spawnArea.position, Quaternion.identity);
        BlinkingIndicator blinkScript = indicator.GetComponent<BlinkingIndicator>();
        if (blinkScript != null) blinkScript.StartBlinking(DropWarningDuration);

        yield return new WaitForSeconds(DropWarningDuration);

        yield return StartCoroutine(RainSide(spawnArea));
    }

    private IEnumerator RainSide(Transform spawnArea)
    {
        PlaySound(shoeDropSound);
        float timer = 0f;

        while (timer < attack3Duration)
        {
            float randomX = Random.Range(-DropSpawnWidth / 2, DropSpawnWidth / 2);
            Vector2 spawnPos = new Vector2(spawnArea.position.x + randomX, spawnArea.position.y);

            GameObject rainShoe = Instantiate(ShoeDropPrefab, spawnPos, Quaternion.identity);

            FallingShoe shoeScript = rainShoe.GetComponent<FallingShoe>();

            if (shoeScript != null)
            {
                shoeScript.Drop();
            }

            yield return new WaitForSeconds(DropRate);
            timer += DropRate;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    private void HandleBGMSpeed()
    {
        if (bgmAudioSource == null) return;

        if (fightTimer >= phase3StartTime)
        {
            bgmAudioSource.pitch = phase3Speed; 
        }
        else if (fightTimer >= phase2StartTime)
        {
            bgmAudioSource.pitch = phase2Speed; 
        }
        else
        {
            bgmAudioSource.pitch = normalSpeed; 
        }
    }
}