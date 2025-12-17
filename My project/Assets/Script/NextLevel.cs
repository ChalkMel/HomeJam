using TMPro;
using UnityEngine;

public class NextLevel : MonoBehaviour
{
    [SerializeField] private Key key;

    [SerializeField] private int reduceJumpForce;

    [SerializeField] private Transform teleportPoint;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject think;
    [SerializeField] private GameObject thinkAbout;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (key._isPicked)
            {
                player.transform.position = teleportPoint.position;
                PlayerController playerC = collision.gameObject.GetComponent<PlayerController>();
                playerC.jumpForce -= reduceJumpForce;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(key._isPicked == false)
            {
                think.SetActive(true);
                thinkAbout.SetActive(true);
            }      
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            think.SetActive(false);
            thinkAbout.SetActive(false);
        }
    }
}
