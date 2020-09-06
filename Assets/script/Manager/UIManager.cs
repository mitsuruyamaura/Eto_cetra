using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public QuitCheckPouUp quitCheckPouUpPrefab;
    private QuitCheckPouUp quitCheckPouUp = null;
    public Transform canvasTran;
    public Text txtTimer;
    public Text txtScore;

    public Wind wind;

    public Button btnWind;
    public Button btnSkill;

    public Image imgSkill;

    private Tweener tweener = null;

    private UnityEvent unityEvent;

    void Start() {
        // シャッフル機能の設定
        InactiveWind(false);
        wind.SetUpWind(this);

        // シャッフルボタンにメソッドを登録
        btnWind.onClick.AddListener(CreateUpdraft);
    }

    /// <summary>
    /// 選択した干支の持つスキルを登録
    /// </summary>
    /// <param name="skillType"></param>
    /// <returns></returns>
    public IEnumerator SetUpSkillButton(UnityAction unityAction) {
        // UnityEvent初期化
        unityEvent = new UnityEvent();

        // UnityEventにメソッドを登録(干支のタイプを渡して、その干支の持つスキルの呼び出しメソッドを取得)
        unityEvent.AddListener(unityAction);

        // スキルボタンにメソッドを登録
        btnSkill.onClick.AddListener(TriggerSkill);

        // スキルポイントが0からスタートするので、スキルボタンを押せなくしておく
        btnSkill.interactable = false;

        yield break;
    }

    /// <summary>
    /// シャッフル用ゲームオブジェクトのオンオフ切り替え
    /// </summary>
    /// <param name="isSwitch"></param>
    public void InactiveWind(bool isSwitch) {
        btnWind.gameObject.SetActive(isSwitch);
    }

    /// <summary>
    /// シャッフル開始
    /// </summary>
    private void CreateUpdraft() {
        wind.Updraft();
        btnWind.interactable = false;

        // コルーチン化して、Trueに戻してもいいかも
    }

    /// <summary>
    /// シャッフル停止
    /// </summary>
    public void StopUpdraft() {
        btnWind.interactable = true;
    }

    /// <summary>
    /// スキル使用
    /// </summary>
    public void TriggerSkill() {
        // ボタンの重複タップ防止
        btnSkill.interactable = false;

        // 登録されているスキルを使用
        unityEvent.Invoke();

        // スキルポイント関連を初期化
        imgSkill.DOFillAmount(0, 1.0f);
        tweener.Kill();
        tweener = null;

        imgSkill.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// スコア加算処理
    /// </summary>
    /// <param name="amount"></param>
    public void UpdateDisplayScore(int amount, bool isChooseEto) {
        GameData.instance.score += amount;
        Debug.Log(GameData.instance.score);

        if (isChooseEto) {
            // 自分の干支の場合には点数を大きく表示する演出を入れる
            Sequence sequence = DOTween.Sequence();
            sequence.Append(txtScore.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f)).SetEase(Ease.InCirc);
            sequence.AppendInterval(0.1f);
            sequence.Append(txtScore.transform.DOScale(Vector3.one, 0.1f)).SetEase(Ease.Linear);
        }
        
        // 徐々に加算処理にする

        txtScore.text = GameData.instance.score.ToString();
    }

    /// <summary>
    /// スキルポイント加算
    /// </summary>
    /// <param name="count"></param>
    public void AddSkillPoint(int count) {
        float a = imgSkill.fillAmount;
        float value = a += count * 0.05f;
        imgSkill.DOFillAmount(value, 0.5f);

        if (imgSkill.fillAmount >= 1.0f && tweener == null) {
            Debug.Log(imgSkill.fillAmount);
            btnSkill.interactable = true;
            tweener = imgSkill.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.25f).SetEase(Ease.InCirc).SetLoops(-1, LoopType.Yoyo);
        }
    }

    /// <summary>
    /// ゲームの残り時間の表示更新
    /// </summary>
    /// <param name="time"></param>
    public void UpdateDisplayGameTime(float time) {
        txtTimer.text = time.ToString("F0");
    }

    public void InActiveButtons() {
        btnSkill.interactable = false;
        btnWind.interactable = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (quitCheckPouUp == null) {
                // 終了確認用のポップアップを生成する。そのポップアップの中で終了確認を行う
                quitCheckPouUp = Instantiate(quitCheckPouUpPrefab, canvasTran, false);
            } else {
                // ポップアップがない場合には、すぐにゲームを終了する処理を呼び出す
                //QuitGame();
            }
            //QuitGameManager.ExitGame();
        }
    }

    /// <summary>
    /// ゲームの終了処理
    /// </summary>
    public static void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
