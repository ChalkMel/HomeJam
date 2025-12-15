using UnityEngine;

public class Bush : MonoBehaviour
{
    [SerializeField] private float addForce = 1.5f;
    [SerializeField] private float boostDuration = 3f;
    [SerializeField] private float respawnDuration = 10f;
    [SerializeField] private ParticleSystem boostParticles;

    private bool isInteractable;
    private SpriteRenderer rend;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            StartCoroutine(ApplyBoost(player));
            boostParticles.Play();
        }
    }

    private System.Collections.IEnumerator ApplyBoost(PlayerController player)
    {
        isInteractable = false;
        rend.color = Color.red;
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
        isInteractable = true;
        rend.color = Color.green;
        StopCoroutine(Respawn());
        Debug.Log("End Respawn");
    }
}
