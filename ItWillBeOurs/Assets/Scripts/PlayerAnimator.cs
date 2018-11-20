using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : CharacterAnimator {

    public float deadPauseDuration;
    public float deadFadeDuration;
    public float deadTargetLightFallOffExponent;
    public float deadTargetAngularLightFallOffExponent;
    [Range(0, 1)]
    public float deadTargetLightFallOffTimeOffset;
    [Range(0, 1)]
    public float deadTargetAngularLightShutoffPoint;
    [Range(0, 1)]
    public float deadTargetCompleteShutoffPoint;

    public Light2D light2D;
    public Light2DCamera light2DCamera;
    public MeshRenderer lightMeshRenderer;

    private float lightOriginalFallOffExponent;
    private HistoryAgent historyAgent;

    public override void Initialize()
    {
        base.Initialize();
        if(lightOriginalFallOffExponent <= 0) lightOriginalFallOffExponent = light2D.falloffExponent;
        if (historyAgent == null) historyAgent = GetComponent<HistoryAgent>();
        
        
        light2D.gameObject.layer =  ToLayer(GameManager.teamData[historyAgent.historyAgentStats.teamIndex].teamLightLayer);

        if (light2DCamera != null)
        {
            light2DCamera.detectLayers = new LayerMask[0];
            light2DCamera.detectLayers = GameManager.teamData[historyAgent.historyAgentStats.teamIndex].teamCamDetectLightLayers;
        }
        
        Color _curColor = spriteRenderer.color;
        light2D.angleFalloffExponent = 0;
        light2D.falloffExponent = lightOriginalFallOffExponent;
        Debug.Log(lightOriginalFallOffExponent);
        lightMeshRenderer.enabled = true;
        _curColor.a = 1;
        spriteRenderer.color = _curColor;
    }

    public override IEnumerator DieAnim()
    {
        if (isDead) { Debug.Log("Dead body dying once more alert!"); yield break; }
        isDead = true;
        SetFrame(deathFrame, this.transform.position);
        yield return new WaitForSeconds(deadPauseDuration);

        float _progress = 0;
        float _smoothProgress = 0;
        float _startTime = Time.time;
        Color _curColor = spriteRenderer.color;
        while (_progress < 1)
        {
            _progress = (Time.time - _startTime) / deadFadeDuration;
            _smoothProgress = Mathf.Lerp(0, 1, _progress);
            _curColor.a = Mathf.Lerp(1, 0, 1.6f * _smoothProgress);
            spriteRenderer.color = _curColor;
            light2D.falloffExponent = Mathf.Lerp(lightOriginalFallOffExponent, deadTargetLightFallOffExponent, Mathf.Pow(_smoothProgress + deadTargetLightFallOffTimeOffset, 6));
            if (_progress > deadTargetAngularLightShutoffPoint) light2D.angleFalloffExponent = Mathf.Lerp(0, deadTargetAngularLightFallOffExponent, Mathf.Pow((_progress - deadTargetAngularLightShutoffPoint) / (1 - deadTargetAngularLightShutoffPoint) + deadTargetLightFallOffTimeOffset, 5));
            if (_progress > deadTargetCompleteShutoffPoint) lightMeshRenderer.enabled = false;
            yield return new WaitForFixedUpdate();
        }
    }

    public static int ToLayer(int bitmask)
    {
        int result = bitmask > 0 ? 0 : 31;
        while (bitmask > 1)
        {
            bitmask = bitmask >> 1;
            result++;
        }
        return result;
    }
}
