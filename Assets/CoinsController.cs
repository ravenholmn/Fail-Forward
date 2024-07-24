using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsController : MonoBehaviour
{
    private List<CoinController> coins = new List<CoinController>();

    void Awake()
    {
        CoinController[] coins = GetComponentsInChildren<CoinController>();

        if (coins.Length <= 0)
            return;
        foreach (var coin in coins)
        {
            this.coins.Add(coin);
        }
    }

    public void EnableCoins()
    {
        if (coins.Count <= 0)
            return;
        foreach (var coin in coins)
        {
            coin.Enable();
        }
    }
}
