using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputReader input;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private Tiger tiger;
    [SerializeField] private GameObject hubPortal;


    private void Start(){
        input.PauseEvent += HandlePause;
        input.ResumeEvent += HandleResume;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if(tiger.isDead)
        {
            playerWin();
        }
    }

    private void HandlePause(){
        pauseMenu.SetActive(true);
        CursorOn();
    }

    private void HandleResume(){
        pauseMenu.SetActive(false);
        CursorOff();
    }

    private void CursorOff(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void CursorOn(){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void gameOver()
    {
        gameOverUI.SetActive(true);
        CursorOn();
    }
    
    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void mainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void hubWorld()
    {
        SceneManager.LoadScene("HubWorld");
    }

    public void playerWin()
    {
            hubPortal.SetActive(true);
    }
}
