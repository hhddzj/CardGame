using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public Card data;
    private RectTransform rect;
    private Canvas canvas;
    private Camera uiCam;

    // 鼠标偏移
    private Vector2 dragOffset;
    private Image cardImage;
    // 交互状态
    private bool isDragging;
    private bool isHovered;
    private bool justDropped;

    // 原始布局状态（只在非交互时由 OnLayoutRefreshed 或悬停/拖拽开始时更新）
    private Vector2 layoutAnchoredPos;
    private Transform layoutParent;
    private int layoutSiblingIndex;
    private Quaternion layoutRotation;
    private Vector3 layoutScale;

    public void SetCardData(Card cardData) => data = cardData;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        uiCam = (canvas.renderMode == RenderMode.ScreenSpaceCamera) ? canvas.worldCamera : null;
        cardImage = GetComponent<Image>();   // 新增
    }

    /// <summary>
    /// 由 HandLayoutHelper 在每次布局后调用，用于重置卡牌的视觉状态并记录新的布局状态。
    /// </summary>
    public void OnLayoutRefreshed()
    {
        if (isDragging || isHovered || justDropped) return;

        // 重置缩放并记录当前布局状态
        rect.localScale = Vector3.one;
        SaveLayoutState();
    }

    /// <summary>
    /// 保存当前卡牌的布局状态（父级、位置、层级、旋转、缩放）。
    /// 必须在卡牌未被移动父级时调用。
    /// </summary>
    void SaveLayoutState()
    {
        layoutParent = transform.parent;
        layoutAnchoredPos = rect.anchoredPosition;
        layoutSiblingIndex = transform.GetSiblingIndex();
        layoutRotation = rect.localRotation;
        layoutScale = rect.localScale;
    }

    /// <summary>
    /// 悬停和拖拽共用的视觉效果：放大、回正、脱离遮罩。
    /// </summary>
    void ApplyHoverEffect(float targetScale)
    {
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
    /// 将卡牌恢复到布局状态（父级、位置、层级、旋转、缩放）。
    /// </summary>
    void RestoreToLayout()
    {
        rect.DOKill();

        // 先切回原始父级
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
        // 恢复射线检测
        if (cardImage != null) cardImage.raycastTarget = true;
    }

    // ---------- 悬停 ----------
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging) return;
        isHovered = true;

        SaveLayoutState();          // 记录当前布局状态（此时还未移动父级）
        ApplyHoverEffect(1.15f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDragging) return;
        isHovered = false;
        RestoreToLayout();
    }

    // ---------- 拖拽 ----------
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        // 关键修复：如果当前正处于悬停状态，说明悬停时已经保存了正确的布局状态，
        // 不要再保存，否则会记录到被悬停改变后的 Canvas 坐标。
        if (!isHovered)
        {
            SaveLayoutState();
        }
        isHovered = false; // 正式进入拖拽，清除悬停标记
        // 关闭自身射线，让射线能穿透卡牌击中怪物
        if (cardImage != null) cardImage.raycastTarget = false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, uiCam, out dragOffset);
        ApplyHoverEffect(1.15f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 canvasLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, eventData.position, uiCam, out canvasLocal);
        rect.anchoredPosition = canvasLocal - dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        // 检测是否丢到敌人身上
        GameObject hitObj = eventData.pointerEnter;
        Enemy targetEnemy = hitObj?.GetComponent<Enemy>();
        Debug.Log($"targetEnemy: {targetEnemy}");
        Debug.Log("状态=" + BattleManager.Instance.currentState);
        Debug.Log($"能量检查: 当前能量={BattleManager.Instance.player.energy}, 卡牌费用={data.cost}");
        if (targetEnemy != null && BattleManager.Instance.player.energy >= data.cost)
        {
            BattleManager.Instance.PlayCard(data, targetEnemy);
            BattleUIManager.Instance.RefreshAllUI();
            return;
        }

        // 未打出：恢复布局状态，并短暂保护防止意外刷新
        RestoreToLayout();
        StartCoroutine(ResetDropProtection());
    }

    IEnumerator ResetDropProtection()
    {
        justDropped = true;
        yield return new WaitForSeconds(0.2f);
        justDropped = false;
    }
}