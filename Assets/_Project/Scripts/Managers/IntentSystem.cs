using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IntentSystem : MonoBehaviour
{
    [SerializeField] private Image intentIcon;
    [SerializeField] private TextMeshProUGUI intentValue;
    [SerializeField] private GameObject intentPanel;

    private Intent currentIntent;

    public Intent CurrentIntent => currentIntent;

    private void Start()
    {
        if (intentPanel != null)
        {
            intentPanel.SetActive(false);
        }
    }

    public void SetIntent(Intent intent)
    {
        currentIntent = intent;
        UpdateIntentUI();
    }

    public void ClearIntent()
    {
        currentIntent = null;
        if (intentPanel != null)
        {
            intentPanel.SetActive(false);
        }
    }

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

    private Sprite GetAttackSprite()
    {
        return CreateSimpleSprite(Color.red);
    }

    private Sprite GetDefendSprite()
    {
        return CreateSimpleSprite(Color.blue);
    }

    private Sprite GetBuffSprite()
    {
        return CreateSimpleSprite(Color.magenta);
    }

    private Sprite GetDebuffSprite()
    {
        return CreateSimpleSprite(Color.gray);
    }

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

public enum IntentType
{
    None,
    Attack,
    Defend,
    Buff,
    DeBuff
}

public class Intent
{
    public IntentType type;
    public int value;
    public string description;

    public Intent(IntentType type, int value, string description = "")
    {
        this.type = type;
        this.value = value;
        this.description = description;
    }
}
