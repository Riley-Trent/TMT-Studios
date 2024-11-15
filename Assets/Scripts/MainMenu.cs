using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string newGameScene;
    public string optionsScene;
    public void NewGame()
    {
        SceneManager.LoadScene(newGameScene);
    }

    public void LoadGame()
    {

    }

    public void Options()
    {
        SceneManager.LoadScene(optionsScene);
    }
    public void Shop()
    {
      
    }
    public void Achievements()
    {
      
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
