using TMPro;
using UnityEngine;

public class NextLevel : MonoBehaviour
{
    [SerializeField] private Key key;

    [SerializeField] private int reduceJumpForce;

    [SerializeField] private string failMessage;
    [SerializeField] private GameObject dialogueScreen;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [SerializeField] private Transform teleportPoint;
    [SerializeField] private GameObject player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (key.isPicked)
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
            if(key.isPicked == false)
            {
                dialogueScreen.SetActive(true);
                dialogueText.text = failMessage;
            }      
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            dialogueScreen.SetActive(false);
    }
}
