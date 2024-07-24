using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomTransitionController : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
        if (other.CompareTag("Player"))
        {
            if (playerMovement)
            {
                RoomManager.Instance.EnteredRoom(playerMovement, transform.parent.GetComponent<RoomController>());
            }
        }
    }

}
