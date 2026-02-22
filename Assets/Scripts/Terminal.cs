using System;
using UnityEngine;

public class Terminal : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] MiningManager miningManager;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] Animator anim;

    [Header("Rock Samples")]
    [SerializeField] int samplesRequired;
    
    // public for testing
    public int samplesDelivered;
    public bool sampleObjectiveComplete;
    public bool terminalActive; 
    public bool playerDepositing;

    bool wasDepositingLastFrame;

    // To UIManager
    public static event Action<bool> OnTerminalInteract;

    void Update()
    {
        ReadInput();
        HandleDepositInput();
        wasDepositingLastFrame = playerDepositing;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        OnTerminalInteract?.Invoke(true);
        terminalActive = true;
        anim.SetBool("isActive", true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        OnTerminalInteract?.Invoke(false);
        terminalActive = false;
        anim.SetBool("isActive", false);
    }

    void HandleDepositInput()
    {
        if (terminalActive && playerDepositing && !wasDepositingLastFrame) 
        {
            DepositSamples();
        }
    }

    void DepositSamples()
    {
        if (!terminalActive) return;
        if (sampleObjectiveComplete) return;

        int samples = miningManager.samplesCollected;
        
        if (samples <= 0) return;

        AddDeliveredSamples(samples);
        miningManager.ClearSamples(); // <--- Consider not clearing all samples later
    }

    void AddDeliveredSamples(int amount)
    {
        samplesDelivered += amount;

        Debug.Log($"Samples: {samplesDelivered} / {samplesRequired}");

        if (samplesDelivered >= samplesRequired)
        {
            CompleteObjectives();
        }
    }

    void CompleteObjectives()
    {
        sampleObjectiveComplete = true;
        Debug.Log("Sample objective complete!");
    }

    void ReadInput()
    {
        playerDepositing = playerInput.SystemInteractPressed;
    }

    void OnDrawGizmos()
    {
        SphereCollider terminalCollider = GetComponent<SphereCollider>();
        float worldRadius = terminalCollider.radius * terminalCollider.transform.lossyScale.x;

        if (terminalCollider == null) return;

        Gizmos.color = terminalActive ? Color.green : Color.red;

        Gizmos.DrawWireSphere(terminalCollider.transform.position, worldRadius);
    }
}
