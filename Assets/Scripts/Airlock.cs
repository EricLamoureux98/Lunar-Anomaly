using System;
using System.Collections;
using UnityEngine;

public class Airlock : MonoBehaviour
{
    [SerializeField] BoxCollider exteriorTrigger;
    [SerializeField] BoxCollider interiorTrigger;
    [SerializeField] Animator animExt;
    [SerializeField] Animator animInt;
    [SerializeField] float pressurizationTime = 3f;

    bool isCycling = false;

    [SerializeField] bool testEnterFromExterior = false;
    [SerializeField] bool testEnterFromInterior = false;

    // Add a check for if player has entered

    void Update()
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

        // Close exterior
        animExt.SetBool("IsOpen", false);
        //Debug.Log("Exterior closing");
        yield return new WaitForSeconds(pressurizationTime);

        // Pressurize chamber

        // Open interior
        animInt.SetBool("IsOpen", true);
        //Debug.Log("Interior opening");

        isCycling = false;
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

        // Close interior
        animInt.SetBool("IsOpen", false);
        yield return new WaitForSeconds(pressurizationTime);

        // Pressurize chamber

        // Open exterior
        animExt.SetBool("IsOpen", true);

        isCycling = false;
    }
}
