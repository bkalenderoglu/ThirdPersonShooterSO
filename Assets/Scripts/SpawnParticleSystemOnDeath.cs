using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(IDamageable))]
public class SpawnParticleSystemOnDeath : MonoBehaviour
{
    [SerializeField] private ParticleSystem deathSystem;
    public IDamageable damageable;

    private void Awake() {
        damageable = GetComponent<IDamageable>();
    }

    private void OnEnable() {
        damageable.OnDeath += damageable_OnDeath;
    }

    private void damageable_OnDeath(Vector3 position) {
        Instantiate(deathSystem, position, Quaternion.identity);
        gameObject.SetActive(false);
        ScoreCounter.Instance.AddScore(10);
    }
}
