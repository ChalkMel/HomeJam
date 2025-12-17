using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private Button returnButton;
    [SerializeField] private Button mainMenuButton;

    private bool _isPaused;
    void Start()
    {
        returnButton.onClick.AddListener(Play);
        mainMenuButton.onClick.AddListener(MainMenu);
    }


    public void Play()
    {
        ExitMenu();
    }
    void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !_isPaused)
        {
            OpenMenu();
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && _isPaused)
        {
            ExitMenu();
        }

    }
    private void OpenMenu()
    {
        _isPaused = true;
        menu.SetActive(true);
        Time.timeScale = 0.0f;
    }
    private void ExitMenu() 
    {
        _isPaused = false;
        menu.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
