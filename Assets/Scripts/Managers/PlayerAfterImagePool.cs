using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImagePool : MonoBehaviour
{
    public float distanceBetweenImages;
    [SerializeField]
    private GameObject afterImagePrefab;
    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    public static PlayerAfterImagePool Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        GrowPool();
    }

    void GrowPool()
    {
        for (int i = 0; i < 10; i++)
        {
            var instanceToAdd = Instantiate(afterImagePrefab);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }

    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableObjects.Enqueue(instance);
    }

    public GameObject GetFromPool()
    {
        if (availableObjects.Count == 0)
        {
            GrowPool();
        }

        var instance = availableObjects.Dequeue();
        instance.SetActive(true);
        return instance;
    }
}
