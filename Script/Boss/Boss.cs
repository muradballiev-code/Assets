using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private float _bossSpeed = 1.5f;
    [SerializeField]
    private float _leftLimit = -6f;
    [SerializeField]
    private float _rightLimit = 6.2f;
    private float _stopPositionY = 5.5f;
    private float _startPositionY = 15f;
    private bool _isBossStop = false;
    private bool _isBossKilled = false;
    private bool _isBossStartMoving = false;

    private int _countWeapons = 5;

    private UIManager _uiManager;


    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, _startPositionY, 0);

        _uiManager = GameObject.Find("UI_Manager").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.Log("_uiManager component not found");
        }
        _uiManager.UpdateBossLiveSlider(_countWeapons);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isBossStop)
        {
            transform.Translate(Vector3.down * _bossSpeed * Time.deltaTime);

            if (transform.position.y <= _stopPositionY)
            {
                _isBossStop = true;
                transform.position = new Vector3(transform.position.x, _stopPositionY, 0);
                StartCoroutine(MoveBoss());
            }
        }
    }

    public bool GetBossStartMoving()
    {
        return _isBossStartMoving;
    }

    public void BossDeath()
    {
        _countWeapons --;

        Debug.Log(_countWeapons);

        if (_countWeapons <= 0)
        {
            transform.position = new Vector3(transform.position.x, _stopPositionY, 0);

            _isBossKilled = true;
            _isBossStartMoving = false;

            StartCoroutine(BossDestroyRoutine());
        }

        _uiManager.UpdateBossLiveSlider(_countWeapons);
    }

    IEnumerator MoveBoss()
    {
        yield return new WaitForSeconds(5f);

        bool movingLeft = true;
        bool movingRight = false;
        bool movingCenter = false;
        _isBossStartMoving = true;

        while (_isBossKilled == false)
        {
            if (movingLeft == true)
            {
                transform.Translate(Vector3.left * _bossSpeed * Time.deltaTime);
                if (transform.position.x <= _leftLimit)
                {
                    transform.position = new Vector3(_leftLimit, _stopPositionY, 0);
                    yield return new WaitForSeconds(2f);
                    movingLeft = false;
                    movingCenter = false;
                    movingRight = true;
                }
            }
            else if (movingRight == true && movingCenter == false && movingLeft == false)
            {
                transform.Translate(Vector3.right * _bossSpeed * Time.deltaTime);
                if (transform.position.x >= _rightLimit)
                {
                    transform.position = new Vector3(_rightLimit, _stopPositionY, 0);
                    yield return new WaitForSeconds(2f);
                    movingLeft = false;
                    movingCenter = true;
                    movingRight = false;
                }
            }
            else if (movingCenter == true && movingRight == false && movingLeft == false)
            {
                Vector3 target = new Vector3(0f, _stopPositionY, 0);
                transform.position = Vector3.MoveTowards(transform.position, target, _bossSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, target) < 0.01f)
                {
                    transform.position = target;
                    yield return new WaitForSeconds(1.5f);

                    movingLeft = true;
                    movingRight = false;
                    movingCenter = false;
                }

            }
            yield return null;
        }
    }

    IEnumerator BossDestroyRoutine()
    {
        yield return new WaitForSeconds(5f);

        Destroy(this.gameObject);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
        }
    }
}
