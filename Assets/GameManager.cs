using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Enemies Data")]
    public int TotalEnemies;
    public int EnemiesKilled;

    bool isLevelComplete;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateEnemyStatus()
    {
        EnemiesKilled++;

        if (EnemiesKilled == TotalEnemies && !isLevelComplete)
        {
            LevelCompleted();
        }
    }
    public void LevelCompleted() 
    {
        print("LEVEL COMPLETED");
    }

    public void LevelFailed()
    {
        print("LEVEL FAILED");
    }
}
