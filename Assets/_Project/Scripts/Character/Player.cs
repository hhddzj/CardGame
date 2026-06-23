using UnityEngine;

public class Player : Character
{
    public int energy { get; private set; }
    public int MaxEnergy { get; private set; }

    public void InitPlayer(int maxHp, int maxEne)
    {
        maxHealth = maxHp;
        currentHealth = maxHp;
        MaxEnergy = maxEne;
        energy = MaxEnergy;
        block = 0;
        UpdateHealthUI();
        UpdateBlockUI();
    }

    public void SpendEnergy(int cost)
    {
        energy -= cost;
    }

    public void ResetEnergy() => energy = MaxEnergy;
}