using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public bool start;
    public AnimationCurve curve;
    public float duration = 1f;


    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            start = false;
            StartCoroutine(Shaking(1));
        }
    }

    public void ShakeOnce(float shakeFactor) 
    {
        StartCoroutine(Shaking(shakeFactor));
    }
    
    IEnumerator Shaking(float factor)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.position = startPosition + Random.insideUnitSphere * (strength * factor);
            yield return null;
        }

        transform.position = startPosition;
    }
}
