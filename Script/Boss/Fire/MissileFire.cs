using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileFire : MonoBehaviour
{
    private float _missileSpeed = 9.0f;

    private void Update()
    {
        //Set Laser direction by Y
        transform.Translate(Vector2.down * _missileSpeed * Time.deltaTime, Space.World);

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
