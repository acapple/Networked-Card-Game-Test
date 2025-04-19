using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject enemy;
    [SerializeField]
    internal List<Enemy> enemyList;

    internal void spawnEnemies()
    {
        enemyList = new List<Enemy>();
        Instantiate(enemy).GetComponent<NetworkObject>().Spawn();
    }
}
