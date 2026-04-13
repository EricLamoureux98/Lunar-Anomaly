using LunarAnomaly.Input;
using UnityEngine;

	
namespace LunarAnomaly.Player
{
	// Create a base class for this
	public class RepairTool : MonoBehaviour
	{
		[SerializeField] SphereCollider sphereCollider;
		[SerializeField] LayerMask outpostLayer;
		InputHandler inputHandler;

		[Header("Mining")]
        bool isRepairing;

		Outpost currentOutpost;

		void Awake()
        {
            inputHandler = GetComponentInParent<InputHandler>();
            if (inputHandler == null) Debug.Log("InputHandler not found!");
        }

        void Update()
        {
            ReadInput();
			RepairController();
        }

		void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Outpost") && other.TryGetComponent(out Outpost structure))
			{
				Debug.Log("Player near outpost");
				currentOutpost = structure;
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (other.TryGetComponent(out Outpost outpost) && outpost == currentOutpost)
			{
				currentOutpost = null;
			}
		} 

		void RepairController()
		{
			if (isRepairing && currentOutpost != null)
			{
				currentOutpost.RepairStructure();
			}
		}

		void ReadInput()
        {
            isRepairing = inputHandler.UseToolHeld;
        }
    }
}
