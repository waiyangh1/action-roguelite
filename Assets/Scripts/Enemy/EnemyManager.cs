using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    private List<EnemyShell> activeEnemies = new List<EnemyShell>(1000);
    public int ActiveEnemiesCount => activeEnemies.Count;

    private Transform player;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    public void RegisterEnemy(EnemyShell enemy)
    {
        enemy.enemyListIndex = activeEnemies.Count;
        activeEnemies.Add(enemy);
    }

    public void UnregisterEnemy(EnemyShell enemy)
    {
        int index = enemy.enemyListIndex;
        if (index < 0 || index >= activeEnemies.Count) return;

        int lastIndex = activeEnemies.Count - 1;
        if (index != lastIndex)
        {
            EnemyShell lastEnemy = activeEnemies[lastIndex];
            activeEnemies[index] = lastEnemy;
            lastEnemy.enemyListIndex = index;
        }
        activeEnemies.RemoveAt(lastIndex);
        enemy.enemyListIndex = -1;
    }

    private void Update()
    {
        if (player == null) return;
        float dt = Time.deltaTime;
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            activeEnemies[i].OnUpdate(player, dt);
        }
    }

    private void FixedUpdate()
    {
        if (player == null) return;
        float fixedDt = Time.fixedDeltaTime;
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            activeEnemies[i].OnFixedUpdate(player, fixedDt);
        }
    }
}