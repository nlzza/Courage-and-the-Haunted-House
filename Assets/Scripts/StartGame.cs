using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("LevelNumber")+1);
    }
}
