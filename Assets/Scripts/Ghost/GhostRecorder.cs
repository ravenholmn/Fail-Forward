using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostRecorder : MonoBehaviour
{
    public Ghost ghost;
    public CharacterController2D characterController2D;
    public PlayerMovement playerMovement;
    private float timer;
    private float timeValue;
    private bool recording;

    void Awake()
    {
        StartRecording();
    }

    public void StartRecording()
    {
        ghost.ResetData();
        timeValue = 0f;
        timer = 0f;
        recording = true;
    }

    void Update()
    {
        if (recording)
        {
            timer += Time.deltaTime;
            timeValue += Time.deltaTime;

            if (timer >= 10f)
            {
                recording = false;
            }

            if ((Mathf.Abs(playerMovement.HorizontalMove) > 0 || Mathf.Abs(characterController2D.RB.velocity.y) > 0) && timer >= 1 / ghost.recordFrequency)
            {
                ghost.Add(timeValue, transform.position, playerMovement.HorizontalMove, playerMovement.Jumping, playerMovement.Crouching, playerMovement.dead);

                timer = 0f;
            }
        }
    }

    public void StopRecording()
    {
        recording = false;
        ghost.TransferData();
        StartRecording();
    }

    public void KillRecording()
    {
        recording = false;
        ghost.ResetData();
        timeValue = 0f;
        timer = 0f;
    }
}
