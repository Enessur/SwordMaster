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
        if (health >= 0)
        {
            health -= damage;
            if (OnDamageTaken != null)
            {
                OnDamageTaken(damage);
            }
        }
        else
        {
            Instantiate(healParticleRb, transform.position, transform.rotation);
        }
    }
}