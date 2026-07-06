using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private TextMeshProUGUI damageText;
    private RectTransform rect;

    public void ShowDamage(int damage, bool isBlock = false)
    {
        if (damageText == null)
            damageText = GetComponent<TextMeshProUGUI>();
        if (rect == null)
            rect = GetComponent<RectTransform>();

        if (damageText == null || rect == null)
            return;

        damageText.text = damage.ToString();
        damageText.color = isBlock ? Color.green : Color.red;
        damageText.alpha = 1f;

        rect.localScale = Vector3.zero;
        rect.DOScale(Vector3.one * 1.5f, 0.3f).SetEase(Ease.OutBack);
        rect.DOAnchorPosY(rect.anchoredPosition.y + 50f, 0.8f).SetEase(Ease.OutQuad);

        damageText.DOFade(0f, 0.8f).SetDelay(0.4f).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    public static DamagePopup CreateForEnemy(Transform enemyTransform)
    {
        return CreateAtParent("EnemyTerritory", enemyTransform);
    }

    public static DamagePopup CreateForPlayer(Transform playerTransform)
    {
        return CreateAtParent("Player", playerTransform);
    }

    private static DamagePopup CreateAtParent(string parentName, Transform targetTransform)
    {
        if (targetTransform == null)
            return null;

        Transform parent = GameObject.Find(parentName)?.transform;
        if (parent == null)
            return null;

        GameObject popupObj = new GameObject("DamagePopup");
        popupObj.transform.SetParent(parent, false);
        popupObj.transform.SetAsLastSibling();

        RectTransform rect = popupObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(100, 50);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        Vector3 localPos = parent.InverseTransformPoint(targetTransform.position);
        rect.anchoredPosition = new Vector2(localPos.x, localPos.y + 80f);

        TextMeshProUGUI text = popupObj.AddComponent<TextMeshProUGUI>();
        text.text = "0";
        text.fontSize = 48;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.red;
        text.outlineWidth = 0.2f;
        text.outlineColor = Color.black;

        DamagePopup popup = popupObj.AddComponent<DamagePopup>();
        popup.damageText = text;
        popup.rect = rect;

        return popup;
    }
}
