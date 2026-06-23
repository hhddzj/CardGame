using Assets._Project.Scripts.Managers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    [Header("UI绑定")]
    public TextMeshProUGUI energyText;
    public Image energyImage;
    public Transform handContent;
    public GameObject cardPrefab;
    public HandLayoutHelper handLayoutHelper;

    public static BattleUIManager Instance;
    public Button btnEndTurn;

    private List<GameObject> activeCardObjects = new List<GameObject>();

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

    public void RefreshAllUI()
    {
        Debug.Log("状态=" + BattleManager.Instance.currentState);

        UpdateHandUI();
        UpdateEnergyUI();
    }

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

    public void StartGame()
    {
        Debug.Log("初始化游戏");
        Debug.Log("状态=" + BattleManager.Instance.currentState);
        Debug.Log("btnEndTurn是否为空：" + (btnEndTurn == null));

        btnEndTurn.onClick.AddListener(BattleManager.Instance.NextTurn);
        BattleManager.Instance.InitBattle();
        Instance.RefreshAllUI();
    }
}
