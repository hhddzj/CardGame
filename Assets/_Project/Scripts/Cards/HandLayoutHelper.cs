using UnityEngine;
using System.Collections.Generic;

public class HandLayoutHelper : MonoBehaviour
{
    [Header("手牌布局参数（仿杀戮尖塔）")]
    public float cardWidth = 160f;      // 单张卡牌宽度
    public float baseOffsetX = 80f;     // 每张卡牌横向错开距离
    public float maxRotateAngle = 8f;   // 最大左右旋转角度
    public float verticalRaise = 15f;   // 中间卡牌向上抬高
    public float maxHandWidth = 1400f;  // 手牌总最大宽度，超出自动缩小间距

    private List<RectTransform> cardList = new List<RectTransform>();

    // 外部调用：刷新所有手牌位置
    public void RefreshHandLayout()
    {
        cardList.Clear();
        // 收集所有子卡牌
        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform rect = transform.GetChild(i).GetComponent<RectTransform>();
            if (rect != null) cardList.Add(rect);
        }

        int totalCount = cardList.Count;
        if (totalCount == 0) return;

        // 总基础宽度
        float totalRawWidth = (totalCount - 1) * baseOffsetX;
        float realOffset = baseOffsetX;

        // 手牌过多时缩小间距，防止超出屏幕
        if (totalRawWidth > maxHandWidth)
        {
            realOffset = maxHandWidth / (totalCount - 1);
        }

        // 第一张卡牌起始X，整体居中
        float startX = -((totalCount - 1) * realOffset) / 2f;
        int centerIndex = totalCount / 2;

        for (int i = 0; i < totalCount; i++)
        {
            RectTransform cardRect = cardList[i];
            float xPos = startX + i * realOffset;
            // 离中心越远，向下偏移
            float distanceToCenter = Mathf.Abs(i - centerIndex);
            float yPos = -distanceToCenter * verticalRaise;

            // 左右旋转：中间0度，两边正负maxRotateAngle
            float rotateRatio = (i - centerIndex) / (float)centerIndex;
            float rotateAngle = rotateRatio * maxRotateAngle;

            // 设置位置与旋转
            cardRect.anchoredPosition = new Vector2(xPos, yPos);
            cardRect.localEulerAngles = new Vector3(0, 0, rotateAngle);

            // 层级：中间卡牌在上，两边在下，不会被挡住
            cardRect.SetSiblingIndex(i);
        }
    }
}