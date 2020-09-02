using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    public enum GameMode {
        Random,
        Normal
    }
    public GameMode gameMode;

    [Header("選択中の干支")]
    public EtoType etoType;

    [Header("選択中のスキル")]
    public SkillType skillType;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}
