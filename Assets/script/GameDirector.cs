using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    public QuitCheckPouUp quitCheckPouUpPrefab;
    public QuitCheckPouUp quitCheckPouUp = null;
    public Transform canvasTran;
    public Text txtTimer;

    float timer;

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

        timer += Time.deltaTime;
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
