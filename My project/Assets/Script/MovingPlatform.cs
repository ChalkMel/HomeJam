using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform[] points;
    [SerializeField] private float speed = 3;
    [SerializeField] private bool speedy = true;
    private int _currPoint = 0;
    private float pSpeed;
    private PlayerController player;

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
        {
            collision.gameObject.transform.parent = transform;
            player = collision.gameObject.GetComponent<PlayerController>();
            pSpeed = player.speed;
            //if(speedy)
            //    player.speed = 2;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = null;
            player.speed = pSpeed;
        }
    }
}