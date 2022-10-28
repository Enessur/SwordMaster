using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum TaskCycleEnemy
    {
        Chase,
        Patrol,
        Stop
    }
    
    
    [SerializeField] private TaskCycleEnemy taskCycleEnemy;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;
    [SerializeField] public float startWaitTime = 1f;
    [SerializeField] private float chasingDistance;
    [SerializeField] private float killDistance;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool oneTime;
    public Transform moveSpot;
    private Transform _target;
    private float _patrolTimer;
    
    public int health;
    public float speed;

    //ChickenHunter Dog as a referance
    
    void Start()
    {
        oneTime = false;
        moveSpot.SetParent(null);
        _target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, _target.position) < chasingDistance)
        {
            taskCycleEnemy = TaskCycleEnemy.Chase;
        }
        else
        {
            taskCycleEnemy = TaskCycleEnemy.Patrol;
        }

        if (Vector2.Distance(transform.position, _target.position) < killDistance)
        {
            taskCycleEnemy = TaskCycleEnemy.Stop;
        }
        
        switch (taskCycleEnemy)
        {
            case TaskCycleEnemy.Chase:
                transform.position =
                    Vector2.MoveTowards(transform.position, _target.position, chaseSpeed * Time.deltaTime);
                FlipSprite(_target);
                break;
            case TaskCycleEnemy.Patrol:
                PatrolPosition();
                transform.position =
                    Vector2.MoveTowards(transform.position, moveSpot.position, speed * Time.deltaTime);
                FlipSprite(moveSpot);
                break;
            case TaskCycleEnemy.Stop:
                GameOver();

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
}
