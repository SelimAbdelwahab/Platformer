using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private const uint SPAWN_TIMER = 2; // Every 2 seconds
    private float _spawnTime = 0;
    GameObject _projectilePrefab;

    // Start is called before the first frame update
    void Awake()
    {
        _projectilePrefab = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        _projectilePrefab.transform.localScale = new Vector3(0.2f, 0.3f, 0.2f);
        _projectilePrefab.AddComponent<Projectile>();
        _projectilePrefab.gameObject.tag = "Projectile";     
    }

    // Update is called once per frame
    void Update()
    {
        _spawnTime += Time.deltaTime;

        if (_spawnTime >= SPAWN_TIMER)
        {
            _spawnTime = 0;
            SpawnProjectile();
        }
    }

    void SpawnProjectile()
    {
        GameObject projectile = GameObject.Instantiate(_projectilePrefab, this.transform.position, this.transform.rotation);

        projectile.SetActive(true);
    }
}
