using UnityEngine;
using System.Collections.Generic;

public class HandLayoutHelper : MonoBehaviour
{
    [Header("手牌布局参数（仿杀戮尖塔）")]
    public float cardWidth = 50f;      // 单张卡牌宽度
    public float baseOffsetX = 30f;     // 每张卡牌横向错开距离
    public float maxRotateAngle = 8f;   // 加大旋转，扇形层次更明显
    public float verticalRaise = 18f;    // 加大下沉幅度，圆弧更清晰
    public float maxHandWidth = 1200f;  // 手牌总最大宽度，超出自动缩小间距
    [Header("自动适配Viewport")]
    public RectTransform viewportRect;  // 绑定层级里的Viewport子物体
    public float verticalPadding = 80f; // 加大上下留白，防止裁切扇形两端

    private List<RectTransform> cardList = new List<RectTransform>();

    // 外部调用：刷新所有手牌位置
    public void RefreshHandLayout(List<GameObject> activeCardObjects)
    {
        Debug.Log("布局已刷新");
        Debug.Log($"子物体总数: {activeCardObjects.Count}");
        cardList.Clear();
        // 收集所有子卡牌（原有卡牌收集逻辑完全不变）
        foreach (GameObject cardObj in activeCardObjects)
        {
            RectTransform rect = cardObj.GetComponent<RectTransform>();
            if (rect != null) cardList.Add(rect);
            
        }
        Debug.Log($"cardList子物体总数: {cardList.Count}");

        int totalCount = cardList.Count;
        if (totalCount == 0)
        {
            // 无手牌时收缩Viewport高度
            if (viewportRect != null)
                viewportRect.sizeDelta = new Vector2(viewportRect.sizeDelta.x, 100);
            return;
        }
        // 浮点中心，保证所有牌以组为中心对称
        float centerF = (totalCount - 1) / 2f;
        // 横向间距计算
        float totalRawWidth = (totalCount - 1) * baseOffsetX;
        float realOffset = baseOffsetX;
        if (totalRawWidth > maxHandWidth)
            realOffset = maxHandWidth / (totalCount - 1);

        // ========= 修复横向居中算法 =========
        float totalSpan = (totalCount - 1) * realOffset;
        // 起点 = 总宽度一半向左偏移，保证整套卡牌中心点在Content原点(0,0)
        float startX = -totalSpan / 2f;
        // ======================================

        int centerIndex = (totalCount - 1) / 2;

        // 记录卡牌上下极值（局部Y坐标）
        float minCardY = float.MaxValue;
        float maxCardY = float.MinValue;

        for (int i = 0; i < totalCount; i++)
        {
            //RectTransform cardRect = cardList[i];
            //float xPos = startX + i * realOffset;
            //float distanceToCenter = Mathf.Abs(i - centerIndex);

            //// ========== 仅这里改动，换成平滑圆弧公式，其余全部沿用你原有代码 ==========
            //float normalizeDist = distanceToCenter / (centerIndex == 0 ? 1f : centerIndex);
            //// 平方曲线，模拟杀戮尖塔柔和扇形
            //float curveDrop = normalizeDist * normalizeDist * verticalRaise;
            //// 整体基准抬高，预留下沉空间，不会贴底被Mask切掉
            //float yPos = 20f - curveDrop;
            //// =======================================================================

            //// 更新卡牌上下边界（原逻辑不动）
            //float cardHalfHeight = cardRect.sizeDelta.y / 2f;
            //float cardTop = yPos + cardHalfHeight;
            //float cardBottom = yPos - cardHalfHeight;
            //maxCardY = Mathf.Max(maxCardY, cardTop);
            //minCardY = Mathf.Min(minCardY, cardBottom);

            //// 设置卡牌位置旋转（旋转逻辑完全保留，仅参数面板调大角度）
            //cardRect.anchoredPosition = new Vector2(xPos, yPos);
            //float rotateRatio = (i - centerIndex) / (float)centerIndex;
            //float rotateAngle = -rotateRatio * maxRotateAngle;
            //cardRect.localEulerAngles = new Vector3(0, 0, rotateAngle);
            //cardRect.SetSiblingIndex(i);
            RectTransform cardRect = cardList[i];
            float xPos = startX + i * realOffset;
            // 用浮点差值代替整数中心索引
            float distMid = i - centerF;                     // 可正可负
            float absDist = Mathf.Abs(distMid);
            float normalizeDist = absDist / (centerF == 0 ? 1f : centerF);
            float curveDrop = normalizeDist * normalizeDist * verticalRaise;
            float yPos = 20f - curveDrop;
            // 边界计算不变
            float cardHalfHeight = cardRect.sizeDelta.y / 2f;
            float cardTop = yPos + cardHalfHeight;
            float cardBottom = yPos - cardHalfHeight;
            maxCardY = Mathf.Max(maxCardY, cardTop);
            minCardY = Mathf.Min(minCardY, cardBottom);

            cardRect.anchoredPosition = new Vector2(xPos, yPos);
            float rotateAngle = -distMid / (centerF == 0 ? 1f : centerF) * maxRotateAngle;
            cardRect.localRotation = Quaternion.Euler(0, 0, rotateAngle);
            cardRect.localScale = Vector3.one;
            cardRect.SetSiblingIndex(i);

            var dragComp = cardRect.GetComponent<CardDrag>();
            if (dragComp != null)
                dragComp.OnLayoutRefreshed();
        }

        // 自动计算Viewport高度（原有逻辑完全不变）
        float totalCardHeight = maxCardY - minCardY;
        float targetViewportHeight = totalCardHeight + verticalPadding * 2;
        Vector2 viewportSize = viewportRect.sizeDelta;
        viewportSize.y = targetViewportHeight;
        viewportRect.sizeDelta = viewportSize;
        //for (int i = 0; i < cardList.Count; i++)
        //{
        //    Debug.Log($"第{i}张卡牌 X={cardList[i].anchoredPosition.x}");
        //}
    }

}