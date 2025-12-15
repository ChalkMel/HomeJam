using UnityEngine;

public class Bush : MonoBehaviour
{
    [SerializeField] private float addForce = 1.5f;
    [SerializeField] private float boostDuration = 3f;
    [SerializeField] private float respawnDuration = 10f;
    [SerializeField] private ParticleSystem boostParticles;

    private bool _isInteractable;
    private SpriteRenderer _rend;

    private void Awake()
    {
        _isInteractable = true;
        _rend = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (_isInteractable)
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                StartCoroutine(ApplyBoost(player));
                boostParticles.Play();
            }
            else return;
        }
    }

    private System.Collections.IEnumerator ApplyBoost(PlayerController player)
    {
        _isInteractable = false;
        _rend.color = Color.red;
        float originalJumpForce = player.jumpForce;
        player.jumpForce += addForce;
        yield return new WaitForSeconds(boostDuration);
        player.jumpForce = originalJumpForce;
        boostParticles.Stop();
        StartCoroutine(Respawn());
        StopCoroutine(ApplyBoost(player));
        Debug.Log("Start Respawn");
    }
    private System.Collections.IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDuration);
        _isInteractable = true;
        _rend.color = Color.green;
        StopCoroutine(Respawn());
        Debug.Log("End Respawn");
    }
}
