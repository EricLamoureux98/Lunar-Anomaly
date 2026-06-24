using LunarAnomaly.Gameplay;
using LunarAnomaly.Input;
using NUnit.Framework;
using Unity.Cinemachine;
using UnityEngine;

namespace LunarAnomaly.Player
{
    public class Pickaxe : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] CinemachineImpulseSource impulseSource;
        [SerializeField] ParticleSystem rockParticles;
        [SerializeField] SphereCollider sphereCollider;
        [SerializeField] GameObject pickaxeObj;
        [SerializeField] LayerMask rockLayer;
        [SerializeField] Animator anim;
        InputHandler inputHandler;

        [Header("Mining")]
        [SerializeField] float pickaxeDamage = 1f;
        [SerializeField] float shakeAmount = 0.03f;
        bool isMining;
        Rock currentRock;

        bool isActive;

        const string MINING_BOOL = "IsMining";

        void OnEnable()
        {
            ObjectiveManager.OnToolActive += SetActive;
        }

        void OnDisable()
        {
            ObjectiveManager.OnToolActive -= SetActive;
        }

        void Awake()
        {
            inputHandler = GetComponentInParent<InputHandler>();
            if (inputHandler == null) Debug.Log("InputHandler not found!");
        }

        void Update()
        {
            if (!isActive) return; 

            ReadInput();
            HandleMiningInput();
            HandlePickaxeVisibility();
        }

        void SetActive(ToolType type, bool active)
        {
            if (type == ToolType.pickaxe)
                isActive = active;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Rock") && other.TryGetComponent(out Rock rock))
            {
                currentRock = rock;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Rock rock) && rock == currentRock)
            {
                currentRock = null;
            }
        }

        void HandlePickaxeVisibility()
        {
            pickaxeObj.SetActive(currentRock != null);
        }

        void CheckForRock()
        {
            bool rockHit = false;

            // This is overkill now
            Collider[] hits = Physics.OverlapSphere(sphereCollider.transform.position, sphereCollider.radius, rockLayer);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out Rock rock))
                {
                    SoundManager.PlaySound(SoundType.Mining);
                    CameraShakeManager.Instance.CameraShake(impulseSource, shakeAmount);
                    rock.DamageRock(pickaxeDamage);
                    rockHit = true;
                }
            }

            if (rockHit && rockParticles != null)
            {
                rockParticles.Play();
            }
        }

        public void OnPickImpact()
        {
            CheckForRock();
        }

        void HandleMiningInput()
        {
            anim.SetBool(MINING_BOOL, isMining && currentRock != null);
        }

        void ReadInput()
        {
            isMining = inputHandler.UseToolHeld;
        }
    }
}
// NOTES

// Allow hold to mine - done
// Add pickaxe model - done
// Add basic animation - done
// Run mine command with animation - done

// Screen shake - done
// different sounds for hitting/not mining rock
