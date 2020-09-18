using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Eto : MonoBehaviour
{
    [Header("干支の番号")]
    public EtoType etoType;

    [Header("干支の画像変更用")]
    public Image imgEto;

    public bool isSelected;

    public int num;

    /// <summary>
    /// Etoの設定
    /// </summary>
    public void SetUpEto(EtoType etoType, Sprite sprite) {
        this.etoType = etoType;

        // 名前を変更
        name = this.etoType.ToString(); ;

        // ボールの番号に合わせて色を変更
        ChangeEtoSprite(sprite);
    }

    /// <summary>
    /// 干支のイメージを変更
    /// </summary>
    /// <param name="changeSprite"></param>
    public void ChangeEtoSprite(Sprite changeSprite) {
        imgEto.sprite = changeSprite;
    }
}
