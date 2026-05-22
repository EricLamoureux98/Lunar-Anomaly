using System.Collections.Generic;
using UnityEngine;

public class Cable : MonoBehaviour
{
    [SerializeField] Transform player;

    [SerializeField] LineRenderer cable;
    [SerializeField] LayerMask collMask;

    [SerializeField] Transform cableStart;
    [SerializeField] Transform cableEnd; // Implement this

    bool isActive;

    public List<Vector3> cablePositions { get; set; } = new List<Vector3>();

    void Awake() => AddPosToCable(cableStart.position);

    void Update()
    {
        if (!isActive) return; 

        UpdateCablePositions();
        LastSegmentGoToPlayerPos();

        DetectCollisionEnter();
        if (cablePositions.Count > 2) DetectCollisionExits();  
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }

    public void SetLastPosition()
    {
        cable.SetPosition(cable.positionCount - 1, cableEnd.position);
    }

    void DetectCollisionEnter()
    {
        RaycastHit hit;
        if (Physics.Linecast(player.position, cablePositions[cablePositions.Count - 2], out hit, collMask))
        {
            cablePositions.RemoveAt(cablePositions.Count - 1);
            AddPosToCable(hit.point);
        }
    }

    void DetectCollisionExits()
    {
        RaycastHit hit;
        if (!Physics.Linecast(player.position, cablePositions[cablePositions.Count - 3], out hit, collMask))
        {
            cablePositions.RemoveAt(cablePositions.Count - 2);
        }
    }
    void AddPosToCable(Vector3 _pos)
    {
        cablePositions.Add(_pos);
        cablePositions.Add(player.position); //Always the last pos must be the player
    }

    void UpdateCablePositions()
    {
        cable.positionCount = cablePositions.Count;
        cable.SetPositions(cablePositions.ToArray());
    }

    void LastSegmentGoToPlayerPos() => cable.SetPosition(cable.positionCount - 1, player.position);
}
