using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerEnemy : MonoBehaviour
{
    public enum TaskCycleEnemy
    {
        Chase,
        Patrol,
        Attack,
        CastSpell,
        Death,
        Idle
    }

    [SerializeField] private TaskCycleEnemy taskCycleEnemy;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject shadowRb;
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
    [SerializeField] private float castDistanceMax;
    [SerializeField] private float castDistanceMin;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private bool oneTime;
    
    

    public Transform moveSpot;
    public float speed;

    private EnemyHealth _enemyHealth;
   // private PlayerContoller _playerHealth;
    //private int _playerCurrentHealt;
    private Transform _target;
    private Animator _animator;
    private int _currentHealth;
    private float _patrolTimer;
    private string _currentAnimation;
    private bool _canAttack = true;
    private bool _canMove = true;
    private bool _isDamageTaken = false;
    private bool _IsPlayerAlive = true;
    
    //Animation States
    const string ENEMY_IDLE = "Idle";
    const string ENEMY_RUN = "Run";
    const string ENEMY_ATTACK = "Attack";
    const string ENEMY_CASTSPELL = "SpellCast";
    const string ENEMY_DEATH = "Death";
    const string ENEMY_TAKEDAMAGE = "TakeDamage";

    void Start()
    {
        oneTime = false;
        moveSpot.SetParent(null);
        _target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _animator = GetComponent<Animator>();
        
    }

    void Update()
    {
        _enemyHealth = GetComponent<EnemyHealth>();
        _currentHealth = GetComponent<EnemyHealth>().health;
        
       // _playerHealth = GetComponent<PlayerContoller>();
       // _playerCurrentHealt = GetComponent<PlayerContoller>().playerHealth;
        // if (_playerCurrentHealt < 1)
        // {
        //     _IsPlayerAlive = false;
        // }
        if (!_isDamageTaken)
        {
            _enemyHealth.OnDamageTaken += OnDamageTaken;
                
            _isDamageTaken = true;
        }

        
        if (_currentHealth >= 1)
        {
            if (_canMove == true)
            {
                if (Vector2.Distance(transform.position, _target.position) < chasingDistance)
                {
                    taskCycleEnemy = TaskCycleEnemy.Chase;
                }
                else if ((Vector2.Distance(transform.position, _target.position) > castDistanceMax) &&
                         (Vector2.Distance(transform.position, _target.position) < castDistanceMin))
                {
                    taskCycleEnemy = TaskCycleEnemy.CastSpell;
                }
                else
                {
                    taskCycleEnemy = TaskCycleEnemy.Patrol;
                }
            }

            if ((Vector2.Distance(transform.position, _target.position) < attackDistance))
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
                    Vector2.MoveTowards(transform.position, moveSpot.position, speed * Time.deltaTime);
                FlipSprite(moveSpot);
                ChangeAnimationState(ENEMY_RUN);
                break;

            case TaskCycleEnemy.Idle:
                ChangeAnimationState(ENEMY_IDLE);
                GameOver();
                break;

            case TaskCycleEnemy.Attack:
                Attack();
                break;

            case TaskCycleEnemy.CastSpell:
                ChangeAnimationState(ENEMY_CASTSPELL);
                break;

            case TaskCycleEnemy.Death:
                ChangeAnimationState(ENEMY_DEATH);
                break;
        }
    }

    private void OnDamageTaken(int damage)
    {
        Debug.Log("Damage taken: " + damage);
        _isDamageTaken = false;
        ChangeAnimationState(ENEMY_TAKEDAMAGE);
    }
    private void OnDestroy()
    {
        _enemyHealth.OnDamageTaken -= OnDamageTaken;
    }

    private void FlipSprite(Transform dest)
    {
        spriteRenderer.flipX = (transform.position.x - dest.position.x < 0);
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
            if (transform.position.x - _target.position.x > 0f)
            {
                attackPos.position = transform.position + new Vector3(-2f, 0f, 0f);
            }
            else
            {
                attackPos.position = transform.position + new Vector3(+2f, 0f, 0f);
            }

            _canMove = false;
            ChangeAnimationState(ENEMY_ATTACK);
            _canAttack = false;
        }
    }

    private void GameOver()
    {
        if (oneTime) return;
        // SoundManager.Instance.PlaySound("");
        // SceneManager.Instance.LoseGame();
        oneTime = true;
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
        Collider2D[] playerToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsPlayer);
        for (int i = 0; i < playerToDamage.Length; i++)
        {
            playerToDamage[i].GetComponent<PlayerContoller>().TakeDamage(damage);
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

    private void ShadowAttack()
    {
        Instantiate(shadowRb, _target.position, _target.rotation);
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
    
    public void gethit()
    {
        ChangeAnimationState(ENEMY_TAKEDAMAGE);
    }

    public void die()
    {
        ChangeAnimationState(ENEMY_DEATH);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}