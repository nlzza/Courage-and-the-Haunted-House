using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewards : MonoBehaviour
{
 
    string time = System.DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy");
    int date;

    public GameObject DailyRewardsPanel;
    public Transform BrightLightBG;
    public GameObject ChestAvailable;
    public GameObject ChestUnAvailable;
    public GameObject DailyTag;

    private void Start()
    {
      
    }
    private void Update()
    {
        BrightLightBG.Rotate(0,0,0.51f);

        string str = time.Substring(0, 2);
        date = int.Parse(str);

        if (PlayerPrefs.GetInt("RewardDate") == date)
        {
            DailyTag.SetActive(false);
        }

    }
    public void RewardDeclared()
    {
        string str = time.Substring(0, 2);
        date = int.Parse(str);

        if (PlayerPrefs.GetInt("RewardDate") == date)
        {
            ChestUnAvailable.SetActive(true);
            ChestAvailable.SetActive(false);
            //Reward UnAvailable
        }
        else 
        {
            ChestAvailable.SetActive(true);
            ChestUnAvailable.SetActive(false);
            //Reward Available
        }    
    }

    public void RewardRecieved() 
    {
        PlayerPrefs.SetInt("RewardDate", date);

        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 100);

    }
}
