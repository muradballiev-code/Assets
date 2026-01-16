using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserFire : MonoBehaviour
{
    private float _laserSpeed = 8.0f;

    private LaserBossRoot _root;

    private void Start()
    {
        _root = GetComponentInParent<LaserBossRoot>();
    }

    // Update is called once per frame
    void Update()
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

            //Destroy Laser object after 0.5f
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
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
    }
}
