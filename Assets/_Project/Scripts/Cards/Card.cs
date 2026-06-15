using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
[CreateAssetMenu]
public class Card : ScriptableObject
{
    public string cardName;
    public int cost;
    public CardType type; // Attack, Skill, Power
    public string description;

    // 效果执行
    public virtual void Play(Character source, Character target)
    {
        // 子类重写

    }

}
public enum CardType
{
    Attack, Skill, Power
}