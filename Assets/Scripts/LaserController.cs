using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    [SerializeField] AudioSource[] LaserHeads;
    [SerializeField] GameObject Laser;
    [SerializeField] int LaserONtime;
    [SerializeField] int LaserOFFtime;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LaserSystem()); 
    }

    IEnumerator LaserSystem() 
    {
        for (int i = 0; i < LaserHeads.Length; i++)
        {
            LaserHeads[i].mute = true;
        }
        Laser.SetActive(false);

        yield return new WaitForSeconds(LaserOFFtime);

        for (int i = 0; i < LaserHeads.Length; i++)
        {
            LaserHeads[i].mute = false;
        }
        Laser.SetActive(true);

        yield return new WaitForSeconds(LaserONtime);

        StartCoroutine(LaserSystem());

    }
}
