using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotDetect : Bot
{
    [SerializeField] private int index;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "BotSpeedZone")
        {
            this.gameObject.GetComponent<SpaceShip>().SetMaxSpeed((_maxSpeed[index] - collision.gameObject.GetComponent<BotSpeedZone>().BotSpeed) / (2 - difficulty[index]) * _agility);
        }
    }
}
