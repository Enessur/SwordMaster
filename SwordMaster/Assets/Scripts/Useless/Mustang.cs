using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mustang : Car
{

    private const string LevelKey = "LevelIndex";
    private void Start()
    {
        SaveSpeed();
        SpeedUp(1);
    }

    protected override void SpeedUp(float value)
    {
        speed *= value;
    }

    
    private void SaveSpeed()
    {
       var a = ES3.Load(LevelKey, "a",1);
       ES3.Save(LevelKey,1);
    }
}
