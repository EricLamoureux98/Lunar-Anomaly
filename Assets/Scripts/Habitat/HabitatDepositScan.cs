using LunarAnomaly;
using UnityEngine;

public class HabitatDepositScan : MonoBehaviour
{
    [SerializeField] LightFlicker lightFlicker;
    [SerializeField] LineRenderer lineRendererL;
    [SerializeField] LineRenderer lineRendererR;
    [SerializeField] Light laserLight;

    [Header("Scan Settings")]
    [SerializeField] float lineWidth = 0.02f;
    [SerializeField] float flashIntensity = 8f;

    [Header("Targets")]
    [SerializeField] Transform laserLStart;
    [SerializeField] Transform laserRStart;
    [SerializeField] Transform targetPoint;

    public bool laserActive = false;

    void Update()
    {
        if (laserActive)
        {
            StartLaser();
            
        }
        else
        {
            StopLaser();            
        }
    }

    void PrepareLine()
    {
        lineRendererL.startWidth = lineWidth;
        lineRendererL.endWidth = lineWidth;

        lineRendererR.startWidth = lineWidth;
        lineRendererR.endWidth = lineWidth;

        lineRendererL.positionCount = 2;
        lineRendererR.positionCount = 2;
    }

    void StartLaser()
    {
        if (laserLStart == null || laserRStart == null || targetPoint == null) return;

        PrepareLine();        

        lineRendererL.SetPosition(0, laserLStart.position);
        lineRendererL.SetPosition(1, targetPoint.position);

        lineRendererR.SetPosition(0, laserRStart.position);
        lineRendererR.SetPosition(1, targetPoint.position);     

        lineRendererL.enabled = true;
        lineRendererR.enabled = true;

        laserLight.enabled = true;
        laserLight.intensity = flashIntensity;   

        lightFlicker.StartFlicker(0.2f);
    }
    
    void StopLaser()
    {
        if (laserLStart == null || laserRStart == null || targetPoint == null) return;

            laserLight.enabled = false;

            lineRendererL.enabled = false;
            lineRendererR.enabled = false;

            lineRendererL.positionCount = 2;
            lineRendererR.positionCount = 2;
    }
}
