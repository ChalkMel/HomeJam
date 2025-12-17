using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed;
    public float jumpForce;
    public float baseJump;
    public float baseSpeed;
    [Header("Ground check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundCheckMask;

    [SerializeField] private float animationDelay = 0.5f;

    private AudioSource _as;
    private SpriteRenderer _sr;
    private Rigidbody2D _rb;
    private Animator _animator; 
    private float _currInputX;
    public bool _isGrounded;
    public int _hp;

    private bool _isOnLadder = false;
    private bool _isClimbing = false;

    public void SetOnLadder(bool onLadder)
    {
        _isOnLadder = onLadder;
        if (!onLadder)
        {
            _isClimbing = false;
        }
    }

    public void SetClimbing(bool climbing)
    {
        _isClimbing = climbing;
    }

    public void Jump()
    {
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            _animator.SetBool("IsJumping", true);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                _isGrounded = true;
                _animator.SetBool("IsJumping", false);
                break;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _isGrounded = false;
    }

    public void GetDamage()
    {
        _as.Play();
        _animator.Play("Death");
        float animationLength = _animator.GetCurrentAnimatorStateInfo(0).length - animationDelay;
        Invoke(nameof(Respawn), animationLength);
    }

    private void Respawn()
    {
        CheckPointSys checkpointSystem = GetComponent<CheckPointSys>();
        checkpointSystem.Respawn();
    }

    private void Move()
    {
        if (_isClimbing) 
            return;
        _currInputX = Input.GetAxis("Horizontal");
        _rb.linearVelocity = new Vector2(_currInputX * speed, _rb.linearVelocity.y);

        bool isRunning = Mathf.Abs(_currInputX) > 0.1f && _isGrounded;
        _animator.SetBool("IsRunning", isRunning);

        if (_currInputX != 0)
        {
            _sr.flipX = _currInputX < 0;
        }
    }

    private void Update()
    {
        Move();
        Jump();
        UpdateAnimationStates();
    }

    private void UpdateAnimationStates()
    {

         if (!_isGrounded && _rb.linearVelocity.y > 0.1f)
        {
            _animator.SetBool("IsJumping", true);
        }

        if (_isGrounded && Mathf.Abs(_currInputX) < 0.1f)
        {
            _animator.SetBool("IsRunning", false);
        }
    }

    private void Awake()
    {
        baseJump = jumpForce;
        baseSpeed = speed;
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _as = GetComponent<AudioSource>();
    }

}