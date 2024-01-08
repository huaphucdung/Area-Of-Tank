using Photon.Pun;
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
        Debug.Log(other.name);
        if (!PhotonManager.IsHost()) return;
        TakeDamageModule takeDamage = other.gameObject.GetComponent<TakeDamageModule>();
        if (takeDamage == null) return;
        takeDamage.view.RPC("Attack", RpcTarget.All, null, 9999);
    }

    private void OnDrawGizmos()
    {
        foreach (Transform playerTrans in playerSpawnPosition)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(playerTrans.position, 1f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerTrans.position, rangeCheck);
        }

        foreach (Transform boxItemTrans in itemSpawnPosition)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(boxItemTrans.position, 1f);
        }
    }
}
