using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject shoePrefab;
    [SerializeField] private Transform[] shoeSpawnPoints;

    [Header("Attack Settings")]
    [SerializeField] private float Coodown = 3f; 

    
    [Header("Phase Timings (in Seconds)")]
    [SerializeField] private float phase2StartTime = 60f;  
    [SerializeField] private float phase3StartTime = 120f; 
    [SerializeField] private float FightEndTime = 240f;

    [SerializeField] private GameObject fallingShoePrefab;
    [SerializeField] private Transform[] fallingShoeSpawnPoints;

    private int lastAttackIndex = -1; 
    private bool isFighting = false;
    private float fightTimer = 0f; 

    
    public void StartBossFight()
    {
        if (isFighting) return;

        isFighting = true;
        fightTimer = 0f; 
        lastAttackIndex = -1; 
        Debug.Log("BOSS FIGHT HAS BEGUN!");
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
                    Attack2_Placeholder();
                    break;
                case 3:
                    Attack3_Placeholder();
                    break;
            }
       
            yield return new WaitForSeconds(Coodown);
                      fightTimer += Coodown;
            if (fightTimer >= FightEndTime)
            {
                EndBossFight();
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
        gameObject.SetActive(false); 
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
            shoeInstance.GetComponent<ShoeAttack>().SetPlayerTarget(playerTransform);
            yield return new WaitForSeconds(0.2f);
        }
        yield return null;
    }

    private void Attack2_Placeholder()
    {
        if (fallingShoePrefab == null || fallingShoeSpawnPoints.Length == 0)
        {
            Debug.LogWarning("Attack 2 is not set up! Check Prefab/SpawnPoints in Inspector.");
            return;
        }

        foreach (Transform spawnPoint in fallingShoeSpawnPoints)
        {
            Instantiate(fallingShoePrefab, spawnPoint.position, Quaternion.identity);
        }
    }

    private void Attack3_Placeholder()
    {
        Debug.Log("Boss uses Attack 3: Placeholder!");
    }
}
