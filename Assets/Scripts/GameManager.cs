using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    [Header("General Data")]
    public GameObject Joystick;
    public Transform MainCamera;
    public GameObject Roof;
    public GameObject VCam;
    public AudioSource BGMSoundSource;
    public AudioSource MainSoundsSource;
    public AudioClip ButtonSound;
    public Text LevelNumberText;
    public Slider SpeedSlider;
    public int LevelNumber;
    public int StarsAchieved;
    public GameObject[] Tags;
    public GameObject DragIcon;


    [Header("Enemies Data")]
    public int TotalEnemies;
    public int EnemiesKilled;



    [Header("Player Data")]
    [Tooltip("Runtime Player Reference")]
    public PlayerController player;

    [Header("Tutorial Panel Data")]
    public GameObject TutorialPanel;
    public GameObject TapToContinue;
    public GameObject TutorialCloseButton;
    public GameObject message1;
    public Animator TutorialAnim;
    public AudioClip TutorialpanelSound;
    [SerializeField] Text TextBody;
    [SerializeField] string text1;
    [SerializeField] string text2;
    [SerializeField] string text3;
    [SerializeField] Image CourageImage;
    [SerializeField] Sprite Courage1;
    [SerializeField] Sprite Courage2;
    [SerializeField] Sprite Courage3;
    bool isPlayable;
    int TouchNum;
    [Header("Level Complete Data")]
    public GameObject LevelCompletePanel;
    public AudioSource LevelCompleteSource;
    public AudioClip StarsSound;
    public Transform StarsParent;
    bool isLevelComplete;

    [Header("Level Fail Data")]
    public GameObject LevelFailPanel;
    public AudioSource LevelFailSource;
    bool isFail;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<PlayerController>();
        BGMSoundSource = GameObject.FindGameObjectWithTag("BGM").GetComponent<AudioSource>();
        LevelNumberText.text = LevelNumber.ToString();

        Roof = GameObject.FindGameObjectWithTag("Roof");

        StartCoroutine(TagsAnim());

        if (PlayerPrefs.GetInt("Hat") != 0)
        {
            Tags[1].SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {


        if (!isPlayable)
        {
            TouchNum = Input.touchCount;
        }

        if (isPlayable)
        {
            if (Input.touchCount>0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    DragIcon.SetActive(false);
                }
            }
        }
    }

    public void StartLevel()
    {
        player.StartPlaying();
        Roof.SetActive(false);
        StartCoroutine(CamAnim());
    }
    public void PauseLevel()
    {
        Time.timeScale = 0;
    }
    public void ResumeLevel()
    {
        Time.timeScale = 1;
    }
    public void PurchaseHat(int num)
    {
        PlayerPrefs.SetInt("Hat", num);
    }
    public void UpdateEnemyStatus()
    {
        EnemiesKilled++;
        StarsAchieved++;

        if (EnemiesKilled == TotalEnemies && !isLevelComplete && !isFail)
        {
            LevelCompleted();
        }
    }
    public void LevelCompleted()
    {
        isLevelComplete = true;
        Joystick.SetActive(false);
        BGMSoundSource.Stop();
        LevelCompleteSource.PlayDelayed(2f);
        LevelCompletePanel.SetActive(true);
        PlayerPrefs.SetInt("LevelNumber", PlayerPrefs.GetInt("LevelNumber") + 1);

        StartCoroutine(StarsAward(1));
    }

    public void LevelFailed()
    {
        isFail = true;
        BGMSoundSource.Stop();
        LevelFailSource.PlayDelayed(2f);
        LevelFailPanel.SetActive(true);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void NextLevel()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("LevelNumber") + 1);
    }

    public void TutorialDisplay()
    {
        MainSoundsSource.PlayOneShot(TutorialpanelSound);
        TapToContinue.SetActive(false);
        TutorialCloseButton.SetActive(false);
        Invoke("TurnCloseButton",(Time.timeScale*2)/1);
        if (LevelNumber == 1 || LevelNumber == 2)
        {
            if (LevelNumber == 2)
            {
                TextBody.text = text3;
                CourageImage.sprite = Courage3;
            }
            DragIcon.SetActive(false);
            TutorialPanel.SetActive(true);
            message1.SetActive(true);
        }
    }
    public void CLoseTutorial()
    {
        Time.timeScale = 1;
        MainSoundsSource.PlayOneShot(TutorialpanelSound);
        TutorialAnim.SetTrigger("CloseTutorial");
        Invoke("TurnOffTutorial", 0.5f);

    }
    public void TurnOffTutorial()
    {
        TextBody.text = text2;
        CourageImage.sprite = Courage2;
        TutorialPanel.SetActive(false);
        DragIcon.SetActive(true);
    }
    void TurnCloseButton() 
    {
        print("Here");
        TapToContinue.SetActive(true);
        TutorialCloseButton.SetActive(true);
    }

    IEnumerator CamAnim()
    {
        Joystick.SetActive(false);
        while (MainCamera.position.x > VCam.transform.position.x + 0.01f)
        {
            MainCamera.position = Vector3.Lerp(MainCamera.position, VCam.transform.position, 2 * Time.deltaTime);
            MainCamera.LookAt(player.transform.position);

            yield return new WaitForSeconds(0);
        }
        Joystick.SetActive(true);
        VCam.SetActive(true);
        isPlayable = true;
        DragIcon.SetActive(true);


        if (LevelNumber == 1 || LevelNumber == 2)
        {
            TutorialDisplay();
        }

    }

    IEnumerator StarsAward(int num)
    {
        yield return new WaitForSeconds(num);

        Transform[] starsHolders = new Transform[3];
        for (int i = 0; i < starsHolders.Length; i++)
        {
            starsHolders[i] = StarsParent.GetChild(i);
        }

        float StarSize = 0.98227f;

        for (int i = 0; i < 3; i++)
        {
            starsHolders[i].transform.GetChild(0).transform.localScale = Vector3.zero;

            MainSoundsSource.PlayOneShot(StarsSound);

            while (starsHolders[i].transform.GetChild(0).transform.localScale.x < StarSize)
            {
                starsHolders[i].transform.GetChild(0).transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
                yield return new WaitForSeconds(0);
            }

            yield return new WaitForSeconds(0.3f);
        }
    }
    IEnumerator TagsAnim()
    {
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                Tags[j].transform.Translate(0, 2 * Time.timeScale, 0);
            }
            yield return new WaitForSeconds(0);
        }
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                Tags[j].transform.Translate(0, -2 * Time.timeScale, 0);
            }
            yield return new WaitForSeconds(0);
        }
        StartCoroutine(TagsAnim());

    }

    public void StartSpeedBar() 
    {
        StartCoroutine(SpeedBar());
    }
    IEnumerator SpeedBar() 
    {
        SpeedSlider.gameObject.SetActive(true);
        for (int i = 0; i < 200; i++)
        {
            SpeedSlider.value -= 0.5f;
            yield return new WaitForSeconds(0);
        }
        SpeedSlider.gameObject.SetActive(false);
        player.PlayerSpeed = player.PermenantSpeed;
        player.HighSpeed = false;
    }

    public void ButtonClick() 
    {
        MainSoundsSource.PlayOneShot(ButtonSound);
    }
}
