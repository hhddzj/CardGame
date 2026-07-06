using Assets._Project.Scripts.Managers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 战斗UI管理器，负责战斗界面的UI更新和交互
/// 采用单例模式，确保全局唯一
/// </summary>
public class BattleUIManager : MonoBehaviour
{
    [Header("UI绑定")]

    /// <summary>
    /// 能量文字显示组件
    /// </summary>
    public TextMeshProUGUI energyText;

    /// <summary>
    /// 能量条图像组件
    /// </summary>
    public Image energyImage;

    /// <summary>
    /// 手牌容器Transform
    /// </summary>
    public Transform handContent;

    /// <summary>
    /// 卡牌预制件
    /// </summary>
    public GameObject cardPrefab;

    /// <summary>
    /// 手牌布局辅助器
    /// </summary>
    public HandLayoutHelper handLayoutHelper;

    /// <summary>
    /// 战斗UI管理器单例实例
    /// </summary>
    public static BattleUIManager Instance;

    /// <summary>
    /// 结束回合按钮
    /// </summary>
    public Button btnEndTurn;

    /// <summary>
    /// 当前活跃的卡牌对象列表，用于清理
    /// </summary>
    private List<GameObject> activeCardObjects = new List<GameObject>();

    /// <summary>
    /// 初始化单例，确保场景切换时不被销毁
    /// </summary>
    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        else Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 刷新所有战斗UI元素
    /// </summary>
    public void RefreshAllUI()
    {
        Debug.Log("状态=" + BattleManager.Instance.currentState);

        UpdateHandUI();
        UpdateEnergyUI();
    }

    /// <summary>
    /// 更新手牌UI，销毁旧卡牌并创建新卡牌
    /// </summary>
    private void UpdateHandUI()
    {
        foreach (var obj in activeCardObjects)
            Destroy(obj);
        activeCardObjects.Clear();

        List<Card> handList = CardManager.Instance.hand;
        Debug.Log("当前手牌数量：" + handList.Count);

        foreach (var data in handList)
        {
            GameObject cardObj = Instantiate(cardPrefab, handContent);
            activeCardObjects.Add(cardObj);

            TextMeshProUGUI nameTMP = cardObj.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI costTMP = cardObj.transform.Find("CostText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI descriptionTMP = cardObj.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();

            nameTMP.text = data.cardName;
            costTMP.text = data.cost.ToString();
            descriptionTMP.text = data.description;

            CardDrag dragComp = cardObj.GetComponent<CardDrag>();
            if (dragComp != null)
            {
                dragComp.SetCardData(data);
            }
        }

        if (handLayoutHelper != null)
        {
            handLayoutHelper.RefreshHandLayout(activeCardObjects);
        }
    }

    /// <summary>
    /// 更新能量UI，显示当前能量/最大能量并更新能量条
    /// </summary>
    private void UpdateEnergyUI()
    {
        Player player = BattleManager.Instance?.player;
        if (player == null) return;

        if (energyText != null)
        {
            energyText.text = $"{player.energy}/{player.MaxEnergy}";
        }

        if (energyImage != null && player.MaxEnergy > 0)
        {
            float ratio = (float)player.energy / player.MaxEnergy;
            ratio = Mathf.Clamp(ratio, 0f, 1f);

            if (!float.IsNaN(ratio) && !float.IsInfinity(ratio))
            {
                energyImage.fillAmount = ratio;

                if (ratio == 0)
                    energyImage.color = new Color(0.5f, 0.5f, 0.5f);
                else if (ratio <= 0.3f)
                    energyImage.color = new Color(1f, 0.5f, 0.5f);
                else
                    energyImage.color = new Color(0.5f, 1f, 0.5f);
            }
        }
    }

    /// <summary>
    /// 开始游戏，初始化战斗并刷新UI
    /// </summary>
    public void StartGame()
    {
        Debug.Log("初始化游戏");
        Debug.Log("状态=" + BattleManager.Instance.currentState);
        Debug.Log("btnEndTurn是否为空：" + (btnEndTurn == null));

        btnEndTurn.onClick.RemoveAllListeners();
        btnEndTurn.onClick.AddListener(BattleManager.Instance.NextTurn);
        BattleManager.Instance.InitBattle();
        Instance.RefreshAllUI();
    }
}
