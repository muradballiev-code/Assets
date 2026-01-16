using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeamActivate : MonoBehaviour
{
    private LaserBeam _laserBeam;
    private Animator _anim;

    // Start is called before the first frame update
    void Start()
    {
        _laserBeam = GetComponentInChildren<LaserBeam>(true);
        //GameObject _laserBeamGO = GetComponentInChildren<LaserBeam>(true).gameObject;
        _anim = GetComponent<Animator>();
    }

    public void LaserBeamSetActive(bool _laserBeamActiveStatus)
    {
        if (_laserBeamActiveStatus == true)
        {
            _anim.SetBool("LaserBeam", true);
        }
        else
        {
            _anim.SetBool("LaserBeam", false);
            _laserBeam.gameObject.SetActive(false);
        }
    }

    public void LaserBeamGetActive()
    {
        _laserBeam.gameObject.SetActive(true);
    }
}
