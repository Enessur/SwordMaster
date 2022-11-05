using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum TaskCycleEnemy
    {
        Chase,
        Patrol,
        Attack,
        CastSpell,
        Stop
    }
    
    [SerializeField] private TaskCycleEnemy taskCycleEnemy;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject shadowRb;
    [SerializeField] private float startWaitTime = 1f;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;
    [SerializeField] private float chasingDistance;
    [SerializeField] private float killDistance;
    [SerializeField] private float castDistanceMax;
    [SerializeField] private float castDistanceMin;
    [SerializeField] private bool oneTime;
    
    public Transform moveSpot;
    public int health;
    public float speed;
   
    private Transform _target;
    private float _patrolTimer;
    private string _currentAnimation;
    private Animator _animator;
    private bool _canAttack = true;
    private bool _canMove = true;

    //Animation States
    const string ENEMY_IDLE = "Idle";
    const string ENEMY_RUN = "Run";
    const string ENEMY_ATTACK = "Attack"; 
    const string ENEMY_CASTSPELL = "SpellCast";
    void Start()
    {
        oneTime = false;
        moveSpot.SetParent(null);
        _target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (_canMove == true)
        {
            
        if (Vector2.Distance(transform.position, _target.position) < chasingDistance)
        {
            taskCycleEnemy = TaskCycleEnemy.Chase;
        } 
        else if ((Vector2.Distance(transform.position, _target.position)>castDistanceMax) && (Vector2.Distance(transform.position,_target.position)<castDistanceMin))
        {
            taskCycleEnemy = TaskCycleEnemy.CastSpell;
        }
        else
        {
            taskCycleEnemy = TaskCycleEnemy.Patrol;
        }
        }

        if (Vector2.Distance(transform.position, _target.position) < killDistance)
        {
            taskCycleEnemy = TaskCycleEnemy.Attack;
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
            case TaskCycleEnemy.Stop:
                ChangeAnimationState(ENEMY_IDLE);
                GameOver();
                break;
            case TaskCycleEnemy.Attack:
                Attack();
                break;
            case TaskCycleEnemy.CastSpell:
                ChangeAnimationState(ENEMY_CASTSPELL);
                break;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Damage Taken");
    }

    private void PatrolPosition()
    {
        _patrolTimer += Time.deltaTime;

        if (!(_patrolTimer >= startWaitTime)) return;
        _patrolTimer = 0;

        moveSpot.position = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
    }

    public void Attack()
    {
        if (_canAttack == true)
        {
            _canMove = false;
            ChangeAnimationState(ENEMY_ATTACK);
            _canAttack = false;
        }
    }

    private void GameOver()
    {
        if (oneTime) return;
        // SoundManager.Instance.PlaySound("dogBark");
        // SceneManager.Instance.LoseGame();
        oneTime = true;
    }

    public void FlipSprite(Transform dest)
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
    
}