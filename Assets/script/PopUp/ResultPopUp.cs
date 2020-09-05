using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ResultPopUp : MonoBehaviour
{
    public Text txtScore;
    public Text txtEraseEtoCount;
    public Button btnClosePopUp;

    private float posY;
    private GameManager gameManager;

    void Start() {
        posY = transform.position.y;
        btnClosePopUp.gameObject.GetComponent<CanvasGroup>().alpha = 0.0f;
    }

    public void DisplayResult(int eraseEtoCount, GameManager gameManager) {
        this.gameManager = gameManager;

        btnClosePopUp.onClick.AddListener(OnClickMovePopUp);

        int initValue = 0;
        Sequence sequence = DOTween.Sequence();

        sequence.Append(DOTween.To(() => initValue,
                    (num) => {
                        initValue = num;
                        txtScore.text = num.ToString();
                    },
                    GameData.instance.score,
                    1.0f).SetEase(Ease.InCirc));
        sequence.AppendInterval(0.5f);

        initValue = 0;
        sequence.Append(DOTween.To(() => initValue,
                    (num) => {
                        initValue = num;
                        txtEraseEtoCount.text = num.ToString();
                    },
                    eraseEtoCount,
                    1.0f).SetEase(Ease.InCirc)).OnComplete(() => {
                        sequence.AppendInterval(0.5f);
                        btnClosePopUp.gameObject.GetComponent<CanvasGroup>().alpha = 1.0f;       
                        }
                    );           
    }

    private void OnClickMovePopUp() {
        btnClosePopUp.interactable = false;

        transform.DOMoveY(posY, 1.0f);

        StartCoroutine(GameData.instance.RestartGame());
    }
}
