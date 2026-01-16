using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    [SerializeField]
    private GameObject _laserLeftPrefab;
    [SerializeField]
    private GameObject _laserRightPrefab;
    [SerializeField]
    private GameObject _missileLeftPrefab;
    [SerializeField]
    private GameObject _missileRightPrefab;

    [SerializeField]
    private GameObject[] _weapons;

    private float _enemyCanFireLaser = 5f;
    private float _laserFireRate = 1.5f;

    private float _enemyCanFireLaserRight = 5f;
    private float _laserRightFireRate = 1.5f;

    private float _enemyCanFireRightMissile = 5f;
    private float _missileRightFireRate = 5f;

    private float _enemyCanFireLeftMissile = 5f;
    private float _missileLeftFireRate = 5f;

    private bool _laserBeam = true;
    private bool _laserBeamActive = false;
    private bool _laserBeamActiveStatus = false;

    private Coroutine _laserRoutine;

    private Boss _boss;

    private UIManager _uiManager;
    
    // Start is called before the first frame update
    void Start()
    {
        _boss = GameObject.Find("Main_Boss").GetComponent<Boss>();
        if (_boss == null)
        {
            Debug.Log("Boss component not found");
        }

        //Call UI_Manager component to show Boss live
        _uiManager = GameObject.Find("UI_Manager").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.Log("_uiManager component not found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Check when Boss stop and get moving
        if (_boss.GetBossStartMoving() == true)
        {
            StartFiring();
            LaserBeamTurretCheck();
            _uiManager.BossSliderActive();

            if (_laserRoutine == null)
            {
                _laserRoutine = StartCoroutine(LaserBeamRoutine());
            }
        }
    }

    public void StartFiring()
    {
        if (_weapons.Length > 0)
        {
            foreach (GameObject weapon in _weapons)
            {
                if (weapon == null)
                continue;

                if (weapon.CompareTag("LaserBeamTurret"))
                {
                    LaserBeamActivate laserBeam = weapon.GetComponent<LaserBeamActivate>();
                    laserBeam.LaserBeamSetActive(_laserBeamActiveStatus);
                }

                if (weapon.CompareTag("RightCanon"))
                {
                    if (_laserBeamActive == true)
                    return;

                    if (Time.time > _enemyCanFireRightMissile)
                    {
                        _enemyCanFireRightMissile = _missileRightFireRate + Time.time;

                        Instantiate(_missileRightPrefab, transform.position, Quaternion.identity);
                    }
                }

                if (weapon.CompareTag("LeftCanon"))
                {
                    if (_laserBeamActive == true)
                    return;

                    if (Time.time > _enemyCanFireLeftMissile)
                    {
                        _enemyCanFireLeftMissile = _missileLeftFireRate + Time.time;

                        Instantiate(_missileLeftPrefab, transform.position, Quaternion.identity);
                    }
                }

                if (weapon.CompareTag("RightTurret"))
                {
                    if (_laserBeamActive == true)
                    return;

                    if (Time.time > _enemyCanFireLaserRight)
                    {
                        _enemyCanFireLaserRight = _laserRightFireRate + Time.time;

                        Instantiate(_laserRightPrefab, transform.position, Quaternion.identity);
                    }
                }

                if (weapon.CompareTag("LeftTurret"))
                {
                    if (_laserBeamActive == true)
                    return;

                    if (Time.time > _enemyCanFireLaser)
                    {
                        _enemyCanFireLaser = _laserFireRate + Time.time;

                        Instantiate(_laserLeftPrefab, transform.position, Quaternion.identity);
                    }
                }
            }

        }
    }

    public void LaserBeamTurretCheck()
    {
        LaserBeamActivate laserBeamObject = GetComponentInChildren<LaserBeamActivate>(true);

        if (!laserBeamObject.gameObject.activeInHierarchy)
        {
            _laserBeam = false;
        }
    }

    IEnumerator LaserBeamRoutine()
    {
        yield return new WaitForSeconds(10f);

        while (_laserBeam == true)
        {
            _laserBeamActive = true;
            _laserBeamActiveStatus = true;
            yield return new WaitForSeconds(5f);

            _laserBeamActive = false;
            _laserBeamActiveStatus = false;
            yield return new WaitForSeconds(10f);
        }
    }
}
