using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeScript : MonoBehaviour
{

	private float time = 60;
	//********** 追記 **********// 
	public GameObject exchangeButton;
	public GameObject gameOverText;
	public GameObject sukiruButton;
	//********** 追記 **********//
	public Text timeText;
	void Start()
	{
		//********** 追記 **********// 
		gameOverText.SetActive(false);
		//********** 追記 **********// 
		timeText = GetComponent<Text>();
		timeText.text = ((int)time).ToString();
	}

     public void Update()
	{
		time -= Time.deltaTime;
		//********** 追記 **********// 
		if (time < 0)
		{
			StartCoroutine("GameOver");
		}
		//********* 追記 **********// 
		if (time < 0) time = 0;
		 timeText.text = ((int)time).ToString();
	}
	//********** 追記 **********// 
	IEnumerator GameOver()
	{
		gameOverText.SetActive(true);
		exchangeButton.GetComponent<Button>().interactable = false;

		yield return new WaitForSeconds(2.0f);
		if (Input.GetMouseButtonDown(0))
		{

		}
	}
	public void AddTime(float amountTime)
	{
		Debug.Log(amountTime);
		time += amountTime;
		
		Debug.Log(time);
	}
}