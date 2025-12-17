using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform[] _points;
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _arrivalThreshold = 0.01f;
    [SerializeField] private bool _cyclic = true;

    private int _currentPoint = 0;
    private Transform _currentTarget;

    private void Start()
    {
        _currentTarget = _points[_currentPoint];
    }

    private void Update()
    {
        MovePlatform();
        CheckArrival();
    }

    private void MovePlatform()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            _currentTarget.position,
            _speed * Time.deltaTime
        );
    }

    private void CheckArrival()
    {
        float distanceToTarget = Vector3.Distance(transform.position, _currentTarget.position);

        if (distanceToTarget < _arrivalThreshold)
        {
            UpdateTarget();
        }
    }

    private void UpdateTarget()
    {
        _currentPoint++;

        if (_currentPoint >= _points.Length)
        {
            if (_cyclic)
            {
                _currentPoint = 0;
            }
            else
            {
                enabled = false;
                return;
            }
        }

        _currentTarget = _points[_currentPoint];
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
