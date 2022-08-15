using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector2 windDirection;

    [SerializeField] Joystick Joystick;
    
    [SerializeField] Transform PlayerMesh;

    [SerializeField] float PlayerSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ///Player movement and rotation controls system
        float xMov = Joystick.Horizontal;
        float zMov = Joystick.Vertical;

        transform.Translate( xMov * Time.deltaTime * PlayerSpeed , 0 , zMov * Time.deltaTime * PlayerSpeed );

        windDirection.x = zMov;
        windDirection.y = xMov;
        
        Vector2 direction = windDirection - Vector2.zero;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        PlayerMesh.rotation = Quaternion.Euler(0, angle, 0);
    }
}
