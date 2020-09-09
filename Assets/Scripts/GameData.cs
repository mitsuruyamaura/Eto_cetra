using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    [Header("選択中の干支")]
    public EtoType etoType;

    [Header("選択中のスキル")]
    public SkillType skillType;

    [Header("選択中の干支の情報")]    // 受け取っているが、まだ使っていない
    public EtoDetail etoDetail;

    [Header("ゲームに登場する干支の最大種類数")]
    public int etoTypeCount = 5;

    [Header("ゲーム内の干支の数")]
    public int createEtoCount = 50;

    public int score = 0;

    public int etoPoint = 100;

    public float etoRate = 3;

    public int initTime = 60;

    [Header("ゲーム時間")]
    public float gameTime;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        // 初期化
        InitGame();
    }

    /// <summary>
    /// ゲーム初期化
    /// </summary>
    private void InitGame() {
        score = 0;
        gameTime = initTime;
        Debug.Log("Init");
    }

    public IEnumerator RestartGame() {
        yield return new WaitForSeconds(1.0f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        InitGame();
    }
}
