using System;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using Script;
using UnityEngine.UI;


public class PlayerContoller : MonoBehaviour
{
    public static PlayerContoller Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public enum TaskCycles
    {
        Move,
        Attack,
    }


    public int playerHealth;
    public HealthBar healthbar;
    public Ghost ghost;
    public bool makeGhost = false;

    [SerializeField] private float moveSpeed = 60f;
    [SerializeField] private float dashSpeed = 5f;
    [SerializeField] private Transform attackPos;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask whatIsEnemies;
    [SerializeField] private int damage;
    [SerializeField] private TaskCycles taskCycle;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private bool oneTime;
    [SerializeField] private Text Armor;
    [SerializeField] private GameObject armorCanvas;


    private Rigidbody2D _rb;
    private Vector3 _moveDir;
    private Vector3 _rollDir;
    private Animator _animator;
    private string _currentAnimation;
    private bool _isDashButtonDown;
    private bool _canMove;
    private float _attackTimer;
    private bool _canAttack = true;
    private int _attackNum = 0;
    private Shake _shake;
    private bool _notWearArmor;
    private int _playerMaxHealt;

    //Animation States
    const string PLAYER_IDLE = "Idle";
    const string PLAYER_RUN = "Run";
    const string PLAYER_IDLE_NO_ARMOR = "IdleNoArmor";
    const string PLAYER_RUN_NO_ARMOR = "RunNoArmor";
    const string PLAYER_ATTACK1 = "Attack1";
    const string PLAYER_ATTACK2 = "Attack2";
    const string PLAYER_ATTACK3 = "Attack3";
    const string PLAYER_ATTACK4 = "Attack4";
    const string PLAYER_ATTACK5 = "Attack5";
    const string ARMOR_UPGRADE = "WearArmor";
    const string TAKE_DAMAGE = "TakeDamage";
    const string DEATH = "Death";
    const string DASH = "Dash";
    const string DEATH_NO_ARMOR = "DeathNoArmor";
    const string TAKE_DAMAGE_NO_ARMOR = "TakeDamageNoArmor";


    private void Start()
    {
        oneTime = false;
        _shake = GameObject.FindWithTag("ScreenShake").GetComponent<Shake>();
        _rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _canMove = true;
        _canAttack = true;
        _playerMaxHealt = playerHealth;
        healthbar.SetMaxHealth(playerHealth);
        armorCanvas.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            taskCycle = TaskCycles.Attack;
        }

        else
        {
            taskCycle = TaskCycles.Move;
        }

        switch (taskCycle)
        {
            case TaskCycles.Move:

                HandleControl();
                break;

            case TaskCycles.Attack:
                Attack();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (_isDashButtonDown)
        {
            _rb.velocity = _moveDir * (moveSpeed + dashSpeed);
        }
        else
        {
            _rb.velocity = _moveDir * moveSpeed;
        }
    }


    private void HandleControl()
    {
        float moveX = 0f;
        float moveY = 0f;
        if (_canMove == true)
        {
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
                ghost.makeGhost = true;
                if (_isDashButtonDown != true)
                {
                    if (_notWearArmor == true)
                    {
                        ChangeAnimationState(PLAYER_RUN_NO_ARMOR);
                    }
                    else
                    {
                        ChangeAnimationState(PLAYER_RUN);
                    }
                }

                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    Dash();
                }
            }
            else
            {
                if (_notWearArmor == true)
                {
                    ChangeAnimationState(PLAYER_IDLE_NO_ARMOR);
                }
                else
                {
                    ChangeAnimationState(PLAYER_IDLE);
                }

                ghost.makeGhost = false;
                _isDashButtonDown = false;
            }
        }
        else
        {
            _moveDir = new Vector3(0, 0).normalized;
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

    private void Dash()
    {
        if (_notWearArmor == false)
        {
            _canAttack = false;
            _isDashButtonDown = true;
            ghost.makeGhost = true;
            ChangeAnimationState(DASH);
        }
    }

    private void FinishDash()
    {
        _isDashButtonDown = false;
    }

    private void hit()
    {
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            _shake.CamShake();
            enemiesToDamage[i].GetComponent<EnemyHealth>().TakeDamage(damage);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (_notWearArmor == true)
        {
            if (col.CompareTag("Armor"))
            {
                Debug.Log("WearArmor!");
                ChangeAnimationState(ARMOR_UPGRADE);
                Armor.text = "You Find The Armor!!";
                _canMove = false;
                armorCanvas.SetActive(true);
            }

            else if (col.CompareTag("Empty"))
            {
                Debug.Log("Empty!");
                Armor.text = "This House is Empty!!";
                armorCanvas.SetActive(true);
            }
            else
            {
                armorCanvas.SetActive(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        armorCanvas.SetActive(false);
    }

    private void Attack()
    {
       
        ghost.makeGhost = true;
        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
        Vector3 attackDir = (mousePosition - transform.position);

        if (attackDir.x < 0)
        {
            _renderer.flipX = true;
            attackPos.position = transform.position + new Vector3(-1.4f, 0f, 0f);
        }
        else
        {
            _renderer.flipX = !true;
            attackPos.position = transform.position + new Vector3(+1.4f, 0f, 0f);
        }

        if (_canAttack == true)
        {
            Debug.Log("Player Attack");
            _canMove = false;
            _attackNum++;


            if (_attackNum == 1)
            {
                ChangeAnimationState(PLAYER_ATTACK1);
            }

            if (_attackNum == 2)
            {
                ChangeAnimationState(PLAYER_ATTACK2);
            }

            if (_attackNum == 3)
            {
                ChangeAnimationState(PLAYER_ATTACK3);
            }

            if (_attackNum == 4)
            {
                ChangeAnimationState(PLAYER_ATTACK4);
            }

            if (_attackNum == 5)
            {
                ChangeAnimationState(PLAYER_ATTACK5);
                _attackNum = 0;
            }

            _canAttack = false;
        }
        else
        {
            Debug.Log("Player Can't Attack");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    private void CanMove()
    {
        _canMove = true;
    }

    private void AttackInterval()
    {
        _canAttack = true;
    }

    public void Stop()
    {
        _canAttack = false;
        _canMove = false;
        _isDashButtonDown = false;
    }

    public void Cleanse()
    {
        _canAttack = true;
        _canMove = true;
    }

    public void TakeDamage(int takendamage)
    {
        playerHealth -= takendamage;
        if (_notWearArmor == true)
        {
            ChangeAnimationState(TAKE_DAMAGE_NO_ARMOR);
        }
        else
        {
            ChangeAnimationState(TAKE_DAMAGE);
        }

        _shake.CamShake();
        Debug.Log("Player took damage:"+takendamage);
        Debug.Log("Player remaining health :"+playerHealth);
        Stop();

        if (playerHealth < 1)
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
            if (_notWearArmor == true)
            {
                ChangeAnimationState(DEATH_NO_ARMOR);
            }
            else
            {
                ChangeAnimationState(DEATH);
            }

            Stop();
        }


        healthbar.SetHealth(playerHealth);
    }

    private void GameOver()
    {
        if (oneTime) return;
        SceneManager.Instance.LoseGame();
        oneTime = true;
    }

    public void GetHeal(int getHeal)
    {
        playerHealth += getHeal;
        if (playerHealth > _playerMaxHealt)
        {
            playerHealth = _playerMaxHealt;
        }

        healthbar.SetHealth(playerHealth);
    }

    public void WearArmor(bool _wearingArmor)
    {
        if (_wearingArmor == true)
        {
            _notWearArmor = true;
            _canAttack = false;
        }

        if (_wearingArmor == false)
        {
            _notWearArmor = false;
        }
    }

    public void BasicArmor()
    {
        _canAttack = false;
    }

    public void ArmorUpgrade()
    {
        _canAttack = true;
        _notWearArmor = false;
        _canMove = true;
    }
}