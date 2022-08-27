using UnityEngine;
using UnityEngine.AI;

public class EnemyController: MonoBehaviour
{
    [SerializeField] Transform EnemyCharacter;
    [SerializeField] float MinimumDistanceLimit;
    [SerializeField] float AlarmDistance;
    [SerializeField] ParticleSystem SpawnParticles;

    [Header("Runtime Game Hierarchy References")]

    Transform[] EnemyChildTransforms;
    GameObject Player;
    Animator EnemyAnimator;
    AudioSource alarm;
    AudioSource scream;
    NavMeshAgent EnemyAgent;
    float DistanceFromPlayer;
    bool isDead = false; // Initially, the enemy is alive
    bool isDetected = false; // Initially, the enemy has not been detected

    [Header("Hash Conversions for Optimisation")]

    readonly int isRunninghash = Animator.StringToHash("isRunning");
    readonly int isDeadhash = Animator.StringToHash("isDead");

    // Start is called before the first frame update
    void Start()
    {
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
            scream.Play();
            EnemyAnimator.SetBool(isRunninghash, true);
        }
    }

    private void Chase() 
    {
        EnemyAgent.SetDestination(Player.transform.position);
        transform.rotation = Quaternion.LookRotation(Player.transform.position - transform.position);
    }

    private void UpdateLayerMask(string layerName) 
    {
        int LayerNum = LayerMask.NameToLayer(layerName);

        for (int i = 0; i < EnemyChildTransforms.Length; i++)
        {
            EnemyChildTransforms[i].gameObject.layer = LayerNum;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trap"))
        {
            EnemyAnimator.SetBool(isDeadhash, true);
            isDead = true;
            
            // to stop the ghost movement once it dies
            EnemyAgent.velocity = Vector3.zero;
            EnemyAgent.angularSpeed = 0;
            GetComponent<Rigidbody>().freezeRotation = true;

            // disable the collider so that Courage does not kick around the dead ghost
            GetComponent<CapsuleCollider>().enabled = false;
        }
    }
}
