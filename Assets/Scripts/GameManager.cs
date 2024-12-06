using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputReader input;
    [SerializeField] private GameObject pauseMenu;


    private void Start(){
        input.PauseEvent += HandlePause;
        input.ResumeEvent += HandleResume;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
}
