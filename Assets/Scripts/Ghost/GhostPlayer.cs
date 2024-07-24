using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;

public class GhostPlayer : MonoBehaviour
{
    public Ghost ghost;
    public Animator animator;
    [SerializeField] private List<Transform> playerCheck = new List<Transform>();
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Collider2D platformCollider;
    const float playerCheckRadius = 0.2f;
    private float timeValue;
    private int index1;
    private int index2;
    private bool playing;

    public void StartPlaying()
    {
        timeValue = 0f;
        playing = true;
    }

    public void StopPlaying()
    {
        playing = false;
    }

    void Update()
    {
        if (playing)
        {
            if (ghost.ghostDatas.Count <= 0)
            {
                return;
            }
            timeValue += Time.deltaTime;
            if (timeValue >= 10)
            {
                playing = false;
            }

            GetIndex();
            if (ghost.ghostDatas.Count < index1 - 1)
                return;
            SetTransform();
            SetAnimation();
            if (ghost.ghostDatas[index1].dead)
            {
                platformCollider.enabled = false;
            }
        }

        platformCollider.enabled = !CheckCollision();

    }

    private bool CheckCollision()
    {
        for (int i = 0; i < 3; i++)
        {
            if (Physics2D.OverlapCircle(playerCheck[i].position, playerCheckRadius, whatIsPlayer))
            {
                return true;
            }
        }

        return false;
    }

    void GetIndex()
    {
        for (int i = 0; i < ghost.ghostDatas.Count - 2; i++)
        {
            var ghostTimeStamp = ghost.ghostDatas[i].timeStamp;
            var nextGhostTimeStamp = ghost.ghostDatas[i + 1].timeStamp;
            if (ghostTimeStamp == timeValue)
            {
                index1 = i;
                index2 = i;
                return;
            }
            else if (ghostTimeStamp < timeValue && timeValue < nextGhostTimeStamp)
            {
                index1 = i;
                index2 = i + 1;
                return;
            }
        }
        index1 = ghost.ghostDatas.Count - 1;
        index2 = ghost.ghostDatas.Count - 1;
    }

    void SetTransform()
    {
        if (index1 == index2)
        {
            transform.position = ghost.ghostDatas[index1].position;
        }
        else
        {
            float lerpFactor = (timeValue - ghost.ghostDatas[index1].timeStamp) / (ghost.ghostDatas[index2].timeStamp - ghost.ghostDatas[index1].timeStamp);

            transform.position = Vector2.Lerp(ghost.ghostDatas[index1].position, ghost.ghostDatas[index2].position, lerpFactor);
        }

        if (ghost.ghostDatas[index1].speed > 0 && transform.localScale.x < 0)
        {
            Flip();
        }
        else if (ghost.ghostDatas[index1].speed < 0 && transform.localScale.x > 0)
        {
            Flip();
        }
    }

    void SetAnimation()
    {
        animator.SetFloat("Speed", MathF.Abs(ghost.ghostDatas[index1].speed));
        animator.SetBool("IsJumping", ghost.ghostDatas[index1].jumping);
        animator.SetBool("IsCrouching", ghost.ghostDatas[index1].crouching);
    }

    private void Flip()
    {
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
