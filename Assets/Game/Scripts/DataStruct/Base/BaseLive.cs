using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILiving
{
    void SetTarget(params BaseLive[] other);
    void AddBuff(BaseBuff buf);
    void RemoveBuf(BaseBuff buf);
    void BeHurted(DamagePack pack);
    void TimeOneUpdate();
}

public enum LivingExState
{
    InNone, InBattle
}

public abstract class BaseLive : ILiving
{
    protected int m_onlyID;
    protected int m_typeID;

    #region 生物基础属性

    /// <summary>
    /// 等级
    /// </summary>
    protected int m_level;
    /// <summary>
    /// 攻击力
    /// </summary>
    protected ChangeFloat m_attack;
    /// <summary>
    /// 生命值
    /// </summary>
    protected RCFloat m_heal;
    /// <summary>
    /// 生命回复
    /// </summary>
    protected ChangeFloat m_healRecovery;
    /// <summary>
    /// 回气速度
    /// </summary>
    protected ChangeFloat m_kiSpeed;
    /// <summary>
    /// 气 当气达到100，发动攻击
    /// </summary>
    protected RangeFloat m_kiRange;
    /// <summary>
    /// 护盾值，无上限，可以一直叠加，用来抵挡伤害
    /// </summary>
    protected float m_shieldVal;
    #endregion

    protected LivingExState m_curState;
    protected bool m_islive;
    public bool LiveStatus
    {
        get { return m_islive; }
    }

    protected BaseLive[] m_otherTargets;

    protected BuffReceive m_buffReceive;

    protected DamagePack DamagePackUs;

    public BaseLive(int only_id, int id)
    {
        DamagePackUs = new DamagePack(this);
        m_buffReceive = new BuffReceive(this);
        m_onlyID = only_id;
        m_typeID = id;
        m_islive = true;
    }

    public Dictionary<int, BaseBuff> GetBuf(ShowType _type)
    {
        return m_buffReceive.GetTypeBuffDict(_type);
    }

    public void AddBuff(BaseBuff buf)
    {
        m_buffReceive.MountBuff(buf);
    }

    public void RemoveBuf(BaseBuff buf)
    {
        m_buffReceive.RemoveBuff(buf);
    }

    protected virtual void Attack(BaseLive other)
    {
        m_buffReceive.OnPreAttackFunc.Invoke(DamagePackUs);
        DamagePack pack = DealAttack(other);
        other.BeHurted(pack);
        m_buffReceive.OnNextAttackFunc.Invoke(DamagePackUs);
    }

    protected virtual void Attack(params BaseLive[] others)
    {
        m_buffReceive.OnPreAttackFunc.Invoke(DamagePackUs);
        for (int i = 0; i < others.Length; i++)
        {
            DamagePack pack = DealAttack(others[i]);
            others[i].BeHurted(pack);
        }
        m_buffReceive.OnNextAttackFunc.Invoke(DamagePackUs);
    }

    public virtual void BeHurted(DamagePack pack)
    {
        m_buffReceive.OnPreHurtFunc.Invoke(pack);
        DealHurted(pack);
        m_buffReceive.OnNextHurtFunc.Invoke(pack);
    }

    protected abstract void DealHurted(DamagePack pack);
    protected abstract DamagePack DealAttack(BaseLive other);

    protected virtual void OnDead()
    {
        m_islive = false;
    }

    public virtual void SetTarget(params BaseLive[] other)
    {
        m_otherTargets = other;
    }

    public void TimeOneUpdate()
    {
        m_kiRange.CurVal += m_kiSpeed.EndVal;
        if (m_kiRange.CurVal >= 100)
        {
            
        }
    }
}
