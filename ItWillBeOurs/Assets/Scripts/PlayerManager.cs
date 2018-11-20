using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class PlayerManager : HistoryAgent {

    public int I_teamIndex;
    //public Camera playerCam;

    public float walkingSpeed = 1.0f;
    public float acceleration;
    public float velocityLoss;


    private Vector2 velocity;

    private Rigidbody2D rb;

    protected override void Awake()
    {
        base.Awake();
        Initialize(-1, -1, Vector3.zero, 0);
    }

    protected void Start()
    {
        //Initialize(-1, -1, Vector3.zero, 0);
    }

    public override void Initialize(int _teamIndex, int _recordingIndex, Vector2 _position, float _angle)
    {
        rb = this.GetComponent<Rigidbody2D>();
        if (_teamIndex < 0) { _teamIndex = I_teamIndex; Debug.Log("invalid _teamIndex, using I_teamIndex"); }
        Debug.Log(historyManager);
        if(_recordingIndex < 0) { _recordingIndex = historyManager.teamHistoryData[_teamIndex].recordings.Count; Debug.Log("invalid _recordingIndex, using teamHistoryData recording Count"); }

        Debug.Log("Player Initialization");
        base.Initialize(_teamIndex, _recordingIndex, _position, _angle);
    }

    public void Update()
    {
        float dt = Time.deltaTime;

        // Movement
        if (isAlive)
        {
            Move(dt);
        }
        else
        {
            velocity -= velocity * (velocityLoss * dt);
        }

        rb.velocity = velocity;

        position = transform.position;

        // Vector2 mouseDelta = MainCamera.mousePosition - position;
        // angle = Mathf.Atan2(mouseDelta.y,mouseDelta.x) * Mathf.Rad2Deg;

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            StartNewRecording();
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            historyManager.teamHistoryData[historyAgentStats.teamIndex].recordings.Add(GetAndEndCurrentRecording());
            StartCoroutine(historyManager.Playback(historyAgentStats.teamIndex, 0));
        }
        if (Input.GetKey(KeyCode.T)) ; //Kill();

    }



    void Move(float dt)
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(h, v).normalized;

        velocity = Vector2.ClampMagnitude(velocity - velocity * (velocityLoss * dt) + input * acceleration, walkingSpeed);
        angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);

        if (Mathf.Abs(h) < Mathf.Epsilon && Mathf.Abs(v) < Mathf.Epsilon)
        {
            animator.Halt();
        }
        else animator.Move(position, dt);
    }
}
