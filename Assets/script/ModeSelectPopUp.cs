using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ModeSelectPopUp : MonoBehaviour
{
    // Prefab
    public EtoDetail etoDetailPrefab;

    public Transform etoDetailTran;

    // List
    public List<EtoDetail> etoDetailList = new List<EtoDetail>();

    public BallManager ballManager;

    public int createEtoCount;

    public Button btnRandomMode;
    public Button btnNormalMode;

    public Button btnStart;

    public CanvasGroup canvasGroup;

    
    IEnumerator Start() {
        yield return null;

        // 干支のボタンを生成
        StartCoroutine(CreateEtoDetails((int)EtoType.Count));

        // 初期設定されているモードのボタンは押せない
        btnRandomMode.interactable = false;

        // 干支のボタンが準備できるまでスタートは押せない
        btnStart.interactable = false;

        // 各ボタンへのメソッドの登録
        btnRandomMode.onClick.AddListener(OnClickRandomMode);
        btnNormalMode.onClick.AddListener(OnClickNormalMode);
        btnStart.onClick.AddListener(OnClickStart);
    }

    // 各干支のボタンを生成する処理
    private IEnumerator CreateEtoDetails(int createCount) {
        // Resources
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/eto");

        for (int i = 0; i < createCount; i++) {
            EtoDetail etoDetail = Instantiate(etoDetailPrefab, etoDetailTran, false);
            etoDetail.SetUpEtoDetail(this, i, sprites[i]);           
            if (i == 0) {
                // 初期は子を選択している状態にする
                etoDetail.imgEto.color = new Color(0.65f, 0.65f, 0.65f);
            }
            etoDetailList.Add(etoDetail);
            yield return new WaitForSeconds(0.15f);
        }
        btnStart.interactable = true;
    }


    // モードの切り替えを行う処理
    private void OnClickRandomMode() {
        GameData.instance.gameMode = GameData.GameMode.Random;
        btnRandomMode.interactable = false;
        btnNormalMode.interactable = true;
    }

    private void OnClickNormalMode() {
        GameData.instance.gameMode = GameData.GameMode.Normal;
        btnNormalMode.interactable = false;
        btnRandomMode.interactable = true;
    }

    // ボタンを押されたら遷移する処理
    private void OnClickStart() {
        btnStart.interactable = false;
        StartCoroutine(ballManager.DropBall(createEtoCount));

        canvasGroup.DOFade(0.0f, 0.5f);

        // 破棄
        Destroy(gameObject, 5.0f);
    }

    public void InactivateEtoDetailList(EtoType etoType, float waitTime) {
        // 待機後、ボタンを操作できるようにする
        for (int i = 0; i < etoDetailList.Count; i++) {
            // 対象以外のEtoDaitlのボタンを非選択状態にする
            if (etoDetailList[i].etoType == etoType) {
                // 色を選択した色に変える(灰色)
                etoDetailList[i].imgEto.color = new Color(0.65f, 0.65f, 0.65f);
            } else {
                etoDetailList[i].imgEto.color = new Color(1f, 1f, 1f);
            }
        }
    }
}
