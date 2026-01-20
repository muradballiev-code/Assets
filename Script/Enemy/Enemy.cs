using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int _enemyID;
    private int _enemyMoveType;
    private int _enemyFireType;
    private bool _isEnemyAlive = false;
    
    private float _enemySpeed;
    private float _enemyCanFire = -1f;
    private float _enemyFireRate = 2f;
    private float _enemyCanFireBoost = -1f;
    private float _enemyFireBoostRate = 0.5f;
    private float _waitTime = 1.5f;

    [SerializeField]
    private GameObject _enemyLaser;

    //Enemy Shied 
    private bool _isEnemyShieildActive = false;
    private int _enemyShieldRandom;
    [SerializeField]
    private GameObject _enemyShield;

    //Enemy Aggressive
    private bool _aggressiveEnemy = false;
    private float enemyRammingDist = 4.0f;

    //Enemy Detect Boosts
    private float _checkRange = 10f;
    [SerializeField]
    private LayerMask boostMask;

    //Avoid Player Shot
    private float _avoidDistance = 0.5f;
    private bool _isAvoidShot = false;
    private Vector3 _moveDirection;
    private float _avoidDuration = 0.5f;
    private float _avoidTimer = 0f;

    private Animator _enemyAnim;

    //Player Status
    private int _score = 10;
    private bool _playerBehind = false;
    private bool _isPlayerAlive = true;
    private Player _player;

    private bool _boostDetected = false;

    //Enemy new movement Zig-Zag
    private float _movingAmplitude = 2.5f;
    private float _movingFrequency = 3f;
    private float _movingstartPosition;
    private float _timeOffset;

    //Enemy new movement Side to Side
    private float currentY = 5.0f;
    private int directionX;

    private ExplosionSound _explosionSound;

    private EnemyLaserBeam _enemyBeam;
    private Coroutine _laserRoutine;

    private SpawnManager _spawnManager;

    private void Start()
    {
        _enemyShield.SetActive(false);
        _enemyShieldRandom = Random.Range(1, 5);
        _enemySpeed = Random.Range(2.5f, 3.5f);

        //Get Animator component from Enemy object
        _enemyAnim = GetComponent<Animator>();
        if (_enemyAnim == null)
        {
            Debug.Log("Enemy animation is null, can't find component Animator");
        }

        _enemyBeam = GetComponentInChildren<EnemyLaserBeam>(true);
        if (_enemyBeam != null)
        {
            _enemyBeam.gameObject.SetActive(false);
        }

        //Get Player script from Player object
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.Log("Player object is null, Player component not found");
        }

        //Get Eexplosion Sound script from Explosion_Sound object
        _explosionSound = GameObject.Find("Explosion_Sound").GetComponent<ExplosionSound>();
        if (_explosionSound == null)
        {
            Debug.Log("Explosion_Sound object is null, ExplosionSound component not found");
        }

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.Log("SpawnManager is null, can't find component GameManager");
        }

        _movingstartPosition = transform.position.x;
        _timeOffset = Random.Range(0f, 100f);
        directionX = Random.value > 0.5f ? 1 : -1;

        if (_enemyID == 3)
        {
            _enemyMoveType = Random.Range(0, 3);
            _enemyFireType = Random.Range(0, 1);
        }
    }

    private void Update()
    {
        //Check if Player Dead or Alive
        if (_player == null)
        {
            _isPlayerAlive = false;
        }

        if (_aggressiveEnemy == true)
        {
            RamPlayer();
        }

        switch (_enemyID)
        {
            case 0:
                EnemyMovement();
                if (_isEnemyAlive == false)
                {
                    EnemyLaserFire();
                }
                PlayerBehindEnemy();
            break;
            case 1:
                SideToSideEnemyMovement();
                if (_isEnemyAlive == false)
                {
                    EnemyLaserFire();
                }
                PlayerBehindEnemy();
                EnemyShield();
                BoostDetected();
                Debug.Log(_enemyID);
            break;
            case 2:
                ZigZagEnemyMovement();
                if (_isEnemyAlive == false)
                {
                    EnemyLaserBeam();
                }
                EnemyShield();
                EnemyAggression();
                EnemyAvoidShot();
                BoostDetected();
                PlayerBehindEnemy();
            break;
            case 3:
                EnemyMovementFire();
                EnemyShield();
                EnemyAggression();
                EnemyAvoidShot();
                BoostDetected();
                PlayerBehindEnemy();
            break;

            default:
            break;
        }
    }

    public void EnemyMovementFire()
    {
        switch (_enemyMoveType)
        {
            case 0:
                EnemyMovement();
                break;
            case 1:
                SideToSideEnemyMovement();
                break;
            case 2:
                ZigZagEnemyMovement();
                break;

            default:
                EnemyMovement();
                break;
        }

        if (_isEnemyAlive == false)
        {
            switch (_enemyFireType)
            {
                case 0:
                    EnemyLaserFire();
                break;
                case 1:
                    EnemyLaserBeam();
                break;

                default:
                    EnemyLaserFire();
                break;
            }
        }

        
    }

    //Enemy standart move
    public void EnemyMovement()
    {
        //Move Enemy down
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);

        //Check Enemy position by X coordinate
        if (transform.position.y <= -7.5f)
        {
            if (_isPlayerAlive == false)
            {
                Destroy(this.gameObject);
                return;
            }

            float randomPosX = Random.Range(-8f, 8f);
            //Set new random position by X for Enemy for moving
            transform.position = new Vector3(randomPosX, 11f, 0);
        }
    }

    //Enemy standart move
    public void SideToSideEnemyMovement()
    {
        if (_aggressiveEnemy == true)
        {
            return;
        }

        if (transform.position.y > currentY)
        {
            transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
            return;
        }

        transform.position = new Vector3(transform.position.x, currentY, 0);

        MoveSideToSide();
    }

    void MoveSideToSide()
    {
        transform.Translate(Vector3.right * directionX * 8 * Time.deltaTime);

        if (transform.position.x <= -8.5f || transform.position.x >= 8.5f)
        {
            directionX *= -1;
            currentY -= 0.5f;
        }

        //Check Enemy position by X coordinate
        if (transform.position.y <= -7f)
        {
            if (_isPlayerAlive == false)
            {
                Destroy(this.gameObject);
                return;
            }

            float randomPosX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomPosX, 11f, 0);

            currentY = 5f;
            directionX = Random.value > 0.5f ? 1 : -1;

            return;
        }
    }

    //Enemy New Movement (Zig-Zag)
    public void ZigZagEnemyMovement()
    {
        if (_aggressiveEnemy == true)
        {
            return;
        }

        float newPosX = _movingstartPosition + Mathf.Cos((Time.time + _timeOffset) * _movingFrequency) * _movingAmplitude;

        //Move Enemy down
        transform.Translate(Vector3.down * Time.deltaTime);

        float borderX = Mathf.Clamp(newPosX, -9f, 9f);
        transform.position = new Vector3(borderX, transform.position.y, 0);

        //Check Enemy position by X coordinate
        if (transform.position.y <= -7.5f)
        {
            if (_isPlayerAlive == false)
            {
                Destroy(this.gameObject);
                return;
            }

            //Set new random position by X for Enemy for moving
            float randomPosX = Random.Range(-7f, 7f);
            transform.position = new Vector3(randomPosX, 11f, 0);

            _movingstartPosition = Random.Range(-8f, 8f);
            _timeOffset = Random.Range(0f, 100f);
        }
    }

    //Enemy Laser fire
    public void EnemyLaserFire()
    {
        if (_isPlayerAlive == false)
        return;

        if (transform.position.y >= 7f || transform.position.y <= -4f)
        return;

        if (_aggressiveEnemy == true)
        return;

        if (Time.time > _enemyCanFire)
        {
            _enemyCanFire = _enemyFireRate + Time.time;

            Vector3 _laserPosition;

            //Check if Player behind Enemy and change Laser positon
            if (_playerBehind == true)
            {
                _laserPosition = transform.position + new Vector3(0, 2.5f, 0);
            }
            else
            {
                _laserPosition = transform.position;
            }

            GameObject enemyLaser = Instantiate(_enemyLaser, _laserPosition, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            if (lasers.Length > 0)
            {
                foreach (Laser laser in lasers)
                {
                    //Send Player position and Laser status in method in Laser Script
                    laser.AssignEnemyLaser(_playerBehind);
                }
            }
        }
    }

    public void EnemyLaserBeam()
    {
        if (_isPlayerAlive == false)
        return;

        if (transform.position.y >= 7f || transform.position.y <= -4f)
        return;

        if (_aggressiveEnemy == true)
        return;

        if (_playerBehind == true)
        {
            if (Time.time > _enemyCanFire)
            {
                _enemyCanFire = _enemyFireRate + Time.time;

                Vector3 _laserPosition;

                _laserPosition = transform.position + new Vector3(0, 2.5f, 0);

                GameObject enemyLaser = Instantiate(_enemyLaser, _laserPosition, Quaternion.identity);
                Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

                if (lasers.Length > 0)
                {
                    foreach (Laser laser in lasers)
                    {
                        //Send Player position and Laser status in method in Laser Script
                        laser.AssignEnemyLaser(_playerBehind);
                    }
                }
            }
        }

        if (_laserRoutine == null)
        {
            _laserRoutine = StartCoroutine(LaserBeamCoroutine());
        }
    }

    IEnumerator LaserBeamCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (_isPlayerAlive == true)
        {
            _enemyBeam.gameObject.SetActive(true);

            yield return new WaitForSeconds(1.5f);

            _enemyBeam.gameObject.SetActive(false);

            yield return new WaitForSeconds(1.5f);
        }
    }

    //Enemy Sheild Activate
    public void EnemyShield()
    {
        if (_enemyShieldRandom == 3)
        {
            _enemyShield.SetActive(true);
            _isEnemyShieildActive = true;
        }
    }

    //Check Distance between Player and Enemy to get agression and Ram Player
    public void EnemyAggression()
    {
        if (_isPlayerAlive == false)
        return;

        float _distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

        if (_distanceToPlayer < enemyRammingDist)
        {
            _aggressiveEnemy = true;
        }
        else
        {
            _aggressiveEnemy = false;
        }
    }

    //Ram Player
    public void RamPlayer()
    {
        if (_isPlayerAlive == false)
        return;
        transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, _enemySpeed * 2f * Time.deltaTime);
    }

    //Boost Detect by Enemy
    private void BoostDetected()
    {
        if (_isPlayerAlive == false)
        return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, _checkRange, boostMask);
        if (hit.collider != null)
        {
            // Проверяем тег
            if (hit.collider.CompareTag("Boost"))
            {
                _boostDetected = true;

                GameObject hitObject = hit.collider.gameObject;

                if (transform.position.y >= 7f)
                    return;
                if (_aggressiveEnemy == true)
                    return;

                if (Time.time > _enemyCanFireBoost)
                {
                    _enemyCanFireBoost = _enemyFireBoostRate + Time.time;

                    Vector3 _laserPosition = transform.position;

                    GameObject enemyLaser = Instantiate(_enemyLaser, _laserPosition, Quaternion.identity);
                    Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

                    if (lasers.Length > 0)
                    {
                        foreach (Laser laser in lasers)
                        {
                            //Send Laser status to Laser Script
                            //laser.AssignEnemyLaser(_playerBehind);
                            laser.BoostDetected(_boostDetected);
                        }
                    }
                }
            }
        }
        else
        {
            _boostDetected = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, Vector2.down * _checkRange);
    }

    //Avoid Shot By Enemy
    private void EnemyAvoidShot()
    {
        LayerMask _laserLayer = LayerMask.GetMask("Laser");

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2.5f, _laserLayer);

        foreach (var hit in hits)
        {
            Laser _laser = hit.GetComponent<Laser>();

            if (_laser != null && _laser.GetOwner() != null)
            {
                if (_laser.GetOwner().CompareTag("Player"))
                {
                    Vector3 laserPos = _laser.transform.position;

                    float horizontal = laserPos.x - transform.position.x;

                    if (Mathf.Abs(horizontal) < 0.1f)
                    {
                        _moveDirection = (Random.value > 0.5f ? Vector3.right : Vector3.left);
                    }
                    else
                    {
                        _moveDirection = horizontal < 0 ? Vector3.right : Vector3.left;
                    }

                    _isAvoidShot = true;

                    _avoidTimer = _avoidDuration;

                    AvoidActivate();

                    break;
                }
            }
        }
    }

    public void AvoidActivate()
    {
        //If Laser Detected
        if (_isAvoidShot)
        {
            //Set new position to move
            transform.Translate(_moveDirection* _enemySpeed * Time.deltaTime * _avoidDistance);
            _avoidTimer -= Time.deltaTime;

            if (_avoidTimer <= 0f)
            {
            _isAvoidShot = false;
            }

             return;
        }
        else
        {
            EnemyAvoidShot();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2.5f);
    }

    //Check if Player behind Enemy
    public void PlayerBehindEnemy()
    {
        if (_isPlayerAlive == false)
        return;

        if (transform.position.y <= -3f)
        return;

        Vector3 directionToPlayer = (_player.transform.position - transform.position).normalized;
        float dot = Vector3.Dot(directionToPlayer, -transform.up);
        if (dot < -0.9f)
        {
            _playerBehind = true;
        }
        else
        {
            _playerBehind = false;
        }
    }

    //Destroy Enemy when Granade Explode
    public void EnemyGranadeDestroy()
    {
        StartCoroutine(EnemyDestoyByGranadeRoutine());
    }

    IEnumerator EnemyDestoyByGranadeRoutine()
    {
        yield return new WaitForSeconds(2.5f);
        _enemyAnim.SetTrigger("OnEnemyDeath");
        _explosionSound.ExplosionAudio();
        _spawnManager.EnemyKilled();
        Destroy(GetComponent<Collider2D>());
        _isEnemyAlive = true;
        Destroy(this.gameObject, _waitTime);
    }

    //Enemy trigger with other object
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            //Check if Enemy has shield
            if (_isEnemyShieildActive == true)
            {
                _isEnemyShieildActive = false;
                _enemyShield.SetActive(false);
                _enemyShieldRandom = Random.Range(1, 5);
            }

            _player.PlayerHealthDamage();
            _player.PlayerScore(_score);
            _enemyAnim.SetTrigger("OnEnemyDeath");
            _explosionSound.ExplosionAudio();
            _spawnManager.EnemyKilled();
            Destroy(GetComponent<Collider2D>());
            _isEnemyAlive = true;
            _enemyBeam.gameObject.SetActive(false);
            Destroy(this.gameObject, _waitTime);
        }

        if (other.tag == "Laser")
        {
            Debug.Log("sdfsfdsf");

            Laser laser = other.GetComponent<Laser>();
            if (laser == null)
            return;
            //Check if this Enemy Fire laser from Laser script
            if (laser.IsEnemyLaser())
            return;

            if (_isEnemyShieildActive == true)
            {
                _isEnemyShieildActive = false;
                _enemyShield.SetActive(false);
                _enemyShieldRandom = Random.Range(1, 5);

                return;
            }

            _player.PlayerScore(_score);
            _enemyAnim.SetTrigger("OnEnemyDeath");
            _explosionSound.ExplosionAudio();
            _spawnManager.EnemyKilled();
            Destroy(GetComponent<Collider2D>());
            _isEnemyAlive = true;
            _enemyBeam.gameObject.SetActive(false);
            Destroy(this.gameObject, _waitTime);
            Destroy(other.gameObject);
        }

        if (other.tag == "Missile")
        {
            if (_isEnemyShieildActive == true)
            {
                _isEnemyShieildActive = false;
                _enemyShield.SetActive(false);
                _enemyShieldRandom = Random.Range(1, 5);
            }

            _player.PlayerScore(_score);
            _enemyAnim.SetTrigger("OnEnemyDeath");
            _explosionSound.ExplosionAudio();
            _spawnManager.EnemyKilled();
            Destroy(GetComponent<Collider2D>());
            _isEnemyAlive = true;
            _enemyBeam.gameObject.SetActive(false);
            Destroy(this.gameObject, _waitTime);
            Destroy(other.gameObject);
        }
    }
}
