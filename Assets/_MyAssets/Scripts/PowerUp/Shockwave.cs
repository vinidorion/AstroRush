using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Shockwave : MonoBehaviour
{

    private SpaceShip _ship = default;
    private PU _pu;
    [SerializeField] private int _dmg = default;
    [SerializeField] private float _slow = default;
    [SerializeField] private float _slowTime = default;
    [SerializeField] private GameObject _explosion = default;
    private GameManager _gm;

    private void Start()
    {
        _gm = GameManager.Instance;
        _pu = GetComponent<PU>();
        _ship = transform.parent.GetComponent<SpaceShip>();
        transform.position = transform.parent.position;
    }

    void Update()
    {
        transform.localScale = transform.localScale + Vector3.one/3;
    }

    private void OnCollisionEnter(Collision collision)
    {
        SpaceShip _shipTouche = collision.gameObject.GetComponent<SpaceShip>();
        if (_shipTouche != null && _shipTouche.gameObject != _ship.gameObject)
        {
            _shipTouche.SetCurrentLife(_shipTouche.GetHP() - _dmg);
            _shipTouche.Slow(_slow, _slowTime);
            Instantiate(_explosion, transform.position, transform.rotation);
        }
    }
}
