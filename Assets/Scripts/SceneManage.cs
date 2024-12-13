using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour
{
    [Header("Levels To Load")]
    public string LevelOne;
    public string LevelTwo;
    public void Level1()
    {
        SceneManager.LoadScene(LevelOne);
    }
    public void Level2()
    {
        SceneManager.LoadScene(LevelTwo);
    }
}
