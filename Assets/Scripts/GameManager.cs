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
    }

    private void HandlePause(){
        pauseMenu.SetActive(true);
    }

    private void HandleResume(){
        pauseMenu.SetActive(false);
    }
}
