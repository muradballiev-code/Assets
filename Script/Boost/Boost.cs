using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Boost : MonoBehaviour
{
    [Header("Boost")]
    [SerializeField]
    private float _boostSpeed;
    [SerializeField]
    private int _boostID;
    [SerializeField]
    private AudioClip _clip;

    private bool _moveTowardsPlayer = false;
    private bool _playerDead = false;
    private Player _player;

    // Start is called before the first frame update
    void Start()
    {
        _boostSpeed = Random.Range(3f, 4f);

        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.Log("Player component not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_player == null)
        {
            _playerDead = true;
        }


        if (_moveTowardsPlayer == false)
        {
            //Move Down
            transform.Translate(Vector3.down * _boostSpeed * Time.deltaTime);
        }
        else if (_moveTowardsPlayer == true)
        {
            //Move Towards to player
            transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, _boostSpeed * 2.0f * Time.deltaTime);
        }

        if (transform.position.y < -6f)
        {
            Destroy(this.gameObject);

            if (_playerDead == true)
            return;
        }
    }

    //Move Towards Player activate  when "C" pushed
    public void MoveTowardsPlayerEnable()
    {
        _moveTowardsPlayer = true;
    }

    //Move Towards Player deactivate  when "C" off
    public void MoveTowardsPlayerDisable()
    {
        _moveTowardsPlayer = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            AudioSource.PlayClipAtPoint(_clip, transform.position);

            switch (_boostID)
            {
                case 0:
                    _player.PlayerShieldBoostdActivate();
                    break;
                case 1:
                    _player.PlayerSpeedBoostActivate();
                    break;
                case 2:
                    _player.PlayerTripleShotBoostActivate();
                    break;
                case 3:
                    _player.PlayerAmmoBoostActivate();
                    break;
                case 4:
                    _player.PlayerHealthBoostActivate();
                    break;
                case 5:
                    _player.NegativeBoostActivate();
                    break;
                default:
                    break;
            }

            Destroy(this.gameObject);
        }
    }
}
