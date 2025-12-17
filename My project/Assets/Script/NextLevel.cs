using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    [SerializeField] private Key key;
    [SerializeField] private bool needKey = true;
    [SerializeField] private bool final;
    [SerializeField] private int reduceJumpForce;
    [SerializeField] private Transform teleportPoint;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject fBtn;
    [SerializeField] private GameObject think;
    [SerializeField] private GameObject thinkAbout;
    [SerializeField] Bush bush;
    private AudioSource audioSource;
    private bool _playerInTrigger;
    private PlayerController _playerController;

    private void Update()
    {
        if (!_playerInTrigger) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            TryTeleportPlayer();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        _playerInTrigger = true;
        _playerController = collision.GetComponent<PlayerController>();
        UpdateUI();
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        UpdateUI();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        _playerInTrigger = false;
        _playerController = null;
        HideAllUI();
    }

    private void TryTeleportPlayer()
    {
        if (needKey && (!key || !key._isPicked))
        {
            return;
        }

        TeleportPlayer();
    }

    private void TeleportPlayer()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        if (!final)
        {
            _playerController.baseJump -= reduceJumpForce;
            _playerController.jumpForce = _playerController.baseJump;
            player.transform.position = teleportPoint.position;
            HideAllUI();
        }
        else
            SceneManager.LoadScene("Major Ursu");
    }

    private void UpdateUI()
    {
        if (needKey)
        {
            if (!key._isPicked)
            {
                ShowThoughts();
                fBtn.SetActive(false);
            }
            else
            {
                HideThoughts();
                fBtn.SetActive(true);
            }
        }
        else
        {
            HideThoughts();
            fBtn.SetActive(true);
        }
    }

    private void ShowThoughts()
    {
        if (think) think.SetActive(true);
        if (thinkAbout) thinkAbout.SetActive(true);
    }

    private void HideThoughts()
    {
        if (think) think.SetActive(false);
        if (thinkAbout) thinkAbout.SetActive(false);
    }

    private void HideAllUI()
    {
        HideThoughts();
        if (fBtn) fBtn.SetActive(false);
    }
}