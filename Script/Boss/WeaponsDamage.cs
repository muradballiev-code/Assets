using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponsDamage : MonoBehaviour
{
    [SerializeField]
    private int _weaponsID;
    [SerializeField]
    private int _laserBeamTurretHealth = 10;
    [SerializeField]
    private int _rightFighterCanonHealth = 5;
    [SerializeField]
    private int _leftFighterCanonHealth = 5;
    [SerializeField]
    private int _rightTurret = 7;
    [SerializeField]
    private int _leftTurret = 7;
    [SerializeField]
    private int _countWeapons = 5;
    [SerializeField]
    private int _bossLiveLevel;
    private int _bossLiveSlide = 1;


    [SerializeField]
    private GameObject _weaponsExplosionPrefab;
    [SerializeField]
    private ExplosionSound _explosionSound;

    private Animator _anim;

    private Boss _boss;

    void Start()
    {
        _anim =GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.Log("Animator component not found");
        }

        _boss = GameObject.Find("Main_Boss").GetComponent<Boss>();
        if (_boss == null)
        {
            Debug.Log("Main_Boss component not found");
        }
    }

    public void LaserBeamTurret()
    {
        _laserBeamTurretHealth--;

        if (_laserBeamTurretHealth <= 0)
        {
            _boss.BossDeath();
            Instantiate(_weaponsExplosionPrefab, transform.position, Quaternion.identity);

            _explosionSound.ExplosionAudio();

            gameObject.SetActive(false);
            //Destroy(this.gameObject);
        }
    }

    public void RightMissileTurret()
    {
        

        if (_rightFighterCanonHealth <= 0)
        {
            _boss.BossDeath();

            Instantiate(_weaponsExplosionPrefab, transform.position, Quaternion.identity);

            _explosionSound.ExplosionAudio();

            gameObject.SetActive(false);
            //Destroy(this.gameObject);
        }
    }

    public void LeftMissileTurret()
    {
        _rightFighterCanonHealth--;

        if (_leftFighterCanonHealth <= 0)
        {
            _boss.BossDeath();

            Instantiate(_weaponsExplosionPrefab, transform.position, Quaternion.identity);

            _explosionSound.ExplosionAudio();

            gameObject.SetActive(false);
            //Destroy(this.gameObject);
        }
    }
    public void RightTurret()
    {
        _rightTurret--;

        if (_rightTurret <= 0)
        {
            _boss.BossDeath();

            Instantiate(_weaponsExplosionPrefab, transform.position, Quaternion.identity);

            _explosionSound.ExplosionAudio();

            gameObject.SetActive(false);
            //Destroy(this.gameObject);
        }
    }
    public void LeftTurret()
    {
        _leftTurret--;
        if (_leftTurret <= 0)
        {
            _boss.BossDeath();

            Instantiate(_weaponsExplosionPrefab, transform.position, Quaternion.identity);

            _explosionSound.ExplosionAudio();

            gameObject.SetActive(false);
            //Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            if (_boss.GetBossStartMoving() == false)
            return;

            switch (_weaponsID)
            {
                case 0:
                    _anim.SetTrigger("LaserBeamTuret");
                    Destroy(other.gameObject);
                    LaserBeamTurret();
                break;
                case 1:
                    _anim.SetTrigger("RightMissileWeapon");
                    Destroy(other.gameObject);
                    RightMissileTurret();
                break;
                case 2:
                    _anim.SetTrigger("LeftMissileWeapon");
                    Destroy(other.gameObject);
                    LeftMissileTurret();
                break;
                case 3:
                    _anim.SetTrigger("RightTurret");
                    Destroy(other.gameObject);
                    RightTurret();
                break;
                case 4:
                    _anim.SetTrigger("LeftTurret");
                    Destroy(other.gameObject);
                    LeftTurret();
                break;
                default:
                break;
            }
        }
    }
}
