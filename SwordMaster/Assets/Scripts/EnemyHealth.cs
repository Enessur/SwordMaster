using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public GameObject healParticleRb;
    public int health = 30;
    public event Action<int> OnDamageTaken;
    private bool onHeal = true;

    public void TakeDamage(int damage)
    {
        if (health > 0)
        {
            health -= damage;
            if (OnDamageTaken != null)
            {
                OnDamageTaken(damage);
            }
        }

        if (health < 0 && (onHeal))
        {
            Instantiate(healParticleRb, transform.position, transform.rotation);
            onHeal = false;
        }
    }
}