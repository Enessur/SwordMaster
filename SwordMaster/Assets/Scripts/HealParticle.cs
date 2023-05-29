using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class HealParticle : MonoBehaviour
{
 
    private Transform _target;
    [SerializeField] private GameObject healRb;
    [SerializeField] private float particleSpeed;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private int heal = 5;
    
    private Animator _animator;
    private float followdistance = 0.5f;
    private string _currentAnimation;

    const string HEAL = "Heal"; 
    const string IDLE = "Idle";
    
    private void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _animator = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        transform.position =
            Vector2.MoveTowards(transform.position, _target.position,
                particleSpeed * Time.deltaTime / 0.5f);
     
        if (Vector2.Distance(_target.position, transform.position)< followdistance)
        {
            ChangeAnimationState(HEAL);
           
        }
        else
        {
            ChangeAnimationState(IDLE);
        }
       
    }
    
    private void Heal()
    {
        PlayerContoller.Instance.GetHeal(heal);
        followdistance = 10f;

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
    private void Destroy()
    {
        Destroy(healRb);
    }
}
