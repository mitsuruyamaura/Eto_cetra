using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitCheckPouUp : MonoBehaviour
{
    public Button btnQuitGame;        // ゲーム終了ボタン
    public Button btnClosePopup;      // ポップアップを閉じてゲームに戻るボタン

    void Start()
    {
        // 各ボタンに処理を登録
        btnQuitGame.onClick.AddListener(GameDirector.QuitGame);
        btnClosePopup.onClick.AddListener(OnClickClosePopUp);

        // ゲーム内時間の流れを停止
        Time.timeScale = 0;
    }

    /// <summary>
    /// ポップアップを閉じてゲームに戻る
    /// </summary>
    private void OnClickClosePopUp() {
        // ゲーム内時間の流れを再開する
        Time.timeScale = 1.0f;
        Destroy(gameObject);
    }
}
