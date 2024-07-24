using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public Transform PlayerSpawn
    {
        get
        {
            return playerSpawn;
        }
    }

    public bool dontSpawnGhost;
    public Vector3 entranceBlockOffset;
    public CinemachineVirtualCamera virtualCamera;
    private CoinsController coinsController;
    private Transform playerSpawn;
    private Transform entranceBlock;
    private Vector3 entranceBlockPosition;

    public void Init()
    {
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        coinsController = GetComponentInChildren<CoinsController>();
        playerSpawn = transform.Find("PlayerSpawn");
        entranceBlock = transform.Find("Grid").Find("Tilemap_Entrance_Block");
        entranceBlockPosition = entranceBlock.position;
        entranceBlock.position += entranceBlockOffset;
        entranceBlock.gameObject.SetActive(false);
    }

    public void EnableCamera()
    {
        virtualCamera.gameObject.SetActive(true);
        CloseOffLevel();
    }

    public void DisableCamera()
    {
        virtualCamera.gameObject.SetActive(false);
    }

    public void CloseOffLevel()
    {
        StartCoroutine(CloseOffLevelCoroutine());
    }

    public void OnRespawn()
    {
        coinsController.EnableCoins();
    }

    private IEnumerator CloseOffLevelCoroutine()
    {
        float t = 0f;
        float dur = 0.25f;
        Vector3 startPos = entranceBlock.position;

        yield return new WaitForSeconds(1f);
        entranceBlock.gameObject.SetActive(true);
        while (t <= dur)
        {
            t += Time.deltaTime;

            entranceBlock.position = Vector3.Lerp(startPos, entranceBlockPosition, t / dur);

            yield return new WaitForEndOfFrame();
        }
    }
}
