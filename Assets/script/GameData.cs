using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    [Header("選択中の干支")]
    public EtoType etoType;

    [Header("選択中のスキル")]
    public SkillType skillType;

    [Header("選択中の干支の情報")]
    public EtoDetail etoDetail;

    [Header("ゲームに登場する干支の最大種類数")]
    public int etoTypeCount = 5;

    [Header("ゲーム内の干支の数")]
    public int createEtoCount = 50;

    public int score;


    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}
