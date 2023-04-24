using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingSword : MonoBehaviour
{
    public enum TaskCycleSword
    {
        Chase,
        Attack,
        Follow,
    }

    public float swordPatrolRange = 10f;

    [SerializeField] private TaskCycleSword taskCycleSword;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackArea = 1.5f;
    [SerializeField] private Transform attackPos;
    [SerializeField] private LayerMask whatIsEnemies;
    [SerializeField] private int damage;
    
    private Transform _target;
    private Transform _enemyTarget;
    private Vector2 _followPosition;
    private Vector2 _offsetLeft = new Vector2(-1.09f, 0.63f);
    private Vector2 _offsetRight = new Vector2(1.09f, 0.63f);
    private float _rotationSpeed = 15f;
    private float _minAngle = -10f;
    private float _maxAngle = 10f;
    private float _randomX;
    private float _randomY;
    private float currentAngle;
    private int _attackNum;

    //Animation
    private string _currentAnimation;
    private Animator _animator;
    private bool _canAttack = true;
    private bool _canMove = true;

    const string ATTACK_1 = "SliceAttack";
    const string ATTACK_2 = "DownUpAttack";
    const string ATTACK_3 = "UpDownAttack";
    const string IDLE = "Idle";

    void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
       // _enemyTarget = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Transform>();
        _animator = GetComponent<Animator>();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        TargetManager.Instance.GetSword(swordPatrolRange);
    }

    void FixedUpdate()
    {
        FindEnemy();

        _randomX = Random.Range(0.2f, -0.2f);
        _randomY = Random.Range(0.2f, -0.2f);

        Vector2 randomMovement = new Vector2(_randomX, _randomY);
        Vector2 targetPosition = new Vector2(_target.position.x, _target.position.y);
        Vector2 swordPosition = new Vector2(transform.position.x, transform.position.y);

        if (_canMove)
        {
            if ((_enemyTarget != null))
            {
                if ((Vector2.Distance(_enemyTarget.position, transform.position) < attackRange) &&
                    (Vector2.Distance(_enemyTarget.position, _target.position) < swordPatrolRange))
                {
                    taskCycleSword = TaskCycleSword.Attack;
                }
                else if ((Vector2.Distance(_enemyTarget.position, _target.position) < swordPatrolRange))
                {
                    taskCycleSword = TaskCycleSword.Chase;
                }
                else
                {
                    taskCycleSword = TaskCycleSword.Follow;
                }
            }
            else
            {
                taskCycleSword = TaskCycleSword.Follow;
            }
        }

        switch (taskCycleSword)
        {
            case TaskCycleSword.Follow:

                ChangeAnimationState(IDLE);

                Vector2 direction = (_followPosition - swordPosition);
                Vector2 newPosition = swordPosition + direction * chaseSpeed * Time.deltaTime;
                newPosition += randomMovement * Time.deltaTime * 0.7f;


                transform.Rotate(0f, 0f, _rotationSpeed * Time.deltaTime);
                currentAngle += _rotationSpeed * Time.deltaTime;
                if (currentAngle > _maxAngle || currentAngle < _minAngle)
                {
                    _rotationSpeed *= -1f;
                }

                if (targetPosition.x - swordPosition.x > 0)
                {
                    _followPosition = targetPosition + _offsetLeft;
                    _animator.SetBool("Mirror", false);
                    transform.rotation = Quaternion.Euler(0, 180f, 41f);
                    Debug.Log("Left attack");
                    //attackPos.position = transform.position + new Vector3(-1.4f, 0f, 0f);
                }
                else
                {
                    _followPosition = targetPosition + _offsetRight;
                    _animator.SetBool("Mirror", true);
                    transform.rotation = Quaternion.Euler(0, 0, 41f);
                    Debug.Log("Right attack");
                    //attackPos.position = transform.position + new Vector3(+1.4f, 0f, 0f);
                }

                transform.position = new Vector2(newPosition.x, newPosition.y);

                break;

            case TaskCycleSword.Chase:

                ChangeAnimationState(IDLE);


                Vector2 enemyPosition = new Vector2(_enemyTarget.position.x, _enemyTarget.position.y);
                if (enemyPosition.x - swordPosition.x > 0)
                {
                    _followPosition = enemyPosition + _offsetLeft;
                   
                    transform.rotation = Quaternion.Euler(0, 0, 41f);
                    Debug.Log("Left attack");
                }
                else
                {
                    _followPosition = enemyPosition + _offsetRight;
                   
                    transform.rotation = Quaternion.Euler(0, 180f, 41f);
                    Debug.Log("Right attack");
                }

                direction = (_followPosition - swordPosition);
                newPosition = swordPosition + direction * chaseSpeed * Time.deltaTime;
                transform.position = new Vector2(newPosition.x, newPosition.y);
                if (_enemyTarget == null)
                {
                    taskCycleSword = TaskCycleSword.Follow;
                }

                break;

            case TaskCycleSword.Attack:

                if (_canAttack == true)
                {
                    attackRange = 20f;
                    AttackAnim();
                    _canAttack = false;
                    _canMove = false;
                }

                break;
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

    private void AttackInterval()
    {
        _canAttack = true;
    }

    private void CanMove()
    {
        _canMove = true;
    }

    private void AttackRangeDecrease()
    {
        attackRange = 1.5f;
    }

    public void FindEnemy()
    {
        _enemyTarget = TargetManager.Instance.FindClosestTarget(gameObject.transform.position);
    }

    private void AttackAnim()
    {
        _attackNum = Random.Range(1,4);
        Debug.Log(_attackNum);
        if (_attackNum == 1)
        {
            ChangeAnimationState(ATTACK_1);
        }

        if (_attackNum == 2)
        {
            ChangeAnimationState(ATTACK_2);
        }

        if (_attackNum == 3)
        {
            ChangeAnimationState(ATTACK_3);
            
        }
    }
    private void hit()
    {
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackArea, whatIsEnemies);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            enemiesToDamage[i].GetComponent<EnemyHealth>().TakeDamage(damage);
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackArea);
    }
}