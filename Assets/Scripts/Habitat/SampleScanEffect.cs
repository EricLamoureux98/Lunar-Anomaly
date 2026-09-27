using System;
using System.Collections;
using UnityEngine;

public class SampleScanEffect : MonoBehaviour
{
    [SerializeField] Renderer sampleRenderer;
    [SerializeField] GameObject sampleObject;

    public void ChangeColor(float changeDuration)
    {
        StartCoroutine(ChangeColorRoutine(changeDuration));
    }

    IEnumerator ChangeColorRoutine(float duration)
    {
        float elapsed = 0f;
        Vector3 startingScale = sampleObject.transform.localScale;
        Color off = Color.cyan * 0f;
        Color on = Color.cyan * 2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;                    
                    
            sampleRenderer.material.SetColor("_EmissionColor", Color.Lerp(off, on, t));
            sampleObject.transform.localScale = Vector3.Lerp(startingScale, startingScale * 1.1f, t);
            yield return null;
        }
    }
}
