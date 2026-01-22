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

    public Vector3 Position => _rigidbody.transform.position;

    private void Awake()
    {
        Initialize();
    }

    private void FixedUpdate()
    {
        if (GetDistanceToTarget() <= _minDistanceToTarget)
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
        _currentTargetPosition = _waypoints[0].transform.position;

        SetMovementDirection();
    }

    private void MoveInDirection()
    {
        _rigidbody.linearVelocity = _currentMovementDirection.normalized * _movementSpeed;
    }

    private void SetNextTargetPosition()
    {
        for (int i = _waypointsArrayMinIndex; i < _waypoints.Length; i++)
        {
            int waypointsArrayNextIndex = i + 1;

            if (GetWaypointPosition(_waypoints[i]) == _currentTargetPosition)
            {
                if (i == _waypointsArrayMaxIndex)
                {
                    _currentTargetPosition = GetWaypointPosition(_waypoints[_waypointsArrayMinIndex]);
                }
                else
                {
                    _currentTargetPosition = GetWaypointPosition(_waypoints[waypointsArrayNextIndex]);
                }

                break;
            }
        }
    }

    private float GetDistanceToTarget()
    {
        return Vector3.Distance(_rigidbody.transform.position, _currentTargetPosition);
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
