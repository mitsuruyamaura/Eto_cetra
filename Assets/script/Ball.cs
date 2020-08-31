using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("ボールの番号")]
    public int ballNum;

    [Header("ボールの色変更用")]
    public SpriteRenderer spriteRenderer;

    [Header("ボールの色の登録用")]
    public Sprite[] ballSprites;

    public bool isSelected;

    public int num;

    /// <summary>
    /// Ballの設定
    /// </summary>
    public void SetUpBall() {
        // ボールの番号をランダムに取得
        ballNum = Random.Range(0, ballSprites.Length);

        // 名前を変更
        this.name = "Ball" + ballNum;

        // ボールの番号に合わせて色を変更
        ChangeBallColor(ballNum);      
    }

    /// <summary>
    /// ボールの色を変更
    /// </summary>
    /// <param name="changeNum">変更する色の番号</param>
    public void ChangeBallColor(int changeNum) {
        spriteRenderer.sprite = ballSprites[changeNum];
    }
}
