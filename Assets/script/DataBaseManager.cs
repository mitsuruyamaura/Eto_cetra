using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DataBaseManager : MonoBehaviour
{
    [Serializable]
    public class EtoData {
        public EtoType etoType;
        public SkillType skillType;
    }
    public List<EtoData> etoDataList = new List<EtoData>();

    public GameManager gameManager;
    public Button btnSkill;

    
    [Header("変更元のボールの番号指定")]
    public int chooseBallColorNum;

    [Header("変更先のボールの番号")]
    public int changeBallColorNum;

    void Start()
    {
        // ボタンにスキルをセット
        SetUpSkill();
    }

    /// <summary>
    /// ボタンにスキルをセット
    /// </summary>
    private void SetUpSkill() {
        foreach (SkillType skillType in Enum.GetValues(typeof(SkillType))) {
            if (GameData.instance.skillType == skillType) {

            }
        }


        switch (GameData.instance.skillType) {
            case SkillType.SingleToSingleChange:
                //btnSkill.onClick.AddListener(() => gameManager.ChangeSpecificBalls(GameData.instance.etoDetail.etoType, GameData.instance.etoDetail.imgEto.sprite));
                break;
            case SkillType.SingleToRandomChange:
                //btnSkill.onClick.AddListener(() => gameManager.ChangeSpecificBallsToRandomColor(chooseBallColorNum));
                break;
            case SkillType.AllRandomChange:
                //btnSkill.onClick.AddListener(gameManager.ChangeRandomBalls);
                break;
            case SkillType.DeleteMaxBall:
                btnSkill.onClick.AddListener(gameManager.DeleteMaxBalls);
                break;


        }

    }
}
