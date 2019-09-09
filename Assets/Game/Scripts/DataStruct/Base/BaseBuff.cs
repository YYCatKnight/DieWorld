using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F2DEngine;

public interface IBuff
{
    void Mount(BaseLive live);
    void Remove();
    void OnPreAttack(DamagePack pack);
    void OnNextAttack(DamagePack pack);
    void OnPreHurted(DamagePack pack);
    void OnNextHurted(DamagePack pack);
    void Overlying(BaseBuff buf);
}

public enum BuffAddState
{
    NoneAdd, Replace, AddValAdd, AddValMul, AddTimeAdd
}

public enum ShowType
{
    UnShow, ShowInBase, ShowInBattle, End
}

public abstract class BaseBuff : IBuff
{
    protected bool m_isMounted = false;
    protected BaseLive m_mountedObj;
    protected int m_funcState;
    protected Living_Buff_TypeProperty m_curPro;

    public BaseBuff(params object[] objs)
    {
        Init(objs);
    }

    public int OnlyID
    {
        get { return m_curPro.ID32; }
    }

    public string BuffName
    {
        get { return m_curPro.buff_name; }
    }

    public BuffAddState BuffAddState
    {
        get { return (BuffAddState)m_curPro.add_type; }
    }

    public ShowType BuffShowType
    {
        get { return (ShowType)m_curPro.show_type; }
    }

    public string Icon
    {
        get { return m_curPro.icon; }
    }

    protected abstract void Init(params object[] objs);

    public abstract string GetDesc();

    public bool Check(int bit)
    {
        return m_funcState.ToCheck(bit);
    }

    public virtual void Mount(BaseLive live)
    {
        if (m_isMounted == false)
        {
            m_isMounted = true;
            m_mountedObj = live;
            OnAdd();
        }
    }

    public virtual void Remove()
    {
        m_isMounted = false;
        m_mountedObj = null;
        OnRemove();
    }

    protected abstract void OnAdd();

    /// <summary>
    /// 在挂载以后，执行OnAdd函数，如果UpdateTime有执行，则在OnAdd添加计时器执行
    /// </summary>
    protected abstract void UpdateTime();

    protected abstract void OnRemove();

    /// <summary>
    /// 1
    /// </summary>
    public abstract void OnPreAttack(DamagePack pack);

    /// <summary>
    /// 2
    /// </summary>
    public abstract void OnNextAttack(DamagePack pack);

    /// <summary>
    /// 3
    /// </summary>
    public abstract void OnPreHurted(DamagePack pack);

    /// <summary>
    /// 4
    /// </summary>
    public abstract void OnNextHurted(DamagePack pack);

    public abstract void Overlying(BaseBuff buf);
}
