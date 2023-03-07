using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHealth : MonoBehaviour
{
    int _currentHealth;
    int _currentMaxHealth;

    public int Health
    {
        get { return _currentHealth; }
        set { _currentHealth = value; }
    }

    public int MaxHealth
    {
        get { return _currentMaxHealth; }
        set { _currentMaxHealth = value; }
    }

    // Consturctor
    public UnitHealth(int health, int maxHealth)
    {
        _currentHealth = health;
        _currentMaxHealth = maxHealth;
    }

    //Methods

    public void DmgUnit(int dmgAmount)
    {
        if (_currentHealth > 0)
        {
            _currentMaxHealth -= dmgAmount;
        }
    }

    public void HealUnit(int healAmount)
    {
        if (_currentHealth < _currentMaxHealth)
        {
            _currentHealth += healAmount;
        }

        if (_currentHealth > _currentMaxHealth)
        {
            _currentHealth = _currentMaxHealth;
        }
    }
}