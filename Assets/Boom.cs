using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boom : MonoBehaviour
{
    private int _dmg = 0;

    private void Start()
    {
        StartCoroutine(TimeLimitCoroutine());
    }

    IEnumerator TimeLimitCoroutine()
    {
        yield return new WaitForSeconds(3);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        SpaceShip _ship = other.GetComponent<SpaceShip>();
        if (_ship != null)
        {
            _ship.GiveHP(-_dmg);
        }
    }
}
