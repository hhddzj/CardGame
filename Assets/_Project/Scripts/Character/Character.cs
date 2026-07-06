using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色基类，包含血量、护盾和受伤逻辑
/// </summary>
public class Character : MonoBehaviour
{
    [Header("基础属性")]

    /// <summary>
    /// 最大生命值
    /// </summary>
    public int maxHealth;

    /// <summary>
    /// 当前生命值
    /// </summary>
    public int currentHealth;

    /// <summary>
    /// 护盾值，优先承受伤害
    /// </summary>
    public int block;

    [Header("UI绑定")]

    /// <summary>
    /// 血条图片组件
    /// </summary>
    public Image hpImage;

    /// <summary>
    /// 血量文字组件，显示"当前/最大"
    /// </summary>
    public TextMeshProUGUI hpText;

    /// <summary>
    /// 护盾文字组件，显示护盾数值
    /// </summary>
    public TextMeshProUGUI blockText;

    /// <summary>
    /// 初始化角色属性和UI
    /// </summary>
    protected virtual void Start()
    {
        if (currentHealth == 0 && maxHealth > 0)
        {
            currentHealth = maxHealth;
        }
        UpdateHealthUI();
        UpdateBlockUI();
    }

    /// <summary>
    /// 承受伤害，优先消耗护盾，剩余伤害扣减生命值
    /// </summary>
    /// <param name="amount">伤害数值</param>
    public virtual void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        int remainingDamage = amount;
        int blockDamage = 0;

        // 优先消耗护盾
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

        // 显示护盾伤害数字
        if (blockDamage > 0)
        {
            ShowDamagePopup(blockDamage, true);
        }

        // 扣减生命值
        currentHealth -= remainingDamage;
        if (currentHealth < 0)
            currentHealth = 0;

        // 显示生命伤害数字
        if (remainingDamage > 0)
        {
            ShowDamagePopup(remainingDamage, false);
        }

        // 更新UI
        UpdateHealthUI();
        UpdateBlockUI();
    }

    /// <summary>
    /// 添加护盾值
    /// </summary>
    /// <param name="amount">护盾数值</param>
    public void AddBlock(int amount)
    {
        if (amount <= 0) return;
        block += amount;
        ShowDamagePopup(amount, true);
        UpdateBlockUI();
    }

    /// <summary>
    /// 清除所有护盾
    /// </summary>
    public void ClearBlock()
    {
        block = 0;
        UpdateBlockUI();
    }

    /// <summary>
    /// 更新血条UI显示，包括血量文字和血条颜色变化
    /// </summary>
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

                // 根据血量百分比改变血条颜色
                if (fillRatio <= 0.3f)
                    hpImage.DOColor(new Color(1f, 0.3f, 0.3f), 0.3f);
                else if (fillRatio <= 0.6f)
                    hpImage.DOColor(new Color(1f, 0.8f, 0.3f), 0.3f);
                else
                    hpImage.DOColor(new Color(0.3f, 1f, 0.3f), 0.3f);
            }
        }
    }

    /// <summary>
    /// 更新护盾UI显示，显示护盾数值或隐藏
    /// </summary>
    protected void UpdateBlockUI()
    {
        if (blockText != null)
        {
            bool hasBlock = block > 0;
            blockText.text = hasBlock ? $"🛡️ {block}" : "";
            blockText.gameObject.SetActive(hasBlock);
        }
    }

    /// <summary>
    /// 显示伤害/护盾弹出数字
    /// </summary>
    /// <param name="amount">数值</param>
    /// <param name="isBlock">是否为护盾（绿色），否则为伤害（红色）</param>
    protected virtual void ShowDamagePopup(int amount, bool isBlock)
    {
        DamagePopup popup = DamagePopup.CreateForEnemy(transform);
        if (popup != null)
        {
            popup.ShowDamage(amount, isBlock);
        }
    }

    /// <summary>
    /// 判断角色是否存活
    /// </summary>
    /// <returns>true表示存活，false表示死亡</returns>
    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    /// <summary>
    /// 获取生命值百分比
    /// </summary>
    /// <returns>0~1之间的浮点数</returns>
    public float GetHealthPercent()
    {
        return (float)currentHealth / maxHealth;
    }
}
