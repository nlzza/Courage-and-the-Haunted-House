using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController: MonoBehaviour
{

    [Header("Player References")]
    [SerializeField] Joystick Joystick;
    [SerializeField] Transform PlayerCharacter;
    [SerializeField] AudioSource footStepsSound;
    [SerializeField] AudioClip DeathSound;
    [SerializeField] AudioClip BonusSound;
    public float PlayerSpeed;
    public float PermenantSpeed;
    [SerializeField] Transform[] TargetsByOrder;
    [SerializeField] GameObject BonusParticles;
    public bool isDead;
    bool invincible;
    public bool HighSpeed;

    [Header("Player Health References")]
    [SerializeField] int PlayerHealth;
    [SerializeField] GameObject HealthCanvas;
    [SerializeField] Slider HealthSlider;

    [Header("Player Bonus References")]
    public Image BonusTag;
    public Sprite FullHealthTag;
    public Sprite NormalHealthTag;
    public Sprite SpeedTag;

    [Header("Direction Arrow References")]
    [SerializeField] Transform DirectionArrow;
    [SerializeField] Renderer ArrowRenderer;

    [Header("Player Outfits References")]
    public GameObject[] Hats;

    [Header("RUNTIME references")]

    GameManager gameManager;

    Vector2 playerDirection;
    Animator playerAnimator;
    Rigidbody PlayerRigidbody;
    Transform Target;
    AudioSource audioSource;
    float distanceFromCurrentTarget;

    CharacterController _characterController;


    //float H, S, V;
    // Start is called before the first frame update
    void Start()
    {
        PermenantSpeed = PlayerSpeed;
        Target = TargetsByOrder[0];
        playerAnimator = PlayerCharacter.gameObject.GetComponent<Animator>();
        PlayerRigidbody = GetComponent<Rigidbody>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        audioSource = GetComponent<AudioSource>();

        DirectionArrow.gameObject.SetActive(false);
        HealthCanvas.SetActive(false);

        BonusTag.gameObject.SetActive(false);

        _characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        #region PLAYER_CONTROLLER
        //Player movement controls system
        if (!isDead)
        {
            float xMov = Joystick.Horizontal;
            float zMov = Joystick.Vertical;

            float xRot = xMov;
            float zRot = zMov;
            //transform.Translate( xMov * Time.deltaTime * PlayerSpeed , 0 , zMov * Time.deltaTime * PlayerSpeed );

            var vectorsample = new Vector3(xMov * Time.deltaTime  * PlayerSpeed, 0, zMov * Time.deltaTime * PlayerSpeed);
            //PlayerRigidbody.AddForce(vectorsample);
            _characterController.Move(vectorsample);
            //PlayerRigidbody.velocity = vectorsample;

            //Player rotation control system
            if (zMov != 0 && xMov != 0)
            {
                playerDirection.x = zRot;
                playerDirection.y = xRot;

            }
            Vector2 direction = playerDirection - Vector2.zero;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            PlayerCharacter.rotation = Quaternion.Euler(0, angle, 0);

            //Player Animation Control System
            if (zMov != 0 || xMov != 0)
            {
                playerAnimator.SetBool("isRunning", true);
                playerAnimator.SetBool("isIdle", false);
                footStepsSound.enabled = true;
            }
            else
            {
                playerAnimator.SetBool("isRunning", false);
                playerAnimator.SetBool("isIdle", true);
                footStepsSound.enabled = false;
            }
        }
        #endregion

        HealthSlider.value = PlayerHealth;
        DirectionArrow.LookAt(Target);
        ArrowColorChange();
        HatsCheck();
    }

    public void HatsCheck() 
    {
        for (int i = 0; i < Hats.Length; i++)
        {
            if (i == PlayerPrefs.GetInt("Hat"))
            {
                Hats[i].SetActive(true);
            }
            else
            {
                Hats[i].SetActive(false);
            }
        }
    }

    public void StartPlaying() 
    {

        DirectionArrow.gameObject.SetActive(true);
        HealthCanvas.SetActive(true);
        BonusTag.gameObject.SetActive(true);

        StartCoroutine(UpdateHealthCanvas(false, 2));
    }

    public void PlayScreamSound() 
    {
        audioSource.Play();
    }

    void ArrowColorChange() 
    {
        if (Target == null)
        {
            DirectionArrow.gameObject.SetActive(false);
            return;
        }
        else 
        {
            float dist = (transform.position - Target.position).magnitude;

           // H = dist / 150; S = 1; V = 1;
        }

       // ArrowRenderer.material.color = Color.HSVToRGB(H,S,V);        
    }

    public void UpdateTarget(string str)
    {
        for (int i = 0; i < TargetsByOrder.Length-1; i++)
        {
            if (Target == TargetsByOrder[i])
            {
                if (str == "")
                {
                    Target = TargetsByOrder[i + 2];
                }
                else if (str == "NextEnemy")
                {
                    Target = TargetsByOrder[i + 1];

                    Target.gameObject.SetActive(true);
                    TargetsByOrder[i + 2].gameObject.SetActive(true);
                    TargetsByOrder[i + 3].gameObject.SetActive(true);
                }
                break;
            }
        }
    }
   

    public void updateHealth(int num, bool TagCheck) 
    {
        StartPlaying();
        BonusTag.gameObject.SetActive(TagCheck);
        PlayerHealth += num;

        if (PlayerHealth <= 0 && !isDead)
        {
            isDead = true;

            audioSource.PlayOneShot(DeathSound);
            footStepsSound.enabled = false;
            PlayerRigidbody.velocity = Vector3.zero;
            ArrowRenderer.gameObject.SetActive(false);
            playerAnimator.Play("Death");
            gameManager.LevelFailed();
        }
    }

    public void UpdateSpeed(float num) 
    {
        if (!HighSpeed)
        {
            HighSpeed = true;
            PlayerSpeed += num;
            gameManager.StartSpeedBar();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Weapon":

                updateHealth(-10,false);
                break;

            case "Trap":

                updateHealth(-100, false);
                break;

            case "Laser":

                updateHealth(-100, false);
                break;

            case "Heart Bonus":
                BonusTagAnimation("FullHealth");
                audioSource.PlayOneShot(BonusSound);
                updateHealth(PlayerHealth + (100 - PlayerHealth), true);
                Destroy(other.gameObject);
                Quaternion rot1 = Quaternion.Euler(90, 0, 0);
                var bp1 = Instantiate(BonusParticles, transform.position, rot1);
                Destroy(bp1, 1f);
                break;

            case "Health Bonus":
                BonusTagAnimation("NormalHealth");
                audioSource.PlayOneShot(BonusSound);
               
                if (PlayerHealth < 80)
                {
                    updateHealth(20, true);
                }
                else
                {
                    updateHealth(0 , true);
                }

                Destroy(other.gameObject);
                Quaternion rot2 = Quaternion.Euler(90, 0, 0);
                var bp2 = Instantiate(BonusParticles,transform.position,rot2);
                Destroy(bp2,1f);
                break;

            case "Speed Bonus":
                BonusTagAnimation("Speed");
                StartPlaying();
                audioSource.PlayOneShot(BonusSound);

                UpdateSpeed(PlayerSpeed/2);
                
                Destroy(other.gameObject);
                Quaternion rot3 = Quaternion.Euler(90,0,0);
                var bp3 = Instantiate(BonusParticles, transform.position += new Vector3(0,1,0), rot3) ;
                Destroy(bp3, 1f);
                break;

            default:
                break;
        }
        //if (other.CompareTag("Weapon"))
        //{
        //    updateHealth(-10);
        //}
        //else if (other.CompareTag("Trap"))
        //{

        //    updateHealth(-100);
        //}
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon") && !invincible)
        {
            updateHealth(-10, false);
            invincible = true;
            Invoke("NonInvincible",1);
        }

    }

    void NonInvincible() 
    {
        invincible = false;
    }

    public void BonusTagAnimation(string bonusName) 
    {
        switch (bonusName)
        {
            case "FullHealth":
                BonusTag.sprite = FullHealthTag;
                break;

            case "NormalHealth":
                BonusTag.sprite = NormalHealthTag;
                break;

            case "Speed":
                BonusTag.sprite = SpeedTag;
                break;

            default:
                break;
        }
        BonusTag.gameObject.GetComponent<Animator>().Play("TagAnim");
    }

    IEnumerator UpdateHealthCanvas(bool status, float delay) 
    {
        yield return new WaitForSeconds(delay);

        HealthCanvas.SetActive(status);
        
    }
    
}