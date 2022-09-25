using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageble
{
    public float startHealth = 100f;
    public float health { get; private set; }
    public bool dead { get; private set; }
    public event Action onDeath;

    protected virtual void OnEnable()
    {
        dead = false;
        health = startHealth;
    }

    public virtual void RestoreHealth(float restore)
    {
        if (dead) return;
        health += restore;
    }

    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        health -= damage;
        if(health <= 0 )
        {
            health = 0;
            if(!dead)
            {
                Die();
            }
        }
    }

    public virtual void Die()
    {
        if (dead) return;
        if(onDeath != null)
        {
            onDeath();
        }
        dead = true;
    }
}
