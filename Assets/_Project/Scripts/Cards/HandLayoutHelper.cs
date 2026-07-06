using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 手牌布局辅助器，负责将手牌排列成类似杀戮尖塔的弧形布局
/// </summary>
public class HandLayoutHelper : MonoBehaviour
{
    [Header("手牌布局参数（仿杀戮尖塔）")]

    /// <summary>
    /// 单张卡牌的宽度
    /// </summary>
    public float cardWidth = 50f;

    /// <summary>
    /// 卡牌之间的基础水平偏移量
    /// </summary>
    public float baseOffsetX = 30f;

    /// <summary>
    /// 卡牌最大旋转角度（边缘卡牌向外倾斜）
    /// </summary>
    public float maxRotateAngle = 8f;

    /// <summary>
    /// 垂直提升量（边缘卡牌下沉的幅度）
    /// </summary>
    public float verticalRaise = 18f;

    /// <summary>
    /// 手牌最大宽度限制
    /// </summary>
    public float maxHandWidth = 1200f;

    [Header("自动适配Viewport")]

    /// <summary>
    /// 视口RectTransform，用于调整视口高度以容纳所有卡牌
    /// </summary>
    public RectTransform viewportRect;

    /// <summary>
    /// 内容RectTransform，用于调整内容区域高度
    /// </summary>
    public RectTransform contentRect;

    /// <summary>
    /// 垂直方向的留白量
    /// </summary>
    public float verticalPadding = 80f;

    /// <summary>
    /// 卡牌RectTransform列表，用于布局计算
    /// </summary>
    private List<RectTransform> cardList = new List<RectTransform>();

    /// <summary>
    /// 刷新手牌布局，将手牌排列成弧形布局
    /// </summary>
    /// <param name="activeCardObjects">当前手牌对象列表</param>
    public void RefreshHandLayout(List<GameObject> activeCardObjects)
    {
        Debug.Log("布局已刷新");
        Debug.Log($"子物体总数: {activeCardObjects.Count}");
        cardList.Clear();

        foreach (GameObject cardObj in activeCardObjects)
        {
            if (cardObj == null)
            {
                Debug.LogError("[HandLayoutHelper] cardObj is null!");
                continue;
            }
            RectTransform rect = cardObj.GetComponent<RectTransform>();
            if (rect != null)
            {
                cardList.Add(rect);
            }
            else
            {
                Debug.LogError($"[HandLayoutHelper] cardObj {cardObj.name} has no RectTransform!");
            }
        }
        Debug.Log($"cardList子物体总数: {cardList.Count}");

        int totalCount = cardList.Count;

        // 处理空手牌情况
        if (totalCount == 0)
        {
            if (viewportRect != null)
                viewportRect.sizeDelta = new Vector2(viewportRect.sizeDelta.x, 100f);
            if (contentRect != null)
                contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, 100f);
            return;
        }

        // 计算布局参数
        float centerF = (totalCount - 1) / 2f;
        float totalRawWidth = (totalCount - 1) * baseOffsetX;
        float realOffset = baseOffsetX;

        // 如果手牌太宽超过限制，则压缩间距
        if (totalCount > 1 && totalRawWidth > maxHandWidth)
            realOffset = maxHandWidth / (totalCount - 1);

        float totalSpan = (totalCount - 1) * realOffset;
        float startX = -totalSpan / 2f;

        float minCardY = float.MaxValue;
        float maxCardY = float.MinValue;
        float cardHalfHeight = 0f;

        if (totalCount > 0 && cardList[0] != null)
        {
            cardHalfHeight = Mathf.Max(cardList[0].sizeDelta.y / 2f, 50f);
        }

        // 逐个设置卡牌位置和旋转
        for (int i = 0; i < totalCount; i++)
        {
            RectTransform cardRect = cardList[i];
            if (cardRect == null) continue;

            float xPos = startX + i * realOffset;
            float distMid = i - centerF;
            float absDist = Mathf.Abs(distMid);
            float normalizeDist = absDist / (centerF == 0 ? 1f : centerF);
            float curveDrop = normalizeDist * normalizeDist * verticalRaise;
            float yPos = 20f - curveDrop;

            // 记录卡牌边界用于计算总高度
            float currentHalfHeight = Mathf.Max(cardRect.sizeDelta.y / 2f, 50f);
            float cardTop = yPos + currentHalfHeight;
            float cardBottom = yPos - currentHalfHeight;
            maxCardY = Mathf.Max(maxCardY, cardTop);
            minCardY = Mathf.Min(minCardY, cardBottom);

            // 设置卡牌位置和旋转
            cardRect.anchoredPosition = new Vector2(xPos, yPos);
            float rotateAngle = -distMid / (centerF == 0 ? 1f : centerF) * maxRotateAngle;
            cardRect.localRotation = Quaternion.Euler(0, 0, rotateAngle);
            cardRect.localScale = Vector3.one;
            cardRect.SetSiblingIndex(i);

            // 通知CardDrag组件布局已刷新
            var dragComp = cardRect.GetComponent<CardDrag>();
            if (dragComp != null)
            {
                RectTransform rect = dragComp.GetComponent<RectTransform>();
                if (rect != null)
                    dragComp.OnLayoutRefreshed();
                else
                    Debug.LogError($"[HandLayoutHelper] CardDrag rect is null! GameObject: {dragComp.gameObject.name}");
            }
        }

        // 调整Viewport和Content高度
        float totalCardHeight = Mathf.Max(maxCardY - minCardY, 0f);
        float targetViewportHeight = Mathf.Max(totalCardHeight + verticalPadding * 2, 100f);

        if (viewportRect != null)
        {
            Vector2 viewportSize = viewportRect.sizeDelta;
            viewportSize.y = targetViewportHeight;
            viewportRect.sizeDelta = viewportSize;
        }

        if (contentRect != null)
        {
            Vector2 contentSize = contentRect.sizeDelta;
            contentSize.y = targetViewportHeight;
            contentRect.sizeDelta = contentSize;
        }
    }
}
