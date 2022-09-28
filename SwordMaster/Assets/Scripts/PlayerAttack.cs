using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float timeBtwAttack;
    public float attackRate;

    public Transform attackPos;
    public float attackRange;
    public LayerMask whatIsEnemies;
    public int damage;

    void Update()
    {
        if (timeBtwAttack <= 0)
        {
            //left button, 1 for right
            if (Input.GetMouseButtonDown(0))
            {
                Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange,whatIsEnemies);
                for (int i = 0; i < enemiesToDamage.Length; i++)
                {
                    enemiesToDamage[i].GetComponent<Enemy>().TakeDamage(damage);
                }
            }

            timeBtwAttack = attackRate;
        }
        else
        {
            timeBtwAttack -= Time.deltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}