using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F2DEngine;

public class SaveDataManager : ManagerTemplate<SaveDataManager>
{
    protected List<Save_Object> m_allSaveData = new List<Save_Object>();

    public void AddSaveObject(Save_Object obj)
    {
        if (m_allSaveData.Contains(obj))
        {
            Debug.Log("重复加入存储列表");
            return;
        }
        obj.ReadFile();
        m_allSaveData.Add(obj);
    }

    public void RemoveSaveObject(Save_Object obj)
    {
        if (m_allSaveData.Contains(obj))
        {
            m_allSaveData.Remove(obj);
        }
        else
        {
            Debug.Log("要移除的存储结构不在存储列表中");
        }
    }

    public void SaveAllData()
    {
        for (int i = 0; i < m_allSaveData.Count; i++)
        {
            m_allSaveData[i].SaveFile();
        }
    }
}
