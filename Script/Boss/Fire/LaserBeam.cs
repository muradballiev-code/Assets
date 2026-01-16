using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.PlayerDeath();
            }
        }
    }
}
