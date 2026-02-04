using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [SerializeField] float oxygenGracePeriod = 3f;

    float graceTimer;
    CurrentState currentState;

    void Update()
    {
        switch (currentState)
        {
            case CurrentState.Suffocating:
                HandleSuffocating();
                break;
        }
    }

    void OnEnable()
    {
        Oxygen.OnOxygenDepleted += OnOxygenDepleted;
    }

    void OnDisable()
    {
        Oxygen.OnOxygenDepleted -= OnOxygenDepleted;
    }

    void OnOxygenDepleted()
    {
        ChangeState(CurrentState.Suffocating);
    }

    void HandleAlive()
    {
        // Not sure if needed. Maybe use for UI
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
        // Airlock.cancelCycle
        // Signal to RespawnManager
        Debug.Log("Player dead");
    }

    void HandleRespawning()
    {
        // Refill oxygen
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
                graceTimer = oxygenGracePeriod;
                break;
            
            case CurrentState.Dead:
                HandleDeath();
                break;
        }
    }

    void ExitState(CurrentState state)
    {
        // Not used yet - This is for turning off things like audio and UI
        switch (state)
        {
            case CurrentState.Suffocating:
                graceTimer = 0f;
                // Stop suffocation SFX
                // Hide oxygen warning UI
                // Reset post-processing effects
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
