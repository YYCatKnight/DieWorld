public enum DamageType
{
    CommonAttack, CounterAttack, RealAttack
}

public class DamagePack
{
    public BaseLive attacker;
    public float damageVal;
    public DamageType damageType;
    public DamagePack(BaseLive _attcker = null, float _damage = 0f, DamageType _type = DamageType.CommonAttack)
    {
        attacker = _attcker;
        damageVal = _damage;
        damageType = _type;
    }
}

/// <summary>
/// 增幅浮点数
/// </summary>
public class ChangeFloat
{
    protected float m_baseVal;
    public float BaseVal
    {
        get { return m_baseVal; }
        //基础值变化以后，最终增幅值也要发生变化
        set
        {
            float stg = value - m_baseVal;
            m_endVal += stg;
            m_baseVal = value;
        }
    }
    protected float m_endVal;
    public float EndVal
    {
        get { return m_endVal; }
        //最终增幅值发生改变，与基础值无关
        set { m_endVal = value; }
    }
    public ChangeFloat(float initVal)
    {
        m_baseVal = initVal;
        m_endVal = initVal;
    }
}

/// <summary>
/// 范围浮点数
/// </summary>
public class RangeFloat
{
    protected float m_maxVal;
    public float MaxVal
    {
        get { return m_maxVal; }
        //当最大值发生改变，如果当前值大于最大值，则当前值等于最大值
        set
        {
            if (value > 0)
            {
                m_maxVal = value;
                if (m_curVal > value)
                {
                    m_curVal = value;
                }
            }
            else
            {
                m_curVal = 0;
                m_maxVal = 0;
            }
        }
    }
    protected float m_curVal;
    public float CurVal
    {
        get { return m_curVal; }
        //当前值改变不影响最大值，但是不可以超过最大值
        set
        {
            if (value > m_maxVal)
            {
                m_curVal = m_maxVal;
            }
            else
            {
                m_curVal = value;
            }
        }
    }

    public bool IsMax
    {
        get
        {
            if (m_curVal >= m_maxVal)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public RangeFloat(float initVal, bool isMax = true)
    {
        m_maxVal = initVal;
        if (!isMax)
        {
            m_curVal = 0;
        }
        else
        {
            m_curVal = initVal;
        }
    }
}

/// <summary>
/// 范围增幅浮点数RangeChangeFloat
/// </summary>
public class RCFloat
{
    protected float m_baseVal;
    public float BaseVal
    {
        get { return m_baseVal; }
        //基础值发生变化，影响最大值发生变化
        set
        {
            float stg = value - m_baseVal;
            MaxVal += stg;
            m_baseVal = value;
        }
    }
    protected float m_maxVal;
    public float MaxVal
    {
        get { return m_maxVal; }
        //当最大值发生改变，如果当前值大于最大值，则当前值等于最大值
        set
        {
            if (value > 0)
            {
                m_maxVal = value;
                if (m_curVal > m_maxVal)
                {
                    m_curVal = m_maxVal;
                }
            }
            else
            {
                m_curVal = 0;
                m_maxVal = 0;
            }
        }
    }
    protected float m_curVal;
    public float CurVal
    {
        get { return m_curVal; }
        //当前值改变不影响最大值，但是不可以超过最大值
        set
        {
            if (value > m_maxVal)
            {
                m_curVal = m_maxVal;
            }
            else
            {
                m_curVal = value;
            }
        }
    }

    public bool IsMax
    {
        get
        {
            if (m_curVal >= m_maxVal)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public RCFloat(float initVal, bool isMax)
    {
        if (!isMax)
        {
            m_curVal = 0;
            m_maxVal = m_baseVal = initVal;
        }
        else
        {
            m_curVal = m_maxVal = m_baseVal = initVal;
        }
    }
}