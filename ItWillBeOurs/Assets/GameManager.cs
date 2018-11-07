using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public HistoryManager I_historyManager;
    public static HistoryManager historyManager;

    public static int numOfTeams = 1;//

    private void Awake()
    {
        historyManager = I_historyManager;
    }
}
