using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject shoePrefab;
    [SerializeField] private Transform[] shoeSpawnPoints;
    [SerializeField] private Transform[] shoeWaitingPoints;
    [SerializeField] private GameObject bossFightUIPanel;

    [Header("Attack Settings")]
    [SerializeField] private float Coodown = 3f; 
    
    [Header("Phase Timings (in Seconds)")]
    [SerializeField] private float phase2StartTime = 60f ;  
    [SerializeField] private float phase3StartTime = 120f ; 
    [SerializeField] private float FightEndTime = 240f ;

    [Header("UI")]
    [SerializeField] private Slider timerBarSlider;

    [SerializeField] private GameObject fallingShoePrefab;
    [SerializeField] private Transform[] fallingShoeSpawnPoints;

    private int lastAttackIndex = -1; 
    private bool isFighting = false;
    private float fightTimer = 0f;

    private void Update()
    {
        if (isFighting)
        {
            fightTimer += Time.deltaTime;

            if (timerBarSlider != null)
            {
                timerBarSlider.value = fightTimer;
            }

            if (fightTimer >= FightEndTime)
            {
                EndBossFight();
            }
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
            int nextAttack = ChooseNextAttack();
            lastAttackIndex = nextAttack; 
          
            switch (nextAttack)
            {
                case 1:
                    StartCoroutine(Attack1_Shoe());
                    break;
                case 2:
                    FallingShoe();
                    break;
                case 3:
                    Attack3();
                    break;
            }
       
            yield return new WaitForSeconds(Coodown);
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
        gameObject.SetActive(false);
        Time.timeScale = 0f;
        SceneManager.LoadScene("EndingScene");
    }


    private IEnumerator Attack1_Shoe()
    {
        Debug.Log("Boss uses Attack 1: Shoe!");
        int shoeCount = 1;
        if (fightTimer >= phase3StartTime) 
        {
            shoeCount = 3;
        }
        else if (fightTimer >= phase2StartTime) 
        {
            shoeCount = 2;
        }
        for (int i = 0; i < shoeCount; i++)
        {
            Transform randomSpawnPoint = shoeSpawnPoints[Random.Range(0, shoeSpawnPoints.Length)];
            GameObject shoeInstance = Instantiate(shoePrefab, randomSpawnPoint.position, Quaternion.identity);
            ShoeAttack shoe = shoeInstance.GetComponent<ShoeAttack>(); 
            shoe.SetPlayerTarget(playerTransform); 
            shoe.SetRestPoints(shoeWaitingPoints);
            yield return new WaitForSeconds(0.2f);
        }
        yield return null;
    }

    private void FallingShoe()
    {
        if (fallingShoePrefab == null || fallingShoeSpawnPoints.Length == 0)
        {
            Debug.Log("Boss uses Attack 2: FallingShoe!");
            return;
        }

        foreach (Transform spawnPoint in fallingShoeSpawnPoints)
        {
            Instantiate(fallingShoePrefab, spawnPoint.position, Quaternion.identity);
        }
    }

    private void Attack3()
    {
       //
    }
}
