using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人布局辅助器，负责计算和管理敌人在战斗场景中的位置
/// </summary>
public class EnemyLayoutHelper : MonoBehaviour
{
    [Header("怪物布局参数")]

    /// <summary>
    /// 单个敌人的宽度（像素）
    /// </summary>
    public float enemyWidth = 200f;

    /// <summary>
    /// 敌人间的间距（像素）
    /// </summary>
    public float enemySpacing = 80f;

    /// <summary>
    /// 布局区域最大宽度（像素），超过时自动压缩间距
    /// </summary>
    public float maxAreaWidth = 1200f;

    /// <summary>
    /// 怪物区的中心基准位置
    /// </summary>
    public Vector3 spawnBasePosition = new Vector3(0, 0, 0);

    /// <summary>
    /// 垂直偏移量
    /// </summary>
    public float verticalOffset = 0f;

    /// <summary>
    /// 当前所有敌人的Transform列表
    /// </summary>
    private List<Transform> enemyTransforms = new List<Transform>();

    /// <summary>
    /// 敌人Transform到位置的映射字典
    /// </summary>
    private Dictionary<Transform, Vector3> enemyPositions = new Dictionary<Transform, Vector3>();

    /// <summary>
    /// 添加敌人到布局系统并重新计算位置
    /// </summary>
    /// <param name="enemy">敌人的Transform组件</param>
    public void AddEnemy(Transform enemy)
    {
        if (!enemyTransforms.Contains(enemy))
        {
            enemyTransforms.Add(enemy);
            CalculateAndAssignPositions();
        }
    }

    /// <summary>
    /// 从布局系统移除敌人
    /// </summary>
    /// <param name="enemy">敌人的Transform组件</param>
    public void RemoveEnemy(Transform enemy)
    {
        if (enemyTransforms.Contains(enemy))
        {
            enemyTransforms.Remove(enemy);
            enemyPositions.Remove(enemy);
        }
    }

    /// <summary>
    /// 计算所有敌人的位置并分配
    /// </summary>
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

    /// <summary>
    /// 刷新布局，重新计算所有敌人位置
    /// </summary>
    public void RefreshLayout()
    {
        CalculateAndAssignPositions();
    }

    /// <summary>
    /// 清空所有敌人记录
    /// </summary>
    public void ClearAll()
    {
        enemyTransforms.Clear();
        enemyPositions.Clear();
    }
}