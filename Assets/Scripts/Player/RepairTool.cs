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
		[SerializeField] Animator anim;
		[SerializeField] GameObject wrenchObj;
		InputHandler inputHandler;

		[Header("Mining")]
        bool isRepairing;

		bool isActive;

		RepairNode currentNode;

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
			RepairController();
			HandleRepairInput();
        }

		void SetActive(ToolType type, bool active)
        {
            if (type == ToolType.repairTool)
                isActive = active;
        }

		void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("OutpostNode") && other.TryGetComponent(out RepairNode node))
			{
				currentNode = node;

				if (currentNode.canBeRepaired)
					wrenchObj.SetActive(true);
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (other.TryGetComponent(out RepairNode node) && node == currentNode)
			{
				currentNode = null;
				wrenchObj.SetActive(false);
			}
		} 

		void RepairController()
		{
			if (isRepairing && currentNode != null && currentNode.canBeRepaired)
			{
				currentNode.RepairCurrentNode();
			}
		}

		void HandleRepairInput()
		{
			if (currentNode == null) return;

			bool shouldRepair = isRepairing && currentNode.canBeRepaired;

			//wrenchObj.SetActive(shouldRepair);
			anim.SetBool("IsRepairing", shouldRepair);
		}

		public void HandleRepairSound()
		{
			SoundManager.PlaySound(SoundType.Repair, 0.25f, false);
		}

		void ReadInput()
        {
            isRepairing = inputHandler.UseToolHeld;
        }
    }
}
