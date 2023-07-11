using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projectile : MonoBehaviour
{
    public float _speed;
    public Vector3 _dir;
    private float _timeAlive;

    // Start is called before the first frame update
    void Awake()
    {
        _speed = 25;
        _dir = Vector3.Cross(transform.forward, transform.right);

    }

    // Update is called once per frame
    void Update()
    {
        _timeAlive += Time.deltaTime;
        transform.position += _speed * _dir * Time.deltaTime;

        if (_timeAlive > 5)
            Destroy(this.gameObject);
    }
}
