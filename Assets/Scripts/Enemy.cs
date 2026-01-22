using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private Rigidbody _rigidbody;

    private Target _currentTarget;
    
    private Vector3 _currentTargetPosition;
    private Vector3 _currentMovementDirection;

    public event Action<Enemy> CollisionOccured;

    private void FixedUpdate()
    {
        PrepareToMove();
        MoveInDirection();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent<Floor>(out _) == false)
        {
            CollisionOccured?.Invoke(this);
        }
    }

    public void Initialize(Vector3 position, Target currentTarget)
    {
        transform.position = position;
        _currentTarget = currentTarget;

        PrepareToMove();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    private void SetMovementDirection()
    {
        _currentMovementDirection = _currentTargetPosition - _rigidbody.transform.position;
    }

    private void MoveInDirection()
    {
        _rigidbody.linearVelocity = _currentMovementDirection.normalized * _movementSpeed;
    }

    private void GetCurrentTargetPosition()
    {
        _currentTargetPosition = _currentTarget.Position;
    }

    private void LookAtCurrentTarget()
    {
        transform.LookAt(_currentTargetPosition);
    }

    private void PrepareToMove()
    {
        GetCurrentTargetPosition();
        SetMovementDirection();
        LookAtCurrentTarget();
    }
}
