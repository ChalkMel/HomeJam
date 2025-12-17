using UnityEngine;

public class Key : MonoBehaviour
{
    
    [SerializeField] private float animationDelay = 0.5f;
    [SerializeField] private AudioSource audioSource;

    public bool _isPicked;
    private Animator animator;
    private Collider2D col;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _isPicked = true;
            audioSource.Play();
            col.enabled = false;
            animator.Play("exit");
            Invoke(nameof(DisableKey), animationDelay);
        } 
    }
    private void DisableKey()
    {
        gameObject.SetActive(false);
    }
}

