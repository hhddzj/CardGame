using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public int block;

    public virtual void TakeDamage(int amount)
    {
        int remaining = amount - block;
        if (block > 0)
        {
            if (amount >= block)
            {
                block = 0;
                currentHealth -= remaining;
            }
            else
            {
                block -= amount;
            }
        }
        else
        {
            currentHealth -= amount;
        }
    }
}
