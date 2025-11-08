using UnityEngine;
using System.Collections.Generic; 
public class HealthPointUI : MonoBehaviour
{
    public static HealthPointUI Instance { get; private set; }

    [Header("Hp References")]
    [SerializeField] private List<GameObject> Hppoint;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); }
        else { Instance = this; }
    }

    public void SetupHP(int maxHp)
    {
        for (int i = 0; i < Hppoint.Count; i++)
        {
            if (i < maxHp)
            {
                Hppoint[i].SetActive(true);
            }
            else
            {
                Hppoint[i].SetActive(false);
            }
        }
    }

    public void UpdateHealth(int currentHp)
    {
        for (int i = 0; i < Hppoint.Count; i++)
        {
            if (i < currentHp)
            {
                Hppoint[i].SetActive(true);
            }
            else
            {
                Hppoint[i].SetActive(false);
            }
        }
    }
}
