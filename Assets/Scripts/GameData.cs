using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    //[Header("選択中の干支")]
     
    [HideInInspector]
    public EtoType etoType;

    //[Header("選択中の干支の情報")]    // 受け取っているが、まだ使っていない
    [HideInInspector]
    public EtoDetail etoDetail;

    [Header("ゲームに登場する干支の最大種類数")]
    public int etoTypeCount = 5;

    [Header("ゲーム内の干支の数")]
    public int createEtoCount = 50;

    [Header("現在のスコア")]
    public int score = 0;

    [Header("消した干支の数")]
    public int eraseEtoCount = 0;  // 全部でいくつ消したか

    [Header("干支を消した際に加算されるスコア")]
    public int etoPoint = 100;

    [Header("1回辺りのゲーム時間")]
    public int initTime = 60;

    [Header("現在のゲームの残り時間")]
    public float gameTime;

    [Header("選択している干支")]
    public EtoData selectedEtoData;

    /// <summary>
    /// 干支の基本情報
    /// </summary>
	[System.Serializable]
    public class EtoData {
        public EtoType etoType;
        public Sprite sprite;

        public EtoData(EtoType etoType, Sprite sprite)
        {
            this.etoType = etoType;
            this.sprite = sprite;
        }
    }

    [Header("干支12種類のリスト")]
    public List<EtoData> etoDataList = new List<EtoData>();

    [Header("選択している干支を消した時のスコア倍率")]
    public float etoRate = 3;

    [Header("選択中のスキル")]
    public SkillType selectedSkillType;


    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ゲーム初期化
    /// </summary>
    public void InitGame() {
        score = 0;
        eraseEtoCount = 0;

        gameTime = initTime;
        Debug.Log("Init");
    }

    /// <summary>
    /// 現在のゲームシーンを再読み込み
    /// </summary>
    /// <returns></returns>
    public IEnumerator RestartGame() {
        //yield return new WaitForSeconds(1.0f);

        yield return StartCoroutine(TransitionManager.instance.FadePanel(1.0f));

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 干支データのリストを作成
    /// </summary>
    /// <returns></returns>
    public IEnumerator InitEtoDataList() {
        // 干支の画像を読みこむための変数を配列で用意
        Sprite[] etoSprites = new Sprite[(int)EtoType.Count];

        // Resources.LoadAllを行い、分割されている干支の画像を順番にすべて読み込んで配列に代入
        etoSprites = Resources.LoadAll<Sprite>("Sprites/eto");

        // ゲームに登場する12種類の干支の情報を作成
        for (int i = 0; i < (int)EtoType.Count; i++)
        {
            // 干支の情報を扱うクラス Eto をインスタンスし、コンストラクタを使って初期化
            EtoData etoData = new EtoData((EtoType)i, etoSprites[i]);

            // 干支をListへ追加
            etoDataList.Add(etoData);
        }

        yield break;
    }
}
