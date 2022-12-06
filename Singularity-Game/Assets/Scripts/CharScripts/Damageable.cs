using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable
{
    int maxHealth { get; }  
    int currentHealth { get; set; }

    public Damageable(int maxHealth)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;
    }
    void ApplyDamage(int damage)
    {
        this.currentHealth -= damage;
    }
}
