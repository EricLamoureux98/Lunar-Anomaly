using System;
using LunarAnomaly.Player;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerState playerState;
    [SerializeField] PlayerLook playerLook;
    [SerializeField] Oxygen oxygen;
    PauseMenu pauseMenu;

    [Header("Sliders")]
    [SerializeField] Slider mouseSenseSlider;
    [SerializeField] Slider walkSpeedSlider;
    [SerializeField] Slider sprintSpeedSlider;
    [SerializeField] Slider jumpHeightSlider;
    [SerializeField] Slider oxygenDrainSlider;

    [Header("Buttons & Toggles")]
    [SerializeField] Toggle respawnInHabitatToggle;
    [SerializeField] Button teleportPlayerButton;
    [SerializeField] Button forceRespawnButton;

    void Awake()
    {
        pauseMenu = GetComponent<PauseMenu>();
    }

    void Start()
    {
        InitializeTeleportSettings();
        InitializeMovementSettings();
        InitializeOxygenSettings();
        InitializeMouseSense();
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

    void InitializeTeleportSettings()
    {
        if (playerState == null) return;

        respawnInHabitatToggle.isOn = playerState.RespawnInHabitat;
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

    // Controlled by Toggle
    public void HandleRespawnInHabitat(bool inHabitat)
    {
        // ***** EITHER ADD A NEW POS OR REMOVE
        playerState.UpdateRespawnInHabitat(inHabitat); 
    }

    // Controlled by Button
    public void HandleForceRespawn()
    {
        pauseMenu.ResumeGame();
        playerState.HandleRespawn();
    }

    // Controlled by Button
    public void HandleTeleport()
    {
        pauseMenu.ResumeGame();
        playerState.HandleDebugTeleport();
    }
}
