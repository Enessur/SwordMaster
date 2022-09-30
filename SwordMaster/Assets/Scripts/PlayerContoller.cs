using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContoller : MonoBehaviour
{

    public enum TaskCycles
    {
        Move,
        Attack
    }
    [SerializeField] private float moveSpeed = 60f;
    [SerializeField] private float dashAmount = 50f;
    [SerializeField] private Transform attackPos;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask whatIsEnemies;
    [SerializeField] private int damage;
    [SerializeField] private float attackInterval = 1.5f;
    [SerializeField] private TaskCycles taskCycle;
    
    private Rigidbody2D _rb;
    private Vector3 _moveDir;
    private Animator _animator;
    private string _currentAnimation;
    private bool _isDashButtonDown;
    
    private float _attackTimer;
    private bool _canAttack = true;


   [SerializeField]private SpriteRenderer _renderer;

    //Animation States
    const string PLAYER_IDLE = "Idle";
    const string PLAYER_RUN = "Run";
    const string PLAYER_ATTACK1 = "Attack1";
    const string PLAYER_ATTACK2 = "Attack2";
    const string PLAYER_ATTACK3 = "Attack3";




    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            taskCycle = TaskCycles.Attack;
        }
        else
        {
            taskCycle = TaskCycles.Move;
        }
        
        
        _attackTimer += Time.deltaTime;
        if (_attackTimer >= attackInterval)
        {
            _attackTimer = 0;
            _canAttack = true;
        }

         
        switch(taskCycle)
        {
        case TaskCycles.Move:
            
            HandleControl();
            break;
        
        case TaskCycles.Attack:
            
            Attack();
            break;
        }
    }

    // add roll and collider to dash 
    private void FixedUpdate()
    {
        _rb.velocity = _moveDir * moveSpeed;

        if (_isDashButtonDown)
        {
            Vector3 dashPosition = transform.position + _moveDir * dashAmount;

            _rb.MovePosition(dashPosition);
            _isDashButtonDown = false;
        }
    }

   
    private void HandleControl()
    {
        float moveX = 0f;
        float moveY = 0f;

        //todo:find a better way to use flipX

        if (Input.GetKey(KeyCode.W))
        {
            moveY = +1f;
            _renderer.flipX = !true;
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveY = -1f;
            _renderer.flipX = !true;
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveX = -1f;
            _renderer.flipX = true;
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveX = +1f;
            _renderer.flipX = !true;
        }

        _moveDir = new Vector3(moveX, moveY).normalized;

        if (moveX != 0 || (moveY != 0))
        {
            ChangeAnimationState(PLAYER_RUN);
        }
        else
        {
            ChangeAnimationState(PLAYER_IDLE);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _isDashButtonDown = true;
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
                enemiesToDamage[i].GetComponent<Enemy>().TakeDamage(damage);
            }
            ChangeAnimationState(PLAYER_ATTACK1);
            
            //todo : IENUM Animation Controller all animations must end before another starts.
            // if (attackNum == 0)
            // {
            //      ChangeAnimationState(PLAYER_ATTACK1);
            //      
            // }
            // else if (attackNum == 1)
            // {
            //     ChangeAnimationState(PLAYER_ATTACK2);
            //     
            // }
            // else
            // {
            //     ChangeAnimationState(PLAYER_ATTACK3);
            //     attackNum = 0;
            // }
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}