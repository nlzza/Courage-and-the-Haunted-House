using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyController : MonoBehaviour
{
    [SerializeField] NavMeshAgent EnemyAgent;
    [SerializeField] Transform EnemyCharacter;
    [SerializeField] float DistanceFromPlayer;
    [SerializeField] float MinimumDistanceLimit;
    [SerializeField] bool isDetected;
    [SerializeField] ParticleSystem SpawnParticles;


    [Header("Runtime Game Hierarchy References")]

    Transform[] EnemyChildTransforms;
    GameObject Player;
    Animator EnemyAnimator;



    // Start is called before the first frame update
    void Start()
    {
        EnemyAgent = GetComponent<NavMeshAgent>();
        Player = GameObject.FindGameObjectWithTag("Player");
        EnemyAnimator = EnemyCharacter.gameObject.GetComponent<Animator>();
        EnemyChildTransforms = EnemyCharacter.transform.GetComponentsInChildren<Transform>();

        UpdateLayerMask("Hidden");




    }

    // Update is called once per frame
    void Update()
    {
        DistanceCalculation();

        if (isDetected)
        {
            Chase();
        }
    }

    private void DistanceCalculation() 
    {
        DistanceFromPlayer = (Player.transform.position - transform.position).magnitude;
        
        if (DistanceFromPlayer < MinimumDistanceLimit && !isDetected)
        {
            isDetected = true;
            UpdateLayerMask("Default");

            Instantiate(SpawnParticles,transform.position,Quaternion.identity);
        }
    }

    private void Chase() 
    {
        EnemyAgent.SetDestination(Player.transform.position);

        transform.rotation = Quaternion.LookRotation(Player.transform.position - transform.position);
    }
    private void UpdateLayerMask(string layerName) 
    {
        int LayerNum = 0;

        LayerNum = LayerMask.NameToLayer(layerName);

        for (int i = 0; i < EnemyChildTransforms.Length; i++)
        {
            EnemyChildTransforms[i].gameObject.layer = LayerNum;
        }


    }
}
