using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform[] points;
    [SerializeField] private float speed = 3;

    private int _currPoint = 0;

    void Update()
    {

        Transform target = points[_currPoint];

        Vector3 newPosition = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        transform.position = newPosition;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget < 0.01f)
        {
            _currPoint++;

            if (_currPoint >= points.Length)
                _currPoint = 0;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            collision.gameObject.transform.parent = transform;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            collision.transform.parent = null;
    }
}