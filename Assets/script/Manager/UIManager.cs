using DG.Tweening;
using Packages.Rider.Editor.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public QuitCheckPouUp quitCheckPouUpPrefab;
    private QuitCheckPouUp quitCheckPouUp = null;
    public Transform canvasTran;
    public Text txtTimer;
    public Text txtScore;

    public float timer;
    public Wind wind;

    public Button btnWind;
    public Button btnSkill;

    public GameManager gameManager;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }    
    }

    void Start() {
        InactiveWind(false);
        btnWind.onClick.AddListener(CreateUpdraft);
    }

    public void InactiveWind(bool isSwitch) {
        btnWind.gameObject.SetActive(isSwitch);
    }


    private void CreateUpdraft() {
        if (gameManager.gameState != GameManager.GameState.Play) {
            return;
        }
        wind.Updraft();
        btnWind.interactable = false;

        // コルーチン化して、Trueに戻してもいいかも

    }

    public void StopUpdraft() {
        btnWind.interactable = true;
    }

    /// <summary>
    /// スコア加算処理
    /// </summary>
    /// <param name="amount"></param>
    public void AddScore(int amount, bool isChooseEto) {
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


    void Update()
    {
        if (gameManager.gameState != GameManager.GameState.Play) {
            return;
        }

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

        timer -= Time.deltaTime;
        
        if (timer <= 0) {
            timer = 0;
            gameManager.GameUp();
        }
        txtTimer.text = timer.ToString("F1");
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
