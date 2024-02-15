using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    float inputx = 0;
    float inputz = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        move();
    }

    private void move()
    {
        inputx = Input.GetAxis("Horizontal");
        inputz = Input.GetAxis("Vertical");
        if (inputz > 0) SpaceShip.Instance.Forward();
        if (inputx > 0) SpaceShip.Instance.Turn(false);
        if (inputx < 0) SpaceShip.Instance.Turn(true);
        if (Input.GetKey(KeyCode.Mouse0)) SpaceShip.Instance.AirBrake(true);
        if (Input.GetKey(KeyCode.Mouse1)) SpaceShip.Instance.AirBrake(false);
    }    
}
