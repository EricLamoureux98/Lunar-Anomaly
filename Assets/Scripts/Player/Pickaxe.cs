using UnityEngine;
using UnityEngine.InputSystem;

public class Pickaxe : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ParticleSystem rockParticles;
    [SerializeField] SphereCollider sphereCollider;
    [SerializeField] LayerMask rockLayer;
    [SerializeField] Animator anim;

    [Header("Mining")]
    [SerializeField] float pickaxeDamage = 1f;

    const string MINING_BOOL = "IsMining";

    void CheckForRock()
    {
        bool rockHit = false;

        Collider[] hits = Physics.OverlapSphere(sphereCollider.transform.position, sphereCollider.radius, rockLayer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Rock rock))
            {
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

    public void Mine(InputAction.CallbackContext context)
    {
        if (context.performed) 
        {
            anim.SetBool(MINING_BOOL, true);
        }

        if (context.canceled)
        {
            anim.SetBool(MINING_BOOL, false);
        }
    }
}

// NOTES

// Allow hold to mine - done
// Add pickaxe model - done
// Add basic animation - done
// Run mine command with animation - done

// Screen shake
// different sounds for hitting/not mining rock
