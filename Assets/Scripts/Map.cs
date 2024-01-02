using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private List<Transform> playerSpawnPosition;
    [SerializeField] private List<Transform> itemSpawnPosition;
    [SerializeField] private LayerMask layerCheck;
    [SerializeField] [Range(0, 10f)] private float rangeCheck = 5f;

    public List<Transform> GetBoxPositions => itemSpawnPosition;

    public Vector3 GetSpawnPositionByIndex(int index)
    {
        return playerSpawnPosition[index].position;
    }

    public Vector3 GetSpawnPosition()
    {
        foreach(var spawnPosition in playerSpawnPosition)
        {
            if(!Physics.CheckSphere(spawnPosition.position, rangeCheck, layerCheck))
            {
                return spawnPosition.position;
            }
        }
        return Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        /*ITakeDamage takeDame = other.GetComponent<ITakeDamage>();
        if (takeDame != null)
        {
            takeDame.Attack(0, true);
        }*/
    }
}
