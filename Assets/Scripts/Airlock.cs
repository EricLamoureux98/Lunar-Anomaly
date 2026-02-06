
using System.Collections;

using UnityEngine;

public class Airlock : MonoBehaviour
{
    [SerializeField] Animator animExt;
    [SerializeField] Animator animInt;
    [SerializeField] AtmosphereZone atmosphereZone;
    [SerializeField] float pressurizationTime = 3f;

    bool isCycling = false;
    bool playerInside = false;
    public bool cancelCycle; // Use this if/when the player is killed or level reset etc

    [SerializeField] bool testEnterFromExterior = false;
    [SerializeField] bool testEnterFromInterior = false;

    void Awake()
    {
        atmosphereZone = GetComponentInChildren<AtmosphereZone>();
        if (atmosphereZone == null) Debug.Log("Atmosphere zone not found");
    }

    void Update()
    {
        AirlockTesting();
    }

    public void EnterFromExterior()
    {
        if (isCycling) return;
        StartCoroutine(CycleFromExterior());
    }

    IEnumerator CycleFromExterior()
    {
        isCycling = true;

        // Ensure interior is closed
        animInt.SetBool("IsOpen", false);
        //Debug.Log("Interior closing");
        yield return new WaitForSeconds(1f);

        // Open exterior
        animExt.SetBool("IsOpen", true);
        //Debug.Log("Exterior opening");
        yield return new WaitForSeconds(2f);

        // Check if player has entered
        yield return new WaitUntil(() => playerInside || cancelCycle);

        if (cancelCycle)
        {
            ResetAirlock();
            yield break; // End coroutine early if cancelled
        }

        // Close exterior
        animExt.SetBool("IsOpen", false);
        //Debug.Log("Exterior closing");
        yield return new WaitForSeconds(pressurizationTime);

        // Pressurize chamber
        atmosphereZone.SetPressuized(true);

        // Open interior
        animInt.SetBool("IsOpen", true);
        //Debug.Log("Interior opening");

        isCycling = false;
        playerInside = false;
    }

    // Can this be optimized? Not D.R.Y
    public void EnterFromInterior()
    {
        if (isCycling) return;
        StartCoroutine(CycleFromInterior());
    }

    IEnumerator CycleFromInterior()
    {
        isCycling = true;

        // Ensure exterior is closed
        animExt.SetBool("IsOpen", false);
        yield return new WaitForSeconds(1f);

        // Open interior
        animInt.SetBool("IsOpen", true);
        yield return new WaitForSeconds(2f);

        // Check if player has entered
        yield return new WaitUntil(() => playerInside || cancelCycle);

        if (cancelCycle)
        {
            ResetAirlock();
            yield break;
        }

        // Close interior
        animInt.SetBool("IsOpen", false);
        yield return new WaitForSeconds(pressurizationTime);

        // Pressurize chamber
        atmosphereZone.SetPressuized(false);

        // Open exterior
        animExt.SetBool("IsOpen", true);

        isCycling = false;
        playerInside = false;
    }

    public void PlayerInsideAirlock()
    {
        Debug.Log("Player inside airlock");
        playerInside = true;
    }

    public void ResetAirlock()
    {
        atmosphereZone.SetPressuized(true); // This needs to be smarter
        animExt.SetBool("IsOpen", false);
        animInt.SetBool("IsOpen", false);

        isCycling = false;
        playerInside = false;
        cancelCycle = false;
    }

    void AirlockTesting()
    {
        if (testEnterFromExterior)
        {
            testEnterFromExterior = false;
            EnterFromExterior();
        }

        if (testEnterFromInterior)
        {
            testEnterFromInterior = false;
            EnterFromInterior();
        }
    }
}
