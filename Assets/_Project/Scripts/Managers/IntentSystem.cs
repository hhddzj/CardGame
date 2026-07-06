using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 意图系统，负责显示敌人下回合将要执行的行动意图
/// </summary>
public class IntentSystem : MonoBehaviour
{
    /// <summary>
    /// 意图图标显示组件
    /// </summary>
    [SerializeField] private Image intentIcon;

    /// <summary>
    /// 意图数值显示组件（如伤害值、格挡值）
    /// </summary>
    [SerializeField] private TextMeshProUGUI intentValue;

    /// <summary>
    /// 意图面板游戏对象
    /// </summary>
    [SerializeField] private GameObject intentPanel;

    /// <summary>
    /// 当前意图数据
    /// </summary>
    private Intent currentIntent;

    /// <summary>
    /// 获取当前意图数据
    /// </summary>
    public Intent CurrentIntent => currentIntent;

    /// <summary>
    /// 初始化，隐藏意图面板
    /// </summary>
    private void Start()
    {
        if (intentPanel != null)
        {
            intentPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 设置意图并更新UI显示
    /// </summary>
    /// <param name="intent">意图数据</param>
    public void SetIntent(Intent intent)
    {
        currentIntent = intent;
        UpdateIntentUI();
    }

    /// <summary>
    /// 清除当前意图并隐藏面板
    /// </summary>
    public void ClearIntent()
    {
        currentIntent = null;
        if (intentPanel != null)
        {
            intentPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 更新意图UI显示，根据意图类型设置图标和颜色
    /// </summary>
    private void UpdateIntentUI()
    {
        if (intentPanel == null || intentIcon == null || intentValue == null)
            return;

        if (currentIntent == null)
        {
            intentPanel.SetActive(false);
            return;
        }

        intentPanel.SetActive(true);

        switch (currentIntent.type)
        {
            case IntentType.Attack:
                intentIcon.sprite = GetAttackSprite();
                intentIcon.color = new Color(1f, 0.3f, 0.3f);
                intentValue.text = currentIntent.value.ToString();
                break;
            case IntentType.Defend:
                intentIcon.sprite = GetDefendSprite();
                intentIcon.color = new Color(0.2f, 0.7f, 1f);
                intentValue.text = currentIntent.value.ToString();
                break;
            case IntentType.Buff:
                intentIcon.sprite = GetBuffSprite();
                intentIcon.color = new Color(0.7f, 0.3f, 1f);
                intentValue.text = currentIntent.value.ToString();
                break;
            case IntentType.DeBuff:
                intentIcon.sprite = GetDebuffSprite();
                intentIcon.color = new Color(0.5f, 0.5f, 0.5f);
                intentValue.text = currentIntent.value.ToString();
                break;
            case IntentType.None:
            default:
                intentPanel.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// 获取攻击意图图标
    /// </summary>
    /// <returns>攻击图标Sprite</returns>
    private Sprite GetAttackSprite()
    {
        return CreateSimpleSprite(Color.red);
    }

    /// <summary>
    /// 获取防御意图图标
    /// </summary>
    /// <returns>防御图标Sprite</returns>
    private Sprite GetDefendSprite()
    {
        return CreateSimpleSprite(Color.blue);
    }

    /// <summary>
    /// 获取增益意图图标
    /// </summary>
    /// <returns>增益图标Sprite</returns>
    private Sprite GetBuffSprite()
    {
        return CreateSimpleSprite(Color.magenta);
    }

    /// <summary>
    /// 获取减益意图图标
    /// </summary>
    /// <returns>减益图标Sprite</returns>
    private Sprite GetDebuffSprite()
    {
        return CreateSimpleSprite(Color.gray);
    }

    /// <summary>
    /// 创建简单颜色的Sprite
    /// </summary>
    /// <param name="color">Sprite颜色</param>
    /// <returns>创建的Sprite</returns>
    private Sprite CreateSimpleSprite(Color color)
    {
        Texture2D texture = new Texture2D(32, 32);
        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 32; y++)
            {
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
    }
}

/// <summary>
/// 意图类型枚举，定义敌人可能的行动类型
/// </summary>
public enum IntentType
{
    /// <summary>
    /// 无意图
    /// </summary>
    None,

    /// <summary>
    /// 攻击意图
    /// </summary>
    Attack,

    /// <summary>
    /// 防御意图
    /// </summary>
    Defend,

    /// <summary>
    /// 增益意图
    /// </summary>
    Buff,

    /// <summary>
    /// 减益意图
    /// </summary>
    DeBuff
}

/// <summary>
/// 意图数据类，存储敌人行动意图的详细信息
/// </summary>
public class Intent
{
    /// <summary>
    /// 意图类型
    /// </summary>
    public IntentType type;

    /// <summary>
    /// 意图数值（如伤害值、格挡值）
    /// </summary>
    public int value;

    /// <summary>
    /// 意图描述文本
    /// </summary>
    public string description;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="type">意图类型</param>
    /// <param name="value">意图数值</param>
    /// <param name="description">意图描述（可选）</param>
    public Intent(IntentType type, int value, string description = "")
    {
        this.type = type;
        this.value = value;
        this.description = description;
    }
}
