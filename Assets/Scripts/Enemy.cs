using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private Rigidbody _rigidbody;

    private Vector3 movementDirection;

    public event Action<Enemy> ReadyToBePooled;

    private void FixedUpdate()
    {
        _rigidbody.linearVelocity = movementDirection.normalized * _movementSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag != "Floor")
        {
            ReadyToBePooled?.Invoke(this);
        }
    }

    public void Activate(Vector3 position, Vector3 target)
    {
        transform.position = position;
        transform.LookAt(target);
        movementDirection = (target - transform.position);
        gameObject.SetActive(true);
    }
}
