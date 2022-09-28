using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;
    public float speed;

    //ChickenHunter Dog as a referance
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Damage Taken");
    }
}
