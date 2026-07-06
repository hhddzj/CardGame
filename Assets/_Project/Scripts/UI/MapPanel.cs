using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 地图面板，负责显示地图节点、连接线路和处理节点点击事件
/// </summary>
public class MapPanel : UIPanel
{
    /// <summary>
    /// 标题文本对象
    /// </summary>
    public GameObject titleText;

    /// <summary>
    /// 地图滚动视图组件
    /// </summary>
    public ScrollRect mapScrollRect;

    /// <summary>
    /// 地图内容容器
    /// </summary>
    public RectTransform mapContent;

    /// <summary>
    /// 节点按钮预制件
    /// </summary>
    public Button nodePrefab;

    /// <summary>
    /// 连接线预制件
    /// </summary>
    public Image linePrefab;

    /// <summary>
    /// 节点大小（像素）
    /// </summary>
    private const float NODE_SIZE = 60f;

    /// <summary>
    /// 内容区域宽度（像素）
    /// </summary>
    private const float CONTENT_WIDTH = 700f;

    /// <summary>
    /// 地图节点到按钮的映射字典
    /// </summary>
    private Dictionary<MapNode, Button> nodeButtons = new Dictionary<MapNode, Button>();

    /// <summary>
    /// 连接线列表
    /// </summary>
    private List<Image> lines = new List<Image>();

    /// <summary>
    /// 保存的滚动位置，用于战斗结束后恢复视图
    /// </summary>
    private Vector2 savedScrollPosition = new Vector2(0.5f, 0f);

    /// <summary>
    /// 是否为首次生成地图（决定是否滚动到起始节点）
    /// </summary>
    private bool isFirstMap = true;

    /// <summary>
    /// 初始化，记录组件引用信息
    /// </summary>
    public override void Awake()
    {
        base.Awake();
        Debug.Log($"[MapPanel] Awake - mapScrollRect: {mapScrollRect}, mapContent: {mapContent}");
        if (mapContent != null)
        {
            Debug.Log($"[MapPanel] Content anchorMin: {mapContent.anchorMin}, anchorMax: {mapContent.anchorMax}, pivot: {mapContent.pivot}");
        }
    }

    /// <summary>
    /// 面板进入时调用，播放标题动画并生成地图UI
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        
        if (titleText != null)
        {
            titleText.transform.localScale = Vector3.zero;
            titleText.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
        }

        bool wasMapRegenerated = !MapManager.Instance.isMapGenerated;
        
        ClearMap();
        GenerateMapUI();
        
        if (wasMapRegenerated)
        {
            isFirstMap = true;
            savedScrollPosition = new Vector2(0.5f, 0f);
            Debug.Log("[MapPanel] Map regenerated, resetting scroll position");
        }
    }

    /// <summary>
    /// 清空地图上的所有节点按钮和连接线
    /// </summary>
    private void ClearMap()
    {
        foreach (Button btn in nodeButtons.Values)
        {
            Destroy(btn.gameObject);
        }
        nodeButtons.Clear();

        foreach (Image line in lines)
        {
            Destroy(line.gameObject);
        }
        lines.Clear();
    }

    /// <summary>
    /// 生成地图UI，包括节点按钮和连接线
    /// </summary>
    private void GenerateMapUI()
    {
        Debug.Log("[MapPanel] GenerateMapUI - Start");
        
        Canvas.ForceUpdateCanvases();
        
        if (MapManager.Instance == null)
        {
            GameObject mapManagerObj = new GameObject("MapManager");
            mapManagerObj.AddComponent<MapManager>();
        }

        if (!MapManager.Instance.isMapGenerated)
        {
            MapManager.Instance.GenerateMap();
            Debug.Log("[MapPanel] Map generated for the first time");
        }
        else
        {
            Debug.Log("[MapPanel] Map already generated, using existing");
        }

        AdjustContentSize();

        foreach (MapNode node in MapManager.Instance.allNodes)
        {
            CreateNodeButton(node);
        }

        foreach (MapNode node in MapManager.Instance.allNodes)
        {
            foreach (MapNode next in node.nextNodes)
            {
                CreateLine(node, next);
            }
        }

        UpdateNodeVisuals();
        ScrollToStartNode();
    }

    /// <summary>
    /// 创建节点按钮并设置位置和样式
    /// </summary>
    /// <param name="node">地图节点</param>
    private void CreateNodeButton(MapNode node)
    {
        Button btn = Instantiate(nodePrefab, mapContent);
        RectTransform rect = btn.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(NODE_SIZE, NODE_SIZE);
        
        float centerX = CONTENT_WIDTH / 2f;
        float posX = centerX + node.position.x;
        float posY = node.position.y;

        rect.anchoredPosition = new Vector2(posX, posY);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);

        Debug.Log($"[MapPanel] Node {node.id} ({node.type}): pos=({node.position.x:F1}, {node.position.y:F1}) -> calc=({posX:F1}, {posY:F1})");

        TextMeshProUGUI text = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = node.isCleared ? "✓" : node.GetNodeIcon();
            text.fontSize = 24;
            text.color = node.isLocked ? Color.black : Color.white;
        }

        Image image = btn.GetComponent<Image>();
        if (image != null)
        {
            image.color = node.isLocked ? Color.gray : node.GetNodeColor();
            image.fillAmount = node.isCleared ? 0.5f : 1f;
        }

        btn.interactable = !node.isLocked && !node.isCleared;

        btn.onClick.AddListener(() => OnNodeClicked(node));
        nodeButtons[node] = btn;
    }

    /// <summary>
    /// 创建两个节点之间的连接线
    /// </summary>
    /// <param name="from">起始节点</param>
    /// <param name="to">目标节点</param>
    private void CreateLine(MapNode from, MapNode to)
    {
        Image line = Instantiate(linePrefab, mapContent);
        line.transform.SetAsFirstSibling();

        float centerX = CONTENT_WIDTH / 2f;

        Vector2 startPos = new Vector2(centerX + from.position.x, from.position.y);
        Vector2 endPos = new Vector2(centerX + to.position.x, to.position.y);

        Vector2 midPos = (startPos + endPos) / 2f;
        float distance = Vector2.Distance(startPos, endPos);

        RectTransform rect = line.GetComponent<RectTransform>();
        rect.anchoredPosition = midPos;
        rect.sizeDelta = new Vector2(distance, 4f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);

        float angle = Mathf.Atan2(endPos.y - startPos.y, endPos.x - startPos.x) * Mathf.Rad2Deg;
        rect.rotation = Quaternion.Euler(0f, 0f, angle);

        if (from.isCurrent && !to.isLocked)
        {
            line.color = new Color(1f, 1f, 0f, 1f);
        }
        else if (from.isLocked)
        {
            line.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
        }
        else if (from.isCleared)
        {
            line.color = new Color(0.8f, 0.8f, 0.8f, 0.5f);
        }
        else if (!from.isLocked)
        {
            line.color = new Color(1f, 1f, 1f, 0.8f);
        }
        else
        {
            line.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
        lines.Add(line);
    }

    /// <summary>
    /// 节点点击事件处理
    /// </summary>
    /// <param name="node">被点击的节点</param>
    private void OnNodeClicked(MapNode node)
    {
        if (node.isLocked) return;

        MapManager.Instance.VisitNode(node);
        UpdateNodeVisuals();

        if (node.type == NodeType.Battle || node.type == NodeType.Elite || node.type == NodeType.Boss)
        {
            StartBattle(node);
        }
        else if (node.type == NodeType.Rest)
        {
            ShowRestMenu();
        }
        else if (node.type == NodeType.Shop)
        {
            ShowShop();
        }
        else if (node.type == NodeType.Event)
        {
            ShowEvent();
        }
        else if (node.type == NodeType.Treasure)
        {
            ShowTreasure();
        }
    }

    /// <summary>
    /// 更新所有节点的视觉状态（颜色、图标、交互性）
    /// </summary>
    private void UpdateNodeVisuals()
    {
        foreach (var pair in nodeButtons)
        {
            MapNode node = pair.Key;
            Button btn = pair.Value;
            Image image = btn.GetComponent<Image>();
            TextMeshProUGUI text = btn.GetComponentInChildren<TextMeshProUGUI>();

            if (image != null)
            {
                image.color = node.isLocked ? Color.gray : node.GetNodeColor();
                image.fillAmount = node.isCleared ? 0.5f : 1f;
            }

            if (text != null)
            {
                text.text = node.isCleared ? "✓" : node.GetNodeIcon();
                text.color = node.isLocked ? Color.black : Color.white;
            }

            btn.interactable = !node.isLocked && !node.isCleared;
        }

        foreach (Image line in lines)
        {
            Destroy(line.gameObject);
        }
        lines.Clear();

        foreach (MapNode node in MapManager.Instance.allNodes)
        {
            foreach (MapNode next in node.nextNodes)
            {
                CreateLine(node, next);
            }
        }
    }

    /// <summary>
    /// 开始战斗，保存滚动位置并设置战斗难度
    /// </summary>
    /// <param name="node">战斗节点</param>
    private void StartBattle(MapNode node)
    {
        if (mapScrollRect != null)
        {
            savedScrollPosition = mapScrollRect.normalizedPosition;
            Debug.Log($"[MapPanel] Saved scroll position: {savedScrollPosition}");
        }

        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.SetBattleDifficulty(node.type == NodeType.Elite ? 2 : (node.type == NodeType.Boss ? 3 : 1));
        }
        UIFrame.Instance.OpenPanel("BattlePanel");
    }

    /// <summary>
    /// 显示休息菜单（预留）
    /// </summary>
    private void ShowRestMenu()
    {
        Debug.Log("休息点");
    }

    /// <summary>
    /// 显示商店（预留）
    /// </summary>
    private void ShowShop()
    {
        Debug.Log("商店");
    }

    /// <summary>
    /// 显示事件（预留）
    /// </summary>
    private void ShowEvent()
    {
        Debug.Log("事件");
    }

    /// <summary>
    /// 显示宝箱（预留）
    /// </summary>
    private void ShowTreasure()
    {
        Debug.Log("宝箱");
    }

    /// <summary>
    /// 根据节点位置调整内容区域大小
    /// </summary>
    private void AdjustContentSize()
    {
        if (mapContent == null || mapScrollRect == null || mapScrollRect.viewport == null) 
        {
            Debug.LogError("[MapPanel] AdjustContentSize - missing references!");
            return;
        }

        float maxX = float.MinValue;
        float minX = float.MaxValue;
        float maxY = float.MinValue;
        float minY = float.MaxValue;

        foreach (MapNode node in MapManager.Instance.allNodes)
        {
            maxX = Mathf.Max(maxX, node.position.x + NODE_SIZE);
            minX = Mathf.Min(minX, node.position.x - NODE_SIZE);
            maxY = Mathf.Max(maxY, node.position.y + NODE_SIZE);
            minY = Mathf.Min(minY, node.position.y - NODE_SIZE);
        }

        float width = CONTENT_WIDTH;
        float height = maxY - minY + 100f;

        Debug.Log($"[MapPanel] Viewport size: {mapScrollRect.viewport.rect.width} x {mapScrollRect.viewport.rect.height}");
        Debug.Log($"[MapPanel] Node bounds: minX={minX:F1}, maxX={maxX:F1}, minY={minY:F1}, maxY={maxY:F1}");
        Debug.Log($"[MapPanel] Content size: {width:F1} x {height:F1}");

        mapContent.anchorMin = new Vector2(0, 0);
        mapContent.anchorMax = new Vector2(1, 0);
        mapContent.pivot = new Vector2(0, 0);
        mapContent.sizeDelta = new Vector2(0, height);
        mapContent.anchoredPosition = new Vector2(0, 0);

        Debug.Log($"[MapPanel] Content after: anchorMin={mapContent.anchorMin}, sizeDelta={mapContent.sizeDelta}");
    }

    /// <summary>
    /// 滚动到起始节点位置
    /// </summary>
    private void ScrollToStartNode()
    {
        if (mapScrollRect == null || mapContent == null)
        {
            Debug.LogError("[MapPanel] ScrollToStartNode - missing references!");
            return;
        }

        Invoke(nameof(SetScrollPosition), 0.1f);
    }

    /// <summary>
    /// 设置滚动位置（首次生成地图滚动到底部，否则恢复之前位置）
    /// </summary>
    private void SetScrollPosition()
    {
        if (mapScrollRect != null)
        {
            if (isFirstMap || !MapManager.Instance.isMapGenerated)
            {
                mapScrollRect.normalizedPosition = new Vector2(0.5f, 0f);
                Debug.Log($"[MapPanel] Scrolled to bottom (start nodes) - first map");
                isFirstMap = false;
            }
            else
            {
                mapScrollRect.normalizedPosition = savedScrollPosition;
                Debug.Log($"[MapPanel] Restored scroll position: {savedScrollPosition}");
            }
        }
    }

    /// <summary>
    /// 面板退出时调用，清空地图
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
        ClearMap();
    }

    /// <summary>
    /// 面板暂停时调用（仅 Window 层使用，Panel 可留作备用）
    /// </summary>
    public override void OnPause()
    {
        base.OnPause();
    }

    /// <summary>
    /// 面板恢复时调用（仅 Window 层使用，Panel 可留作备用）
    /// </summary>
    public override void OnResume()
    {
        base.OnResume();
    }

    /// <summary>
    /// 重置滚动位置，用于新游戏开始时
    /// </summary>
    public void ResetScrollPosition()
    {
        isFirstMap = true;
        savedScrollPosition = new Vector2(0.5f, 0f);
        Debug.Log("[MapPanel] Scroll position reset for new game");
    }
}
