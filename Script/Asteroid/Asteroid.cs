using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    //
    [SerializeField]
    private float _asteroidRotatedSpeed = 10f;

    //
    [SerializeField]
    private GameObject _asteroidExplosionPrefab;

    //
    private SpawnManager _spawnManager;

    //
    private ExplosionSound _explosionAudio;

    // Start is called before the first frame update
    void Start()
    {
        //Get Spawn Manager component
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.Log("Spawn_Manager is null, can't find component SpawnManager");
        }

        //Get Explosion Sound component
        _explosionAudio = GameObject.Find("Explosion_Sound").GetComponent<ExplosionSound>();
        if (_explosionAudio == null)
        {
            Debug.Log("Explosion_Audio is null, can't find component ExplosionSound");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * _asteroidRotatedSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Instantiate(_asteroidExplosionPrefab, transform.position, Quaternion.identity);

            Destroy(other.gameObject);

            _spawnManager.StartSpawn();

            Destroy(this.gameObject);

            _explosionAudio.ExplosionAudio();
        }

        if (other.gameObject.tag == "Player")
        {
            Instantiate(_asteroidExplosionPrefab, transform.position, Quaternion.identity);

            Destroy(other.gameObject);

            _spawnManager.StopSpawning();

            Destroy(this.gameObject);

            _explosionAudio.ExplosionAudio();
        }
    }
}
