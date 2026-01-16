using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Player score")]
    [SerializeField]
    private Text _playerScore;

    [Header("Game Over")]
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartGameText;

    [Header("Ammo")]
    [SerializeField]
    private Text _ammoText;

    [Header("Grenade & Missile Value")]
    [SerializeField]
    private Text _grenadeValue;
    [SerializeField]
    private Text _missileValue;

    [Header("Shield")]
    [SerializeField]
    private Text _shieldValue;

    [Header("Wave Value")]
    [SerializeField]
    private Text _waveText;

    [Header("Live Image Value")]
    [SerializeField]
    private Image _playerLiveDisplayImage;
    [SerializeField]
    private Sprite[] _liveImgChange;

    [Header("Button")]
    [SerializeField]
    private Button _mainMenuButton;

    [Header("Slider Value")]
    [SerializeField]
    private Slider _thrusterSlider;

    [Header("Boss Slider")]
    [SerializeField]
    private Slider _bossSlider;

    private SpawnManager _spawnManager;
    private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        _playerScore.text = "Score: " + 0;
        _shieldValue.text = "Shield: " + 0;

        _gameOverText.gameObject.SetActive(false);
        _restartGameText.gameObject.SetActive(false);

        _mainMenuButton.gameObject.SetActive(false);

        _bossSlider.gameObject.SetActive(false);

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.Log("SpawnManager component not found");
        }

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.Log("GameManager is null, can't find component GameManager");
        }
    }

    //Set Score to Player, using from Player Script
    public void UpdatePlayerScore(int score)
    {
        _playerScore.text = "Score: " + score.ToString();
    }

    //Get Score to Player and update, using from Player Script
    public void AmmoCount(int fireLimit, int fireLimitMax)
    {
        _ammoText.text = "Ammo: " + fireLimit + "/" + fireLimitMax;
    }

    //Get Shield count from Player and update, using from Player Script
    public void UpdateShieldCount(int shieldValue)
    {
        if (shieldValue >= 0 && shieldValue <= 3)
        {
            _shieldValue.text = "Shield: " + shieldValue;
        }
    }

    public void UpdatePlayerLive(int currentlive)
    {
        if (currentlive < 0 || currentlive >= _liveImgChange.Length)
        {
            return;
        }

        _playerLiveDisplayImage.sprite = _liveImgChange[currentlive];

        if (currentlive == 0)
        {
            GameOver();
        }
    }

    //Get Grenade count from Player and update, using from Player Script
    public void UpdateGrenade(int grenadeValue)
    {
        _grenadeValue.text = ": " + grenadeValue;
    }

    public void UpdateMissile(int missileValue)
    {
        _missileValue.text = ": " + missileValue;
    }

    //Update thruster, using Player Script
    public void UpdateThrustersSlider(float thrusterValue)
    {
        if (thrusterValue >= 0 && thrusterValue <= 10)
        {
            _thrusterSlider.value = thrusterValue;
        }
    }

    //Boss slider ON
    public void BossSliderActive()
    {
        _bossSlider.gameObject.SetActive(true);
    }

    //Update thruster, using Player Script
    public void UpdateBossLiveSlider(float bossValue)
    {
        if (bossValue >= 0 && bossValue <= 5)
        {
            _bossSlider.value = bossValue;
        }
    }

    //Show Wave on Screen and start Game by Coroutine
    public void WaveDiplsayText(int wavenum)
    {
        _waveText.text = "W A V E : " + wavenum;
        _waveText.gameObject.SetActive(true);
        StartCoroutine(WaveStartRoutine());
    }

    IEnumerator WaveStartRoutine()
    {
        yield return new WaitForSeconds(3f);
        _waveText.gameObject.SetActive(false);
        _spawnManager.StartSpawn();
    }

    public void GameOver()
    {
        _gameOverText.gameObject.SetActive(true);
        _restartGameText.gameObject.SetActive(true);
        _mainMenuButton.gameObject.SetActive(true);

        _gameManager.GameOver();

        StartCoroutine(GameOverFlickerRoutine());
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
