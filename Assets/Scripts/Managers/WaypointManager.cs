using UnityEngine.UI;
using UnityEngine;
using TMPro;
using LunarAnomaly.Gameplay;

public class WaypointManager : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TMP_Text distanceText;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Vector3 offset;
    [SerializeField] float minDistance = 5f;
    
    Transform target;
    bool isActive;

    void OnEnable()
    {
        HabitatController.OnUpdateWaypointTarget += UpdateWaypointTarget;
        HabitatController.OnUpdateWaypointActive += WaypointActive;
        OutpostRevealCinematic.OnUpdateWaypointTarget += UpdateWaypointTarget;
        OutpostRevealCinematic.OnUpdateWaypointActive += WaypointActive;
    }

    void OnDisable()
    {
        HabitatController.OnUpdateWaypointTarget -= UpdateWaypointTarget;
        HabitatController.OnUpdateWaypointActive -= WaypointActive;
        OutpostRevealCinematic.OnUpdateWaypointTarget -= UpdateWaypointTarget;
        OutpostRevealCinematic.OnUpdateWaypointActive -= WaypointActive;
    }

    void Update()
    {
        if (isActive)
            UpdateWaypoint();
    }

    void UpdateWaypoint()
    {
        float minX = image.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY = image.GetPixelAdjustedRect().height / 2;
        float maxY = Screen.width - minY;

        Vector2 pos = Camera.main.WorldToScreenPoint(target.position) + offset;

        if (Vector3.Dot(target.position - Camera.main.transform.position, Camera.main.transform.forward) < 0)
        {
            if (pos.x < Screen.width / 2)
            {
                pos.x = maxX;
            }            
            else
            {
                pos.x = minX;
            }
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        image.transform.position = pos;

        float distance = Vector3.Distance(target.position, Camera.main.transform.position);
        
        if (distance >= minDistance)
        {
            //distanceText.text = ((int)Vector3.Distance(target.position, Camera.main.transform.position)).ToString() + " m";
            WaypointVisible(true);
            distanceText.text = ((int)distance).ToString() + " m";
        }
        else
        {
            WaypointVisible(false);
        }
    }

    void UpdateWaypointTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void WaypointActive(bool active)
    {
        isActive = active;

        if (isActive)
            WaypointVisible(true);
        else
            WaypointVisible(false);
    }

    void WaypointVisible(bool visible)
    {
        if (visible)
            canvasGroup.alpha = 1f;
        else   
            canvasGroup.alpha = 0f;
    }
}
