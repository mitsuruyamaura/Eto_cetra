using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Eto : MonoBehaviour
{
    [Header("ボールの番号")]
    public EtoType etoType;

    [Header("ボールの色変更用")]
    public Image imgEto;

    public bool isSelected;

    public int num;

    /// <summary>
    /// Ballの設定
    /// </summary>
    public void SetUpBall(EtoType etoType, Sprite sprite) {
        this.etoType = etoType;

        // 名前を変更
        name = this.etoType.ToString(); ;

        // ボールの番号に合わせて色を変更
        ChangeBallColor(sprite);
    }

    /// <summary>
    /// 干支のイメージを変更
    /// </summary>
    /// <param name="changeSprite"></param>
    public void ChangeBallColor(Sprite changeSprite) {
        imgEto.sprite = changeSprite;
    }
}
