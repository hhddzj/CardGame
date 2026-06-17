using UnityEngine;
using System.Collections.Generic;

public class HandLayoutHelper : MonoBehaviour
{
    [Header("手牌布局参数（仿杀戮尖塔）")]
    public float cardWidth = 50f;      // 单张卡牌宽度
    public float baseOffsetX = 30f;     // 每张卡牌横向错开距离
    public float maxRotateAngle = 5f;   // 最大左右旋转角度
    public float verticalRaise = 6f;   // 中间卡牌向上抬高
    public float maxHandWidth = 1200f;  // 手牌总最大宽度，超出自动缩小间距
    [Header("自动适配Viewport")]
    public RectTransform viewportRect; // 拖拽你的 HandAreaView（Viewport）
    public float verticalPadding = 15f; // 上下预留边距，防止卡牌贴边被遮罩切掉
    public float hegit = 15f; // 上下预留边距，防止卡牌贴边被遮罩切掉

    private List<RectTransform> cardList = new List<RectTransform>();


    // 外部调用：刷新所有手牌位置
    public void RefreshHandLayout()
    {
        Debug.Log("布局已刷新");
        cardList.Clear();
        // 收集所有子卡牌
        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform rect = transform.GetChild(i).GetComponent<RectTransform>();
            if (rect != null) cardList.Add(rect);
        }

        int totalCount = cardList.Count;
        if (totalCount == 0)
        {
            // 无手牌时收缩Viewport高度
            if (viewportRect != null)
                viewportRect.sizeDelta = new Vector2(viewportRect.sizeDelta.x, 100);
            return;
        }

        // 横向间距计算
        float totalRawWidth = (totalCount - 1) * baseOffsetX;
        float realOffset = baseOffsetX;
        if (totalRawWidth > maxHandWidth)
            realOffset = maxHandWidth / (totalCount - 1);

        float startX = -((totalCount - 1) * realOffset) / 2f;
        int centerIndex = totalCount / 2;

        // 记录卡牌上下极值（局部Y坐标）
        float minCardY = float.MaxValue;
        float maxCardY = float.MinValue;

        for (int i = 0; i < totalCount; i++)
        {
            RectTransform cardRect = cardList[i];
            float xPos = startX + i * realOffset;
            float distanceToCenter = Mathf.Abs(i - centerIndex);
            float yPos = -distanceToCenter * verticalRaise+hegit;

            // 更新卡牌上下边界
            float cardHalfHeight = cardRect.sizeDelta.y / 2f;
            float cardTop = yPos + cardHalfHeight;
            float cardBottom = yPos - cardHalfHeight;
            maxCardY = Mathf.Max(maxCardY, cardTop);
            minCardY = Mathf.Min(minCardY, cardBottom);

            // 设置卡牌位置旋转
            cardRect.anchoredPosition = new Vector2(xPos, yPos);
            float rotateRatio = (i - centerIndex) / (float)centerIndex;
            float rotateAngle = -rotateRatio * maxRotateAngle;
            cardRect.localEulerAngles = new Vector3(0, 0, rotateAngle);
            cardRect.SetSiblingIndex(i);
        }

        // ========== 自动计算Viewport高度 ==========
        float totalCardHeight = maxCardY - minCardY;
        float targetViewportHeight = totalCardHeight + verticalPadding * 2;
        Vector2 viewportSize = viewportRect.sizeDelta;
        viewportSize.y = targetViewportHeight;
        viewportRect.sizeDelta = viewportSize;

        // 垂直居中Viewport，让扇形手牌刚好卡在可视区中间
        float centerOffset = (maxCardY + minCardY) / 2f;
        viewportRect.anchoredPosition = new Vector2(viewportRect.anchoredPosition.x, centerOffset);
    }
}