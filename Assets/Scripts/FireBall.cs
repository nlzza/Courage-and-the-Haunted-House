using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] int speed;

    private void OnEnable()
    {
        int num = Random.Range(0,transform.childCount);
        
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i != num)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward*speed*Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(this.gameObject);
    }
}
