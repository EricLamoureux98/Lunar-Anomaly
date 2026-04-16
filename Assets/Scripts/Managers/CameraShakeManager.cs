using Unity.Cinemachine;
using UnityEngine;

namespace LunarAnomaly.Gameplay
{
    public class CameraShakeManager : MonoBehaviour
    {
        public static CameraShakeManager Instance { get; private set; }

        //[SerializeField] float globalShakeForce = 0.05f;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void CameraShake(CinemachineImpulseSource impulseSource, float shakeForce, float min = 0.01f, float max = 0.5f)
        {
            shakeForce = Mathf.Clamp(shakeForce, min, max);
            impulseSource.GenerateImpulseWithForce(shakeForce);
        }
    }
}