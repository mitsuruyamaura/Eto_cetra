using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class ScoreScript : MonoBehaviour
{
	private int score = 0;
	void Start()
	{
		//初期スコア(0点)を表示
		GetComponent<Text>().text = "Score: " + score.ToString();
	}
	//ballScriptからSendMessageで呼ばれるスコア加算用メソッド
	public void AddPoint(int point)
	{
		Debug.Log(point);        // <= 追記
		score = score + point;
		GetComponent<Text>().text = "Score: " + score.ToString();
	}
}