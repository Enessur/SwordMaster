using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevel : MonoBehaviour
{
    private bool _nextLevel;
    private void OnTriggerEnter2D(Collider2D col)
    {
        _nextLevel = TargetManager.Instance.IsEnemyEmpty();
        if (_nextLevel == true)
        {
            SceneManager.Instance.WinGame();
        }
    }
    
}
