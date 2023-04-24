using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpell : MonoBehaviour
{
    [SerializeField] private GameObject spellRb;
    [SerializeField] private Transform attackPos;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private int damage;
    private bool chase;
    private Transform _target;

    void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        transform.position = _target.position + new Vector3(0f, -1f, 0f);
        chase = true;
    }

    private void FixedUpdate()
    {
        if (chase == true)
        {
            transform.position = _target.position + new Vector3(0f, -1f, 0f);
        }
        else
        {
            transform.position = transform.position;
        }
    }


    private void hit()
    {
        Collider2D[] playerToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsPlayer);
        for (int i = 0; i < playerToDamage.Length; i++)
        {
            playerToDamage[i].GetComponent<PlayerContoller>().TakeDamage(damage);
        }
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    private void StopChase()
    {
        chase = false;
    }
}