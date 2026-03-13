using UnityEngine;

	
namespace LunarAnomaly.Gameplay
	{
	public class InsanityManager : MonoBehaviour
	{
		
		float currentInsanity;
		float maxInsanity;

        void Start()
        {
            currentInsanity = 0f;
        }

		public void IncreaseInsanity()
		{
			if (currentInsanity < maxInsanity)
			{
				currentInsanity++;
			}
		}
    }
}
