using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CheckPointSys checkpointSystem = other.GetComponent<CheckPointSys>();
            checkpointSystem.SetCheckpoint(gameObject.transform.position);
        }
    }
}
