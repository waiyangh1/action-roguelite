using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<EnemyAI> activeEnemies = new List<EnemyAI>();
    private int currentIndex = 0;
    [SerializeField] private int updatesPerFrame = 2;

    public void RegisterEnemy(EnemyAI enemy)
    {
        if (!activeEnemies.Contains(enemy)) activeEnemies.Add(enemy);
    }

    public void UnregisterEnemy(EnemyAI enemy)
    {
        if (activeEnemies.Contains(enemy)) activeEnemies.Remove(enemy);
    }

    void Update()
    {
        if (activeEnemies.Count == 0) return;

        for (int i = 0; i < updatesPerFrame; i++)
        {
            if (activeEnemies.Count == 0) break;

            currentIndex++;
            if (currentIndex >= activeEnemies.Count) currentIndex = 0;

            if (activeEnemies[currentIndex].gameObject.activeInHierarchy)
            {
                activeEnemies[currentIndex].Think();
            }
        }
    }
}