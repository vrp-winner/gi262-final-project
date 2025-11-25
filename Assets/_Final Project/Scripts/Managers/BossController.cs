using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    
    [Header("Attack 1 (Shoe)")]
    [SerializeField] private GameObject shoePrefab;
    [SerializeField] private Transform[] shoeSpawnPoints;
    [SerializeField] private Transform[] shoeWaitingPoints;
    [SerializeField] private float Coodown = 3f;
   
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
    [SerializeField] private float phase2StartTime = 60f ;  
    [SerializeField] private float phase3StartTime = 120f ; 
    [SerializeField] private float FightEndTime = 240f ;

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

    private AudioSource audioSource;


    private int lastAttackIndex = -1; 
    private bool isFighting = false;
    private float fightTimer = 0f;

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
                //timerBarSlider.value = fightTimer;
                timerBarSlider.value = Mathf.Clamp(fightTimer, 0f, FightEndTime);
            }

            //if (fightTimer >= FightEndTime)
            //{
            //    EndBossFight();
            //}
        }
    }
    
    public void StartBossFight()
    {
        if (isFighting) return;

        if (bossFightUIPanel != null)
        {
            bossFightUIPanel.SetActive(true);
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

            if (nextAttack == 3)
            {
                yield return StartCoroutine(Attack3_ShoeDrops());
            }
            else
            {
                if (nextAttack == 1) yield return StartCoroutine(Attack1_Shoe());
                if (nextAttack == 2) yield return StartCoroutine(Attack2_FallingShoe());

                yield return new WaitForSeconds(Coodown);
            }
        }
    }

    private int ChooseNextAttack()
    {
        int maxAttackID; 
        
        if (fightTimer >= phase3StartTime) 
        {
            maxAttackID = 3;
        }
        else if (fightTimer >= phase2StartTime) 
        {
            maxAttackID = 2;
        }
        else 
        {
            maxAttackID = 1;
        }

        
        if (maxAttackID == 1)
        {
            return 1;
        }
        int newAttackIndex;
        do
        {
           
            newAttackIndex = Random.Range(1, maxAttackID + 1);
        }
        while (newAttackIndex == lastAttackIndex); 

        return newAttackIndex;
    }
    private void EndBossFight()
    {
        isFighting = false;
        Debug.Log("BOSS FIGHT ENDED! (Time limit reached)");
        //gameObject.SetActive(false);
        //Time.timeScale = 0f;
        //SceneManager.LoadScene("EndingScene");
        StopAllCoroutines();
        

        StartCoroutine(LoadSceneSequence());
    }

    private IEnumerator LoadSceneSequence()
    {
        if (fadePanel != null)
        {
            fadePanel.blocksRaycasts = true;
        }

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime; 
            if (fadePanel != null)
            {
                fadePanel.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            }
            yield return null;
        }

        fadePanel.alpha = 1f;
        Time.timeScale = 0f; 
        SceneManager.LoadScene("EndingScene");
    }


    private IEnumerator Attack1_Shoe()
    {
        Debug.Log("Boss uses Attack 1: Shoe!");
        int shoeCount = 1;
        if (fightTimer >= phase3StartTime) 
        {
            shoeCount = 2;
        }
        else if (fightTimer >= phase2StartTime) 
        {
            shoeCount = 2;
        }
        for (int i = 0; i < shoeCount; i++)
        {
            PlaySound(shoeAttackSpawnSound);
            Transform randomSpawnPoint = shoeSpawnPoints[Random.Range(0, shoeSpawnPoints.Length)];
            GameObject shoeInstance = Instantiate(shoePrefab, randomSpawnPoint.position, Quaternion.identity);
            ShoeAttack shoe = shoeInstance.GetComponent<ShoeAttack>(); 
            shoe.SetPlayerTarget(playerTransform); 
            shoe.SetRestPoints(shoeWaitingPoints);
            yield return new WaitForSeconds(0.2f);
        }
        yield return null;
    }

    private IEnumerator Attack2_FallingShoe()
    {
        if (fallingShoePrefab == null || fallingShoeSpawnPoints.Length == 0)
        {
            
            yield break;
        }

        foreach (Transform spawnPoint in fallingShoeSpawnPoints)
        {
            PlaySound(fallingShoeSpawnSound);
            Instantiate(fallingShoePrefab, spawnPoint.position, Quaternion.identity);
        }
    }

    private IEnumerator Attack3_ShoeDrops()
    {
        int sideIndex = Random.Range(0, 2);
        Transform spawnArea = (sideIndex == 0) ? DropSpawnPoint_Left : DropSpawnPoint_Right;

        
       
        GameObject indicator = Instantiate(Indicator, spawnArea.position, Quaternion.identity);

        BlinkingIndicator blinkScript = indicator.GetComponent<BlinkingIndicator>();
        if (blinkScript != null)
        {
           
            blinkScript.StartBlinking(DropWarningDuration);
        }

        
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
            
            Instantiate(ShoeDropPrefab, spawnPos, Quaternion.identity);
            
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
}
