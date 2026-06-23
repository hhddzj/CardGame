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

    private Vector2 dragOffset;
    private Image cardImage;
    private bool isDragging;
    private bool isHovered;
    private bool justDropped;

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
        cardImage = GetComponent<Image>();
    }

    public void OnLayoutRefreshed()
    {
        if (isDragging || isHovered || justDropped) return;

        rect.localScale = Vector3.one;
        SaveLayoutState();
    }

    void SaveLayoutState()
    {
        if (rect == null || transform.parent == null) return;
        
        layoutParent = transform.parent;
        layoutAnchoredPos = rect.anchoredPosition;
        layoutSiblingIndex = transform.GetSiblingIndex();
        layoutRotation = rect.localRotation;
        layoutScale = rect.localScale;
    }

    void ApplyHoverEffect(float targetScale)
    {
        if (rect == null || canvas == null) return;
        
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

    void RestoreToLayout()
    {
        if (rect == null || layoutParent == null) return;
        
        rect.DOKill();

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
        
        if (cardImage != null) cardImage.raycastTarget = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging || data == null) return;
        isHovered = true;

        SaveLayoutState();
        ApplyHoverEffect(1.15f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDragging) return;
        isHovered = false;
        RestoreToLayout();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (data == null) return;
        
        isDragging = true;

        if (!isHovered)
        {
            SaveLayoutState();
        }
        isHovered = false;
        
        if (cardImage != null) cardImage.raycastTarget = false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, uiCam, out dragOffset);
        ApplyHoverEffect(1.15f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || rect == null || canvas == null) return;
        
        Vector2 canvasLocal;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, eventData.position, uiCam, out canvasLocal))
        {
            rect.anchoredPosition = canvasLocal - dragOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        if (data == null)
        {
            RestoreToLayout();
            return;
        }

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
