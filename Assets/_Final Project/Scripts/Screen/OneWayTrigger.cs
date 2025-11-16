using UnityEngine;
using Unity.Cinemachine;
using System.Collections; 
using System.Collections.Generic;
using System;

public class OneWayTrigger : MonoBehaviour
{
    [Header("Blocker Settings")]
    [SerializeField] private Collider2D colliderToActivate;

    [Header("Camera Settings")]
    [SerializeField] private CinemachineConfiner2D cinemachineConfiner;
    [SerializeField] private Collider2D newCameraBoundary;

    [SerializeField] private CinemachineCamera virtualCamera; 
    [SerializeField] private float bossFightZoomSize = 8f; 

    [Header("Boss Trigger")]
    [SerializeField] private BossController bossToTrigger;

    [Header("Debug")]
    [SerializeField] private bool showDebugMessages = true;

    [Header("VFX")]
    [SerializeField] private GameObject NormalMom; 
    [SerializeField] private GameObject VFXPrefab; 
    [SerializeField] private float revealWaitTime = 1.5f; 

    private bool hasTriggered = false;

    private void Start()
    {
        if (colliderToActivate != null)
        {
            colliderToActivate.enabled = false;
        }

        if (NormalMom != null)
        {
            NormalMom.SetActive(true);
        }
        
        if (bossToTrigger != null)
        {
            bossToTrigger.gameObject.SetActive(false);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            LockTheRoom();
            TriggerDialogue();
            //gameObject.SetActive(false);


        }
    }
    private IEnumerator BossSequence()
    {
        

        if (NormalMom != null)
        {
            NormalMom.SetActive(false);
        }

        if (VFXPrefab != null)
        {
            Vector3 spawnPos = (bossToTrigger != null) ? bossToTrigger.transform.position : transform.position;
            Instantiate(VFXPrefab, spawnPos, Quaternion.identity);
        }

        yield return new WaitForSeconds(revealWaitTime);

        if (bossToTrigger != null)
        {
            bossToTrigger.gameObject.SetActive(true);
        }

        StartTheBossFight();
        gameObject.SetActive(false);
    }

    private void LockTheRoom()
    {
        {
            
            if (colliderToActivate != null)
            {
                colliderToActivate.enabled = true;
                if (showDebugMessages) Debug.Log("BLOCKER ACTIVATED!");
            }

            if (cinemachineConfiner != null && newCameraBoundary != null)
            {
                cinemachineConfiner.enabled = false;
                cinemachineConfiner.BoundingShape2D = newCameraBoundary;
                cinemachineConfiner.InvalidateBoundingShapeCache();
                cinemachineConfiner.enabled = true;
                if (showDebugMessages) Debug.Log($"CAMERA BOUNDARY SWITCHED TO: {newCameraBoundary.name}");
            }
            if (virtualCamera != null)
            {
               
                virtualCamera.Lens.OrthographicSize = bossFightZoomSize;
            }
        }
    }

    public void TriggerDialogue()
    {
        if (DialogueUI.Instance != null)
        {
            DialogueUI.Instance.StartDialogue(OnDialogueFinished);
        }
        else
        {
            OnDialogueFinished();
        }
    }

    public void OnDialogueFinished()
    {
        StartCoroutine(BossSequence());
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