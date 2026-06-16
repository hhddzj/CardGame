using Assets._Project.Scripts.Managers;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    [Header("UI绑定")]
    public TextMeshProUGUI playerHpText;
    public TextMeshProUGUI monsterHpText;
    public TextMeshProUGUI energyText;
    public Transform handContent; // 手牌父物体（ScrollView Content）
    public GameObject cardPrefab; // 上面做好的卡牌预制体
    public HandLayoutHelper handLayoutHelper;
    public static BattleUIManager Instance;
    public Button btnEndTurn;
    void Awake()
    {
        if (Instance)
        { Destroy(gameObject);
        return; // 重复物体直接终止代码，不再执行下面逻辑
        }
        else Instance = this;
        DontDestroyOnLoad(gameObject);
        //btnEndTurn.onClick.AddListener(() => FindObjectOfType<BattleManager>().EndPlayerTurn());
    }
    //全量刷新UI
    public void RefreshAllUI()
    {
        // 1. 更新血量、费用文本
        playerHpText.text = $"{BattleManager.Instance.player.currentHealth}/{BattleManager.Instance.player.maxHealth}";
        //monsterHpText.text = $"{BattleManager.Instance.enemies[0].currentHealth}/{BattleManager.Instance.enemies[0].maxHealth}";
        energyText.text = $"能量：{BattleManager.Instance.player.energy}";

        // 2. 清空手牌区所有旧卡牌
        for (int i = handContent.childCount - 1; i >= 0; i--)
        {
            Destroy(handContent.GetChild(i).gameObject);
        }

        // 3. 循环手牌数据，实例化卡牌节点
        List<Card> handList = CardManager.Instance.hand;
        Debug.Log("当前手牌数量：" + handList.Count);
        for (int i = 0; i < handList.Count; i++)
        {
            Card data = handList[i];
            // 实例化卡牌到手牌父物体下
            GameObject cardObj = Instantiate(cardPrefab, handContent);

            // 填充卡牌文字
            TextMeshProUGUI nameTMP = cardObj.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI costTMP = cardObj.transform.Find("CostText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI descriptionTMP = cardObj.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
            nameTMP.text = data.cardName;
            costTMP.text = data.cost.ToString();
            descriptionTMP.text = data.description;
            // 绑定点击出牌事件，传递当前手牌下标i
            CardDrag dragComp = cardObj.GetComponent<CardDrag>();
            if (dragComp != null)
            {
                dragComp.SetCardData(data);
            }
            if (handLayoutHelper != null)
            {
                handLayoutHelper.RefreshHandLayout();
            }
        }
    }

    public void StartGame()
    {
        Debug.Log("btnEndTurn是否为空：" + (btnEndTurn == null));
        btnEndTurn.onClick.AddListener(BattleManager.Instance.NextTurn);
        BattleManager.Instance.InitBattle();                    // 创建怪物 + 初始化卡组
        BattleManager.Instance.ChangeState(BattleState.PlayerTurnStart);
        Instance.RefreshAllUI();

    }
}