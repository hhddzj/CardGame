using UnityEngine;

/// <summary>
/// 地图节点类型枚举，定义不同类型的地图节点
/// </summary>
public enum NodeType
{
    /// <summary>
    /// 无类型节点
    /// </summary>
    None,

    /// <summary>
    /// 普通战斗节点
    /// </summary>
    Battle,

    /// <summary>
    /// 精英战斗节点
    /// </summary>
    Elite,

    /// <summary>
    /// 事件节点
    /// </summary>
    Event,

    /// <summary>
    /// 休息节点（回复生命）
    /// </summary>
    Rest,

    /// <summary>
    /// 商店节点
    /// </summary>
    Shop,

    /// <summary>
    /// Boss战斗节点
    /// </summary>
    Boss,

    /// <summary>
    /// 起始节点
    /// </summary>
    Start,

    /// <summary>
    /// 宝箱节点
    /// </summary>
    Treasure
}

/// <summary>
/// 地图节点类，存储单个地图节点的属性和状态
/// </summary>
public class MapNode
{
    /// <summary>
    /// 节点唯一标识符
    /// </summary>
    public int id;

    /// <summary>
    /// 节点所在行索引（从上到下）
    /// </summary>
    public int row;

    /// <summary>
    /// 节点所在列索引（从左到右）
    /// </summary>
    public int col;

    /// <summary>
    /// 节点类型
    /// </summary>
    public NodeType type;

    /// <summary>
    /// 是否已访问过该节点
    /// </summary>
    public bool isVisited;

    /// <summary>
    /// 是否为当前选中节点
    /// </summary>
    public bool isCurrent;

    /// <summary>
    /// 是否已锁定（不可点击）
    /// </summary>
    public bool isLocked;

    /// <summary>
    /// 是否已清除（战斗/事件已完成）
    /// </summary>
    public bool isCleared;

    /// <summary>
    /// 节点在地图上的位置坐标
    /// </summary>
    public Vector2 position;

    /// <summary>
    /// 上一个节点（单向链表结构）
    /// </summary>
    public MapNode previous;

    /// <summary>
    /// 下一个可访问的节点数组
    /// </summary>
    public MapNode[] nextNodes;

    /// <summary>
    /// 构造函数，初始化地图节点
    /// </summary>
    /// <param name="id">节点唯一标识符</param>
    /// <param name="row">节点所在行索引</param>
    /// <param name="col">节点所在列索引</param>
    /// <param name="type">节点类型</param>
    public MapNode(int id, int row, int col, NodeType type)
    {
        this.id = id;
        this.row = row;
        this.col = col;
        this.type = type;
        isVisited = false;
        isCurrent = false;
        isLocked = true;
        isCleared = false;
        nextNodes = new MapNode[0];
    }

    /// <summary>
    /// 获取节点对应的颜色，用于UI显示
    /// </summary>
    /// <returns>节点颜色</returns>
    public Color GetNodeColor()
    {
        switch (type)
        {
            case NodeType.Battle: return new Color(0.8f, 0.2f, 0.2f);
            case NodeType.Elite: return new Color(1f, 0.6f, 0f);
            case NodeType.Event: return new Color(0.2f, 0.5f, 0.8f);
            case NodeType.Rest: return new Color(0.3f, 0.8f, 0.3f);
            case NodeType.Shop: return new Color(0.8f, 0.6f, 0.2f);
            case NodeType.Boss: return new Color(0.6f, 0.2f, 0.8f);
            case NodeType.Start: return new Color(0.5f, 0.5f, 0.5f);
            case NodeType.Treasure: return new Color(1f, 1f, 0.5f);
            default: return Color.gray;
        }
    }

    /// <summary>
    /// 获取节点对应的图标文字，用于UI显示
    /// </summary>
    /// <returns>节点图标文字</returns>
    public string GetNodeIcon()
    {
        switch (type)
        {
            case NodeType.Battle: return "战";
            case NodeType.Elite: return "精";
            case NodeType.Event: return "事";
            case NodeType.Rest: return "休";
            case NodeType.Shop: return "商";
            case NodeType.Boss: return "主";
            case NodeType.Start: return "起";
            case NodeType.Treasure: return "宝";
            default: return "?";
        }
    }
}
