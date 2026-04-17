using LunarAnomaly.Gameplay;
using LunarAnomaly.Input;
using UnityEngine;

	
namespace LunarAnomaly.Player
{
	// Create a base class for tools
	public class RepairTool : MonoBehaviour
	{
		[SerializeField] SphereCollider sphereCollider;
		[SerializeField] LayerMask outpostLayer;
		InputHandler inputHandler;

		[Header("Mining")]
        bool isRepairing;

		RepairNode currentNode;

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
			if (other.CompareTag("OutpostNode") && other.TryGetComponent(out RepairNode node))
			{
				currentNode = node;
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (other.TryGetComponent(out RepairNode node) && node == currentNode)
			{
				currentNode = null;
			}
		} 

		void RepairController()
		{
			if (isRepairing && currentNode != null)
			{
				currentNode.RepairCurrentNode();
			}
		}

		void ReadInput()
        {
            isRepairing = inputHandler.UseToolHeld;
        }
    }
}
