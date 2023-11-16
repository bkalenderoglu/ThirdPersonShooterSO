using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyHealth health;

    private void Start() {
        health.OnDeath += Die;
    }

    private void Die(Vector3 position) {
        Destroy(gameObject);
    }
}
