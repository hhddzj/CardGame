using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 卡牌拖拽处理组件，负责卡牌的拖拽、悬停和放置逻辑
/// 实现了IBeginDragHandler、IDragHandler、IEndDragHandler、IPointerEnterHandler、IPointerExitHandler接口
/// </summary>
public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// 卡牌数据，存储卡牌的名称、费用、描述等信息
    /// </summary>
    [HideInInspector] public Card data;

    /// <summary>
    /// 卡牌的RectTransform组件，用于位置和大小的控制
    /// </summary>
    private RectTransform rect;

    /// <summary>
    /// 父级Canvas组件，用于坐标转换
    /// </summary>
    private Canvas canvas;

    /// <summary>
    /// UI相机，用于屏幕坐标到UI坐标的转换
    /// </summary>
    private Camera uiCam;

    /// <summary>
    /// 鼠标点击位置与卡牌中心的偏移量
    /// </summary>
    private Vector2 dragOffset;

    /// <summary>
    /// 卡牌图片组件，用于设置射线检测和显示
    /// </summary>
    private Image cardImage;

    /// <summary>
    /// 是否正在拖拽中
    /// </summary>
    private bool isDragging;

    /// <summary>
    /// 是否正在悬停中
    /// </summary>
    private bool isHovered;

    /// <summary>
    /// 是否刚刚放置完成（用于防止布局刷新时立即重置位置）
    /// </summary>
    private bool justDropped;

    /// <summary>
    /// 原始布局状态 - 锚定位置
    /// </summary>
    private Vector2 layoutAnchoredPos;

    /// <summary>
    /// 原始布局状态 - 父级Transform
    /// </summary>
    private Transform layoutParent;

    /// <summary>
    /// 原始布局状态 - 兄弟节点索引
    /// </summary>
    private int layoutSiblingIndex;

    /// <summary>
    /// 原始布局状态 - 旋转角度
    /// </summary>
    private Quaternion layoutRotation;

    /// <summary>
    /// 原始布局状态 - 缩放比例
    /// </summary>
    private Vector3 layoutScale;

    /// <summary>
    /// 设置卡牌数据
    /// </summary>
    /// <param name="cardData">卡牌数据对象</param>
    public void SetCardData(Card cardData) => data = cardData;

    /// <summary>
    /// 初始化组件引用
    /// </summary>
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        uiCam = (canvas.renderMode == RenderMode.ScreenSpaceCamera) ? canvas.worldCamera : null;
        cardImage = GetComponent<Image>();
    }

    /// <summary>
    /// 布局刷新时调用，保存当前布局状态
    /// 仅在非拖拽、非悬停、非刚放置状态下生效
    /// </summary>
    public void OnLayoutRefreshed()
    {
        if (isDragging || isHovered || justDropped) return;
        if (rect == null)
        {
            rect = GetComponent<RectTransform>();
            if (rect == null)
            {
                Debug.LogError($"[CardDrag] OnLayoutRefreshed - rect is null after re-get! GameObject: {gameObject.name}, data: {data?.cardName ?? "null"}");
                return;
            }
            Debug.LogWarning($"[CardDrag] OnLayoutRefreshed - rect was null, re-got successfully! GameObject: {gameObject.name}");
        }

        rect.localScale = Vector3.one;
        SaveLayoutState();
    }

    /// <summary>
    /// 保存当前卡牌的布局状态，用于拖拽或悬停结束后恢复位置
    /// </summary>
    void SaveLayoutState()
    {
        if (rect == null)
        {
            Debug.LogError($"[CardDrag] SaveLayoutState - rect is null! GameObject: {gameObject.name}");
            return;
        }
        if (transform.parent == null)
        {
            Debug.LogError($"[CardDrag] SaveLayoutState - parent is null! GameObject: {gameObject.name}");
            return;
        }
        
        layoutParent = transform.parent;
        layoutAnchoredPos = rect.anchoredPosition;
        layoutSiblingIndex = transform.GetSiblingIndex();
        layoutRotation = rect.localRotation;
        layoutScale = rect.localScale;
    }

    /// <summary>
    /// 应用悬停效果，将卡牌移到Canvas顶层并放大显示
    /// </summary>
    /// <param name="targetScale">目标缩放比例</param>
    void ApplyHoverEffect(float targetScale)
    {
        if (rect == null)
        {
            Debug.LogError($"[CardDrag] ApplyHoverEffect - rect is null! GameObject: {gameObject.name}");
            return;
        }
        if (canvas == null)
        {
            Debug.LogError($"[CardDrag] ApplyHoverEffect - canvas is null! GameObject: {gameObject.name}");
            return;
        }
        
        rect.DOKill();

        Vector3 worldPos = rect.position;
        if (transform.parent != canvas.transform)
        {
            transform.SetParent(canvas.transform, worldPositionStays: false);
            rect.position = worldPos;
        }
        transform.SetAsLastSibling();
        rect.localRotation = Quaternion.identity;
        rect.DOScale(targetScale, 0.1f);
    }

    /// <summary>
    /// 将卡牌恢复到保存的布局状态
    /// </summary>
    void RestoreToLayout()
    {
        if (rect == null)
        {
            Debug.LogError($"[CardDrag] RestoreToLayout - rect is null! GameObject: {gameObject.name}");
            return;
        }
        if (layoutParent == null)
        {
            Debug.LogError($"[CardDrag] RestoreToLayout - layoutParent is null! GameObject: {gameObject.name}");
            return;
        }
        
        rect.DOKill();

        if (transform.parent != layoutParent)
        {
            Vector3 worldPos = rect.position;
            transform.SetParent(layoutParent, worldPositionStays: false);
            rect.position = worldPos;
        }

        rect.localRotation = layoutRotation;
        rect.DOScale(layoutScale, 0.1f);
        rect.anchoredPosition = layoutAnchoredPos;
        transform.SetSiblingIndex(layoutSiblingIndex);
        
        if (cardImage != null) cardImage.raycastTarget = true;
    }

    /// <summary>
    /// 鼠标进入卡牌时触发，应用悬停效果
    /// </summary>
    /// <param name="eventData">指针事件数据</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging) return;
        if (data == null)
        {
            Debug.LogError($"[CardDrag] OnPointerEnter - data is null! GameObject: {gameObject.name}");
            return;
        }
        if (rect == null)
        {
            Debug.LogError($"[CardDrag] OnPointerEnter - rect is null! GameObject: {gameObject.name}");
            return;
        }
        if (canvas == null)
        {
            Debug.LogError($"[CardDrag] OnPointerEnter - canvas is null! GameObject: {gameObject.name}");
            return;
        }
        isHovered = true;

        SaveLayoutState();
        ApplyHoverEffect(1.15f);
    }

    /// <summary>
    /// 鼠标离开卡牌时触发，恢复卡牌到原始布局
    /// </summary>
    /// <param name="eventData">指针事件数据</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDragging) return;
        if (rect == null)
        {
            Debug.LogError($"[CardDrag] OnPointerExit - rect is null! GameObject: {gameObject.name}");
            return;
        }
        isHovered = false;
        RestoreToLayout();
    }

    /// <summary>
    /// 开始拖拽时触发，记录拖拽偏移并应用悬停效果
    /// </summary>
    /// <param name="eventData">指针事件数据</param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (data == null)
        {
            Debug.LogError($"[CardDrag] OnBeginDrag - data is null! GameObject: {gameObject.name}");
            return;
        }
        if (rect == null)
        {
            Debug.LogError($"[CardDrag] OnBeginDrag - rect is null! GameObject: {gameObject.name}");
            return;
        }
        if (canvas == null)
        {
            Debug.LogError($"[CardDrag] OnBeginDrag - canvas is null! GameObject: {gameObject.name}");
            return;
        }
        
        isDragging = true;

        if (!isHovered)
        {
            SaveLayoutState();
        }
        isHovered = false;
        
        if (cardImage != null) cardImage.raycastTarget = false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, uiCam, out dragOffset);
        ApplyHoverEffect(1.15f);
    }

    /// <summary>
    /// 拖拽过程中触发，更新卡牌位置跟随鼠标
    /// </summary>
    /// <param name="eventData">指针事件数据</param>
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        if (rect == null)
        {
            Debug.LogError($"[CardDrag] OnDrag - rect is null! GameObject: {gameObject.name}");
            return;
        }
        if (canvas == null)
        {
            Debug.LogError($"[CardDrag] OnDrag - canvas is null! GameObject: {gameObject.name}");
            return;
        }
        
        Vector2 canvasLocal;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, eventData.position, uiCam, out canvasLocal))
        {
            rect.anchoredPosition = canvasLocal - dragOffset;
        }
    }

    /// <summary>
    /// 拖拽结束时触发，判断是否将卡牌放置到有效目标（敌人）上
    /// 如果放置到敌人且能量足够，则播放卡牌效果
    /// 否则恢复卡牌到原始布局
    /// </summary>
    /// <param name="eventData">指针事件数据</param>
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        if (data == null)
        {
            Debug.LogError($"[CardDrag] OnEndDrag - data is null! GameObject: {gameObject.name}");
            RestoreToLayout();
            return;
        }

        GameObject hitObj = eventData.pointerEnter;
        Enemy targetEnemy = hitObj?.GetComponent<Enemy>();
        
        Debug.Log($"targetEnemy: {targetEnemy}");
        
        if (BattleManager.Instance == null || BattleManager.Instance.player == null)
        {
            Debug.LogError("BattleManager or Player is null");
            RestoreToLayout();
            return;
        }
        
        Debug.Log("状态=" + BattleManager.Instance.currentState);
        Debug.Log($"能量检查: 当前能量={BattleManager.Instance.player.energy}, 卡牌费用={data.cost}");
        
        if (targetEnemy != null && BattleManager.Instance.player.energy >= data.cost)
        {
            BattleManager.Instance.PlayCard(data, targetEnemy);
            BattleUIManager.Instance.RefreshAllUI();
            return;
        }

        RestoreToLayout();
        StartCoroutine(ResetDropProtection());
    }

    /// <summary>
    /// 重置放置保护状态，防止布局刷新时立即重置卡牌位置
    /// </summary>
    /// <returns>协程迭代器</returns>
    IEnumerator ResetDropProtection()
    {
        justDropped = true;
        yield return new WaitForSeconds(0.2f);
        justDropped = false;
    }
}
