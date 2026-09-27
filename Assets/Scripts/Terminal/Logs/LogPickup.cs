using System;
using System.Collections;
using LunarAnomaly;
using UnityEngine;

public class LogPickup : MonoBehaviour
{
    [SerializeField] LogMessage log;
    [SerializeField] LogManager logManager;

    [SerializeField] Renderer logRenderer;
    [SerializeField] float pulseDuration;
    Material[] logPickupMaterials;

    // To NotificationController
    public static Action<NotificationMessage> OnLogPickup;

    void Start()
    {
        logPickupMaterials = logRenderer.materials;

        StartCoroutine(LogPickupPulse(true));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectLog();
            
            Debug.Log($"Log {log} picked up");
        }
    }

    void CollectLog()
    {
        SoundManager.PlaySound(SoundType.Pickup);
        logManager.DiscoverLog(log);
        Destroy(gameObject);
        OnLogPickup?.Invoke(NotificationMessage.LogPickup);
    }

    IEnumerator LogPickupPulse(bool active)
    {
        while (active)
        {
            Color off = Color.orange * 0f;
            Color on = Color.orange * 1f;

            float timer = 0f;
            while (timer < pulseDuration)
            {
                timer += Time.deltaTime;
                logPickupMaterials[1].SetColor("_EmissionColor",Color.Lerp(off, on, timer / pulseDuration));
                yield return null;
            }

            timer = 0f;
            while (timer < pulseDuration)
            {
                timer += Time.deltaTime;
                logPickupMaterials[1].SetColor("_EmissionColor",Color.Lerp(on, off, timer / pulseDuration));
                yield return null;
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
