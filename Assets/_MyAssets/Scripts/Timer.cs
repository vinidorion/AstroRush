using UnityEngine;

public class Timer : MonoBehaviour
{
    private float _timer = 0;
    [SerializeField] private float _lifeTime = default;

    // Update is called once per frame
    void Update()
    {
        _timer = _timer + 1 * Time.deltaTime;
        if (_timer > _lifeTime) Destroy(transform.gameObject);
    }
}
