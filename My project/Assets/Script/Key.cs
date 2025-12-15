using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] public bool isPicked;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPicked = true;
            gameObject.SetActive(false);
        }
    }
}
