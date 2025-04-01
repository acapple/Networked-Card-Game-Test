using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject enemy;
    [SerializeField]
    internal List<Enemy> enemyList;

    internal void spawnEnemies()
    {
        enemyList = new List<Enemy>();
        enemyList.Add(Instantiate(enemy).GetComponent<Enemy>());
    }
}
