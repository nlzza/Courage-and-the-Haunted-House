using UnityEngine;
using UnityEngine.AI;

public class EnemyController: MonoBehaviour
{
    [SerializeField] Transform EnemyCharacter;
    [SerializeField] float MinimumDistanceLimit;
    [SerializeField] float AlarmDistance;
    [SerializeField] float AttackDistance;
    [SerializeField] ParticleSystem SpawnParticles;
    [SerializeField] ParticleSystem[] DeathParticles;
    [SerializeField] GameObject Weapon;
    [SerializeField] AudioClip AttackSound;


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
            UpdateLayerMask("Default");
            Instantiate(SpawnParticles,transform.position,Quaternion.identity);
            Player.GetComponent<PlayerController>().PlayScreamSound() ;
            scream.Play();
            EnemyAnimator.SetTrigger("isAttack");
            print("Here");
            Player.GetComponent<PlayerController>().UpdateTarget("");
            //EnemyAnimator.SetBool(isRunninghash, true);
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
        }
        else if (!isDead && (DistanceFromPlayer <= AttackDistance)) 
        {
            Attack();
        }
        
    }

    void Attack() 
    {
        EnemyAgent.speed = 0;
        EnemyAnimator.SetTrigger("isAttack");
        Weapon.GetComponent<BoxCollider>().enabled = true;
        
        if (playAttackAudio == false)
        {
            alarm.PlayOneShot(AttackSound);
            playAttackAudio = true;
            Invoke("AttackFinish",0.5f);
        }

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
        if (other.CompareTag("Trap"))
        {
            isDead = true;
            EnemyAnimator.SetBool(isDeadhash, true);

            if (!PlayerTargetUpdated)
            {
                Player.GetComponent<PlayerController>().UpdateTarget("NextEnemy");
                PlayerTargetUpdated = true;
            }

            Destroy(other.transform.parent.gameObject,1f);


            int num = Random.Range(0,DeathParticles.Length);
            for (int i = 0; i < DeathParticles.Length; i++)
            {
                if (i == num)
                {
                    Instantiate(DeathParticles[i],transform.position,Quaternion.identity);
                }
            }

            // to stop the ghost movement once it dies
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            // disable the collider so that Courage does not kick around the dead ghost
            GetComponent<CapsuleCollider>().enabled = false;

            Invoke("Die",3f);
        }
    }
}
