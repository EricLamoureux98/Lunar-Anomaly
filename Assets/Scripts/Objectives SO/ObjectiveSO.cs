using LunarAnomaly.UI;
using UnityEngine;

[System.Serializable]
public class ProgressionData
{
    public int AmountNeeded;
    public string ProgressionName; // AmountNeeded + Name (X repaired)
}

[System.Serializable]
public class ObjectiveData
{
    public string Title;
    public ObjectiveType objectiveType;

    public ProgressionData Progression;
}

[CreateAssetMenu(fileName = "ObjectiveSO", menuName = "Scriptable Objects/ObjectiveSO")]
public class ObjectiveSO : ScriptableObject
{
    public ObjectiveData[] Objectives;
}
