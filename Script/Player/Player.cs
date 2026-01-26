using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.Experimental.Video;

public class Player : MonoBehaviour
{
    [Header("Player Health")]
    [SerializeField]
    private int _playerHealth = 3;
    private int _playerHealthMax = 3;
    private int _playerScore;

    [Header("Player Move Speed")]
    [SerializeField]
    private int _playerSpeed = 8;
    private int _playerSpeedDefault = 8;
    [SerializeField]
    private int _playerSpeedMultiple = 2;
    private bool _isShiftActive = false;
    private bool _isPlayerSpeedBoostActive = false;

    [Header("Player Ammo")]
    [SerializeField]
    private int _grenade = 1;
    [SerializeField]
    private int _missile = 3;
    [SerializeField]
    private GameObject _grenadePrefab;
    [SerializeField]
    private GameObject _missilePrefab;

    [Header("Player Shield")]
    [SerializeField]
    private int _shieldHealthMax = 3;
    private int _shieldHealth;
    private bool _isShieldActive = false;
    [SerializeField]
    private GameObject _playerShield;
    [SerializeField]
    private SpriteRenderer _shieldDamageColor;

    [Header("Laser")]
    [SerializeField]
    private int _laserShot = 15;
    private int _laserAmmoMax = 15;
    private float _coolDown = -1;
    private float _fireRate = 0.25f;
    private bool _isLaserTripleShotActive = false;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _laserTripleShotPrefab;
    [SerializeField]
    private AudioClip _laserShotClip;
    [SerializeField]
    private AudioClip _laserEmptyClip;
    private AudioSource _laserAudioSource;

    [Header("Thruster Slide")]
    [SerializeField]
    private float _thrusterChargeLevelMax = 10f;
    private float _thrusterChargeLevel;
    private float _thrusterCoolDown = -1f;
    private float _thrusterRate = 0.1f;
    private float _changeThrusterChargeSlide = 1.5f;

    [Header("Player Explosion")]
    [SerializeField]
    private GameObject _playerExplosionPrefab;
    [SerializeField]
    private ExplosionSound _explosionSound;

    [Header("Player Engine Damage")]
    [SerializeField]
    private GameObject _playerLeftEngineDamage;
    [SerializeField]
    private GameObject _playerRightEngineDamage;

    [Header("PickUp Boost components")]
    [SerializeField]
    private LayerMask _powerUpLayer;
    private float _collectRange = 5f;

    private Animator _playerAnim;

    private bool _isNegativeBoostActive = false;

    //Create UIManager variable for UIManager script component
    private UIManager _uiManager;

    //Create Spawn Manager variable for SpawnManager script component
    private SpawnManager _spawnManager;

    [SerializeField]
    private CameraShake _cameraShake;

    // Start is called before the first frame update
    void Start()
    {
        _playerShield.SetActive(false);
        _playerLeftEngineDamage.SetActive(false);
        _playerRightEngineDamage.SetActive(false);
        _thrusterChargeLevel = _thrusterChargeLevelMax;

        _playerAnim = GetComponent<Animator>();

        _laserAudioSource = GetComponent<AudioSource>();
        _laserAudioSource.clip = _laserShotClip;
        
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.Log("SpawnManager component not found");
        }

        _uiManager = GameObject.Find("UI_Manager").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.Log("UIManager component not found");
        }

        _uiManager.AmmoCount(_laserShot, _laserAmmoMax);
        _uiManager.UpdateGrenade(_grenade);
        _uiManager.UpdateMissile(_missile);
    }

    // Update is called once per frame
    void Update()
    {
        //Call PlayerController method
        PlayerController();
        //Call PlayerFireSystem method
        PlayerFireSystem();
        //Call PlayerPickupCollect method
        PlayerPickupCollect();

        //Check if SHIFT hold by Player OR not
        if (_isShiftActive == true && _isNegativeBoostActive == false)
        {
            //If YES then check Thruster charge
            if (_thrusterChargeLevel >= 0)
            {
                PlayerShiftSpeedActivate();
            }
            else if (_thrusterChargeLevel <= 0)
            {
                PlayerShiftSpeedDeactivate();
            }
        }
        else if (_isShiftActive == false && _isNegativeBoostActive == false)
        {
            PlayerShiftSpeedDeactivate();
            if (_thrusterChargeLevel <= 0.0f || _thrusterChargeLevel >= 0 && _thrusterChargeLevel <= _thrusterChargeLevelMax)
            {
                StartCoroutine(ThrusterSlideRoutine());
            }
        }
    }

    //Create PlayerController method to move Player
    public void PlayerController()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        _playerAnim.SetFloat("PlayerTurn", horizontal);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _isShiftActive = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _isShiftActive = false;
        }

        Vector3 direction = new Vector3(horizontal, vertical, 0).normalized;
        transform.Translate(direction * _playerSpeed * Time.deltaTime);

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -9.45f, 9.45f), Mathf.Clamp(transform.position.y, -3.8f, 5.7f), 0);
    }

    //Player Fire method to shot
    public void PlayerFireSystem()
    {
        //Shot Laser
        if (Input.GetMouseButton(0) && Time.time > _coolDown)
        {
            _coolDown = _fireRate + Time.time;

            if (_isLaserTripleShotActive == false)
            {
                if (_laserShot > 0)
                {
                    _laserShot--;
                    GameObject _playerLaser = Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.0f, 0), Quaternion.identity);
                    Laser _laser = _playerLaser.GetComponent<Laser>();
                    _laser.SetOwner(this.gameObject);

                    _uiManager.AmmoCount(_laserShot, _laserAmmoMax);
                }
                else
                {
                    _laserAudioSource.clip = _laserEmptyClip;
                    _laserAudioSource.Play();
                    return;
                }
            }
            else
            {
                Instantiate(_laserTripleShotPrefab, transform.position, Quaternion.identity);
                _laserAudioSource.clip = _laserShotClip;
            }

            _laserAudioSource.Play();
        }

        //Shot Missile
        if (Input.GetMouseButtonDown(1))
        {
            if (_missile > 0)
            {
                _missile--;
                Instantiate(_missilePrefab, transform.position, Quaternion.identity);
                _uiManager.UpdateMissile(_missile);
            }
        }

        //Shot Grenade
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_grenade > 0)
            {
                _grenade--;
                Instantiate(_grenadePrefab, transform.position, Quaternion.identity);
                _uiManager.UpdateGrenade(_grenade);
            }
        }
    }

    //Player Health Damage method
    public void PlayerHealthDamage()
    {
        _cameraShake.ShakeCamera();

        //Check if Shield Active, if YES
        if (_isShieldActive == true)
        {
            _shieldHealth--;

            if (_shieldHealth == 2)
            {
                _shieldDamageColor.color = Color.yellow;
                _uiManager.UpdateShieldCount(_shieldHealth);
            }
            if (_shieldHealth == 1)
            {
                _shieldDamageColor.color = Color.red;
                _uiManager.UpdateShieldCount(_shieldHealth);
            }
            if (_shieldHealth < 1)
            {
                _isShieldActive = false;
                _playerShield.SetActive(false);
                _uiManager.UpdateShieldCount(_shieldHealth);
            }
            return;
        }

        _playerHealth--;

        _uiManager.UpdatePlayerLive(_playerHealth);

        if (_playerHealth == 2)
        {
            _playerLeftEngineDamage.SetActive(true);
        }
        else if (_playerHealth == 1)
        {
            _playerRightEngineDamage.SetActive(true);
        }
        else if (_playerHealth < 1)
        {
            _spawnManager.StopSpawning();
            Instantiate(_playerExplosionPrefab, transform.position, Quaternion.identity);
            _explosionSound.ExplosionAudio();
            Destroy(this.gameObject);
        }
    }

    //Call method when Boss shot Laser Beam
    public void PlayerDeath()
    {
        _playerHealth = 0;
        _spawnManager.StopSpawning();
        Instantiate(_playerExplosionPrefab, transform.position, Quaternion.identity);
        _explosionSound.ExplosionAudio();
        _uiManager.UpdatePlayerLive(_playerHealth);
        Destroy(this.gameObject);
    }

    //Player pick up Boosts when C key pressed
    public void PlayerPickupCollect()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Collider2D[] _powerUps = Physics2D.OverlapCircleAll(transform.position, _collectRange, _powerUpLayer);

            if (_powerUps.Length > 0)
            {
                for (int i = 0; i < _powerUps.Length; i++)
                {
                    Debug.Log(_powerUps[i]);

                    _powerUps[i].GetComponent<Boost>().MoveTowardsPlayerEnable();
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            Collider2D[] _powerUps = Physics2D.OverlapCircleAll(transform.position, _collectRange, _powerUpLayer);

            if (_powerUps.Length > 0)
            {
                for (int i = 0; i < _powerUps.Length; i++)
                {
                    _powerUps[i].GetComponent<Boost>().MoveTowardsPlayerDisable();
                }
            }
        }
    }

    //Player fast Move method when Shift hold
    public void PlayerShiftSpeedActivate()
    {
        if (_isPlayerSpeedBoostActive == false)
        {
            _playerSpeed = _playerSpeedDefault * _playerSpeedMultiple;
            ThrusterSliderActivate();
        }
    }
    //Player default move method when Shift not hold
    public void PlayerShiftSpeedDeactivate()
    {
        if (_isPlayerSpeedBoostActive == false)
        {
            _playerSpeed = _playerSpeedDefault;
        }
    }

    //Player Shift Thruster Slider Activate method
    public void ThrusterSliderActivate()
    {
        _thrusterChargeLevel -= Time.deltaTime * _changeThrusterChargeSlide;
        _uiManager.UpdateThrustersSlider(_thrusterChargeLevel);
    }

    //Shield Activate
    public void PlayerShieldBoostdActivate()
    {
        if (_isNegativeBoostActive == true)
        {
            return;
        }

        _isShieldActive = true;
        _shieldHealth = _shieldHealthMax;
        _playerShield.SetActive(true);
        _shieldDamageColor.color = Color.white;
        _uiManager.UpdateShieldCount(_shieldHealth);
    }

    //Speed Activate
    public void PlayerSpeedBoostActivate()
    {
        if (_isShiftActive == true || _isNegativeBoostActive == true)
        {
            return;
        }

        if (_isPlayerSpeedBoostActive == false)
        {
            _playerSpeed *= _playerSpeedMultiple;
            _isPlayerSpeedBoostActive = true;
            StartCoroutine(SpeedBoostDeactivateRoutine());
        }
    }

    //Laser Triple Shot Activate
    public void PlayerTripleShotBoostActivate()
    {
        if (_isNegativeBoostActive == true)
        {
            return;
        }

        _isLaserTripleShotActive = true;
        StartCoroutine(TripleShootBoostDeactivateRoutine());
    }

    //Player Ammo Activate
    public void PlayerAmmoBoostActivate()
    {
        if (_isNegativeBoostActive == true)
        {
            return;
        }

        if (_laserShot >= 0 || _laserShot < _laserAmmoMax)
        {
            _laserShot = _laserAmmoMax;
            _laserAudioSource.clip = _laserShotClip;
            _uiManager.AmmoCount(_laserShot, _laserAmmoMax);
        }

        if (_missile <= 0)
        {
            _missile = 3;
            _uiManager.UpdateMissile(_missile);
        }

        if (_grenade <= 0)
        {
            _grenade = 1;
            _uiManager.UpdateGrenade(_grenade);
        }
    }

    //Player Health Activate
    public void PlayerHealthBoostActivate()
    {
        _isNegativeBoostActive = false;

        if (_playerSpeed < _playerSpeedDefault)
        {
            _playerSpeed = _playerSpeedDefault;
        }

        if (_playerHealth < _playerHealthMax)
        {
            _playerHealth = _playerHealthMax;

            _playerLeftEngineDamage.SetActive(false);
            _playerRightEngineDamage.SetActive(false);
            _uiManager.UpdatePlayerLive(_playerHealth);
        }
    }

    //Negative Effects Boos Get
    public void NegativeBoostActivate()
    {
        if (_isNegativeBoostActive == false)
        {
            _isNegativeBoostActive = true;
            NegativeEffects();
        }
    }

    //Negative Effects Activate
    public void NegativeEffects()
    {
        _laserShot = Mathf.Abs(_laserShot / 2);
        _playerHealth -= 2;
        _playerSpeed = _playerSpeedDefault / _playerSpeedMultiple;

        if (_isShieldActive == true)
        {
            _isShieldActive = false;
            _shieldHealth = 0;
            _playerShield.SetActive(false);
        }
    }

    //Get Score from Enemy script
    public void PlayerScore(int score)
    {
        _playerScore += score;
        _uiManager.UpdatePlayerScore(_playerScore);
    }

    //Speed DeActivate Coroutine
    IEnumerator SpeedBoostDeactivateRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        _playerSpeed /= _playerSpeedMultiple;
        _isPlayerSpeedBoostActive = false;
    }

    //Triple Shot DeActivate Coroutine
    IEnumerator TripleShootBoostDeactivateRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        _isLaserTripleShotActive = false;
    }

    //Thruster Charge Activate Coroutine
    IEnumerator ThrusterSlideRoutine()
    {
        yield return new WaitForSeconds(5f);

        while ((_thrusterChargeLevel <= _thrusterChargeLevelMax) && Time.time > _thrusterCoolDown)
        {
            _thrusterCoolDown = Time.time + _thrusterRate;

            _thrusterChargeLevel += Time.deltaTime * _changeThrusterChargeSlide;
            _uiManager.UpdateThrustersSlider(_thrusterChargeLevel);

            yield return new WaitForEndOfFrame();
        }
    }
}
