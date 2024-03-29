﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ResultPopUp : MonoBehaviour
{
    public Text txtScore;
    public Text txtEraseEtoCount;
    public Button btnClosePopUp;

    private float posY;

    void Start() {
        posY = transform.position.y;
        btnClosePopUp.gameObject.GetComponent<CanvasGroup>().alpha = 0.0f;
    }

    /// <summary>
    /// ゲーム結果(点数と消した干支の数)をアニメ表示
    /// </summary>
    /// <param name="eraseEtoCount"></param>
    public void DisplayResult(int score, int eraseEtoCount) {
        btnClosePopUp.onClick.AddListener(OnClickMovePopUp);

        int initValue = 0;
        Sequence sequence = DOTween.Sequence();

        sequence.Append(DOTween.To(() => initValue,
                    (num) => {
                        initValue = num;
                        txtScore.text = num.ToString();
                    },
                    score,
                    1.0f).OnComplete(() => { initValue = 0; }).SetEase(Ease.InCirc));   // 消した干支の数のカウントアップ用に initValueを初期化
        sequence.AppendInterval(0.5f);

        sequence.Append(DOTween.To(() => initValue,
                    (num) => {
                        initValue = num;
                        txtEraseEtoCount.text = num.ToString();
                    },
                    eraseEtoCount,
                    1.0f).SetEase(Ease.InCirc));
                    
        sequence.AppendInterval(1.0f);
        sequence.Append(btnClosePopUp.gameObject.GetComponent<CanvasGroup>().DOFade(1.0f, 1.0f).SetEase(Ease.Linear));
    }

    /// <summary>
    /// リザルト表示を元の位置に戻して、ゲームをリスタートする
    /// </summary>
    private void OnClickMovePopUp() {
        SoundManager.instance.PlayBGM(SoundManager.BGM_Type.Select);
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Transition);

        btnClosePopUp.interactable = false;

        transform.DOMoveY(posY, 1.0f);

        StartCoroutine(GameData.instance.RestartGame());
    }
}
