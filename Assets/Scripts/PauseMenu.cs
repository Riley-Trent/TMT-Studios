using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
public class PauseMenu : MonoBehaviour
{
    public string MainMenu;
    private PlayerInput playerControls;
    private InputAction menu;
    [SerializeField] private bool isPaused;
    [SerializeField] private GameObject pauseMenuUI;

    void Awake()
    {
        playerControls = new PlayerInput();
    }

    private void OnEnable()
    {
        menu = playerControls.Menu.Pause;
        menu.Enable();

        menu.performed += Pause;
    }

    private void OnDisable()
    {
        menu.Disable();

    }

    void Pause(InputAction.CallbackContext context)
    {
        isPaused = !isPaused;
        if(isPaused)
        {
            ActivateMenu();
        }
        else
        {
            DeactivateMenu();
        }
    }
    void ActivateMenu()
    {
        Time.timeScale = 0;
        AudioListener.pause = true;
        pauseMenuUI.SetActive(true);
    }
    public void DeactivateMenu()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        pauseMenuUI.SetActive(false);
        isPaused = false;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        SceneManager.LoadScene(MainMenu);
    }
 
}
