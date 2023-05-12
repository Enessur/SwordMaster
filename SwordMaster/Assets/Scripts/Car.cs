using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    protected float speed;
    protected int []tires;
    
    protected virtual void SpeedUp(float value)
    {
        speed += value;
    }
}
