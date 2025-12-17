using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Bush : MonoBehaviour
{
    [SerializeField] private float addForce = 1.5f;
    [SerializeField] private float boostDuration = 3f;
    [SerializeField] private float respawnDuration = 10f;
    [SerializeField] private GameObject lig;
    [SerializeField] private Sprite eaten;

    private Sprite start;
    private bool _isInteractable;
    private SpriteRenderer _rend;

    private void Awake()
    {
        _isInteractable = true;
        _rend = GetComponent<SpriteRenderer>();
        start = _rend.sprite;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (_isInteractable)
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                SpriteRenderer rendPl = collision.GetComponent<SpriteRenderer>();
                StartCoroutine(ApplyBoost(player, rendPl));
                
                rendPl.color = new Color(1,1,1, 0.5f);
            }
            else return;
        }
    }

    private System.Collections.IEnumerator ApplyBoost(PlayerController player, SpriteRenderer rendPl)
    {
        _isInteractable = false;
        _rend.sprite = eaten;
        lig.SetActive(false);
        float originalJumpForce = player.jumpForce;
        player.jumpForce += addForce;
        yield return new WaitForSeconds(boostDuration);
        player.jumpForce = originalJumpForce;
        rendPl.color = new Color(1, 1, 1, 1f);
        StartCoroutine(Respawn());
        StopCoroutine(ApplyBoost(player, rendPl));
        Debug.Log("Start Respawn");
    }
    private System.Collections.IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDuration);
        _isInteractable = true;
        lig.SetActive(true);
        _rend.sprite = start;
        StopCoroutine(Respawn());
        Debug.Log("End Respawn");
    }
}
