using System;
using LunarAnomaly.Player;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerLook playerLook;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Oxygen oxygen;

    [Header("Sliders")]
    [SerializeField] Slider mouseSenseSlider;
    [SerializeField] Slider walkSpeedSlider;
    [SerializeField] Slider sprintSpeedSlider;
    [SerializeField] Slider jumpHeightSlider;
    [SerializeField] Slider oxygenDrainSlider;

    void Start()
    {
        InitializeMouseSense();
        InitializeMovementSettings();
        InitializeOxygenSettings();
    }

    void InitializeMouseSense()
    {
        if (playerLook == null) return;

        float minSense = playerLook.MinSense;
        float maxSense = playerLook.MaxSense;
        float sense = playerLook.Sensitivity;

        mouseSenseSlider.minValue = minSense;
        mouseSenseSlider.maxValue = maxSense;
        mouseSenseSlider.value = sense;
    }

    void InitializeMovementSettings()
    {
        if (playerMovement == null) return;

        walkSpeedSlider.value = playerMovement.WalkSpeed;
        sprintSpeedSlider.value = playerMovement.SprintSpeed;
        jumpHeightSlider.value = playerMovement.JumpHeight;
    }

    void InitializeOxygenSettings()
    {
        if (oxygen == null) return;

        oxygenDrainSlider.value = oxygen.OxygenDrainRate;
    }

    // Controlled by slider
    public void HandleSenseUpdate(float sense)
    {
        playerLook.UpdateMouseSense(sense);
    }

    // Controlled by slider
    public void HandleWalkSpeedUpdate(float speed)
    {
        playerMovement.UpdateWalkSpeed(speed);
    }

    // Controlled by slider
    public void HandleSprintSpeedUpdate(float speed)
    {
        playerMovement.UpdateSprintSpeed(speed);
    }

    // Controlled by slider
    public void HandleJumpHeightUpdate(float height)
    {
        playerMovement.UpdateJumpHeight(height);
    }

    // Controlled by slider
    public void HandleOxygenDrainUpdate(float drainRate)
    {
        oxygen.UpdateOxygenDrainRate(drainRate);
    }
}
