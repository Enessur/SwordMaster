using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;
using UnityEngine.Android;

public class TargetManager : Singleton<TargetManager>
{
    public List<Transform> enemyTransforms = new List<Transform>();
    
    public GameObject closestTarget;
    [SerializeField] private float detectionDistance;
    private Vector3 _offset;
    private float _currentDistance;
    private float _closestDistance;

    public void GetSword(float swordAttackDistance)
    {
        detectionDistance = swordAttackDistance;
    }

    public void AddEnemy(Transform tr)
    {
        enemyTransforms.Add(tr);
    }

    public void RemoveEnemy(Transform tr)
    {
        enemyTransforms.Remove(tr);
        CheckWinCondition();
    }


    public Transform FindClosestTarget(Vector3 swordPosition)
    {
        if (enemyTransforms.Count != 0)
        {
            _closestDistance = 100000f;
            foreach (var t in enemyTransforms)
            {
                {
                    _offset = swordPosition - t.gameObject.transform.position;
                    _currentDistance = Vector3.Magnitude(_offset);

                    if (_closestDistance > _currentDistance)
                    {
                        _closestDistance = _currentDistance;
                        closestTarget = t.gameObject;
                    }
                }
            }

            if (_closestDistance < detectionDistance)
            {
                return closestTarget.transform;
            }

            //todo: game over ! 
            
            return null;
        }

        return null;
    }
    public bool IsEnemyEmpty()
    {
        return enemyTransforms.Count == 0;
    }

    public bool CanEnterBoss()
    {
        return enemyTransforms.Count == 1;
    }

    public void CheckWinCondition()
    {
        if (IsEnemyEmpty())
        {
           
        }

        if (CanEnterBoss())
        {
            
        }
    }
    
}