using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Transform attackPos;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask whatIsEnemies;
    [SerializeField] private int damage;
    [SerializeField] private float attackInterval = 1.5f;

    private Animator _animator;
    private string _currentAnimation;

    private float _attackTimer;
    private bool _canAttack = true;

    //todo fuse with player controller script for handle animations
    
    //AnimationStates

    const string PLAYER_ATTACK1 = "Attack1";
    const string PLAYER_ATTACK2 = "Attack2";
    const string PLAYER_ATTACK3 = "Attack3";

    void Update()
    {
        _attackTimer += Time.deltaTime;
        if (_attackTimer >= attackInterval)
        {
            _attackTimer = 0;
            _canAttack = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    private void Attack()
    {
        //todo:Switch Case / Enum for attack animations
        int attackNum = 0;

        if (_canAttack)
        {
            attackNum += attackNum;
            Debug.Log("Hit");
            Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
            for (int i = 0; i < enemiesToDamage.Length; i++)
            {
         //       enemiesToDamage[i].GetComponent<DeathBringerEnemy>().TakeDamage(damage);
            }
            ChangeAnimationState(PLAYER_ATTACK1);
            if (attackNum == 0)
            {
                ChangeAnimationState(PLAYER_ATTACK2);
                 
            }
            else
            {
                ChangeAnimationState(PLAYER_ATTACK3);
                attackNum = 0;
            }
        }
    }

    void ChangeAnimationState(string newState)
    {
        //stop the same animation from interrupting itself
        if (_currentAnimation == newState)
        {
            return;
        }

        //play the animation
        _animator.Play(newState);
    }
}