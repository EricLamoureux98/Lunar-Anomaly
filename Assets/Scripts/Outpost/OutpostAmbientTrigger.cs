using System;
using LunarAnomaly;
using UnityEngine;

public class OutpostAmbientTrigger : MonoBehaviour
{

    [SerializeField] bool hasDelay;
    [SerializeField] float delayAmount;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (hasDelay)
            {
                Invoke("PlaySound", delayAmount);
            }
            else
            {
                PlaySound();
            }
            
            GetComponent<SphereCollider>().enabled = false;
        }
    }

    void PlaySound()
    {
        SoundManager.PlaySound(SoundType.Ambience, 1f);
    }
}
