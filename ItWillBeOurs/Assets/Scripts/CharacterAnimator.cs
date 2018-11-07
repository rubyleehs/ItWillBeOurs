using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public Sprite[] frames;
    public int idleFrame;//should be within move frames
    public int deathFrame;
    public Vector2Int moveFrames;

    public float footstepVolume;
    public AudioClip footstepClip;

    public bool[] playFootstep;

    public GameObject audioClipPlayerPrefab;

    public float timePerFrame;
    public float deadPauseDuration;
    public float deadFadeDuration;
    public float deadTargetLightFallOffExponent;
    public float deadTargetAngularLightFallOffExponent;
    [Range (0,1)]
    public float deadTargetLightFallOffTimeOffset;
    [Range(0, 1)]
    public float deadTargetAngularLightShutoffPoint;
    [Range(0, 1)]
    public float deadTargetCompleteShutoffPoint;

    private SpriteRenderer spriteRenderer;
    public Light2D light2D;
    public MeshRenderer lightMeshRenderer;

    private float lightOriginalFallOffExponent;

    bool isMoving;
    bool isDead;

    float t;
    int currentFrame; // 4 states 0 - 3

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lightOriginalFallOffExponent = light2D.falloffExponent;
        Initialize();
    }

    public void Move(Vector2 position, float dt)
    {
        if (isDead) Debug.Log("Dead body is moving alert!");
        if (!isMoving)
        {
            isMoving = true;
            SetFrame(((currentFrame + 1) - moveFrames.x) % (moveFrames.y + 1 - moveFrames.x) + moveFrames.x, position);
        }

        t += dt;
        if (t >= timePerFrame)
        {
            t = 0;
            SetFrame(((currentFrame + 1) - moveFrames.x) % (moveFrames.y + 1- moveFrames.x) + moveFrames.x, position);
        }
    }

    public void Halt()
    {
        if (isDead) Debug.Log("Dead body is halting alert!");
        isMoving = false;
        SetFrame(idleFrame, Vector2.zero);
    }

    public void SetFrame(int f, Vector2 position)
    {
        currentFrame = f;
        spriteRenderer.sprite = frames[f];

        //if (playFootstep[f]) AudioClipPlayer.Play(footstepClip,Random.Range(0.9f,1.1f),footstepVolume,position,audioClipPlayerPrefab);
    }

    public IEnumerator DieAnim()
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

    public void Initialize()
    {
        Color _curColor = spriteRenderer.color;
        light2D.angleFalloffExponent = 0;
        light2D.falloffExponent = lightOriginalFallOffExponent;
        lightMeshRenderer.enabled = true;
        _curColor.a = 1;
        spriteRenderer.color = _curColor;
        isDead = false;
        Halt();
    }

}