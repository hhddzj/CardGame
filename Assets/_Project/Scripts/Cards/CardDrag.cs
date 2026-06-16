using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Card data;
    private RectTransform rect;
    private Canvas canvas;
    private Vector2 originPos; // 拖拽前原位
    private Transform originParent; // 拖拽前父物体（手牌Content）
    // 开始拖拽
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 保存原始位置、父物体
        originPos = rect.anchoredPosition;
        originParent = transform.parent;
        // 拖拽时提升层级，显示在最上层
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
        rect.DOScale(1.15f, 0.1f); // 拖拽放大
    }

    // 拖拽中跟随鼠标
    public void OnDrag(PointerEventData eventData)
    {
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    // 拖拽结束：判定是否拖到敌人区域
    public void OnEndDrag(PointerEventData eventData)
    {
        rect.DOScale(1f, 0.1f); // 松开恢复大小
        // 放回原位默认
        transform.SetParent(originParent);
        rect.anchoredPosition = originPos;

        // 检测鼠标下方是否有敌人碰撞体
        GameObject hitObj = eventData.pointerEnter;
        if (hitObj == null) return;

        Enemy targetEnemy = hitObj.GetComponent<Enemy>();
        if (targetEnemy == null) return;
        if (BattleManager.Instance.player.energy < data.cost)
        {
            Debug.Log("能量不足，无法打出");
            return;
        }
        // 拖拽到敌人，执行出牌
        BattleManager.Instance.PlayCard(data, targetEnemy);
        // 出牌后自动刷新UI
        BattleUIManager.Instance.RefreshAllUI();
    }

    // 外部初始化卡牌数据
    public void SetCardData(Card cardData)
    {
        data = cardData;
    }
}
