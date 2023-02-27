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

    [SerializeField] private TaskCycleSword taskCycleSword;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float swordPatrolRange = 10f;
    
    private Transform _target;
    private Transform _enemyTarget;
   
    private Vector2 _followPosition;
    private Vector2 _offsetLeft = new Vector2(-1.09f, 0.63f);
    private Vector2 _offsetRight = new Vector2(1.09f, 0.63f);
    
    private float _distanceThreshold = 1f;
    private float _rotationSpeed = 15f; 
    private float _moveSpeed = 0.1f; 
    private float _minAngle = -10f;
    private float _maxAngle = 10f;
    private float _randomX;
    private float _randomY;
    private float currentAngle;
    
    
    void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _enemyTarget = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        _randomX = Random.Range(0.2f, -0.2f);
        _randomY = Random.Range(0.2f, -0.2f);
       
        Vector2 randomMovement = new Vector2(_randomX, _randomY);
        
        // transform.position = new Vector2(_randomX, _randomY);
        // _minAngle = Random.Range(-25f, -15f);
        // _maxAngle = Random.Range(15f, 25f);
  
        Vector2 targetPosition =
            new Vector2(_target.position.x, _target.position.y);
        Vector2 swordPosition =
            new Vector2(transform.position.x, transform.position.y);

        if ((_enemyTarget != null) && (Vector2.Distance(_enemyTarget.position, _target.position) < swordPatrolRange))
        {
            taskCycleSword = TaskCycleSword.Chase;
        }

        else
        {
            taskCycleSword = TaskCycleSword.Follow;
        }
        switch (taskCycleSword)
        {
            case TaskCycleSword.Follow:
                
                transform.Rotate(0f, 0f, _rotationSpeed * Time.deltaTime);
                currentAngle += _rotationSpeed * Time.deltaTime;
                if (currentAngle > _maxAngle || currentAngle < _minAngle)
                {
                    _rotationSpeed *= -1f;
                }
                if (targetPosition.x - swordPosition.x > 0)
                {
                    _followPosition = targetPosition + _offsetLeft;
                }
                else
                {
                    _followPosition = targetPosition + _offsetRight;
                }
                Vector2 direction = (_followPosition - swordPosition);
                Vector2 newPosition = swordPosition + direction * chaseSpeed * Time.deltaTime;
                newPosition += randomMovement * Time.deltaTime * 0.7f;
                transform.position = new Vector2(newPosition.x, newPosition.y);
               
                break;

            case TaskCycleSword.Chase:

                Vector2 enemyPosition = new Vector2(_enemyTarget.position.x, _enemyTarget.position.y);
                if (enemyPosition.x - swordPosition.x > 0)
                {
                    _followPosition = enemyPosition + _offsetLeft;
                }
                else
                {
                    _followPosition = enemyPosition + _offsetRight;
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
                    break;
                
        }
    }
}