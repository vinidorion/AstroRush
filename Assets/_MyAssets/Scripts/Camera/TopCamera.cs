using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopCamera : MonoBehaviour
{
    private Transform _plyPos;

    // Start is called before the first frame update
    void Start()
    {
        FindPly();
        transform.parent = _plyPos;
        transform.localPosition = Vector3.up * 50;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FindPly()
    {
        if (Player.Instance)
        {
            _plyPos = Player.Instance.transform;
        }
        else
        {
            Debug.Log("PLAYER NOT FOUND");
        }
    }
}
