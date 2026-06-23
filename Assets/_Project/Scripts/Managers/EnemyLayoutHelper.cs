using System.Collections.Generic;
using UnityEngine;

public class EnemyLayoutHelper : MonoBehaviour
{
    [Header("怪物布局参数")]
    public float enemyWidth = 200f;
    public float enemySpacing = 80f;
    public float maxAreaWidth = 1200f;
    public Vector3 spawnBasePosition = new Vector3(0, 0, 0); // 怪物区的中心位置
    public float verticalOffset = 0f;

    private List<Transform> enemyTransforms = new List<Transform>();
    private Dictionary<Transform, Vector3> enemyPositions = new Dictionary<Transform, Vector3>();

    public void AddEnemy(Transform enemy)
    {
        if (!enemyTransforms.Contains(enemy))
        {
            enemyTransforms.Add(enemy);
            CalculateAndAssignPositions();
        }
    }

    public void RemoveEnemy(Transform enemy)
    {
        if (enemyTransforms.Contains(enemy))
        {
            enemyTransforms.Remove(enemy);
            enemyPositions.Remove(enemy);
            // 不重新布局其他怪物的位置
        }
    }

    private void CalculateAndAssignPositions()
    {
        int count = enemyTransforms.Count;
        if (count == 0) return;

        float totalRawWidth = (count - 1) * enemySpacing;
        float totalAreaWidth = totalRawWidth + enemyWidth;

        float spacing = enemySpacing;
        if (totalAreaWidth > maxAreaWidth && count > 1)
        {
            float availableWidth = maxAreaWidth - enemyWidth;
            spacing = availableWidth / (count - 1);
        }

        float totalSpan = (count - 1) * spacing;
        float startX = -totalSpan / 2f;

        for (int i = 0; i < count; i++)
        {
            Transform enemy = enemyTransforms[i];
            Vector3 targetPos = spawnBasePosition;
            targetPos.x = startX + i * spacing;
            targetPos.y += verticalOffset;

            enemyPositions[enemy] = targetPos;
            enemy.localPosition = targetPos;
        }
    }

    public void RefreshLayout()
    {
        // 只重新计算位置，但不改变已存在怪物的位置
        CalculateAndAssignPositions();
    }

    public void ClearAll()
    {
        enemyTransforms.Clear();
        enemyPositions.Clear();
    }
}