using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class Bush : MonoBehaviour
{
    [SerializeField] private float addForce = 1.5f;
    [SerializeField] private float addSpeed = 1.5f;
    [SerializeField] private float boostDuration = 3f;
    [SerializeField] private float respawnDuration = 10f;
    [SerializeField] private GameObject lig;
    [SerializeField] private Sprite eaten;
    [SerializeField] private GameObject fBtn;

    private float originalJumpForce;
    private float originalSpeed;
    private AudioSource audioSource;
    private Sprite _startSprite;
    public bool _isInteractable = true;
    private SpriteRenderer _rend;
    private bool _isInTrigger;
    private PlayerController player;
    private SpriteRenderer _playerRend;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        _rend = GetComponent<SpriteRenderer>();
        _startSprite = _rend.sprite;
    }

    private void Update()
    {
        if (!_isInTrigger || !_isInteractable) return;
        if (Input.GetKeyDown(KeyCode.F))
        {
            Eat();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        _isInTrigger = true;
        player = collision.GetComponent<PlayerController>();
        _playerRend = collision.GetComponent<SpriteRenderer>();

        if (_isInteractable && fBtn)
            fBtn.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        _isInTrigger = false;
        if (fBtn) 
            fBtn.SetActive(false);
    }

    private void Eat()
    {
        if (!_isInteractable) 
            return;

        StartCoroutine(ApplyBoostCoroutine());
    }

    public IEnumerator ApplyBoostCoroutine()
    {
        _isInteractable = false;

        audioSource.Play();
        if (_rend) _rend.sprite = eaten;
        if (lig) lig.SetActive(false);
        if (fBtn) fBtn.SetActive(false);

        originalJumpForce = player.jumpForce;
        originalSpeed = player.speed;
        player.jumpForce += addForce;
        player.speed *= addSpeed;
        _playerRend.color = new Color(1, 1, 1, 0.5f);

        yield return new WaitForSeconds(boostDuration);
        Stop();
        _playerRend.color = Color.white;

        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDuration);

        _isInteractable = true;

        _rend.sprite = _startSprite;
        lig.SetActive(true);
    }
    public void Stop()
    {
        player.jumpForce = player.baseJump;
        player.speed = player.baseSpeed;
    }
}