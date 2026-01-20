using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    private float _waitTime = 2.5f;
    [SerializeField]
    private Animator _grenadeAnim;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            _grenadeAnim.SetTrigger("GrenadeTrigger");
            other.gameObject.GetComponent<Enemy>().EnemyGranadeDestroy();
            StartCoroutine(BombExplosionRoutine());
        }
    }

    IEnumerator BombExplosionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_waitTime);
            Destroy(this.gameObject);
        }
    }
}
