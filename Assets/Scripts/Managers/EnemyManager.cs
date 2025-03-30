using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject enemy;

    internal void spawnEnemies()
    {
        Instantiate(enemy);
    }
}
