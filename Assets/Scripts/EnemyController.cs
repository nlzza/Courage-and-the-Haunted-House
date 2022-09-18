using UnityEngine;
using UnityEngine.AI;

public class EnemyController: MonoBehaviour
{

    [Header("Normal Enemy Settings")]
    [SerializeField] Transform EnemyCharacter;
    [SerializeField] float MinimumDistanceLimit;
    [SerializeField] float AlarmDistance;
    [SerializeField] float AttackDistance;
    [SerializeField] GameObject GiftBox;
    [SerializeField] GameObject SpawnParticles;
    [SerializeField] GameObject DeathParticles;
    [SerializeField] GameObject Weapon;
    [SerializeField] AudioClip AttackSound;
    [SerializeField] AudioClip DieSound;
    [SerializeField] AudioClip SpawnSound;

    [Header("Shooter Enemy Settings")]
    [SerializeField] bool ShooterEnemy;
    [SerializeField] Transform ShootSpot;
    [SerializeField] GameObject FireBall;
    [SerializeField] float ShootDistance;
    bool shootCheck;

    [Header("Runtime Game Hierarchy References")]

    Transform[] EnemyChildTransforms;
    GameObject Player;
    Animator EnemyAnimator;
    AudioSource alarm;
    AudioSource scream;
    NavMeshAgent EnemyAgent;
    float DistanceFromPlayer;

    GameManager gameManager;

    bool isDead = false; // Initially, the enemy is alive
    bool isDetected = false; // Initially, the enemy has not been detected
    bool playAttackAudio = false;
    bool PlayerTargetUpdated = false;


    [Header("Hash Conversions for Optimisation")]

    readonly int isRunninghash = Animator.StringToHash("isRunning");
    readonly int isDeadhash = Animator.StringToHash("isDead");

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();

        EnemyAgent = GetComponent<NavMeshAgent>();
        Player = GameObject.FindGameObjectWithTag("Player");
        EnemyAnimator = EnemyCharacter.gameObject.GetComponent<Animator>();
        EnemyChildTransforms = EnemyCharacter.transform.GetComponentsInChildren<Transform>();
        alarm = GetComponent<AudioSource>();
        scream = Player.GetComponent<AudioSource>();

        UpdateLayerMask("Hidden");
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) // Do nothing if enemy has died.
            return;
        
        if (isDetected)
            Chase();
        else
            DistanceCalculation();
    }

    private void DistanceCalculation() 
    {
        DistanceFromPlayer = (Player.transform.position - transform.position).magnitude;
        
        if (DistanceFromPlayer < AlarmDistance && !alarm.isPlaying)
            alarm.Play();

        if (DistanceFromPlayer < MinimumDistanceLimit)
        {
            isDetected = true;

            alarm.PlayOneShot(SpawnSound);
            UpdateLayerMask("Default");
            Destroy(GiftBox);

            if (gameManager.LevelNumber == 1)
            {
                Invoke("TutorialBreak", 1);
            }


            GameObject spawnps = Instantiate(SpawnParticles,transform.position,Quaternion.identity);
            Destroy(spawnps,1f);
            Player.GetComponent<PlayerController>().PlayScreamSound() ;
            scream.Play();
            EnemyAnimator.SetTrigger("isAttack");

            Player.GetComponent<PlayerController>().UpdateTarget("");
            
        }
    }

    private void Chase() 
    {
        DistanceFromPlayer = (Player.transform.position - transform.position).magnitude;

        if (!isDead && (DistanceFromPlayer > AttackDistance))
        {
            EnemyAgent.speed = 10;
            EnemyAgent.SetDestination(Player.transform.position);
            transform.rotation = Quaternion.LookRotation(Player.transform.position - transform.position);

            Weapon.GetComponent<BoxCollider>().enabled = false;

            if (!isDead && (DistanceFromPlayer <= ShootDistance) && ShooterEnemy && !shootCheck)
            {
                ShootFireBall();
            }
        }
        else if (!isDead && (DistanceFromPlayer <= AttackDistance)) 
        {
            Attack();
        }
        

    }

    void Attack() 
    {
        if (!Player.GetComponent<PlayerController>().isDead)
        {
          
            EnemyAgent.speed = 0;
            EnemyAnimator.SetTrigger("isAttack");
            Weapon.GetComponent<BoxCollider>().enabled = true;

            if (playAttackAudio == false)
            {
                alarm.PlayOneShot(AttackSound);
                playAttackAudio = true;
                Invoke("AttackFinish", 0.5f);
            }
        }
        else 
        {
            EnemyAnimator.Play("Idle");
        }

    }

    void ShootFireBall() 
    {
        shootCheck = true;
        Invoke("ShootCheckReverse", 1);
        Instantiate(FireBall, ShootSpot.position, ShootSpot.rotation);
    }

    public void ShootCheckReverse() 
    {
        shootCheck = false;
    }

    void AttackFinish() 
    {
        playAttackAudio = false;
    }


    private void UpdateLayerMask(string layerName) 
    {
        int LayerNum = LayerMask.NameToLayer(layerName);

        for (int i = 0; i < EnemyChildTransforms.Length; i++)
        {
            EnemyChildTransforms[i].gameObject.layer = LayerNum;
        }
    }

    public void Die() 
    {

        gameManager.UpdateEnemyStatus();
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trap") || other.CompareTag("Laser"))
        {
            isDead = true;
            EnemyAnimator.SetBool(isDeadhash, true);

            if (!PlayerTargetUpdated)
            {
                Player.GetComponent<PlayerController>().UpdateTarget("NextEnemy");
                PlayerTargetUpdated = true;
            }

            if (other.CompareTag("Trap"))
            {
                Destroy(other.transform.parent.gameObject, 1f);
            }


            Quaternion rot = Quaternion.Euler(-90,180,0);

            GameObject deathps = Instantiate(DeathParticles,transform.position + new Vector3(0,5,0), rot);
            Destroy(deathps,2f);

            // to stop the ghost movement once it dies
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            // disable the collider so that Courage does not kick around the dead ghost
            GetComponent<CapsuleCollider>().enabled = false;
            alarm.PlayOneShot(DieSound);
            Invoke("Die",3f);
        }
       
    }

    public void TutorialBreak() 
    {
        Time.timeScale = 0.05f;
        gameManager.TutorialDisplay();
    }
}
