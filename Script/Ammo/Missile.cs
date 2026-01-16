using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    private float _missileSpeed = 8f;
    private float _distance = 3f;

    private float collectRange = 5f;
    [SerializeField]
    private LayerMask _enemyLayer;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * _missileSpeed * Time.deltaTime);

        Collider2D[] _enemy = Physics2D.OverlapCircleAll(transform.position, collectRange, _enemyLayer);

        if (_enemy.Length > 0)
        {
            for (int i = 0; i < _enemy.Length; i++)
            {
                float distance = Vector3.Distance(transform.position, _enemy[i].transform.position);

                if (distance < _distance)
                {
                    transform.position = Vector3.MoveTowards(transform.position, _enemy[i].transform.position, _missileSpeed * Time.deltaTime);

                    Vector3 direction = (_enemy[i].transform.position - transform.position).normalized;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    float offset = -90f;

                    transform.rotation = Quaternion.Euler(Vector3.forward * (angle + offset));
                }
            }
        }

        if (transform.position.y > 8f)
        {
            Destroy(this.gameObject);
        }
    }
}
