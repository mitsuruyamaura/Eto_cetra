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
            case SkillType.SingleToSingleChange:
                btnSkill.onClick.AddListener(() => ballManager.ChangeSpecificBalls(chooseBallColorNum, changeBallColorNum));
                break;
            case SkillType.SingleToRandomChange:
                btnSkill.onClick.AddListener(() => ballManager.ChangeSpecificBallsToRandomColor(chooseBallColorNum));
                break;
            case SkillType.AllRandomChange:
                btnSkill.onClick.AddListener(ballManager.ChangeRandomBalls);
                break;
            case SkillType.DeleteMaxBall:

                break;
        }
    }
}
