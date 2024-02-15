using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    bool freeze = false;
    float current_speed = 0;
    int current_pu = 0;

    [Header("Stats")]
    [SerializeField] float max_speed = default;
    [SerializeField] float acceleration = default;
    [SerializeField] float airbrake_power = default;
    [SerializeField] int hp = default;
    [SerializeField] float weigth = default;
    [SerializeField] float agility = default;

    [SerializeField] Rigidbody rb = default;




    void Forward()
    {
        if (!freeze && current_speed < max_speed)
        {
            Vector3 force = new Vector3(-1 * acceleration, 0, 0);
            rb.AddForce(force);
        }
    }

    void Turn(bool left)
    {
        if (!freeze)
        {
            if (left)
            {
                float rotation = agility - current_speed;
            }
            if (!left)
            {
                float rotation = (agility - current_speed) * -1;
            }
        }
    }

    void AirBrake(bool left)
    {
        if (!freeze)
        {
            if (left)
            {
                float rotation = agility - current_speed + airbrake_power;
                Vector3 force = new Vector3(1 * current_speed + airbrake_power - weigth, 0, 0);
                rb.AddForce(force);
            }
            if (!left)
            {
                float rotation = (agility - current_speed + airbrake_power) * -1;
                Vector3 force = new Vector3(1 * current_speed + airbrake_power - weigth, 0, 0);
                rb.AddForce(force);
            }
        }
    }

    void UsePU()
    {
        if (!freeze)
        {

        }
    }

    void RemovePU()
    {

    }

    void AddPU()
    {

    }
}


