using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject[] PU = default;
    [SerializeField] private Vector3 gunPosition = default;
    [SerializeField] private Vector3 canonPosition = default;
    [SerializeField] private int current_PU = -1;

    private float inputx = 0;
    private float inputz = 0;
    private int hp = -1;
    private float timer_atk = 0;
    private int laser_uses = 0;

    // Start is called before the first frame update
    void Start()
    {
        hp = SpaceShip.Instance.GetLife();
    }

    // Update is called once per frame
    void Update()
    {
        move();
        UsePU();
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

    private void UsePU()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (current_PU == 0)
            {
                if (timer_atk > 0.2f)
                {
                    Instantiate(PU[current_PU], new Vector3(transform.position.x + gunPosition.x, transform.position.y + gunPosition.y, transform.position.z + gunPosition.z), transform.rotation * Quaternion.Euler(90, 0, 0), transform);
                    Instantiate(PU[current_PU], new Vector3(transform.position.x - gunPosition.x, transform.position.y + gunPosition.y, transform.position.z + gunPosition.z), transform.rotation * Quaternion.Euler(90, 0, 0), transform);
                    timer_atk = 0;
                    laser_uses++;
                    if (laser_uses >= 5) current_PU = -1;
                }
            }
            else if (current_PU == 1)
            {
                Instantiate(PU[current_PU], new Vector3(transform.position.x + canonPosition.x, transform.position.y + canonPosition.y, transform.position.z + canonPosition.z), transform.rotation * Quaternion.Euler(90, 0, 0), transform);
                current_PU = -1;
            }
            else if (current_PU == 2)
            {
                Instantiate(PU[current_PU], new Vector3(transform.position.x + canonPosition.x, transform.position.y + canonPosition.y, transform.position.z + canonPosition.z), transform.rotation * Quaternion.Euler(0, 0, 0), transform);
                current_PU = -1;
            }
        }

        timer_atk += 1 * Time.deltaTime;
    }

    private void damaged(int dmg, string source)
    {
        hp = hp - dmg;
        if (hp <= 0) Death(source);
    }

    private void Death(string source)
    {
        //ANIMATION DE MORT
    }


}
