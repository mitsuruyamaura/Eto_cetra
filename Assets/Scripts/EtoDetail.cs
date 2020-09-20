using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EtoDetail : MonoBehaviour
{
    // 干支の番号
    public EtoType etoType;

    // 干支のイメージ
    public Image imgEto;

    public Button btnEto;

    private EtoSelectPopUp etoSelectPopUp;

    public CanvasGroup canvasGroup;

    // 設定
    public void SetUpEtoDetail(EtoSelectPopUp etoSelectPopUp, int etoNo, Sprite etoSprite) {
        canvasGroup.alpha = 0.0f;

        this.etoSelectPopUp = etoSelectPopUp;

        etoType = (EtoType)etoNo;

        imgEto.sprite = etoSprite;

        btnEto.onClick.AddListener(()=> StartCoroutine(OnClickEtoDetail()));

        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroup.DOFade(1.0f, 0.25f).SetEase(Ease.Linear));
        sequence.Join(transform.DOPunchScale(new Vector3(1, 1, 1), 0.5f));
    }
        
    private IEnumerator OnClickEtoDetail() {
        // 番号をGameDataに渡す
        GameData.instance.etoType = etoType;

        GameData.instance.etoDetail = this;

        // ポップアニメさせる
        transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.15f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.15f);
        transform.DOScale(Vector3.one, 0.15f);

        // ボタンの色を選択中の色に変更する
        //modeSelectPopUp.InactivateEtoDetailList(etoType, 0.3f);
    }
}
