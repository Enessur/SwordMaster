using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public GameObject healParticleRb;
    public int health = 30;
    public event Action<int> OnDamageTaken;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (OnDamageTaken != null)
        {
            OnDamageTaken(damage);
        }

        if (health < 1)
        {
            Instantiate(healParticleRb,transform.position,transform.rotation);
        }
    }
}