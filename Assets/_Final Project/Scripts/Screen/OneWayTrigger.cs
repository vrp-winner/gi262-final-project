using UnityEngine;
using Unity.Cinemachine;
using System;

public class OneWayTrigger : MonoBehaviour
{
    [Header("Blocker Settings")]
    [SerializeField] private Collider2D colliderToActivate;

    [Header("Camera Settings")]
    [SerializeField] private CinemachineConfiner2D cinemachineConfiner;
    [SerializeField] private Collider2D newCameraBoundary;

    [Header("Boss Trigger")]
    [SerializeField] private BossController bossToTrigger;

    [Header("Debug")]
    [SerializeField] private bool showDebugMessages = true;


    private bool hasTriggered = false;

    private void Start()
    {
        if (colliderToActivate != null)
        {
            colliderToActivate.enabled = false;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            LockTheRoom();
            TriggerDialogue();
            gameObject.SetActive(false);
        }
    }

    private void LockTheRoom()
    {
        if (colliderToActivate != null)
        {
            colliderToActivate.enabled = true;
            if (showDebugMessages) Debug.Log("BLOCKER!");
        }

        if (cinemachineConfiner != null && newCameraBoundary != null)
        {
            cinemachineConfiner.enabled = false;
            cinemachineConfiner.BoundingShape2D = newCameraBoundary;
            cinemachineConfiner.InvalidateBoundingShapeCache();
            cinemachineConfiner.enabled = true;
            if (showDebugMessages) Debug.Log($"BOUNDARY SWITCHED TO: {newCameraBoundary.name}");
        }
    }

    public void TriggerDialogue()
    {
        if (DialogueUI.Instance != null)
        {
            DialogueUI.Instance.StartDialogue(StartTheBossFight);
        }
        else
        {
            StartTheBossFight();
        }
    }

    public void StartTheBossFight()
    {
        if (showDebugMessages)
            Debug.Log("Dialogue finished! Starting boss fight!");

        if (bossToTrigger != null)
        {
            bossToTrigger.StartBossFight();
        }
    }
}