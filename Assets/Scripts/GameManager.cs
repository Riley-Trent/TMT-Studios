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
    [SerializeField] private GameObject bossHealthBar;

    public static GameManager instance;
    void Awake(){
        if(instance == null){

            instance = this;

            //DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
        //tiger = FindObjectOfType<Tiger>();
    }
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
            bossHealthBar.SetActive(false);
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

    public void CursorOff(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void CursorOn(){
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
