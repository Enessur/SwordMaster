using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey;
using UnityEngine;
using CodeMonkey.Utils;
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
    // [SerializeField] private float attackInterval = 1.5f;
    [SerializeField] private TaskCycles taskCycle;
    [SerializeField] private SpriteRenderer _renderer;
    

    private Rigidbody2D _rb;
    private Vector3 _moveDir;
    private Animator _animator;
    private string _currentAnimation;
    private bool _isDashButtonDown;
    private bool _canMove;
    private float _attackTimer;
    private bool _canAttack = true;
    private int _attackNum = 0;


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
        
        // _attackTimer  += Time.deltaTime;
        // if (_attackTimer >= attackInterval)
        // {
        //     _attackTimer = 0;
        //     _canAttack = true;
        // }
        
        if (Input.GetMouseButtonDown(0))
        {
            taskCycle = TaskCycles.Attack;
        }
        else
        {
            taskCycle = TaskCycles.Move;
        }

        switch (taskCycle)
        {
            case TaskCycles.Move:
        
                HandleControl();
                break;
        
            case TaskCycles.Attack:
               // Debug.Log("task cycle attack");
                Attack();
                break;
        }
    }
    // todo: add roll and collider to dash 
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
        if (_canMove == true)
        {
            //Debug.Log("canMove True");
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
        else
        {
            _moveDir = new Vector3(0, 0).normalized;
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

    private void hit()
    {
        Debug.Log("Hit");
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            enemiesToDamage[i].GetComponent<Enemy>().TakeDamage(damage);
        }
    }
    private void Attack()
    {
        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
        Vector3 attackDir = (mousePosition - transform.position);
        CMDebug.TextPopupMouse("" + attackDir);
        if (attackDir.x < 0)
        {
            _renderer.flipX = true;
            attackPos.localScale = new Vector3(-1f, 0f, 0f);
        }
        else
        {
            _renderer.flipX = !true;
            attackPos.localScale = new Vector3(10f, 0f, 0f);
        }
        
        if (_canAttack == true)
        {
            _canMove = false;

            

            //ChangeAnimationState(PLAYER_ATTACK1);

            //todo : Animation Event listener when animation ends it returns bool to enable another attack

            _attackNum++;
            
            if (_attackNum == 1)
            {
                 ChangeAnimationState(PLAYER_ATTACK1);

                // Debug.Log("attack1");
            } 
            if (_attackNum == 2)
            {
                ChangeAnimationState(PLAYER_ATTACK2);
              //  Debug.Log("attack2");

            }

            if (_attackNum == 3)
            {
                ChangeAnimationState(PLAYER_ATTACK3);
                _attackNum = 0;
            }
         
            _canAttack = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    private void CanMove()
    {
        _canMove = true;
    }

    private void AttackInterval()
    {
        _canAttack = true;
    }
}