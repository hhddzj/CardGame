using System.Collections;
using System.Collections.Generic;
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
    public Image HpImage;
    public TextMeshProUGUI Hp;
    protected virtual void Start()
    {
        // 初始化血量
        currentHealth = maxHealth;
        UpdateHealthUI();
    }
    public virtual void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        // 护盾减伤逻辑（优化版）
        if (block > 0)
        {
            if (amount >= block)
            {
                amount -= block;
                block = 0;
            }
            else
            {
                block -= amount;
                amount = 0;
            }
        }

        // 扣除血量
        currentHealth -= amount;
        if (currentHealth < 0)
            currentHealth = 0;

        // 更新UI（关键！）
        UpdateHealthUI();
    }

    // 新增：统一更新血条和文本
    protected void UpdateHealthUI()
    {
        if (Hp != null)
        {
            Hp.text = $"{currentHealth}/{maxHealth}";
        }

        if (HpImage != null)
        {
            // 计算血量比例，控制血条进度
            float fillRatio = (float)currentHealth / maxHealth;
            HpImage.fillAmount = fillRatio;
        }
    }
}
