using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int health;
    public int currentHealth { get => health; set => health = value; }

    public int MaxHealth {get => maxHealth; set => maxHealth = value; }

    public event IDamageable.TakeDamageEvent OnTakeDamage;
    public event IDamageable.DeathEvent OnDeath;

    private void OnEnable() {
        currentHealth = maxHealth;
    }
    public void TakeDamage(int damage) {
        int damageTaken = Mathf.Clamp(damage, 0, currentHealth);
        currentHealth -= damageTaken;

        if(damageTaken != 0) {
            OnTakeDamage?.Invoke(damageTaken);
        }

        if(currentHealth == 0 && damageTaken != 0) {
            OnDeath?.Invoke(transform.position);
        }
    }
}
