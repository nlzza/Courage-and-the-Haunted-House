using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSupport : MonoBehaviour
{
    [SerializeField] Transform Player;
    [SerializeField] float distance;
    [SerializeField] bool AnimatePointer;
    [SerializeField] bool StopPointer;
    [SerializeField] GameObject pointerAnimator;
    [SerializeField] int TimeToDestroy;

    [Header("Animation Settings")]
    [SerializeField] int AnimLoopLength; 
    [SerializeField] float DisplacementPerLoop;
        

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LocalAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - Player.position).magnitude < distance)
        {
            if (AnimatePointer)
            {
                pointerAnimator.GetComponent<Animator>().enabled = true;
            }
            else if (StopPointer)
            {
                pointerAnimator.GetComponent<Animator>().enabled = false;
            }

            Destroy(this.gameObject,TimeToDestroy);
        }
    }

    IEnumerator LocalAnimation() 
    {
        for (int i = 0; i < AnimLoopLength; i++)
        {
            transform.Translate(0,DisplacementPerLoop,0);
            yield return new WaitForSeconds(0);
        }
        
        yield return new WaitForSeconds(0.1f);
        
        for (int i = 0; i < AnimLoopLength; i++)
        {
            transform.Translate(0,-DisplacementPerLoop, 0);
            yield return new WaitForSeconds(0);
        }

        StartCoroutine(LocalAnimation());
    }
}
