using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour
{
    [Header("Refernces")]
    [SerializeField] Airlock airlock;
    Oxygen oxygen;
    PlayerMovement playerMovement;

    [Header("UI")]
    [SerializeField] Image fadeToBlack; // This should eventually be moved to its own script

    [SerializeField] Transform respawnPoint;
    [SerializeField] float oxygenGracePeriod = 3f;

    CurrentState currentState;
    float graceTimer;

    void Awake()
    {
        oxygen = GetComponent<Oxygen>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void OnEnable()
    {
        Oxygen.OnOxygenDepleted += OnOxygenDepleted;
    }

    void OnDisable()
    {
        Oxygen.OnOxygenDepleted -= OnOxygenDepleted;
    }

    void Update()
    {
        switch (currentState)
        {
            case CurrentState.Suffocating:
                HandleSuffocating();
                break;
        }
    }

    IEnumerator FadeToBlack()
    {
        float time = 0f;

        while (time < oxygenGracePeriod)
        {
            time += Time.deltaTime;
            float t = time / oxygenGracePeriod;

            Color color = fadeToBlack.color;
                                // alpha
            color.a = Mathf.Lerp(0, 1, t);
            fadeToBlack.color = color;

            yield return null;
        }
    }

    void OnOxygenDepleted()
    {
        ChangeState(CurrentState.Suffocating);
    }

    void HandleAlive()
    {
        // Probably not needed. Maybe use for UI
    }

    void HandleSuffocating()
    {
        // add state interruption - oxygen refilled during suffocation
        graceTimer -= Time.deltaTime;

        if (graceTimer <= 0f)
        {
            ChangeState(CurrentState.Dead);
        }
    }

    void HandleDeath()
    {
        oxygen.SetActive(false);
        playerMovement.SetActive(false);
        airlock.ResetAirlock();
        
        Invoke("HandleRespawn", 1f); // <--- Make this a UI button eventually
        //Debug.Log("Player dead");
    }

    void HandleRespawn()
    {
        if (respawnPoint == null) return;

        Rigidbody rb = GetComponent<Rigidbody>();
        
        // Reset velocity to prevent unexpected movement
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Move player
        rb.position = respawnPoint.position;
        rb.rotation = respawnPoint.rotation;

        oxygen.ResetOxygen();
        ChangeState(CurrentState.Alive);
        Color color = fadeToBlack.color; // Clean this up
        color.a = 0f;
        fadeToBlack.color = color;
    }

    void ChangeState(CurrentState newState)
    {
        if (newState == currentState) return;

        ExitState(currentState);
        currentState = newState;
        EnterState(newState);
    }

    void EnterState(CurrentState state)
    {
        switch (state)
        {
            case CurrentState.Suffocating:
                // Add visuals and sound 
                StartCoroutine(FadeToBlack());
                graceTimer = oxygenGracePeriod;
                break;
            
            case CurrentState.Dead:
                HandleDeath();
                break;
        }
    }

    void ExitState(CurrentState state)
    {
        // This is for turning off things like audio and UI
        switch (state)
        {
            case CurrentState.Suffocating:
                graceTimer = 0f;
                // Stop suffocation SFX
                // Hide oxygen warning UI
                // Reset post-processing effects
                break;
            
            case CurrentState.Dead:
                playerMovement.SetActive(true);
                oxygen.SetActive(true);
                break;
        }
    }
}

public enum CurrentState // Implement later
{   Alive, 
    Suffocating, 
    Dead, 
    Respawning
} 
