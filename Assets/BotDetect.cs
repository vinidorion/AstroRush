using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotDetect : MonoBehaviour
{
    [SerializeField] private int index;
    private float agiBot;
    private float difBotMod;

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("BotSpeedZone")) //pas capable davoir la difficulty des bots
        {
            agiBot = gameObject.GetComponent<SpaceShip>().GetAgility();
            //difBotMod = 2 - gameObject.GetComponent<Bot>().GetDifficulty(index);

            float TSpeed;
            TSpeed = collision.gameObject.GetComponent<BotSpeedZone>().BotSpeed * agiBot;
            Debug.Log(gameObject.GetComponent<Bot>().GetDifficulty(index));

            //gameObject.GetComponent<SpaceShip>().SetMaxSpeed(gameObject.GetComponent<Bot>().GetDifficulty(index));
        }
    }
}
