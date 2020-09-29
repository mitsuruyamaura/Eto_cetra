using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [HideInInspector]
    public QuitCheckPouUp quitCheckPouUpPrefab;

    private QuitCheckPouUp quitCheckPouUp = null;

    [HideInInspector]
    public Transform canvasTran;

    public Text txtTimer;
    public Text txtScore;

    [SerializeField]
    private Shuffle shuffle;

    [SerializeField]
    private Button btnShuffle;

    [SerializeField]
    private Button btnSkill;

    [SerializeField]
    private Image imgSkillPoint;

    private Tweener tweener = null;

    private UnityEvent unityEvent;

    /// <summary>
    /// UIManagerの初期設定
    /// </summary>
    /// <returns></returns>
    public IEnumerator Initialize() {
        // シャッフルボタンを非活性化(半透明の押せない状態にする)
        ActivateShuffleButton(false);

        // シャッフル機能の設定
        shuffle.SetUpShuffle(this);

        // シャッフルボタンにメソッドを登録
        btnShuffle.onClick.AddListener(TriggerShuffle);

        yield break;
    }

    /// <summary>
    /// 選択した干支の持つスキルを登録
    /// </summary>
    /// <param name="skillType"></param>
    /// <returns></returns>
    public IEnumerator SetUpSkillButton(UnityAction unityAction) {
        // スキルポイントが0からスタートするので、スキルボタンを押せなくしておく
        btnSkill.interactable = false;

        // スキルの登録がない場合、スキルボタンには何も登録しない
        if (unityAction == null) {
            yield break;
        }

        // UnityEvent初期化
        unityEvent = new UnityEvent();

        // UnityEventにメソッドを登録(干支のタイプを渡して、その干支の持つスキルの呼び出しメソッドを取得)
        unityEvent.AddListener(unityAction);

        // スキルボタンにメソッドを登録
        btnSkill.onClick.AddListener(TriggerSkill);
    }

    /// <summary>
    /// シャッフルボタンの活性化/非活性化の切り替え
    /// </summary>
    /// <param name="isSwitch"></param>
    public void ActivateShuffleButton(bool isSwitch) {
        btnShuffle.interactable = isSwitch;
    }

    /// <summary>
    /// シャッフル実行
    /// </summary>
    private void TriggerShuffle() {
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Shuffle);

        // シャッフルボタンを押せなくする。重複タップ防止
        ActivateShuffleButton(false);

        // シャッフル開始
        shuffle.StartShuffle();
    }

    /// <summary>
    /// スキル使用
    /// </summary>
    public void TriggerSkill() {
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Skill);

        // ボタンの重複タップ防止
        btnSkill.interactable = false;

        // 登録されているスキルを使用
        unityEvent.Invoke();

        // スキルポイント関連を初期化
        imgSkillPoint.DOFillAmount(0, 1.0f);
        tweener.Kill();
        tweener = null;

        imgSkillPoint.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 画面表示スコアの更新処理
    /// </summary>
    public void UpdateDisplayScore(bool isChooseEto = false) {
        if (isChooseEto) {
            // 自分の干支の場合には点数を大きく表示する演出を入れる
            Sequence sequence = DOTween.Sequence();
            sequence.Append(txtScore.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.1f)).SetEase(Ease.InCirc);
            sequence.AppendInterval(0.1f);
            sequence.Append(txtScore.transform.DOScale(Vector3.one, 0.1f)).SetEase(Ease.Linear);
        }

        // 画面に表示しているスコアの値を更新
        txtScore.text = GameData.instance.score.ToString();
    }

    /// <summary>
    /// スキルポイント加算
    /// </summary>
    /// <param name="count"></param>
    public void AddSkillPoint(int count) {
        float a = imgSkillPoint.fillAmount;
        float value = a += count * 0.05f;
        imgSkillPoint.DOFillAmount(value, 0.5f);

        if (imgSkillPoint.fillAmount >= 1.0f && tweener == null) {
            Debug.Log(imgSkillPoint.fillAmount);
            btnSkill.interactable = true;
            tweener = imgSkillPoint.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.25f).SetEase(Ease.InCirc).SetLoops(-1, LoopType.Yoyo);
        }
    }

    /// <summary>
    /// ゲームの残り時間の表示更新
    /// </summary>
    /// <param name="time"></param>
    public void UpdateDisplayGameTime(float time) {
        txtTimer.text = time.ToString("F0");
    }

    /// <summary>
    /// 複数のボタンを押せないように制御する
    /// </summary>
    public void InActiveButtons() {
        btnSkill.interactable = false;
        btnShuffle.interactable = false;
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
