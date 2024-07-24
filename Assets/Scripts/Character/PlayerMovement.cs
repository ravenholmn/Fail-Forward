using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;
    public Ghost ghost;
    public bool inputsEnabled = true;
    public bool dead = false;
    public bool died = false;

    public float HorizontalMove
    {
        get
        {
            return horizontalMove;
        }
    }

    public bool Jumping
    {
        get
        {
            return jumping;
        }
    }

    public bool Crouching
    {
        get
        {
            return crouching;
        }
    }

    public float speed = 40f;
    float horizontalMove = 0f;
    Vector2 rawDirection;
    bool jumping = false;
    bool dashing = false;
    bool crouching = false;

    // Update is called once per frame
    void Update()
    {
        if (!inputsEnabled || died)
        {
            horizontalMove = 0;
            rawDirection = Vector2.zero;
            jumping = false;
            crouching = false;
            dashing = false;
            animator.SetFloat("Speed", 0);
            return;
        }

        horizontalMove = Input.GetAxisRaw("Horizontal") * speed;

        rawDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        animator.SetFloat("Speed", MathF.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump"))
        {
            jumping = true;
        }

        if (Input.GetButtonDown("Crouch"))
        {
            crouching = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouching = false;
        }

        if (Input.GetButtonDown("Dash"))
        {
            dashing = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ghost.ResetData();
            ghost.record = true;
            ghost.replay = false;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ghost.record = false;
            ghost.replay = true;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            ghost.TransferData();
        }
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);

    }

    public void OnCrouching(bool isCrouching)
    {
        animator.SetBool("IsCrouching", isCrouching);

    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouching, jumping, dashing, rawDirection);
        dashing = false;
        jumping = false;
    }

    public void DisableInput()
    {
        inputsEnabled = false;
        StartCoroutine(EnableInput());
    }

    IEnumerator EnableInput()
    {
        float t = 0f;
        float dur = 0.5f;
        while (t < dur)
        {
            t += Time.deltaTime;
            if (t >= dur)
            {
                inputsEnabled = true;
            }
            yield return null;
        }
    }
}
