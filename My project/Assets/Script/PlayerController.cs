using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
//[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed;
    public float jumpForce;
    [Header("Ground check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundCheckMask;
    [Header("UI")]
    [SerializeField] private int startup;
    [SerializeField] private TextMeshProUGUI hpText;

    private SpriteRenderer _sr;
    private Rigidbody2D _rb;
    //private Animator _animator; 
    private float _currInputX;
    public bool _isGrounded;
    public int _hp;

    public void Jump()
    {
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            //_animator.SetBool("IsJumping", true);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                _isGrounded = true;
                //_animator.SetBool("IsJumping", false);
                //_animator.SetBool("IsFalling", false);
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
        //_animator.SetBool("IsHit", true);
        CheckPointSys checkpointSystem = GetComponent<CheckPointSys>();
        checkpointSystem.Respawn();
    }

    private void Move()
    {
        _currInputX = Input.GetAxis("Horizontal");
        _rb.linearVelocity = new Vector2(_currInputX * speed, _rb.linearVelocity.y);

        //bool isRunning = Mathf.Abs(_currInputX) > 0.1f && _isGrounded;
        //_animator.SetBool("IsRunning", isRunning);

        if (_currInputX != 0)
        {
            _sr.flipX = _currInputX < 0;
        }
    }

    private void Update()
    {
        Move();
        Jump();
        //UpdateAnimationStates();
    }

    //private void UpdateAnimationStates()
    //{
    //    _animator.SetFloat("YVelocity", _rb.linearVelocity.y);

    //    if (!_isGrounded && _rb.linearVelocity.y < -0.1f)
    //    {
    //        _animator.SetBool("IsFalling", true);
    //    }
    //    else if (!_isGrounded && _rb.linearVelocity.y > 0.1f)
    //    {
    //        _animator.SetBool("IsJumping", true);
    //    }

    //    if (_isGrounded && Mathf.Abs(_currInputX) < 0.1f)
    //    {
    //        _animator.SetBool("IsRunning", false);
    //    }
    //}

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        //_animator = GetComponent<Animator>();
    }

}