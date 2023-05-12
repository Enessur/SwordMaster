using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBoss : MonoBehaviour
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
    [SerializeField] private GameObject blastSpellRb;
    [SerializeField] private GameObject meteoriteSpellRb;
    [SerializeField] private GameObject flameSpellRb;
    [SerializeField] private Transform attackPos;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private int damage;
    [SerializeField] private float startWaitTime = 1f;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float xMin, yMin, xMax, yMax;
    [SerializeField] private float chasingDistance;
    [SerializeField] private float attackDistance;
    [SerializeField] private float castDistanceMax;
    [SerializeField] private float castDistanceMin;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private bool oneTime;
    public GameObject healthBar;
    public BossHealthBar bossHealthBar;
    public float patrolSpeed;
    public GameObject PatrolBorders;
    public Transform moveSpot;
    public float speed;

    private Vector3 PatrolPos;
    private EnemyHealth _enemyHealth;
    private Transform _target;
    private Animator _animator;
    private int _currentHealth;
    private int _attackNum;
    private float _patrolTimer;
    private string _currentAnimation;
    private bool _canAttack = true;
    private bool _canMove = true;
    private Material matWhite;
    private Material matDefault;
    private bool _isDamageTaken = false;
    //private bool _IsPlayerAlive = true;

    //Animation States
    const string ENEMY_IDLE = "Idle";
    const string ENEMY_RUN = "Run";
    const string ENEMY_ATTACK = "Attack";
    const string ENEMY_CASTSPELL = "CastSpell";

    const string ENEMY_DEATH = "Death";
    //const string ENEMY_TAKEDAMAGE = "TakeDamage";

    void Start()
    {
        BoxCollider2D squareCollider = PatrolBorders.GetComponent<BoxCollider2D>();

        xMin = PatrolBorders.transform.position.x - squareCollider.size.x / 2;
        xMax = PatrolBorders.transform.position.x + squareCollider.size.x / 2;
        yMin = PatrolBorders.transform.position.y - squareCollider.size.y / 2;
        yMax = PatrolBorders.transform.position.y + squareCollider.size.y / 2;

        PatrolPos = new Vector2(Random.Range(xMin, xMax), Random.Range(yMin, yMax));

        TargetManager.Instance.AddEnemy(this.transform);

        oneTime = false;
        moveSpot.SetParent(null);
        _target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _animator = GetComponent<Animator>();
        PatrolBorders.transform.parent = null;
        
        matWhite = Resources.Load("WhiteFlash",typeof(Material)) as Material;
        matDefault = spriteRenderer.material;
        _currentHealth = GetComponent<EnemyHealth>().health;
        bossHealthBar.SetMaxHealth(_currentHealth);
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
                    healthBar.SetActive(true);
                    taskCycleEnemy = TaskCycleEnemy.CastSpell;
                }
                else
                {
                    taskCycleEnemy = TaskCycleEnemy.Patrol;
                    healthBar.SetActive(false);
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
                    transform.position =
                        Vector2.MoveTowards(transform.position, PatrolPos, patrolSpeed * Time.deltaTime);
                moveSpot.position = PatrolPos;
                FlipSprite(moveSpot);
                ChangeAnimationState(ENEMY_RUN);
                break;

            case TaskCycleEnemy.Idle:
                ChangeAnimationState(ENEMY_IDLE);
                GameOver();
                break;

            case TaskCycleEnemy.Attack:
                FlipSprite(_target);
                Attack();
                break;

            case TaskCycleEnemy.CastSpell:
                ChangeAnimationState(ENEMY_CASTSPELL);
               
                castDistanceMin = 50f;
                castDistanceMax = 0f;
                chasingDistance = 0f;
                attackDistance = 0f;
                break;

            case TaskCycleEnemy.Death:
                ChangeAnimationState(ENEMY_DEATH);
                TargetManager.Instance.RemoveEnemy(this.transform);
                break;
        }
    }

    private void OnDamageTaken(int damage)
    {
       
        _isDamageTaken = false;
        OnDestroy();
    }

    private void OnDestroy()
    {
        Hurt();
        _enemyHealth.OnDamageTaken -= OnDamageTaken;
        _isDamageTaken = false;
      
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

        transform.position = Vector2.MoveTowards(transform.position, PatrolPos, patrolSpeed * Time.deltaTime);

        if (transform.position == (Vector3)PatrolPos)
        {
            PatrolPos = new Vector2(Random.Range(xMin, xMax), Random.Range(yMin, yMax));
        }
    }

    private void Hurt()
    {
      
        spriteRenderer.material = matWhite;
        Invoke(nameof(ResetMaterial),.2f);
    }
    void ResetMaterial()
    {
        spriteRenderer.material = matDefault;
        bossHealthBar.SetHealth(_currentHealth);

    }
    private void Attack()
    {
        if (_canAttack)
        {
            if (transform.position.x - _target.position.x > 0f)
            {
                attackPos.position = transform.position + new Vector3(-2.3f, 0f, 0f);
            }
            else
            {
                attackPos.position = transform.position + new Vector3(+2.3f, 0f, 0f);
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

    private void CastSpell()
    {
        _attackNum = Random.Range(1, 4);
       
        if (_attackNum == 1)
        {
            Instantiate(blastSpellRb, _target.position, _target.rotation);
        }

        if (_attackNum == 2)
        {
            Instantiate(meteoriteSpellRb, _target.position, _target.rotation);
        }

        if (_attackNum == 3)
        {
            Instantiate(flameSpellRb, _target.position, _target.rotation);
        }
    }

    private void AttackRangeDecrease()
    {
        castDistanceMax = 2f;
        castDistanceMin = 10f;
        attackDistance = 2f;
        chasingDistance = 3f;
    }

    private void DestroyEnemy()
    {
        SceneManager.Instance.WinGame();
        Destroy(gameObject);
    }

    private void Cleanse()
    {
        _canAttack = true;
        _canMove = true;
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