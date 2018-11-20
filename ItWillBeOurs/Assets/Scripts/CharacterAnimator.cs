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

    protected SpriteRenderer spriteRenderer;

    protected bool isMoving;
    protected bool isDead;

    float t;
    int currentFrame; // 4 states 0 - 3

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

    public virtual IEnumerator DieAnim()
    {
        if (isDead) { Debug.Log("Dead body dying once more alert!"); yield break; }
        isDead = true;
        SetFrame(deathFrame, this.transform.position);
        Debug.Log("There is no DieAnim available for this character, did you forget to overide me?");
        yield break;
    }

    public virtual void Initialize()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        isDead = false;
        Halt();
    }

}