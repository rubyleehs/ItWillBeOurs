using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterAnimator))]
public class HistoryAgent : MathfExtras {

    [System.Serializable]
    public struct HistoryAgentStats
    {
        public int teamIndex;
        public int recordingIndex;
        public float maxHitPoints;
        public float currentHitPoints;
    }
    new Transform transform;

    //===== Pooling Start =====
    
    public static List<HistoryAgent> pool = new List<HistoryAgent>();
    public static List<HistoryAgent> all = new List<HistoryAgent>();
    public bool inPool;

    public static HistoryAgent GetFromPool(GameObject fallback)
    {
        HistoryAgent result;

        if (pool.Count > 0)
        {
            result = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
        }
        else
        {
            result = Instantiate(fallback).GetComponent<HistoryAgent>();
            all.Add(result);
        }
        result.inPool = false;

        return result;
    }

    public static void PoolAll()
    {
        for (int i = 0; i < all.Count; i++) all[i].Pool();
    }

    public void Pool()
    {
        if (inPool) return;

        inPool = true;
        pool.Add(this);

        // TODO hide renderer
    }

    public virtual void Initialize(int _teamIndex, int _recordingIndex, Vector2 _position, float _angle) //should be called whenever new obj/just got from pool
    {
        Debug.Log("History Agent Initialization");

        historyAgentStats.teamIndex = _teamIndex;
        historyAgentStats.recordingIndex = _recordingIndex;
        historyAgentStats.currentHitPoints = historyAgentStats.maxHitPoints;

        isAlive = true;
        if(transform == null) transform = this.GetComponent<Transform>();
        this.transform.position = _position;
        this.transform.eulerAngles = new Vector3(0, 0, _angle);

        if (animator == null) animator = GetComponent<CharacterAnimator>();
        animator.enabled = true;
        animator.Initialize();
    }

    //===== Pooling End =====

    public bool isAlive = true;

    protected HistoryManager historyManager;
    public HistoryAgentStats historyAgentStats;
    public CharacterAnimator animator;

    public List<InstanceData> historyData;

    private IEnumerator curRecording;
    private float curRecordingStartTime;

    protected Vector2 position;
    protected float angle;

    protected virtual void Awake()
    {
        if (historyManager == null) historyManager = GameManager.historyManager;
    }

    protected void StartNewRecording()
    {
        curRecordingStartTime = Time.time;
        historyData = new List<InstanceData>();
        curRecording = StartRecording();
        StartCoroutine(curRecording);
    }

    private IEnumerator StartRecording()
    {
        while (true)
        {
            RecordCurrentInstanceData();
            yield return new WaitForSeconds(HistoryManager.samplingInterval);
        }
    }

    private void RecordCurrentInstanceData()
    {
        InstanceData _instanceData = new InstanceData
        {
            position = position,
            rotation = angle,
            timecode = Time.time - curRecordingStartTime,
            //hasDied = GameManager.playerStats.hasDied,
        };
        historyData.Add(_instanceData);
    }

    private void EndCurrentRecording()
    {
        if (curRecording != null) StopCoroutine(curRecording);
        curRecording = null;
    }

    protected List<InstanceData> GetAndEndCurrentRecording()
    {
        if(historyData.Count == 0 || curRecording == null) { Debug.Log("Trying to end non-existant recording!"); return historyData; }
        RecordCurrentInstanceData();
        EndCurrentRecording();
        return historyData;
    }

    public void MoveTo(Vector2 _position, float _angle, float dt)
    {
        Vector2 delta = _position - (Vector2)this.transform.position;
        transform.eulerAngles = new Vector3(0, 0, _angle);

        transform.position = _position;

        
        if (Mathf.Abs(delta.x) < Mathf.Epsilon && Mathf.Abs(delta.y) < Mathf.Epsilon)
        {
            animator.Halt();
        }
        else animator.Move(_position, dt);
    }

}
