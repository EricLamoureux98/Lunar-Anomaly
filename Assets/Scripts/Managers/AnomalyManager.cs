using System;
using System.Collections;
using LunarAnomaly;
using LunarAnomaly.Gameplay;
using LunarAnomaly.UI;
using UnityEngine;

public class AnomalyManager : MonoBehaviour
{
    [SerializeField] ScreenEffect screenEffect;

    [SerializeField] Transform anomalyLocation;
    [SerializeField] GameObject silhouette;
    [SerializeField] float rotationTime = 2f;

    Coroutine rotateSilhouetteRoutine;
    Coroutine startAnomalyRoutine;

    public bool isActive; // public for testing
    // To PlayerState
    public static event Action OnPlayerInactive;
    // To WaypointManager
    public static event Action<Transform> OnUpdateWaypointTarget;
    public static event Action<bool> OnUpdateWaypointActive;
    // To ObjectiveManager
    public static event Action<ProgressionStage> OnAnomalyProgress;

    void OnEnable()
    {
        ObjectiveManager.OnBeginAnomalyObjective += BeginAnomalyObjective;
        HabitatAirlock.OnAirlockDoorClose += CheckRotateSignal;
    }

    void OnDisable()
    {
        ObjectiveManager.OnBeginAnomalyObjective -= BeginAnomalyObjective;
        HabitatAirlock.OnAirlockDoorClose -= CheckRotateSignal;
    }

    void BeginAnomalyObjective()
    {     
        if (startAnomalyRoutine == null && !isActive)
            startAnomalyRoutine = StartCoroutine(StartAnomalyObjectiveRoutine());
    }

    void CheckRotateSignal()
    {
        if (isActive && rotateSilhouetteRoutine == null)
        {
            rotateSilhouetteRoutine = StartCoroutine(RotateSilhouetteRoutine());
        }
    }

    IEnumerator StartAnomalyObjectiveRoutine()
    {
        isActive = true;

        TerminalUI.OnRequestNotification(NotificationMessage.PriorityUpdate);
        
        yield return new WaitForSeconds(4f);
        TerminalUI.OnRequestNotification(NotificationMessage.AnomalyDetected);
        
        yield return new WaitForSeconds(4f);
        TerminalUI.OnRequestNotification(NotificationMessage.InvestigateAnomaly);

        yield return new WaitForSeconds(4f);
        OnAnomalyProgress?.Invoke(ProgressionStage.Anomaly);
        OnUpdateWaypointTarget?.Invoke(anomalyLocation);
        OnUpdateWaypointActive?.Invoke(true);

        startAnomalyRoutine = null;
        
    }

    // IEnumerator RotateSilhouetteRoutine()
    // {
    //     Quaternion startRotation = silhouette.transform.rotation;
    //     Quaternion targetRotation = startRotation * Quaternion.Euler(0, 90, 0);
    //     float elapsed = 0f;

    //     silhouette.SetActive(true);

    //     while (elapsed < rotationTime)
    //     {
    //         elapsed += Time.deltaTime;
    //         float t = elapsed / rotationTime;

    //         silhouette.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

    //         yield return null;
    //     }

    //     silhouette.transform.rotation = targetRotation;

    //     yield return new WaitForSeconds(1f);

    //     screenEffect.PlayScreenEffect(ScreenEffectType.ToBlack);

    //     OnPlayerInactive?.Invoke();

    //     TerminalUI.OnRequestNotificationDelayed?.Invoke(NotificationMessage.EndOfDemo, 1f);

    //     yield return new WaitForSeconds(10f);

    //     GameManager.Instance.ReturnToMainMenu();

    //     rotateSilhouetteRoutine = null;
    // }

    // Temp name. This is a blink version
    IEnumerator RotateSilhouetteRoutine()
    {
        screenEffect.PlayScreenEffect(ScreenEffectType.Silhouette);

        // Temp fix
        Quaternion startRotation = silhouette.transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 90, 0);
        silhouette.transform.rotation = targetRotation;

        yield return new WaitForSeconds(0.1f);

        silhouette.SetActive(true);

        yield return new WaitForSeconds(2.5f);

        SoundManager.PlaySound(SoundType.AlienClick, 2f);

        screenEffect.PlayScreenEffect(ScreenEffectType.ToBlack);

        OnPlayerInactive?.Invoke();

        TerminalUI.OnRequestNotificationDelayed?.Invoke(NotificationMessage.EndOfDemo, 1f);

        yield return new WaitForSeconds(8f);

        GameManager.Instance.ReturnToMainMenu();

        rotateSilhouetteRoutine = null;
    }
}
