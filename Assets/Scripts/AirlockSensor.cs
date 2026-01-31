using UnityEngine;

public class AirlockSensor : MonoBehaviour
{
    [SerializeField] bool isExterior;
    [SerializeField] float airlockCooldown = 6f;
    Airlock airlock;

    float cooldown; // ------ Not yet implemented 
                    // ------ Might be able to avoid with player detection inside airlock

    void Awake()
    {
        airlock = GetComponentInParent<Airlock>();

        if (airlock == null) Debug.Log("airlock not found");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");
        if (!other.CompareTag("Player")) return;

        if (isExterior)
        {
            Debug.Log("Player waiting to enter");
            airlock.EnterFromExterior();
        }
        else
        {
            Debug.Log("Player waiting to exit");
            airlock.EnterFromInterior();
        }
    }
}
