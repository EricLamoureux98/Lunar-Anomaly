using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pickaxe : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CinemachineImpulseSource impulseSource;
    [SerializeField] ParticleSystem rockParticles;
    [SerializeField] SphereCollider sphereCollider;
    [SerializeField] LayerMask rockLayer;
    [SerializeField] Animator anim;
    PlayerInput playerInput;

    [Header("Mining")]
    [SerializeField] float pickaxeDamage = 1f;
    bool isMining;

    const string MINING_BOOL = "IsMining";

    void Awake()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        if (playerInput == null) Debug.Log("PlayerInput not found!");
    }

    void Update()
    {
        ReadInput();
        HandleMiningInput();
    }

    void CheckForRock()
    {
        bool rockHit = false;

        Collider[] hits = Physics.OverlapSphere(sphereCollider.transform.position, sphereCollider.radius, rockLayer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Rock rock))
            {
                CameraShakeManager.Instance.CameraShake(impulseSource);
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
        if (isMining)
        {
            anim.SetBool(MINING_BOOL, true);
        }
        else
        {
            anim.SetBool(MINING_BOOL, false);
        }
    }

    void ReadInput()
    {
        isMining = playerInput.UseToolHeld;
    }
}

// NOTES

// Allow hold to mine - done
// Add pickaxe model - done
// Add basic animation - done
// Run mine command with animation - done

// Screen shake - done
// different sounds for hitting/not mining rock
