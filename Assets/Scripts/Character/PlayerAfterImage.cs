using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAfterImage : MonoBehaviour
{
    private Transform player;
    private SpriteRenderer sr;
    private SpriteRenderer playerSR;

    private Color color;

    [SerializeField]
    private float activeTime = 0.1f;
    private float timeActivated;
    private float alpha;
    private float alphaSet = 0.8f;
    private float alphaMultiplier = 0.85f;

    void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerSR = player.GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        sr.sprite = playerSR.sprite;
        transform.position = player.position;
        transform.localScale = player.localScale;

        timeActivated = Time.time;
    }

    void Update()
    {
        alpha *= alphaMultiplier;
        color = new Color(1, 1, 1, alpha);
        sr.color = color;

        if (Time.time > timeActivated + activeTime)
        {
            PlayerAfterImagePool.Instance.AddToPool(gameObject);
        }
    }
}
