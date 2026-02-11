using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int coinCount = 0;

    public void AddCoin(int amount)
    {
        coinCount += amount;
        Debug.Log("Coins: " + coinCount);
    }
}
