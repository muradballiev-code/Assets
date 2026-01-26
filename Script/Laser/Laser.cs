using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class Laser : MonoBehaviour
{
    private float _laserSpeed = 8.0f;

    private bool _isEnemyLaser = false;
    private bool _playerBehind = false;
    private bool _boost = false;

    private LaserRoot _root;

    private GameObject _owner;

    // Start is called before the first frame update
    void Start()
    {
        _root = GetComponentInParent<LaserRoot>();
    }

    // Update is called once per frame
    void Update()
    {
        //Check if Enemy or Player shot AND or Player behind Enemy
        if (_isEnemyLaser == false || _playerBehind == true)
        {
            MoveUp();
        }
        else
        {
            MoveDown();
        }
    }

    //Laser move up
    public void MoveUp()
    {
        transform.Translate(Vector3.up * _laserSpeed * Time.deltaTime);

        if (transform.position.y > 8f)
        {
            //Check if GameObject has a parent. If YES
            if (transform.parent != null)
            {
                //Destory his parent
                Destroy(transform.parent.gameObject);
            }

            //Destory laser
            Destroy(this.gameObject);
        }
    }

    //Laser move down
    public void MoveDown()
    {
        //Set Laser direction by Y
        transform.Translate(Vector3.down * _laserSpeed * Time.deltaTime);

        //Check if Laser above Y point
        if (transform.position.y < -4.5f)
        {
            //Check if GameObject has a parent. If YES
            if (transform.parent != null)
            {
                //Destory his parent
                Destroy(transform.parent.gameObject);
            }

            //Destroy Laser object after
            Destroy(this.gameObject);
        }
    }

    //Set Who Shoting from Player script to avoid shot in Enemy script
    public void SetOwner(GameObject owner)
    {
        _owner = owner;
    }

    //Get Who Shoting to avoid shot in Enemy script
    public GameObject GetOwner()
    {
        return _owner;
    }

    //Get Laser status and Player position from Enemy Script
    public void AssignEnemyLaser(bool playerBehind)
    {
        _isEnemyLaser = true;
        _playerBehind = playerBehind;
    }

    public void BoostDetected(bool _boostDetected)
    {
        _isEnemyLaser = true;
        _boost = _boostDetected;
    }

    //Get this method in Enemy script
    public bool IsEnemyLaser()
    {
        return _isEnemyLaser;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isEnemyLaser == true)
        {
            //Check if Player get 2 or 1 damage
            if (_root.hitProcessed)
            return;

            _root.hitProcessed = true;

            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.PlayerHealthDamage();

                 //Check if GameObject has a parent. If YES
                if (transform.parent != null)
                {
                    //Destory his parent
                    Destroy(transform.parent.gameObject);
                }

                //Destroy Laser object
                Destroy(this.gameObject);
            }
        }

        if (other.tag == "Boost" && _boost == true)
        {
            //Check if Player get 2 or 1 damage
            if (_root.hitProcessed)
            return;

            _root.hitProcessed = true;

            Destroy(other.gameObject);

            //Check if GameObject has a parent. If YES
            if (transform.parent != null)
            {
                //Destory his parent
                Destroy(transform.parent.gameObject);
            }

            //Destroy Laser object
            Destroy(this.gameObject);
        }
    }
}
