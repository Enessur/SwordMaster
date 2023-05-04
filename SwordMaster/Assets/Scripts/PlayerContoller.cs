using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey;
using UnityEngine;
using CodeMonkey.Utils;
using TMPro;


public class PlayerContoller : MonoBehaviour
{
    
    
    public static PlayerContoller Instance;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    
    public enum TaskCycles
    {
        Move,
        Attack,
        // Rolling
    }

    
    public int playerHealth;

    public Ghost ghost;
    public bool makeGhost = false;
    
    [SerializeField] private float moveSpeed = 60f;
    [SerializeField] private float dashAmount = 50f;
    [SerializeField] private Transform attackPos;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask whatIsEnemies;
    [SerializeField] private int damage;
    [SerializeField] private TaskCycles taskCycle;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private LayerMask dashLayerMask;
    
    
    private Rigidbody2D _rb;
    private Vector3 _moveDir;
    private Vector3 _rollDir;
    private Animator _animator;
    private string _currentAnimation;
    private bool _isDashButtonDown;
    private bool _canMove;
    private float _attackTimer;
    private bool _canAttack = true;
    private int _attackNum = 0;
    private Shake _shake;
    private int _playerMaxHealt;
    // private float _rollSpeed;
 
    //Animation States
    const string PLAYER_IDLE = "Idle";
    const string PLAYER_RUN = "Run";
    const string PLAYER_ATTACK1 = "Attack1";
    const string PLAYER_ATTACK2 = "Attack2";
    const string PLAYER_ATTACK3 = "Attack3";
    const string PLAYER_ATTACK4 = "Attack4";
    const string PLAYER_ATTACK5 = "Attack5";
    const string TAKE_DAMAGE = "TakeDamage";
    const string DEATH = "Death";


    private void Start()
    {
        _shake = GameObject.FindWithTag("ScreenShake").GetComponent<Shake>();
        _rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _canMove = true;
        _canAttack = true;
        _playerMaxHealt = playerHealth;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            taskCycle = TaskCycles.Attack;
            
        }
        // else if (Input.GetKeyDown(KeyCode.LeftControl))
        // {
        //     taskCycle = TaskCycles.Rolling;
        // }
        else
        {
            taskCycle = TaskCycles.Move;
        }

        switch (taskCycle)
        {
            case TaskCycles.Move:

                HandleControl();
                break;
          
            // case TaskCycles.Rolling:
            //     _rb.velocity = _rollDir * _rollSpeed;
            //     
            //     break;
                
            case TaskCycles.Attack:
                Attack();
                break;
        }
    }

    private void FixedUpdate()
    {
        _rb.velocity = _moveDir * moveSpeed;

        if (_isDashButtonDown)
        {
            Vector3 dashPosition = transform.position + _moveDir * dashAmount;

            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, _moveDir, dashAmount, dashLayerMask);
            if (raycastHit2D.collider != null)
            {
                dashPosition = raycastHit2D.point;
            }
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
                ghost.makeGhost = true;
                ChangeAnimationState(PLAYER_RUN);
            }
            else
            {
                ChangeAnimationState(PLAYER_IDLE);
                ghost.makeGhost = false;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                _isDashButtonDown = true;
                ghost.makeGhost = true;
            }
            // if (Input.GetKeyDown(KeyCode.LeftControl))
            // {
            //     _rollDir = _moveDir;
            //     ghost.makeGhost = true;
            // }
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
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            _shake.CamShake();
          //  EnemyHealth.Instance.TakeDamage(damage);
            enemiesToDamage[i].GetComponent<EnemyHealth>().TakeDamage(damage);
        }
    }

    private void Attack()
    {
        ghost.makeGhost = true;
        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
        Vector3 attackDir = (mousePosition - transform.position);
        // CMDebug.TextPopupMouse("" + attackDir);
        if (attackDir.x < 0)
        {
            _renderer.flipX = true;
            attackPos.position = transform.position + new Vector3(-1.4f, 0f, 0f);
        }
        else
        {
            _renderer.flipX = !true;
            attackPos.position = transform.position + new Vector3(+1.4f, 0f, 0f);
        }

        if (_canAttack == true)
        {
            _canMove = false;
            _attackNum++;

            if (_attackNum == 1)
            {
                ChangeAnimationState(PLAYER_ATTACK1);
            }

            if (_attackNum == 2)
            {
                ChangeAnimationState(PLAYER_ATTACK2);
            }

            if (_attackNum == 3)
            {
                ChangeAnimationState(PLAYER_ATTACK3);
               
            }
            if (_attackNum == 4)
            {
                ChangeAnimationState(PLAYER_ATTACK4);
               
            }
            if (_attackNum == 5)
            {
                ChangeAnimationState(PLAYER_ATTACK5);
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

    public void Stop()
    {
        _canAttack = false;
        _canMove = false;
    }

    public void Cleanse()
    {
        _canAttack = true;
        _canMove = true;
    }

    public void TakeDamage(int takendamage)
    {
        if (playerHealth < 1)
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
            ChangeAnimationState(DEATH);
            Stop();
        }
        else
        {
            playerHealth -= takendamage;
            ChangeAnimationState(TAKE_DAMAGE);
            _shake.CamShake();
            Stop();
            Debug.Log("Player health : " + playerHealth);
        }
    }

    
    public void GetHeal(int getHeal)
    {
        playerHealth += getHeal;
        if (playerHealth > _playerMaxHealt)
        {
            playerHealth = _playerMaxHealt;
        }
    }
    
}