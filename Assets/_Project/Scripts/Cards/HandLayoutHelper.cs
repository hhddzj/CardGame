using UnityEngine;
using System.Collections.Generic;

public class HandLayoutHelper : MonoBehaviour
{
    [Header("手牌布局参数（仿杀戮尖塔）")]
    public float cardWidth = 50f;
    public float baseOffsetX = 30f;
    public float maxRotateAngle = 8f;
    public float verticalRaise = 18f;
    public float maxHandWidth = 1200f;
    [Header("自动适配Viewport")]
    public RectTransform viewportRect;
    public RectTransform contentRect;
    public float verticalPadding = 80f;

    private List<RectTransform> cardList = new List<RectTransform>();

    public void RefreshHandLayout(List<GameObject> activeCardObjects)
    {
        Debug.Log("布局已刷新");
        Debug.Log($"子物体总数: {activeCardObjects.Count}");
        cardList.Clear();

        foreach (GameObject cardObj in activeCardObjects)
        {
            RectTransform rect = cardObj.GetComponent<RectTransform>();
            if (rect != null) cardList.Add(rect);
        }
        Debug.Log($"cardList子物体总数: {cardList.Count}");

        int totalCount = cardList.Count;

        if (totalCount == 0)
        {
            if (viewportRect != null)
                viewportRect.sizeDelta = new Vector2(viewportRect.sizeDelta.x, 100f);
            if (contentRect != null)
                contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, 100f);
            return;
        }

        float centerF = (totalCount - 1) / 2f;
        float totalRawWidth = (totalCount - 1) * baseOffsetX;
        float realOffset = baseOffsetX;
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

            float currentHalfHeight = Mathf.Max(cardRect.sizeDelta.y / 2f, 50f);
            float cardTop = yPos + currentHalfHeight;
            float cardBottom = yPos - currentHalfHeight;
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
