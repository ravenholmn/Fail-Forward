using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public Animator animator;
    public Collider2D trigger;
    private bool disappear = false;
    private bool blockCoin = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        CharacterController2D characterController2D = other.GetComponent<CharacterController2D>();
        if (!blockCoin && other.CompareTag("Player"))
        {
            blockCoin = true;
            trigger.enabled = false;
            animator.SetBool("Disappeared", true);
            characterController2D.ResetDash();
            StartCoroutine(Disappear());
        }
    }

    private IEnumerator Disappear()
    {
        float t = 0f;
        float dur = 0.5f;

        while (t < dur)
        {
            t += Time.deltaTime;

            if (t >= dur)
            {
                if (!disappear)
                {
                    disappear = true;
                    gameObject.SetActive(false);
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public void Enable()
    {
        disappear = false;
        blockCoin = false;
        trigger.enabled = true;
        gameObject.SetActive(true);
        animator.SetBool("Disappeared", false);
    }
}
