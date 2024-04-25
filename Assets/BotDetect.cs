using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotDetect : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private float dif;
    [SerializeField] private Bot BotManager;
    private float agiBot;
    private float difBotMod;

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("BotSpeedZone"))
        {
            agiBot = gameObject.GetComponent<SpaceShip>().GetAgility();
            difBotMod = 2 - dif;

            float TSpeed;
            TSpeed = collision.gameObject.GetComponent<BotSpeedZone>().BotSpeed * agiBot / difBotMod;

            if (TSpeed > gameObject.GetComponent<SpaceShip>().GetMaxSpeed())
            {
                TSpeed = gameObject.GetComponent<SpaceShip>().GetMaxSpeed();
            }
            Debug.Log(TSpeed);
            BotManager.SetTSpeed(TSpeed);
        }
    }
}
