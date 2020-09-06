using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 未使用
/// </summary>
public class DataBaseManager : MonoBehaviour
{
    [Serializable]
    public class EtoData {
        public EtoType etoType;
        public SkillType skillType;
    }
    public List<EtoData> etoDataList = new List<EtoData>();
}
