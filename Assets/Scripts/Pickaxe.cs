using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pickaxe : MonoBehaviour
{
    [SerializeField] SphereCollider sphereCollider;
    [SerializeField] float pickaxeDamage = 1f;
    [SerializeField] float miningCooldown = 0.5f;
    [SerializeField] Animator anim;
    
    const string MINING_ANIMATION = "Pickaxe_Mine";
    const string MINING_IDLE = "Pickaxe_Idle";

    bool isMining;
    bool miningCRActive = false;
    Rock currentRock;

    void Update()
    {
        MiningController();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rock") && other.TryGetComponent(out Rock rock))
        {
            //Debug.Log("Mining rock triggered");
            //rock.DamageRock(pickaxeDamage);
            currentRock = rock;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Rock"))
        {
            currentRock = null;
        }
    }    

    void MiningController()
    {
        if (isMining && currentRock != null)
        {
            StartMining();
        }
        else if (!isMining)
        {
            StopMining();
        }
    }

    void StartMining()
    {
        if (!isMining || currentRock == null || miningCRActive)
            return;

        anim.Play(MINING_ANIMATION, 0, 0f);
        miningCRActive = true;
        StartCoroutine(MineRock());
    }


    void StopMining()
    {
        anim.Play(MINING_IDLE);
        StopCoroutine(nameof(MineRock));
        miningCRActive = false;
    }

    IEnumerator MineRock()
    {
        while (isMining && currentRock != null)
        {
            currentRock.DamageRock(pickaxeDamage);
            yield return new WaitForSeconds(miningCooldown);
            //Debug.Log("Mining Coroutine started");
        }

        miningCRActive = false;
    }

    public void Mine(InputAction.CallbackContext context)
    {
        if (context.performed) 
        {
            Debug.Log("Attempting to mine");
            isMining = true;
        }

        if (context.canceled)
        {
            isMining = false;
        }
    }
}

// NOTES

// Allow hold to mine - done
// Add pickaxe model - done
// Add basic animation - done
// Run mine command with animation --- FUTURE GOAL
