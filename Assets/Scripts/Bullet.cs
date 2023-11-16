using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    private Rigidbody rigidbody;
    [field: SerializeField] public Vector3 spawnLocation {  get; set; }
    [SerializeField] private float delayedDisableTime = 2f;

    public delegate void CollisionEvent(Bullet bullet, Collision collision);
    public event CollisionEvent OnCollision;

    private void Awake() {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Spawn(Vector3 spawnForce) {
        spawnLocation = transform.position;
        transform.forward = spawnForce.normalized;
        rigidbody.AddForce(spawnForce);
        StartCoroutine(DelayedDisable(delayedDisableTime));
    }

    private IEnumerator DelayedDisable(float time) {
        yield return new WaitForSeconds(time);
        OnCollisionEnter(null);
    }

    private void OnCollisionEnter(Collision collision) {
        OnCollision?.Invoke(this, collision);
    }

    private void OnDisable() {
        StopAllCoroutines();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        OnCollision = null;
    }
}
