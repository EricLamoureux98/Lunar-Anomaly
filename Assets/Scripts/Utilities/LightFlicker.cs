using UnityEngine;
using Random = UnityEngine.Random;

namespace LunarAnomaly
{
    public class LightFlicker : MonoBehaviour
    {
        [SerializeField] float minIntensity = 0.5f;
        [SerializeField] float maxIntensity = 5f;
        [SerializeField] float flickerSpeed = 0.1f;
        
        float flickerTimer;
        bool isFlickering;
    
        [SerializeField] Light spotLight;

        void Update()
        {
            if (!isFlickering) return;
            
            if (flickerTimer >= 0f)
            {
                flickerTimer -= Time.deltaTime;
            }
            else
            {
                if (IsInvoking(nameof(Flicker)))
                    CancelInvoke(nameof(Flicker));
                
                isFlickering = false;
                flickerTimer = 0f;
            }
        }

        public void StartFlicker(float flickerTime)
        {
            isFlickering = true;
            flickerTimer = flickerTime;
            InvokeRepeating(nameof(Flicker), 0f, flickerSpeed);
        }

        void Flicker()
        {
            float randomIntensity = Random.Range(minIntensity, maxIntensity);
            GetComponent<Light>().intensity = randomIntensity;
        }
    }
}

