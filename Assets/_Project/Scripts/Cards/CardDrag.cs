using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Card data;
    private RectTransform rect;
    private Canvas canvas;
    private Vector2 originPos; // 拖拽前原位
    private Transform originParent; // 拖拽前父物体（手牌Content）
    // 拖入手牌根物体HandAreaView
    public ScrollRect handScrollRect;
    private Vector2 startScrollPos;
    // 外部初始化卡牌数据
    public void SetCardData(Card cardData)
    {
        data = cardData;
    }
    private RectTransform _rt;
    private Vector2 _startPos;

    void Awake()
    {
        _rt = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 记录拖拽初始位置
        _startPos = _rt.anchoredPosition;
        // 拖拽时卡牌置顶，不被其他卡牌遮挡
        transform.SetAsLastSibling();
        // 拖拽时提升层级，显示在最上层
        rect.DOScale(1.15f, 0.1f); // 拖拽放大
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 只移动卡牌本身，容器完全不动
        RectTransform viewport = transform.parent.parent.GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, eventData.position, eventData.pressEventCamera, out localPoint);
        _rt.anchoredPosition = _startPos + (localPoint - eventData.pressPosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 拖拽结束卡牌回弹归位
        _rt.anchoredPosition = _startPos;
        // 这里可以加卡牌放下、攻击、弃牌逻辑
        rect.DOScale(1f, 0.1f); // 松开恢复大小

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
}
