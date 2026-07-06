﻿﻿﻿﻿﻿﻿﻿﻿﻿namespace Assets._Project.Scripts.Managers
{
    /// <summary>
    /// 战斗状态枚举，定义战斗流程中的各个阶段
    /// </summary>
    public enum BattleState
    {
        /// <summary>
        /// 玩家回合开始阶段：回能量、抽牌、重置格挡
        /// </summary>
        PlayerTurnStart,

        /// <summary>
        /// 玩家行动阶段：等待玩家出牌
        /// </summary>
        PlayerAction,

        /// <summary>
        /// 玩家回合结束阶段：结算回合结束效果
        /// </summary>
        PlayerTurnEnd,

        /// <summary>
        /// 敌人回合开始阶段：敌人执行意图
        /// </summary>
        EnemyTurnStart,

        /// <summary>
        /// 敌人行动阶段：播放动作
        /// </summary>
        EnemyAction,

        /// <summary>
        /// 敌人回合结束阶段：结算敌方回合结束效果
        /// </summary>
        EnemyTurnEnd,

        /// <summary>
        /// 战斗胜利状态
        /// </summary>
        Victory,

        /// <summary>
        /// 战斗失败状态
        /// </summary>
        Defeat
    }
}
