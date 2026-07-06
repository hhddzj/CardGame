using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 可拖拽窗口组件，实现窗口的拖拽移动功能
/// </summary>
public class DraggableWindow : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler
{
    [Header("拖拽设置")]

    /// <summary>
    /// 是否限制窗口在屏幕内移动
    /// </summary>
    [SerializeField] private bool limitToScreen = true;

    /// <summary>
    /// 边缘留白距离（像素）
    /// </summary>
    [SerializeField] private float edgePadding = 20f;

    /// <summary>
    /// 当前物体的RectTransform组件
    /// </summary>
    private RectTransform rectTransform;

    /// <summary>
    /// 父级Canvas组件
    /// </summary>
    private Canvas parentCanvas;

    /// <summary>
    /// 鼠标与窗口位置的偏移量
    /// </summary>
    private Vector2 offset;

    /// <summary>
    /// 初始化，获取RectTransform和父级Canvas组件
    /// </summary>
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    /// <summary>
    /// 点击窗口时，将其置顶显示
    /// </summary>
    /// <param name="eventData">指针事件数据</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        var window = GetComponent<UIWindow>();
        if (window != null && UIFrame.Instance != null)
        {
            UIFrame.Instance.BringWindowToFront(window);
        }
    }

    /// <summary>
    /// 开始拖拽：记录鼠标与窗口的偏移量
    /// </summary>
    /// <param name="eventData">指针事件数据</param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (rectTransform == null) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPointerPos))
        {
            offset = rectTransform.anchoredPosition - localPointerPos;
        }
    }

    /// <summary>
    /// 拖拽中：更新窗口位置
    /// </summary>
    /// <param name="eventData">指针事件数据</param>
    public void OnDrag(PointerEventData eventData)
    {
        if (rectTransform == null || rectTransform.parent == null) return;

        RectTransform parentRect = rectTransform.parent as RectTransform;
        if (parentRect == null) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, eventData.position, eventData.pressEventCamera, out Vector2 localPointerPos))
        {
            Vector2 newPosition = localPointerPos + offset;

            if (limitToScreen && parentRect != null)
            {
                newPosition = ClampToParent(newPosition, parentRect);
            }

            rectTransform.anchoredPosition = newPosition;
        }
    }

    /// <summary>
    /// 将位置限制在父物体的矩形范围内，考虑窗口大小和边缘留白
    /// </summary>
    /// <param name="position">目标位置</param>
    /// <param name="parent">父级RectTransform</param>
    /// <returns>限制后的位置</returns>
    private Vector2 ClampToParent(Vector2 position, RectTransform parent)
    {
        Vector2 parentSize = parent.rect.size;
        Vector2 halfSize = rectTransform.rect.size * 0.5f;
        float scale = parentCanvas ? parentCanvas.scaleFactor : 1f;

        float minX = -parentSize.x * 0.5f + halfSize.x + edgePadding / scale;
        float maxX = parentSize.x * 0.5f - halfSize.x - edgePadding / scale;
        float minY = -parentSize.y * 0.5f + halfSize.y + edgePadding / scale;
        float maxY = parentSize.y * 0.5f - halfSize.y - edgePadding / scale;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);
        return position;
    }
}