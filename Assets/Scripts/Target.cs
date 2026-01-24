using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _minDistanceToTarget;
    [SerializeField] private Waypoint[] _waypoints;

    private Vector3 _currentTargetPosition;
    private Vector3 _currentMovementDirection;

    private int _waypointsArrayMinIndex;
    private int _waypointsArrayMaxIndex;
    private int _currentWaypointIndex;

    public Vector3 Position => _rigidbody.transform.position;

    private void Awake()
    {
        Initialize();
    }

    private void FixedUpdate()
    {
        if (_rigidbody.transform.position.IsCloseEnough(_currentTargetPosition, _minDistanceToTarget))
        {
            SetNextTargetPosition();
        }

        SetMovementDirection();
        MoveInDirection();
    }

    private void Initialize()
    {
        _waypointsArrayMinIndex = 0;
        _waypointsArrayMaxIndex = _waypoints.Length - 1;
        _currentWaypointIndex = _waypointsArrayMinIndex;
        _currentTargetPosition = _waypoints[_currentWaypointIndex].transform.position;

        SetMovementDirection();
    }

    private void MoveInDirection()
    {
        _rigidbody.linearVelocity = _currentMovementDirection.normalized * _movementSpeed;
    }

    private void SetNextTargetPosition()
    {
        _currentWaypointIndex++;

        if (_currentWaypointIndex > _waypointsArrayMaxIndex)
        {
            _currentWaypointIndex = _waypointsArrayMinIndex;
        }

        _currentTargetPosition = GetWaypointPosition(_waypoints[_currentWaypointIndex]);
    }

    private Vector3 GetWaypointPosition(Waypoint waypoint)
    {
        return waypoint.transform.position;
    }

    private void SetMovementDirection()
    {
        _currentMovementDirection = _currentTargetPosition - _rigidbody.transform.position;
    }
}
