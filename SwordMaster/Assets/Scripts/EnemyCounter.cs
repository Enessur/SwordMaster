using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCounter : Singleton<EnemyCounter>
{
    public int enemyCount;
    public Text enemyCountText;
    public void EnemyCountDecrease(int enemyCount)
    {
        enemyCountText.text = "Enemies Left : "+enemyCount;
    }
}
