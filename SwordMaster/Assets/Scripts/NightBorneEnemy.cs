using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NightBorneEnemy : MonoBehaviour
{
    public enum TaskCycleEnemy
    {
        Chase,
        Attack,
        Idle,
        Death,
        Patrol,
        Teleport
    }


    [SerializeField] private TaskCycleEnemy taskCycleEnemy;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform attackPos;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private int damage;
    [SerializeField] private float startWaitTime = 1f;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float xMin, yMin, xMax, yMax;
    [SerializeField] private float chasingDistance;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float teleportRange;
    [SerializeField] private bool isTeleporting;


    public Transform moveSpot;
    public float patrolSpeed;
    public GameObject PatrolBorders;

    private Vector3 PatrolPos;
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

    //Animation States 
    const string ENEMY_IDLE = "Idle";
    const string ENEMY_RUN = "Run";
    const string ENEMY_ATTACK = "Attack";
    const string ENEMY_DEATH = "Death";
    const string ENEMY_TAKEDAMAGE = "TakeDamage";


    void Start()
    {
        BoxCollider2D squareCollider = PatrolBorders.GetComponent<BoxCollider2D>();

        xMin = PatrolBorders.transform.position.x - squareCollider.size.x / 2;
        xMax = PatrolBorders.transform.position.x + squareCollider.size.x / 2;
        yMin = PatrolBorders.transform.position.y - squareCollider.size.y / 2;
        yMax = PatrolBorders.transform.position.y + squareCollider.size.y / 2;

        PatrolPos = new Vector2(Random.Range(xMin, xMax), Random.Range(yMin, yMax));

        TargetManager.Instance.AddEnemy(this.transform);

        isTeleporting = false;
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
                else if (Vector2.Distance(transform.position, _target.position) <= teleportRange && !isTeleporting)
                {
                    taskCycleEnemy = TaskCycleEnemy.Teleport;
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
                    transform.position =
                        Vector2.MoveTowards(transform.position, PatrolPos, patrolSpeed * Time.deltaTime);
                moveSpot.position = PatrolPos;
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
                TargetManager.Instance.RemoveEnemy(this.transform);
                break;
            case TaskCycleEnemy.Teleport:
                Teleport();
                break;
        }
    }


    private void PatrolPosition()
    {
        _patrolTimer += Time.deltaTime;

        if (!(_patrolTimer >= startWaitTime)) return;
        _patrolTimer = 0;

        transform.position =
            transform.position = Vector2.MoveTowards(transform.position, PatrolPos, patrolSpeed * Time.deltaTime);

        if (transform.position == (Vector3)PatrolPos)
        {
            PatrolPos = new Vector2(Random.Range(xMin, xMax), Random.Range(yMin, yMax));
        }
    }

    private void Attack()
    {
        if (_canAttack)
        {
            if (transform.position.x - _target.position.x > 0f)
            {
                attackPos.position = transform.position + new Vector3(-1.2f, -0.5f, 0f);
            }
            else
            {
                attackPos.position = transform.position + new Vector3(+1.2f, -0.5f, 0f);
            }

            _canMove = false;
            ChangeAnimationState(ENEMY_ATTACK);
            _canAttack = false;
        }
    }

    private void OnDamageTaken(int damage)
    {
        Debug.Log("Damage taken: " + damage);
        _canMove = false;
        ChangeAnimationState(ENEMY_TAKEDAMAGE);
        OnDestroy();
    }

    private void OnDestroy()
    {
        _enemyHealth.OnDamageTaken -= OnDamageTaken;
    }

    private void FlipSprite(Transform dest)
    {
        spriteRenderer.flipX = (transform.position.x - dest.position.x > 0);
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

    private void Teleport()
    {
        Vector3 randomPos = new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f));
        transform.position = _target.position + randomPos;
        isTeleporting = false;
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
}