using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    // STATIC VARIABLES

    public Vector2 mousePosition;
    public float width;
    public float height;

    [Header("Positioning")]

    public float z;
    public Vector2 position;
    public float positionLerpSpeed;

    [Header("Kick")]

    public Vector2 kick;
    public float kickDiminishSpeed;

    [Header("Player")]

    public Transform player;
    public float playerToMouseRatio;

    [Header("Audio")]

    public float masterVolume;

    // PRIVATE VARIABLES

    Transform _transform;
    Camera _camera;

    void Start()
    {

        if (_camera == null) _camera = GetComponent<Camera>();
        mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);

        if (height > 0)
        {
            _camera.orthographicSize = height;
        }
        else height = _camera.orthographicSize;
        width = height * _camera.aspect;

        position = player.position;

        if (_transform == null) _transform = GetComponent<Transform>();
        _transform.position = new Vector3(position.x, position.y, z);

        AudioListener.volume = masterVolume;

    }

    public void SetHeight(float value)
    {

        height = value;
        width = height * _camera.aspect;
        _camera.orthographicSize = height;

    }

    public void SetPosition(Vector2 value)
    {

        position = value;
        _transform.position = new Vector3(position.x, position.y, z);
        mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);

    }

    void FixedUpdate()
    {

        Vector2 lerp = Vector2.LerpUnclamped(player.position, mousePosition, playerToMouseRatio);

        position = Vector2.Lerp(position, lerp, Time.fixedDeltaTime * positionLerpSpeed);

        _transform.position = new Vector3(position.x + kick.x, position.y + kick.y, z);

    }

    void Update()
    {

        mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);

        float dt = Time.deltaTime;
        kick -= kick * kickDiminishSpeed * dt;

    }
}

