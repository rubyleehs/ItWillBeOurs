using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InstanceData
{
    public float timecode;
    public Vector3 position;
    public float rotation;
    public bool IsFiring;
}

public struct TeamHistoryData
{
    public List<List<InstanceData>> recordings;
}

public class HistoryManager : MonoBehaviour
{
    public GameObject shadowGO;
    public Transform shadowParent;

    public List<TeamHistoryData> teamHistoryData;
    public static float samplingInterval;

    public float I_samplingInterval;


    private void Awake()
    {
        HardResetAll();
    }

    private void HardResetAll()//Setups everything too.
    {
        samplingInterval = I_samplingInterval;
        teamHistoryData = new List<TeamHistoryData>();
        for (int i = 0; i < GameManager.teamData.Length; i++)
        {
            teamHistoryData.Add(new TeamHistoryData { recordings = new List<List<InstanceData>>() });
        }
    }

    public IEnumerator Playback(int _teamIndex, int _recordingIndex, bool _loop)
    {
        float _playbackStartTime = Time.time;
        int _sampleIndex = 0;

        List<InstanceData> _playbackRecordingData = teamHistoryData[_teamIndex].recordings[_recordingIndex];

        InstanceData _curInstanceData = _playbackRecordingData[0];
        InstanceData _nextInstanceData = _playbackRecordingData[1];
        int _samplesCount = _playbackRecordingData.Count;
        Debug.Log("Recording Samples: " + _samplesCount);
        float _progress = 0;
        float _prematureDeathDelay = 0;// so dying early do not start next loop earlier. (due to time take for death animation)

        HistoryAgent _shadow = HistoryAgent.GetFromPool(shadowGO);
        _shadow.Initialize(_teamIndex, _recordingIndex, _curInstanceData.position,_curInstanceData.rotation);

        while (_sampleIndex < _samplesCount)
        {
            _progress = (Time.time - _playbackStartTime - _prematureDeathDelay - _curInstanceData.timecode) / (_nextInstanceData.timecode - _curInstanceData.timecode);

            if (_shadow.isAlive)
            {
                if (_shadow.historyAgentStats.currentHitPoints <= 0)
                {
                    _shadow.isAlive = false;
                    _prematureDeathDelay = Time.time;
                    yield return StartCoroutine(_shadow.animator.DieAnim());
                    _prematureDeathDelay = Time.time - _prematureDeathDelay;
                }
                else
                {
                    _shadow.MoveTo(Vector3.Lerp(_curInstanceData.position, _nextInstanceData.position, _progress),
                               Mathf.Lerp(_curInstanceData.rotation, _nextInstanceData.rotation, _progress), Time.fixedDeltaTime);
                }
            }
            yield return new WaitForFixedUpdate();
            if (_progress >= 1)
            {
                _sampleIndex++;
                if (_sampleIndex + 1 >= _samplesCount)
                {
                    //DIE ANIMATION AND RELATED HERE
                    if (_shadow.isAlive)
                    {
                        _shadow.isAlive = false;
                        yield return StartCoroutine(_shadow.animator.DieAnim());
                    }
                    _shadow.Pool();
                    if(_loop) StartCoroutine(Playback(_teamIndex, _recordingIndex, _loop));
                    yield break;
                }
                _curInstanceData = _playbackRecordingData[_sampleIndex];
                _nextInstanceData = _playbackRecordingData[_sampleIndex + 1];
            }
        }
    }
}
