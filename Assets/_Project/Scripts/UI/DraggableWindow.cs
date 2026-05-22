using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWindow : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler
{
    [Header("拖拽设置")]
    [SerializeField] private bool limitToScreen = true;   // 是否限制在屏幕内
    [SerializeField] private float edgePadding = 20f;     // 边缘留白

    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private Vector2 offset;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    /// <summary> 点击窗口时，将其置顶 </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        var window = GetComponent<UIWindow>();
        if (window != null && UIFrame.Instance != null)
        {
            UIFrame.Instance.BringWindowToFront(window);
        }
    }

    /// <summary> 开始拖拽：记录鼠标与窗口的偏移 </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (rectTransform == null) return;

        // 将屏幕坐标转换为本地坐标，计算偏移
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPointerPos))
        {
            offset = rectTransform.anchoredPosition - localPointerPos;
        }
    }

    /// <summary> 拖拽中：更新窗口位置 </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (rectTransform == null || rectTransform.parent == null) return;

        RectTransform parentRect = rectTransform.parent as RectTransform;
        if (parentRect == null) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, eventData.position, eventData.pressEventCamera, out Vector2 localPointerPos))
        {
            Vector2 newPosition = localPointerPos + offset;

            // 限制在父物体（通常是 Canvas）内
            if (limitToScreen && parentRect != null)
            {
                newPosition = ClampToParent(newPosition, parentRect);
            }

            rectTransform.anchoredPosition = newPosition;
        }
    }

    /// <summary>
    /// 将位置限制在父物体的矩形范围内，考虑窗口大小和边缘留白。
    /// </summary>
    private Vector2 ClampToParent(Vector2 position, RectTransform parent)
    {
        Vector2 parentSize = parent.rect.size;
        Vector2 halfSize = rectTransform.rect.size * 0.5f;
        float scale = parentCanvas ? parentCanvas.scaleFactor : 1f;

        // 计算实际像素范围
        float minX = -parentSize.x * 0.5f + halfSize.x + edgePadding / scale;
        float maxX = parentSize.x * 0.5f - halfSize.x - edgePadding / scale;
        float minY = -parentSize.y * 0.5f + halfSize.y + edgePadding / scale;
        float maxY = parentSize.y * 0.5f - halfSize.y - edgePadding / scale;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);
        return position;
    }
}