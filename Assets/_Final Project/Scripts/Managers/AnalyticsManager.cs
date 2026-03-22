using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using Unity.Services.Core.Environments; 
using System.Collections.Generic;
using System;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }

    private async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        try
        {
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
            Debug.Log("<color=green>Unity Analytics Initialized Successfully!</color>");
        }
        catch (Exception e)
        {
            Debug.LogError($"Unity Services Init Error: {e.Message}");
        }
    }


    public void LogBalancingData(float duration, int phaseReached, bool isWin)
    {
        double roundedTime = Math.Round((double)duration, 2);

        int attemptCount = PlayerPrefs.GetInt("AttemptCount", 0) + 1;
        float lastTime = PlayerPrefs.GetFloat("LastAttemptTime", 0f);

        double timeDiff = Math.Round((double)(duration - lastTime), 2);

        bool isImproved = duration > lastTime;

        if (attemptCount == 1)
        {
            timeDiff = 0;
            isImproved = true;
        }

        PlayerPrefs.SetInt("AttemptCount", attemptCount);
        PlayerPrefs.SetFloat("LastAttemptTime", duration);
        PlayerPrefs.Save();

        CustomEvent balancingEvent = new CustomEvent("balancing_event")
        {
            { "total_time", roundedTime },
            { "reached_phase", phaseReached },
            { "is_win", isWin },
            { "attempt_count", attemptCount },
            { "time_diff", timeDiff },
            { "is_improved", isImproved }
        };

        AnalyticsService.Instance.RecordEvent(balancingEvent);

        Debug.Log($"<color=cyan>Analytics Sent:</color> Time: {roundedTime}s, Phase: {phaseReached}, Win: {isWin} | Attempt: {attemptCount}, Diff: {timeDiff}s, Improved: {isImproved}");
    }

    public void ResetProgressionData()
    {
        PlayerPrefs.DeleteKey("AttemptCount");
        PlayerPrefs.DeleteKey("LastAttemptTime");
        PlayerPrefs.Save();
    }
}