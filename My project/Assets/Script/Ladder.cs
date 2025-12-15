using UnityEngine;

public class Ladder : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float climbSpeed = 3f;

    private bool _isPlayerInside = false;
    private GameObject _player;
    private Rigidbody2D _plRb;
    private PlayerController player;
    private bool _isClimbing = false;

    private void Update()
    {
        if (_isPlayerInside)
        {
            HandleClimbing();
        }
    }

    private void HandleClimbing()
    {
        float verticalInput = Input.GetAxis("Vertical");

        if (Mathf.Abs(verticalInput) > 0.1f)
        {
            StartClimbing();
            _plRb.linearVelocity = new Vector2(_plRb.linearVelocity.x, verticalInput * climbSpeed);
            _isClimbing = true;
        }
        else if (_isClimbing)
            _plRb.linearVelocity = new Vector2(_plRb.linearVelocity.x, 0);
    }

    private void StartClimbing()
    {
        if (!_isClimbing)
        {
            _plRb.gravityScale = 0;
            _isClimbing = true;
        }
    }

    private void StopClimbing()
    {
        if (_isClimbing)
        {
            _plRb.gravityScale = 1;
            _isClimbing = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isPlayerInside = true;
            _player = collision.gameObject;
            _plRb = _player.GetComponent<Rigidbody2D>();
            player = _player.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isPlayerInside = false;
            StopClimbing();
        }
    }
}
