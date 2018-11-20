using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TeamData
{
    public int initialTotalLives;
    public int currentLivesLeft;

    public LayerMask teamLightLayer;
    public LayerMask[] teamCamDetectLightLayers;
}

public class GameManager : MonoBehaviour {

    public HistoryManager I_historyManager;
    public static HistoryManager historyManager;

    public TeamData[] I_teamData;
    public static TeamData[] teamData;

    private void Awake()
    {
        teamData = I_teamData;
        historyManager = I_historyManager;
        Debug.Log(teamData[0].teamCamDetectLightLayers.Length);
    }
}
