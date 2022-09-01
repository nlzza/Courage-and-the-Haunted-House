using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapController : MonoBehaviour
{

    [SerializeField] Transform Spikes;
    [SerializeField] int AnimationLoopLength;
    [SerializeField] float SpikesDisplacement;
    [SerializeField] bool isAnimating;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            if (isAnimating == false)
            {
                isAnimating = true;
                StartCoroutine("SpikesAnimation");
            }

        }


    }

    IEnumerator SpikesAnimation() 
    {
        if (isAnimating)
        {
            for (int i = 0; i < AnimationLoopLength; i++)
            {
                Spikes.Translate(0, 0, SpikesDisplacement);
                Spikes.localScale = Spikes.localScale + new Vector3(0,0, 2f);
                yield return new WaitForSeconds(0f);
            }

            yield return new WaitForSeconds(3f);

            for (int i = 0; i < AnimationLoopLength; i++)
            {
                Spikes.Translate(0, 0, -SpikesDisplacement);
                Spikes.localScale = Spikes.localScale + new Vector3(0,0, -2f);
                yield return new WaitForSeconds(0f);
            }

            isAnimating = false;
        }
        
    }
}
