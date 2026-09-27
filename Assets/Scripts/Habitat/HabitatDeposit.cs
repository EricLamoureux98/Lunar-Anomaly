using System;
using System.Collections;
using LunarAnomaly;
using LunarAnomaly.Gameplay;
using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;

public class HabitatDeposit : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ParticleSystem destructionParticlePrefab;
    [SerializeField] MiningManager miningManager;
    [SerializeField] SampleScanEffect sampleScanEffect;
    HabitatDepositScan depositScan;

    [SerializeField] Animator depositAnim;

    [SerializeField] GameObject placedSample;    

    [Header("Deposit Text")]
    [SerializeField] TMP_Text depositText;
    string startingDepositText;
    Color startingColour;

    [Header("Analyser")]
    [SerializeField] Renderer analyserIndicatorL;
    [SerializeField] Renderer analyserIndicatorR;
    [SerializeField] GameObject analyserLightL;
    [SerializeField] GameObject analyserLightR;
    [SerializeField] GameObject analyserSampleL;
    [SerializeField] GameObject analyserSampleR;

    [Header("Deposit Timings")]
    [SerializeField] float doorOpenDelay = 1f;
    [SerializeField] float samplePlaceDelay = 1f;
    [SerializeField] float sampleScanStartDelay = 0.5f;
    [SerializeField] float sampleScanTime = 2f;
    [SerializeField] float sampleGlowEffectTiming = 2.5f;


    Coroutine depositRoutine;
    Coroutine depositTextRoutine;

    // To MiningManager
    public static event Action OnSamplesDeposited;

    void Awake()
    {
        depositScan = GetComponent<HabitatDepositScan>();
    }

    void Start()
    {
        startingDepositText = depositText.text;
        startingColour = depositText.color;
    }

    void OnEnable()
    {
        HabitatController.OnDepositSamples += DepositSample;
    }

    void OnDisable()
    {
        HabitatController.OnDepositSamples -= DepositSample;
    }    

    void DepositSample()
    {
        if (depositRoutine != null) return;
        if (depositTextRoutine != null) return;

        if (miningManager.CarriedSamples <= 0)
        {
            depositTextRoutine = StartCoroutine(DepositTextRoutine());
            return;
        }       

        depositRoutine = StartCoroutine(DepositSampleRoutine());
    }

    IEnumerator DepositTextRoutine()
    {
        //if (samplesHeld) yield break;
        
        depositText.color = Color.gray6;
        depositText.text = "No Samples";

        yield return new WaitForSeconds(1f);

        depositText.color = startingColour;
        depositText.text = startingDepositText;  

        depositTextRoutine = null;      
    }

    IEnumerator DepositSampleRoutine()
    {
        // Open Door
        depositText.color = Color.gray6;
        depositText.text = "Please Wait";
        depositAnim.SetBool("IsOpen", true);
        SoundManager.PlaySound(SoundType.DoorClick);

        yield return new WaitForSeconds(doorOpenDelay);

        // Place Sample
        SoundManager.PlaySound(SoundType.ItemPlace);
        placedSample.SetActive(true);
        yield return new WaitForSeconds(0.75f);

        // Close Door
        SoundManager.PlaySound(SoundType.Suction);
        yield return new WaitForSeconds(0.75f);
        depositAnim.SetBool("IsOpen", false);
        yield return new WaitForSeconds(samplePlaceDelay);

        // Scan Sample
        depositScan.laserActive = true;
        SoundManager.PlaySound(SoundType.LaserScan, 0.8f);
        yield return new WaitForSeconds(sampleScanStartDelay);
        depositAnim.SetBool("IsScanning", true);
        // sampleScanEffect.ChangeColor(sampleGlowEffectTiming);
        sampleScanEffect.ChangeColor(sampleScanTime);
        yield return new WaitForSeconds(sampleScanTime);
        SoundManager.PlaySound(SoundType.RockBreak, 0.3f);
        placedSample.SetActive(false);
        Instantiate(destructionParticlePrefab, placedSample.transform.position, Quaternion.identity);
        depositAnim.SetBool("IsScanning", false);
        depositScan.laserActive = false;

        yield return new WaitForSeconds(1.5f);

        EnableAnalyser();
        OnSamplesDeposited?.Invoke();
        depositText.color = startingColour;
        depositText.text = "Deposit Complete";

        yield return new WaitForSeconds(5f);
        depositText.text = startingDepositText;

        depositRoutine = null;
    }

    void EnableAnalyser()
    {
        if (miningManager.CarriedSamples <= 0) return;

        SetAnalyserLight(false);

        if (miningManager.DepositedSamples > 0 && miningManager.CarriedSamples > 0)
        {
            SetAnalyserLight(true);
        }
            
        // if (miningManager.CarriedSamples == 1)
        // {
        //     SetAnalyserLight(false);
        // }
        // else if (miningManager.CarriedSamples > 1)
        // {
        //     SetAnalyserLight(true);
        // }
    }

    void SetAnalyserLight(bool both)
    {
        if (both)
        {
            analyserIndicatorR.material.SetColor("_BaseColor", Color.green);
		    analyserIndicatorR.material.SetColor("_EmissionColor", Color.green);
            analyserLightR.SetActive(true);
            analyserSampleR.SetActive(true);
        }

        analyserIndicatorL.material.SetColor("_BaseColor", Color.green);
		analyserIndicatorL.material.SetColor("_EmissionColor", Color.green);
        analyserLightL.SetActive(true);
        analyserSampleL.SetActive(true);
    }
}



/*
- Check if player is carrying samples
- Update deposit text to "Standby"
- Door open
- Sample appear in deposit box
- Door close
- Animate rock deconstruction
- Enable analyser and sample
- Update text on screen 
*/