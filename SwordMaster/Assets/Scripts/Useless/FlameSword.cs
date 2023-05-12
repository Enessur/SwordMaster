using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameSword : MonoBehaviour
{
    [SerializeField] private GameObject shadowRb;
    [SerializeField] private Transform attackPos;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private int damage;

    void Start()
    {
        transform.position = transform.position + new Vector3(0.3f, 1.7f, 0f);
    }

    private void hit()
    {
        Collider2D[] playerToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsPlayer);
        for (int i = 0; i < playerToDamage.Length; i++)
        {
            playerToDamage[i].GetComponent<PlayerContoller>().TakeDamage(damage);
        }
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}
