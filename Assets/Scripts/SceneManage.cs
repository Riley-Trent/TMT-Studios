using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour
{
    [Header("Levels To Load")]
    public string newGameScene;
    public void Level1()
    {
        SceneManager.LoadScene(newGameScene);
    }
}