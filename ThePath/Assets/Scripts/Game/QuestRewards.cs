using UnityEngine;

public interface IReward
{
    void DeliverReward(); 
}

public class IntReward : IReward
{
    public int Amount { get; set; }

    public IntReward(int amount)
    {
        Amount = amount;
    }

    public void DeliverReward()
    {
        Debug.Log($"Delivering {Amount} currency to the player.");
    }
}

