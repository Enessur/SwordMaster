using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 30;
    public event Action<int> OnDamageTaken;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (OnDamageTaken != null)
        {
            OnDamageTaken(damage);
        }
    }
}