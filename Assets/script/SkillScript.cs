using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillScript : MonoBehaviour
{
    public BallManager ballManager;
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
        switch (GameData.instance.skillType) {
            case GameData.SkillType.SingleToSingleChange:
                btnSkill.onClick.AddListener(() => ballManager.ChangeSpecificBalls(chooseBallColorNum, changeBallColorNum));
                break;
            case GameData.SkillType.SingleToRandomChange:
                btnSkill.onClick.AddListener(() => ballManager.ChangeSpecificBallsToRandomColor(chooseBallColorNum));
                break;
            case GameData.SkillType.AllRandomChange:
                btnSkill.onClick.AddListener(ballManager.ChangeRandomBalls);
                break;
            case GameData.SkillType.DeleteMaxBall:

                break;
        }
    }
}
