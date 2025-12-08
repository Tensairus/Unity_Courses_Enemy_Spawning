using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private Rigidbody _rigidbody;

    private Vector3 _movementDirection;

    public event Action<Enemy> CollisionOccured;

    private void FixedUpdate()
    {
        _rigidbody.linearVelocity = _movementDirection.normalized * _movementSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent<Floor>(out _) == false)
        {
            CollisionOccured?.Invoke(this);
        }
    }

    public void Initialize(Vector3 position, Vector3 target)
    {
        transform.position = position;
        transform.LookAt(target);
        _movementDirection = (target - transform.position);        
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }
}
