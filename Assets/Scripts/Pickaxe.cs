using UnityEngine;
using UnityEngine.InputSystem;

public class Pickaxe : MonoBehaviour
{
    [SerializeField] SphereCollider sphereCollider;
    [SerializeField] float pickaxeDamage = 1f;
    [SerializeField] float miningCooldown = 0.5f;


    void Start()
    {
        //sphereCollider.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rock") && other.TryGetComponent(out Rock rock))
        {
            Debug.Log("Mining rock triggered");
            rock.DamageRock(pickaxeDamage);
        }
    }

    void StartMining()
    {
        sphereCollider.enabled = true;
        Invoke(nameof(StopMining), miningCooldown);
    }

    void StopMining()
    {
        sphereCollider.enabled = false;
    }

    public void Mine(InputAction.CallbackContext context)
    {
        if (context.performed) 
        {
            //Debug.Log("Attempting to mine");
            StartMining();
        }
    }
}

// NOTES

// Add pickaxe model
// Attach to camera
// Run mine command with animation --- FUTURE GOAL
