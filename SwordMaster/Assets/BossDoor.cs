using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : MonoBehaviour
{
    [SerializeField] private GameObject doorRb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private bool _nextLevel;
    private Animator _animator;
    private string _currentAnimation;
    
    
    const string LOCKED = "Locked"; 
    const string OPEN = "Open";
    
    
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        ChangeAnimationState(LOCKED);
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        _nextLevel = TargetManager.Instance.CanEnterBoss();
        if (_nextLevel == true)
        {
            ChangeAnimationState(OPEN);
          
        }
    }

    private void OpenDoor()
    {
        Destroy(doorRb);
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
