using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    public enum TaskCycleEnemy
    {
        Chase,
        Attack,
        Idle,
        Death,
        Patrol,
    }
    
    
    [SerializeField] private TaskCycleEnemy taskCycleEnemy;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform attackPos;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private int damage;
    [SerializeField] private float startWaitTime = 1f;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;
    [SerializeField] private float chasingDistance;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackRange = 1f;
    
  
    public Transform moveSpot;
    public float patrolSpeed;

    
    private EnemyHealth _enemyHealth;
    private Rigidbody2D _rb;
    private Transform _target;
    private float _patrolTimer;
    private string _currentAnimation;
    private Animator _animator;
    private bool _canAttack = true;
    private bool _canMove = true;
    private int _currentHealth;
    private bool _isDamageTaken = false;
    
    
    
    const string ENEMY_IDLE = "Idle";
    const string ENEMY_RUN = "Run";
    const string ENEMY_ATTACK = "Attack";
    const string ENEMY_DEATH = "Death";
   
    void Start()
    {
        
        _rb = GetComponent<Rigidbody2D>();
        moveSpot.SetParent(null);
        _target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _animator = GetComponent<Animator>();
    }
    
    
    void Update()
    {
        _enemyHealth = GetComponent<EnemyHealth>();
        
            if (!_isDamageTaken)
            {
                _enemyHealth.OnDamageTaken += OnDamageTaken;
                
                _isDamageTaken = true;
            }
       

        _currentHealth = GetComponent<EnemyHealth>().health;

        if (_currentHealth >= 1)
        {
            if (_canMove == true)
            {
                if (Vector2.Distance(transform.position, _target.position) < chasingDistance)
                {
                    taskCycleEnemy = TaskCycleEnemy.Chase;
                }
                else
                {
                    taskCycleEnemy = TaskCycleEnemy.Patrol;
                }
            }

            if (Vector2.Distance(transform.position, _target.position) < attackDistance)
            {
                taskCycleEnemy = TaskCycleEnemy.Attack;
            }
        }
        else
        {
            taskCycleEnemy = TaskCycleEnemy.Death;
        }


        switch (taskCycleEnemy)
        {
            case TaskCycleEnemy.Chase:
                transform.position =
                    Vector2.MoveTowards(transform.position, _target.position, chaseSpeed * Time.deltaTime);
                FlipSprite(_target);
                ChangeAnimationState(ENEMY_RUN);
                break;
            case TaskCycleEnemy.Patrol:
                PatrolPosition();
                transform.position =
                    Vector2.MoveTowards(transform.position, moveSpot.position, patrolSpeed * Time.deltaTime);
                FlipSprite(moveSpot);
                ChangeAnimationState(ENEMY_RUN);
                break;
            case TaskCycleEnemy.Idle:
                ChangeAnimationState(ENEMY_IDLE);
                break;
            case TaskCycleEnemy.Attack:
                FlipSprite(_target);
                Attack();
                break;

            case TaskCycleEnemy.Death:
                ChangeAnimationState(ENEMY_DEATH);
                break;
            
        }
    }
    
    
    private void PatrolPosition()
    {
        _patrolTimer += Time.deltaTime;

        if (!(_patrolTimer >= startWaitTime)) return;
        _patrolTimer = 0;

        moveSpot.position = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
    }
    
    
    
    private void Attack()
    {
        if (_canAttack)
        {
            _canMove = false;
            ChangeAnimationState(ENEMY_ATTACK);
            _canAttack = false;
        }
    }
    
    
    private void AttackInterval()
    {
        _canAttack = true;
    }

    private void CanMove()
    {
        _canMove = true;
    }


    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void Cleanse()
    {
        _canAttack = true;
        _canMove = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
    
    private void hit()
    {
        Collider2D[] playerToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsPlayer);
        for (int i = 0; i < playerToDamage.Length; i++)
        {
            playerToDamage[i].GetComponent<PlayerContoller>().TakeDamage(damage);
        }
    }

    private void OnDamageTaken(int damage)
    {
        Debug.Log("Damage taken: " + damage);
        _canMove = false;
        OnDestroy();
    }
    
    private void OnDestroy()
    {
        _enemyHealth.OnDamageTaken -= OnDamageTaken;
    }

    private void FlipSprite(Transform dest)
    {
        spriteRenderer.flipX = (transform.position.x - dest.position.x < 0);
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
