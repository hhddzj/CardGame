using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private Canvas canvas;

    private RectTransform rect;
    private Camera uiCamera;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        uiCamera = canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;
        gameObject.SetActive(false);
    }

    public void ShowDamage(Vector3 worldPosition, int damage, bool isBlock = false)
    {
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
            uiCamera = canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;
        }

        if (canvas == null) return;

        Vector2 screenPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Camera.main.WorldToScreenPoint(worldPosition),
            uiCamera,
            out screenPosition
        );

        damageText.text = (isBlock ? "🛡️ " : "") + damage.ToString();
        damageText.color = isBlock ? new Color(0.2f, 0.7f, 1f) : new Color(1f, 0.3f, 0.3f);

        rect.anchoredPosition = screenPosition;
        gameObject.SetActive(true);
        damageText.alpha = 1f;

        rect.localScale = Vector3.one;
        rect.DOScale(Vector3.one * 1.2f, 0.15f).SetEase(Ease.OutBack);
        rect.DOAnchorPosY(screenPosition.y + 80f, 0.8f).SetEase(Ease.OutQuad);

        damageText.DOFade(0f, 0.8f).SetDelay(0.2f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public static DamagePopup Create(Transform parent)
    {
        Canvas canvas = parent.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }

        GameObject popupObj = new GameObject("DamagePopup");
        
        if (canvas != null)
        {
            popupObj.transform.SetParent(canvas.transform, false);
        }
        else
        {
            popupObj.transform.SetParent(parent, false);
        }

        RectTransform rect = popupObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(100, 50);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        TextMeshProUGUI text = popupObj.AddComponent<TextMeshProUGUI>();
        text.text = "0";
        text.fontSize = 32;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.red;

        DamagePopup popup = popupObj.AddComponent<DamagePopup>();
        popup.damageText = text;
        popup.canvas = canvas;

        return popup;
    }
}
