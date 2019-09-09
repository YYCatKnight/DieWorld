using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuffReceive
{
    void MountBuff(BaseBuff buf);
    void RemoveBuff(BaseBuff buf);
}

/// <summary>
/// Buff接受器
/// </summary>
public class BuffReceive : IBuffReceive
{
    public Action<DamagePack> OnPreAttackFunc { get; private set; }
    public Action<DamagePack> OnNextAttackFunc { get; private set; }
    public Action<DamagePack> OnPreHurtFunc { get; private set; }
    public Action<DamagePack> OnNextHurtFunc { get; private set; }
    protected Dictionary<ShowType, Dictionary<int, BaseBuff>> m_mountDict = new Dictionary<ShowType, Dictionary<int, BaseBuff>>();

    private readonly BaseLive m_addLive;

    /// <summary>
    /// 初始化时候 必须将生物类传入
    /// </summary>
    /// <param name="live"></param>
    public BuffReceive(BaseLive live)
    {
        m_addLive = live;
        for (int i = 0; i < (int)ShowType.End; i++)
        {
            m_mountDict.Add((ShowType)i, new Dictionary<int, BaseBuff>());
        }
    }

    /// <summary>
    /// 根据显示类型获取不同的buff字典
    /// </summary>
    /// <param name="_type"></param>
    /// <returns></returns>
    public Dictionary<int, BaseBuff> GetTypeBuffDict(ShowType _type)
    {
        if (!m_mountDict.ContainsKey(_type))
        {
            Debug.Log("接收器中不存在Buff的Type=" + _type);
            return null;
        }
        return m_mountDict[_type];
    }

    /// <summary>
    /// 获取Buff通过ShowType和ID
    /// </summary>
    /// <param name="_type"></param>
    /// <param name="_id"></param>
    /// <returns></returns>
    public BaseBuff GetBuff(ShowType _type, int _id)
    {
        if (!m_mountDict.ContainsKey(_type))
        {
            Debug.Log("接收器中不存在Buff的Type=" + _type);
            return null;
        }
        if (!m_mountDict[_type].ContainsKey(_id))
        {
            Debug.Log(_type + "的接收器中不存在Buff的ID=" + _id);
            return null;
        }
        return m_mountDict[_type][_id];
    }

    /// <summary>
    /// 根据BuffID在挂载Buff字典里查找
    /// </summary>
    /// <param name="_id"></param>
    /// <returns></returns>
    public BaseBuff GetBuff(int _id)
    {
        foreach(var val in m_mountDict.Values)
        {
            if (val.ContainsKey(_id))
            {
                return val[_id];
            }
        }
        Debug.Log("接收器中不存在Buff的ID=" + _id);
        return null;
    }

    /// <summary>
    /// 挂载Buff
    /// </summary>
    /// <param name="buf"></param>
    public void MountBuff(BaseBuff buf)
    {
        ShowType curType = buf.BuffShowType;
        if (!m_mountDict.ContainsKey(curType))
        {
            Debug.Log("Dict缺少key=" + curType);
            return;
        }

        if (m_mountDict[curType].ContainsKey(buf.OnlyID))
        {
            m_mountDict[curType][buf.OnlyID].Overlying(buf);
        }
        else
        {
            if (buf.Check(1))
            {
                OnPreAttackFunc += buf.OnPreAttack;
            }
            if (buf.Check(2))
            {
                OnNextAttackFunc += buf.OnNextAttack;
            }
            if (buf.Check(3))
            {
                OnPreHurtFunc += buf.OnPreHurted;
            }
            if (buf.Check(4))
            {
                OnNextHurtFunc += buf.OnNextHurted;
            }
            buf.Mount(m_addLive);
            m_mountDict[curType].Add(buf.OnlyID, buf);
        }
    }

    /// <summary>
    /// 移除Buff
    /// </summary>
    /// <param name="buf"></param>
    public void RemoveBuff(BaseBuff buf)
    {
        ShowType curType = buf.BuffShowType;
        if (!m_mountDict.ContainsKey(curType))
        {
            Debug.Log("Dict缺少key=" + curType);
            return;
        }

        if (!m_mountDict[curType].ContainsKey(buf.OnlyID))
        {
            Debug.Log("Buff->" + buf.OnlyID + "不存在");
            return;
        }
        m_mountDict[curType].Remove(buf.OnlyID);
        if (buf.Check(1))
        {
            OnPreAttackFunc -= buf.OnPreAttack;
        }
        if (buf.Check(2))
        {
            OnNextAttackFunc -= buf.OnNextAttack;
        }
        if (buf.Check(3))
        {
            OnPreHurtFunc -= buf.OnPreHurted;
        }
        if (buf.Check(4))
        {
            OnNextHurtFunc -= buf.OnNextHurted;
        }
        buf.Remove();
    }
}
