using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnManager : MonoBehaviour
{
    private bool _stopSpawning = false;


    [Header("Wave variables")]
    public int waveNum = 1;
    public int waveMaxNum = 5;
    public int enemyNum = 7;
    public int enemyMaxNum;
    public int killedEnemy;

    [SerializeField]
    private GameObject[] _boostPrefabs;

    [SerializeField]
    private GameObject[] _enemyPrefabs;

    [SerializeField]
    private GameObject _mainBoss;

    //Create Parent container for Enemy
    [SerializeField]
    private GameObject _enemyContainer;

    private UIManager _uiManager;

    private GameObject _player;

    // Start is called before the first frame update
    void Start()
    {
        _mainBoss.SetActive(false);

        _uiManager = GameObject.Find("UI_Manager").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.Log("UIManager component not found");
        }

        _uiManager.WaveDiplsayText(waveNum);

        _player = GameObject.Find("Player");
    }

    private void Update()
    {
        if (_player == null)
        {
            _stopSpawning = true;
        }
    }

    public void StartSpawn()
    {
        if (waveNum <= waveMaxNum)
        {
            _stopSpawning = false;
            enemyMaxNum = enemyNum;

            switch (waveNum)
            {
                case 1:
                    StartCoroutine(EasyEnemySpawnRoutine());
                break;
                case 2:
                    StartCoroutine(MiddleEnemySpawnRoutine());
                break;
                case 3:
                    StartCoroutine(HardEnemySpawnRoutine());
                break;
                case 4:
                    StartCoroutine(SuperHardEnemySpawnRoutine());
                break;
                case 5:
                    StartCoroutine(MainBossSpawnRoutine());
                break;
                default:
                break;
            }
        }
        else
        {
            _stopSpawning = true;
        }

        StartCoroutine(BoostSpawnRoutine());
        StartCoroutine(AimBoostSpawnRoutine());
        StartCoroutine(HealthBoostSpawnRoutine());
    }

    IEnumerator EasyEnemySpawnRoutine()
    {
        yield return new WaitForSeconds(2f);

        while (_stopSpawning == false && enemyMaxNum > 0)
        {
            enemyMaxNum--;

            Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 9f, 0);
            GameObject newEnemy = Instantiate(_enemyPrefabs[0], posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;

            //Wait for 5 seconds to Spawn Enemy
            yield return new WaitForSeconds(3.0f);
        }
    }

    IEnumerator MiddleEnemySpawnRoutine()
    {
        yield return new WaitForSeconds(2f);

        while (_stopSpawning == false && enemyMaxNum > 0)
        {
            enemyMaxNum--;

            Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 9f, 0);
            GameObject newEnemy = Instantiate(_enemyPrefabs[1], posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;

            //Wait for 5 seconds to Spawn Enemy
            yield return new WaitForSeconds(3.0f);
        }
    }

    IEnumerator HardEnemySpawnRoutine()
    {
        yield return new WaitForSeconds(2f);

        while (_stopSpawning == false && enemyMaxNum > 0)
        {
            enemyMaxNum--;

            Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 9f, 0);
            GameObject newEnemy = Instantiate(_enemyPrefabs[2], posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;

            //Wait for 5 seconds to Spawn Enemy
            yield return new WaitForSeconds(3.0f);
        }
    }

    IEnumerator SuperHardEnemySpawnRoutine()
    {
        yield return new WaitForSeconds(2f);

        while (_stopSpawning == false && enemyMaxNum > 0)
        {
            enemyMaxNum--;

            Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 9f, 0);
            GameObject newEnemy = Instantiate(_enemyPrefabs[3], posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;

            //Wait for 5 seconds to Spawn Enemy
            yield return new WaitForSeconds(3.0f);
        }
    }

    IEnumerator AimBoostSpawnRoutine()
    {
        if (waveNum < 5)
        {
            yield return new WaitForSeconds(10f);
        }
        else if (waveNum == 5)
        {
            yield return new WaitForSeconds(25f);
        }

        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 9f, 0);
            Instantiate(_boostPrefabs[4], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(3f, 5f));
        }
    }

    IEnumerator BoostSpawnRoutine()
    {
        yield return new WaitForSeconds(15f);

        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 9f, 0);
            Instantiate(_boostPrefabs[Random.Range(0, 3)], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(5f, 10f));
        }
    }

    IEnumerator HealthBoostSpawnRoutine()
    {
        yield return new WaitForSeconds(20);

        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 9f, 0);
            Instantiate(_boostPrefabs[5], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(25f);
        }
    }

    IEnumerator MainBossSpawnRoutine()
    {
        yield return new WaitForSeconds(2);

        while (_stopSpawning == false)
        {
            if (_mainBoss.activeSelf == false)
            { 
                _mainBoss.SetActive(true);
            }

            yield return null;
        }
    }

    //Create StopSpawning method to stop spawning objects and using in Player script
    public void StopSpawning()
    {
        //Set Spawn status true when Player die
        _stopSpawning = true;
    }

    public void EnemyKilled()
    {
        killedEnemy++;

        if (killedEnemy == enemyNum)
        {
            _stopSpawning = true;
            waveNum++;
            enemyNum += 2;
            killedEnemy = 0;
            _uiManager.WaveDiplsayText(waveNum);
        }
    }
}
