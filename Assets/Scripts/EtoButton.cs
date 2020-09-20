using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EtoButton : MonoBehaviour
{
    // 干支の種類
    public GameData.EtoData etoData;

    public Image imgEto;

    public Button btnEto;

    private EtoSelectPopUp etoSelectPopUp;

    public CanvasGroup canvasGroup;

    /// <summary>
    /// 干支ボタンの初期設定
    /// </summary>
    /// <param name="modeSelectPopUp"></param>
    /// <param name="etoData"></param>
    public void SetUpEtoButton(EtoSelectPopUp etoSelectPopUp, GameData.EtoData etoData) {
        canvasGroup.alpha = 0.0f;

        this.etoSelectPopUp = etoSelectPopUp;

        this.etoData = etoData;

        imgEto.sprite = this.etoData.sprite;

        btnEto.onClick.AddListener(() => StartCoroutine(OnClickEtoButton()));

        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroup.DOFade(1.0f, 0.25f).SetEase(Ease.Linear));
        sequence.Join(transform.DOPunchScale(new Vector3(1, 1, 1), 0.5f));
    }

    /// <summary>
    /// 干支ボタンをタップした際の処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator OnClickEtoButton() {
        // 番号をGameDataに渡す
        //GameData.instance.etoType = etoData.etoType;

        // 干支ボタンの保持する干支データをGameDataに代入(選択した干支データとする)
        GameData.instance.selectedEtoData = etoData;

        // 干支ボタンをポップアニメさせる
        transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.15f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.15f);
        transform.DOScale(Vector3.one, 0.15f);

        // 干支ボタンの色を選択中の色に変更し、他の干支ボタンの色を選択中でない色に変更
        etoSelectPopUp.ChangeColorToEtoButton(etoData.etoType, 0.3f);
    }
}
