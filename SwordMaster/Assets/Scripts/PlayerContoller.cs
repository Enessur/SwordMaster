using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContoller : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 60f;
    [SerializeField] private float dashAmount = 50f;
    
    private Rigidbody2D _rb;
    private Vector3 _moveDir;
    private Animator _animator;
    private string _currentAnimation;
    private bool _isDashButtonDown;
    // private bool isAttacking;
    // private bool isAttackPressed;
    
    public SpriteRenderer _renderer;
  
    //Animation States
    private const string PLAYER_IDLE = "Idle"; const string PLAYER_RUN = "Run";
    // private const string PLAYER_ATTACK1 = "Attack1";
    // private const string PLAYER_ATTACK2 = "Attack2";
    // private const string PLAYER_ATTACK3 = "Attack3";
    

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleControl();
    }

    // add roll and collider to dash 
    private void FixedUpdate()
    {
        _rb.velocity = _moveDir * moveSpeed;

        if (_isDashButtonDown)
        {
            Vector3 dashPosition = transform.position + _moveDir * dashAmount;

            _rb.MovePosition(dashPosition);
            _isDashButtonDown = false;
        }
    }

    private void HandleControl()
    {
        float moveX = 0f;
        float moveY = 0f;

        //todo:find a better way to use flipX
        
        if (Input.GetKey(KeyCode.W))
        {
            moveY = +1f;
            _renderer.flipX = !true;
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveY = -1f;
            _renderer.flipX = !true;
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveX = -1f;
            _renderer.flipX = true;
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveX = +1f;
            _renderer.flipX = !true;
        }

        _moveDir = new Vector3(moveX, moveY).normalized;

        if (moveX != 0 || (moveY != 0))
        {
            ChangeAnimationState(PLAYER_RUN);
        }
        else
        {
            ChangeAnimationState(PLAYER_IDLE);
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _isDashButtonDown = true;
        }
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
}