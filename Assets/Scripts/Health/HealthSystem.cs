using System;
using UnityEngine;

public class HealthSystem
{
    private int Health;
    private int MaxHealth;
    public event Action OnGetDead;


    public HealthSystem(int health = 100,int maxHealth = 100) { 
        this.Health = health; 
        this.MaxHealth = maxHealth;
    }

    public void Damage(int damageAmount) {

        if(this.Health > 0 && this.Health <= damageAmount) {
            OnGetDead?.Invoke();
        }

        if(damageAmount < 0) {
            throw new Exception("damage amount is " + damageAmount);
        }

        this.Health = Math.Max(0,this.Health - damageAmount);
    }
    public void SetHealth(int health) {
        this.Health = health;
    }

    public int GetHealth() {
        return this.Health;
    }

    public int GetMaxHealth() {
        return this.MaxHealth;
    }

    public float GetHealthPercent() {
        return (float)this.GetHealth() / this.GetMaxHealth();
    }
}
