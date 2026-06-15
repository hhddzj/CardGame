using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Player : Character
{
    public int energy {  get; private set; }
    public int MaxEnergy;
    private Player _player;
    public Player player => _player;
    public List<Card> hand;
    public List<Card> discardPile;
    public Player(int maxHp, int maxEne)
    {
        maxHealth = maxHp;
        currentHealth = maxHp;
        MaxEnergy = maxEne;
        energy = MaxEnergy;
    }
    public void SpendEnergy(int cost)
    {
        energy-=cost;
    }
    //回合开始回满费用
    public void ResetEnergy() => energy = MaxEnergy;
    public void CheckBattleEnd()
    {

    }
}