using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector2 playerDirection;
    Animator playerAnimator;

    [SerializeField] Joystick Joystick;
    
    [SerializeField] Transform PlayerCharacter;

    [SerializeField] float PlayerSpeed;

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = PlayerCharacter.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Player movement controls system
        
        float xMov = Joystick.Horizontal;
        float zMov = Joystick.Vertical;

        transform.Translate( xMov * Time.deltaTime * PlayerSpeed , 0 , zMov * Time.deltaTime * PlayerSpeed );


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
            playerAnimator.SetBool("isRunning",true);
            playerAnimator.SetBool("isIdle", false);
        }
        else 
        {
            playerAnimator.SetBool("isRunning", false);
            playerAnimator.SetBool("isIdle",true);
        }
    }
}
