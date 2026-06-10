using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class BattleUIManager : MonoBehaviour
{
    [Header("UI控件拖拽赋值")]
    public TextMeshProUGUI txtPlayerHP;
    public TextMeshProUGUI txtEnergy;
    public TextMeshProUGUI txtMonsterHP;
    public Transform handCardParent; //手牌父物体（Content）
    public Button btnEndTurn;
    public GameObject cardItemPrefab; //单张卡牌预制体

    void Awake()
    {
        btnEndTurn.onClick.AddListener(() => FindObjectOfType<BattleManager>().EndPlayerTurn());
    }

    //全量刷新UI
    public void RefreshAllUI(BattleManager battle)
    {
        
    }
}