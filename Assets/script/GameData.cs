using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    public enum SkillType {
        SingleToSingleChange,
        SingleToRandomChange,
        AllRandomChange,
        DeleteMaxBall,
    }

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
