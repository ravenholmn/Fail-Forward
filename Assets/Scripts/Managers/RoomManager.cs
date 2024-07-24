using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;
    public List<RoomController> rooms = new List<RoomController>();
    [SerializeField]
    private RoomController activeRoom;
    [SerializeField]
    private GhostPlayer ghostPlayer;

    public UnityEvent OnAwake;
    public UnityEvent OnRoomChange;
    public UnityEvent OnChangeComplete;
    public UnityEvent OnRespawn;


    private bool respawned;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        OnAwake ??= new UnityEvent();
        OnAwake.Invoke();
        OnRoomChange ??= new UnityEvent();
        OnChangeComplete ??= new UnityEvent();
        OnRespawn ??= new UnityEvent();

        activeRoom = rooms[0];

        foreach (var room in rooms)
        {
            room.Init();
        }

        activeRoom.CloseOffLevel();

        DisableCameras();
    }

    public void EnteredRoom(PlayerMovement playerMovement, RoomController roomController)
    {
        if (activeRoom != roomController)
        {
            activeRoom = roomController;
            playerMovement.DisableInput();
            playerMovement.gameObject.transform.position = activeRoom.PlayerSpawn.position;
            DisableCameras();
            activeRoom.EnableCamera();
            OnRoomChange.Invoke();
            StartCoroutine(RoomChangeTimer());
        }
    }

    void DisableCameras()
    {
        foreach (var room in rooms)
        {
            if (room != activeRoom)
            {
                room.DisableCamera();
            }
        }
    }

    public void RespawnPlayer(GameObject player)
    {
        if (respawned)
        {
            return;
        }

        respawned = true;
        player.GetComponent<PlayerMovement>().died = true;
        player.GetComponent<CharacterController2D>().RB.velocity = new Vector2(0f, 0f);
        player.GetComponent<CharacterController2D>().RB.isKinematic = true;
        StartCoroutine(RespawnTimer(player));
    }

    private IEnumerator RespawnTimer(GameObject player)
    {
        float t = 0f;
        float dur = 1f;

        while (t < dur)
        {
            t += Time.deltaTime;

            if (t >= dur)
            {
                player.GetComponent<CharacterController2D>().RB.isKinematic = false;
                player.transform.position = activeRoom.PlayerSpawn.position;
                OnRespawn.Invoke();
                activeRoom.OnRespawn();
                player.GetComponent<PlayerMovement>().died = false;
                player.GetComponent<PlayerMovement>().dead = false;
                StartCoroutine(RespawnBlock());
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator RespawnBlock()
    {
        float t = 0f;
        float dur = 1f;

        while (t < dur)
        {
            t += Time.deltaTime;

            if (t >= dur)
            {
                respawned = false;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator RoomChangeTimer()
    {
        float t = 0f;
        float dur = 1f;
        bool blockInvoke = false;

        Color color = ghostPlayer.GetComponent<SpriteRenderer>().color;

        while (t < dur)
        {
            t += Time.deltaTime;

            if (t <= 0.5f)
            {
                color.a = (1 - (t * 2)) * 0.2509804f;
                ghostPlayer.GetComponent<SpriteRenderer>().color = color;
            }
            else
            {
                if (!activeRoom.dontSpawnGhost)
                {
                    color.a = (t - 0.5f) * 2 * 0.2509804f;
                    ghostPlayer.GetComponent<SpriteRenderer>().color = color;
                }
            }

            if (t >= 0.5 && !blockInvoke)
            {
                blockInvoke = true;
                ghostPlayer.gameObject.transform.position = activeRoom.PlayerSpawn.position;
                OnChangeComplete.Invoke();
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
