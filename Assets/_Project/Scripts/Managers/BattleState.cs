using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Project.Scripts.Managers
{
    public enum BattleState
    {
        PlayerTurnStart,   // 回能量、抽牌、重置格挡
        PlayerAction,      // 等待玩家出牌
        PlayerTurnEnd,     // 结算回合结束效果
        EnemyTurnStart,    // 敌人执行意图
        EnemyAction,       // 播放动作
        EnemyTurnEnd,      // 结算敌方回合结束效果
        BattleEnd          // 胜利/失败
    }
}
