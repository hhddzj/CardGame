using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [Header("基础属性")]
    public int maxHealth;
    public int currentHealth;
    public int block;

    [Header("UI绑定")]
    public Image hpImage;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI blockText;
    public Transform popupParent;

    private DamagePopup damagePopup;

    protected virtual void Awake()
    {
        if (popupParent == null)
        {
            popupParent = transform;
        }
    }

    protected virtual void Start()
    {
        if (currentHealth == 0 && maxHealth > 0)
        {
            currentHealth = maxHealth;
        }
        UpdateHealthUI();
        UpdateBlockUI();
    }

    public virtual void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        int remainingDamage = amount;
        int blockDamage = 0;

        if (block > 0)
        {
            if (amount >= block)
            {
                blockDamage = block;
                remainingDamage = amount - block;
                block = 0;
            }
            else
            {
                blockDamage = amount;
                block -= amount;
                remainingDamage = 0;
            }
        }

        if (blockDamage > 0)
        {
            ShowDamagePopup(blockDamage, true);
        }

        currentHealth -= remainingDamage;
        if (currentHealth < 0)
            currentHealth = 0;

        if (remainingDamage > 0)
        {
            ShowDamagePopup(remainingDamage, false);
        }

        UpdateHealthUI();
        UpdateBlockUI();
    }

    public void AddBlock(int amount)
    {
        if (amount <= 0) return;
        block += amount;
        ShowDamagePopup(amount, true);
        UpdateBlockUI();
    }

    public void ClearBlock()
    {
        block = 0;
        UpdateBlockUI();
    }

    protected void UpdateHealthUI()
    {
        if (hpText != null)
        {
            hpText.text = $"{currentHealth}/{maxHealth}";
        }

        if (hpImage != null && maxHealth > 0)
        {
            float fillRatio = (float)currentHealth / maxHealth;
            fillRatio = Mathf.Clamp(fillRatio, 0f, 1f);

            if (!float.IsNaN(fillRatio) && !float.IsInfinity(fillRatio))
            {
                hpImage.DOFillAmount(fillRatio, 0.3f);

                if (fillRatio <= 0.3f)
                    hpImage.DOColor(new Color(1f, 0.3f, 0.3f), 0.3f);
                else if (fillRatio <= 0.6f)
                    hpImage.DOColor(new Color(1f, 0.8f, 0.3f), 0.3f);
                else
                    hpImage.DOColor(new Color(0.3f, 1f, 0.3f), 0.3f);
            }
        }
    }

    protected void UpdateBlockUI()
    {
        if (blockText != null)
        {
            bool hasBlock = block > 0;
            blockText.text = hasBlock ? $"🛡️ {block}" : "";
            blockText.gameObject.SetActive(hasBlock);
        }
    }

    protected void ShowDamagePopup(int amount, bool isBlock)
    {
        if (damagePopup == null)
        {
            Transform parent = popupParent != null ? popupParent : transform;
            damagePopup = DamagePopup.Create(parent);
        }
        damagePopup.ShowDamage(transform.position, amount, isBlock);
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    public float GetHealthPercent()
    {
        return (float)currentHealth / maxHealth;
    }
}
