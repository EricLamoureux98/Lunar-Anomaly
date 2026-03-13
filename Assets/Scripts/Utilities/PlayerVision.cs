
using UnityEngine;

namespace LunarAnomaly
{
	public static class PlayerVision
	{
		public static bool IsPointVisible(Camera cam, Transform target, float dotThreshold, LayerMask ignoreMask)
		{
			Vector3 origin = cam.transform.position;
			Vector3 dir = (target.position - origin).normalized;

			float dot = Vector3.Dot(cam.transform.forward, dir);

			if (dot < dotThreshold) return false;

			float distance = Vector3.Distance(origin, target.position);

			Debug.DrawRay(origin, dir * distance, Color.red);

			// ~ is the bitwise NOT operator. It flips the layers bit (everything EXCEPT this layer)
			int mask = ~ignoreMask;

			if (Physics.Raycast(origin, dir, out RaycastHit hit, distance, mask))
			{
				// .IsChildOf is important due to collider being separate
				return hit.transform.IsChildOf(target);
			}

			return false;
		}
	}
}
