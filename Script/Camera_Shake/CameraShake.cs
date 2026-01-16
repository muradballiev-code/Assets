using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    //Variant - 2

    [SerializeField]
    private float shakeFrequency = 1;

    [SerializeField]
    public AnimationCurve _shakeCurve;

    private bool _isShaking;

    public void ShakeCamera()
    {
        StartCoroutine(CameraShakeRoutine());
        if (_isShaking)
        {
            _isShaking = false;
            StopCoroutine(CameraShakeRoutine());
        }
    }

    IEnumerator CameraShakeRoutine()
    {
        _isShaking = true;

        Vector3 _originalCameraPos = transform.position;

        float timeRunnig = 0;

        while (timeRunnig < shakeFrequency)
        {
            timeRunnig += Time.deltaTime;
            float strength = _shakeCurve.Evaluate(timeRunnig / shakeFrequency);
            transform.position = _originalCameraPos + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = _originalCameraPos;
        _isShaking = false;
    }
}
