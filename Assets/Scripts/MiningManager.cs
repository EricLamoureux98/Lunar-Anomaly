using UnityEngine;

public class MiningManager : MonoBehaviour
{
    [SerializeField] GameObject[] rocks;

    float rocksRemaining;

    void OnValidate()
    {
        rocksRemaining = rocks.Length;
        //Debug.Log("Rocks remaining: " + rocksRemaining);
    }

    void OnEnable()
    {
        Rock.OnRockDestroyed += RockDestroyed;
    }

    void OnDisable()
    {
        Rock.OnRockDestroyed -= RockDestroyed;
    }

    void Start()
    {
        rocksRemaining = rocks.Length;
    }

    public void RockDestroyed()
    {
        rocksRemaining--;
        Debug.Log("Rocks remaining: " + rocksRemaining);
    }

    
}
