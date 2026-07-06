using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地图管理器，负责地图生成、节点连接和玩家移动逻辑
/// 采用单例模式，确保全局唯一
/// </summary>
public class MapManager : MonoBehaviour
{
    /// <summary>
    /// 地图管理器单例实例
    /// </summary>
    public static MapManager Instance { get; private set; }

    /// <summary>
    /// 地图最大行数
    /// </summary>
    public int maxRows = 7;

    /// <summary>
    /// 每行最小列数
    /// </summary>
    public int minCols = 3;

    /// <summary>
    /// 每行最大列数
    /// </summary>
    public int maxCols = 4;

    /// <summary>
    /// 节点水平间距（像素）
    /// </summary>
    public float nodeSpacingX = 160f;

    /// <summary>
    /// 节点垂直间距（像素）
    /// </summary>
    public float nodeSpacingY = 130f;

    /// <summary>
    /// 所有地图节点列表
    /// </summary>
    public List<MapNode> allNodes = new List<MapNode>();

    /// <summary>
    /// 当前选中的节点
    /// </summary>
    public MapNode currentNode;

    /// <summary>
    /// 起始节点
    /// </summary>
    public MapNode startNode;

    /// <summary>
    /// 地图是否已生成
    /// </summary>
    public bool isMapGenerated = false;

    /// <summary>
    /// 初始化单例，确保场景切换时不被销毁
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 生成地图，创建节点并建立连接关系
    /// </summary>
    public void GenerateMap()
    {
        allNodes.Clear();
        int nodeId = 0;

        int[] colsPerRow = new int[maxRows];
        for (int i = 0; i < maxRows; i++)
        {
            colsPerRow[i] = Random.Range(minCols, maxCols + 1);
        }

        float totalWidth = maxCols * nodeSpacingX;
        float leftEdge = -totalWidth / 2f + nodeSpacingX / 2f;
        float startY = 0f;

        for (int row = 0; row < maxRows; row++)
        {
            int cols = colsPerRow[row];
            float rowWidth = cols * nodeSpacingX;
            float rowLeft = leftEdge + (totalWidth - rowWidth) / 2f;

            for (int col = 0; col < cols; col++)
            {
                NodeType type = GetNodeType(row, cols, col);
                MapNode node = new MapNode(nodeId++, row, col, type);
                node.position = new Vector2(
                    rowLeft + col * nodeSpacingX,
                    startY + row * nodeSpacingY
                );
                allNodes.Add(node);
            }
            startY += nodeSpacingY;
        }

        ConnectNodes();

        foreach (MapNode node in allNodes)
        {
            if (node.type == NodeType.Start)
            {
                node.isLocked = false;
                if (currentNode == null)
                {
                    node.isCurrent = true;
                    currentNode = node;
                    startNode = node;
                }
                else
                {
                    node.isCurrent = false;
                }
            }
        }

        isMapGenerated = true;
    }

    /// <summary>
    /// 重置地图状态，清空所有节点并重置位置
    /// </summary>
    public void ResetMap()
    {
        isMapGenerated = false;
        currentNode = null;
        startNode = null;
        
        MapPanel mapPanel = FindObjectOfType<MapPanel>(true);
        if (mapPanel != null)
        {
            mapPanel.ResetScrollPosition();
            Debug.Log("[MapManager] ResetMap - scroll position reset via mapPanel");
        }
        else
        {
            Debug.Log("[MapManager] ResetMap - MapPanel not found (will reset in OnEnter)");
        }
    }

    /// <summary>
    /// 根据行列位置确定节点类型
    /// </summary>
    /// <param name="row">行索引</param>
    /// <param name="cols">当前行的总列数</param>
    /// <param name="col">列索引</param>
    /// <returns>节点类型</returns>
    private NodeType GetNodeType(int row, int cols, int col)
    {
        int middleCol = cols / 2;
        
        if (row == 0) return NodeType.Start;
        if (row == maxRows - 1 && col == middleCol) return NodeType.Boss;

        float random = Random.value;
        if (random < 0.4f) return NodeType.Battle;
        if (random < 0.55f) return NodeType.Elite;
        if (random < 0.7f) return NodeType.Event;
        if (random < 0.85f) return NodeType.Rest;
        if (random < 0.95f) return NodeType.Shop;
        return NodeType.Treasure;
    }

    /// <summary>
    /// 建立节点之间的连接关系，形成单向链表结构
    /// </summary>
    private void ConnectNodes()
    {
        for (int i = 0; i < allNodes.Count; i++)
        {
            MapNode node = allNodes[i];
            List<MapNode> next = new List<MapNode>();

            if (node.row < maxRows - 1)
            {
                List<MapNode> nextRowNodes = allNodes.FindAll(n => n.row == node.row + 1);

                if (nextRowNodes.Count == 1)
                {
                    next.Add(nextRowNodes[0]);
                }
                else
                {
                    int minCol = Mathf.Max(0, node.col - 1);
                    int maxCol = Mathf.Min(nextRowNodes.Count - 1, node.col + 1);

                    for (int c = minCol; c <= maxCol; c++)
                    {
                        if (c >= 0 && c < nextRowNodes.Count)
                        {
                            next.Add(nextRowNodes[c]);
                        }
                    }
                }
            }

            node.nextNodes = next.ToArray();

            foreach (MapNode child in next)
            {
                child.previous = node;
            }
        }
    }

    /// <summary>
    /// 访问指定节点，更新节点状态并锁定/解锁相邻节点
    /// </summary>
    /// <param name="node">要访问的节点</param>
    public void VisitNode(MapNode node)
    {
        if (node == null || node.isLocked) return;

        if (currentNode != null)
        {
            currentNode.isCurrent = false;
            currentNode.isVisited = true;
            currentNode.isCleared = true;
        }

        node.isCurrent = true;
        node.isVisited = true;
        currentNode = node;

        foreach (MapNode n in allNodes)
        {
            n.isLocked = true;
        }

        foreach (MapNode next in node.nextNodes)
        {
            next.isLocked = false;
        }
    }

    /// <summary>
    /// 获取所有未锁定的节点列表
    /// </summary>
    /// <returns>未锁定节点列表</returns>
    public List<MapNode> GetUnlockedNodes()
    {
        return allNodes.FindAll(n => !n.isLocked);
    }

    /// <summary>
    /// 根据节点ID查找节点
    /// </summary>
    /// <param name="id">节点ID</param>
    /// <returns>对应的节点，未找到返回null</returns>
    public MapNode GetNodeById(int id)
    {
        return allNodes.Find(n => n.id == id);
    }
}
