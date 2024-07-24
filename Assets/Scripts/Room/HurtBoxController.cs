using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HurtBoxController : MonoBehaviour
{
    public bool disablesGhost;
    private GameObject collidedObject;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerHurtBox"))
        {
            PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerHurtBox>().playerMovement;
            collidedObject = playerMovement.gameObject;
            playerMovement.dead = disablesGhost;
            if (disablesGhost)
            {
                StartCoroutine(RespawnPlayer());
            }
            else
            {
                RoomManager.Instance.RespawnPlayer(collidedObject);
            }
        }
    }

    private IEnumerator RespawnPlayer()
    {
        float t = 0f;
        float dur = 0.25f;

        while (t < dur)
        {
            t += Time.deltaTime;

            if (t >= dur)
            {
                RoomManager.Instance.RespawnPlayer(collidedObject);
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
