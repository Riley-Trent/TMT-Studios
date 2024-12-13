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
    [SerializeField] private GameObject optionsMenuUI;
    [SerializeField] public FPSController fPSController;

    void Awake()
    {
        playerControls = new PlayerInput();
    }

    private void OnEnable()
    {
        menu = playerControls.OnFoot.Pause;
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        AudioListener.pause = true;
        pauseMenuUI.SetActive(true);
        fPSController.lockCamera(true);
    }
    public void DeactivateMenu()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        AudioListener.pause = false;
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        isPaused = false;
        fPSController.lockCamera(false);
        
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        Debug.Log("Quitting game...");
        SceneManager.LoadScene(MainMenu);
    }
    public void ResumeTime(){
        Time.timeScale = 1;
    }
 
}
