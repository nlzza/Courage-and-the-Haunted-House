using UnityEngine;
using UnityEngine.UI;

public class PlayerController: MonoBehaviour
{

    [Header("Player References")]
    [SerializeField] Joystick Joystick;
    [SerializeField] Transform PlayerCharacter;
    [SerializeField] float PlayerSpeed;
    [SerializeField] Transform[] TargetsByOrder;

    [Header("Player Health References")]
    [SerializeField] int PlayerHealth;
    [SerializeField] GameObject HealthCanvas;
    [SerializeField] Slider HealthSlider;

    [Header("Direction Arrow References")]
    [SerializeField] Transform DirectionArrow;
    [SerializeField] Renderer ArrowRenderer;


    [Header("RUNTIME references")]

    GameManager gameManager;

    Vector2 playerDirection;
    Animator playerAnimator;
    Rigidbody PlayerRigidbody;
    Transform Target;
    AudioSource audioSource;
    float distanceFromCurrentTarget;

    bool isDead;

    float H, S, V;
    // Start is called before the first frame update
    void Start()
    {
        Target = TargetsByOrder[0];
        playerAnimator = PlayerCharacter.gameObject.GetComponent<Animator>();
        PlayerRigidbody = GetComponent<Rigidbody>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        audioSource = GetComponent<AudioSource>();

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

            // transform.Translate( xMov * Time.deltaTime * PlayerSpeed , 0 , zMov * Time.deltaTime * PlayerSpeed );
            var vectorsample = new Vector3(xMov * Time.deltaTime * PlayerSpeed, 0, zMov * Time.deltaTime * PlayerSpeed) * 100;
            PlayerRigidbody.velocity = vectorsample;

            //Player rotation control system
            if (zMov != 0 && xMov != 0)
            {
                playerDirection.x = zMov;
                playerDirection.y = xMov;

            }
            Vector2 direction = playerDirection - Vector2.zero;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            PlayerCharacter.rotation = Quaternion.Euler(0, angle, 0);

            //Player Animation Control System
            if (zMov != 0 || xMov != 0)
            {
                playerAnimator.SetBool("isRunning", true);
                playerAnimator.SetBool("isIdle", false);
            }
            else
            {
                playerAnimator.SetBool("isRunning", false);
                playerAnimator.SetBool("isIdle", true);
            }
        }
        #endregion

        HealthSlider.value = PlayerHealth;

        DirectionArrow.LookAt(Target);
        ArrowColorChange();

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

            H = dist / 150; S = 1; V = 1;
        }

        ArrowRenderer.material.color = Color.HSVToRGB(H,S,V);        
    }

    public void UpdateTarget(string str)
    {
        for (int i = 0; i < TargetsByOrder.Length-1; i++)
        {
            if (Target == TargetsByOrder[i])
            {
                Target = TargetsByOrder[i + 1];

                if (str == "NextEnemy")
                {
                    Target.gameObject.SetActive(true);
                    TargetsByOrder[i + 2].gameObject.SetActive(true);
                }
                break;
            }
        }
    }

    public void updateHealth(int num) 
    {
        PlayerHealth += num;

        if (PlayerHealth <= 0)
        {
            isDead = true;
            playerAnimator.Play("Death");
            gameManager.LevelFailed();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            updateHealth(-10);
        }
        else if (other.CompareTag("Trap"))
        {
            updateHealth(-100);
        }
    }
}
