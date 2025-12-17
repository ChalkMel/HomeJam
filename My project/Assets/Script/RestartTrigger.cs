using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CheckPointSys checkpointSystem = other.GetComponent<CheckPointSys>();
            checkpointSystem.Respawn();
        }
    }
}
