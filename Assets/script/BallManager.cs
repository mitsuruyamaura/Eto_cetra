using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class BallManager : MonoBehaviour
{
	public Ball ballPrefab;
	private GameObject firstBall;
	private GameObject lastBall;
	private string currentName;
	public List<GameObject> selectedBallList = new List<GameObject>();
	public GameObject scoreGUI;
	private int point = 100;
	public bool isPlaying = true;

	private List<Ball> ballList = new List<Ball>();

	public Transform canvasTran;
	public int generateBallCount;

	public int linkCount = 0;

	public int etoTypeCount;     // 登場するボールの種類



	void Start() {
		StartCoroutine(DropBall(generateBallCount));
	}

	void Update() {
		if (isPlaying) {
			if (Input.GetMouseButtonDown(0) && firstBall == null) {
				OnDragStart();
			} else if (Input.GetMouseButtonUp(0)) {
				OnDragEnd();
			} else if (firstBall != null) {
				OnDragging();
			}
		}
	}

	private void OnDragStart() {
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		linkCount = 0;
		if (hit.collider != null) {
			GameObject hitObj = hit.collider.gameObject;
			Ball ball = hitObj.GetComponent<Ball>();

			string ballName = hitObj.name;
			if (ballName.StartsWith("Ball")) {
				firstBall = hitObj;
				lastBall = hitObj;
				currentName = hitObj.name;

				ball.isSelected = true;
				ball.num = linkCount;

				selectedBallList = new List<GameObject>();
				AddSelectedBallList(hitObj);
			}
		}
	}

	private void OnDragging() {
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		if (hit.collider != null) {
			GameObject hitObj = hit.collider.gameObject;
			Ball ball = hitObj.GetComponent<Ball>();


			if (hitObj.name == currentName && lastBall != hitObj && !ball.isSelected) {
				float distance = Vector2.Distance(hitObj.transform.position, lastBall.transform.position);
				if (distance < 1.0f) {
					ball.isSelected = true;

					lastBall = hitObj;

					linkCount++;
					ball.num = linkCount;
					AddSelectedBallList(hitObj);
					
				}
			}
			Debug.Log(ball.num);

			if (selectedBallList.Count > 1) {
				Debug.Log(hitObj.GetComponent<Ball>().num);
				if (selectedBallList[linkCount - 1] != lastBall && selectedBallList[linkCount - 1].GetComponent<Ball>().num == hitObj.GetComponent<Ball>().num && hitObj.GetComponent<Ball>().isSelected) {
					
					// 選択中のボールを取り除く 
					RemoveSelectedBallList(lastBall);
					lastBall.GetComponent<Ball>().isSelected = false;

					// 最後のボールの情報を、前のボールに戻す
					lastBall = hitObj;
					linkCount--;
				}
			}
		}
	}

	private void OnDragEnd() {
		if (selectedBallList.Count >= 3) {
			for (int i = 0; i < selectedBallList.Count; i++) {
				Destroy(selectedBallList[i]);
			}
			scoreGUI.SendMessage("AddPoint", point * selectedBallList.Count);
			StartCoroutine(DropBall(selectedBallList.Count));
		} else {
			for (int i = 0; i < selectedBallList.Count; i++) {
				selectedBallList[i].GetComponent<Ball>().isSelected = false;　//選んだ数か2個以下の場合　各ぴよのboolを解除する
				ChangeBallColorAlpha(selectedBallList[i], 1.0f);
			}
		}
		firstBall = null;
		lastBall = null;
	}

	IEnumerator DropBall(int count) {
		for (int i = 0; i < count; i++) {
			// Ballプレファブを生成し、Ballクラスに代入
			Ball ball = Instantiate(ballPrefab, canvasTran, false);
			ball.transform.rotation = Quaternion.AngleAxis(Random.Range(-40, 40), Vector3.forward);
			ball.transform.position = new Vector2(Random.Range(-2.0f, 2.0f), 7f);

			// Ballの初期設定
			ball.SetUpBall();

			// ballListに追加
			ballList.Add(ball);

			yield return new WaitForSeconds(0.05f);
		}
	}


	/// <summary>
	/// つながったボールをリストに追加
	/// </summary>
	/// <param name="obj"></param>
	void AddSelectedBallList(GameObject obj) {
		selectedBallList.Add(obj);
		ChangeBallColorAlpha(obj, 0.5f);
	}

	/// <summary>
	/// 前のボールに戻った際にリストから削除
	/// </summary>
	/// <param name="obj"></param>
	private void RemoveSelectedBallList(GameObject obj) {
		selectedBallList.Remove(obj);
		ChangeBallColorAlpha(obj, 1.0f);
        if (obj.GetComponent<Ball>().isSelected) {
			obj.GetComponent<Ball>().isSelected = false;
		}
	}

	/// <summary>
	/// ボールのアルファを変更
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="transparency"></param>
	void ChangeBallColorAlpha(GameObject obj, float transparency) {
		SpriteRenderer ballTexture = obj.GetComponent<SpriteRenderer>();
		ballTexture.color = new Color(ballTexture.color.r, ballTexture.color.g, ballTexture.color.b, transparency);
	}

	/// <summary>
	/// 特定の色のボールを違う色に変更する
	/// </summary>
	/// <param name="chooseBallColorNum">対象となるボールの番号</param>
	/// <param name="changeBallColorNum">変更する色</param>
	public void ChangeSpecificBalls(int chooseBallColorNum, int changeBallColorNum) {
		// chooseBallColorNumの番号を持つボールを探し出して、その色をchangeBallColorNumに変える

		// 色変更の対象となるボールを入れるリストを用意
		List<Ball> chooseBallList = new List<Ball>();

		// 色の変更となるボールをすべて検索し、照合したボールをリストに代入する
		// ①Linqで書く場合
		chooseBallList = ballList.Where(X => X.ballNum == chooseBallColorNum).ToList();

		// ②foreachで書く場合
		foreach (Ball ball in ballList) {
			if (ball.ballNum == chooseBallColorNum) {
				chooseBallList.Add(ball);
			}
		}

		// 対象となったボールの色を指定された色に変更する
		for (int i = 0; i < chooseBallList.Count; i++) {
			chooseBallList[i].ChangeBallColor(changeBallColorNum);
		}
	}

	/// <summary>
	/// 特定の色のボールの色をランダムに変更する
	/// </summary>
	public void ChangeSpecificBallsToRandomColor(int chooseBallColorNum) { 
		// chooseBallColorNumという番号のボールを探しだして、ランダムな色に変える
		List<Ball> chooseBallList = new List<Ball>();

		// 色の変更となるボールをすべて検索し、照合したボールをリストに代入する
		chooseBallList = ballList.Where(X => X.ballNum == chooseBallColorNum).ToList();

		// foreachで書く場合
		foreach (Ball ball in ballList) {
			if (ball.ballNum == chooseBallColorNum) {
				chooseBallList.Add(ball);
			}
		}

		// 対象となったボールの色を指定された色に変更する
		for (int i = 0; i < chooseBallList.Count; i++) {
			chooseBallList[i].ChangeBallColor(Random.Range(0, etoTypeCount));
		}
	}

	/// <summary>
	/// すべてのボールをランダムな色に変更する
	/// </summary>
	public void ChangeRandomBalls() {
		for (int i = 0; i < ballList.Count; i++) {
			ballList[i].ChangeBallColor(Random.Range(0, etoTypeCount));
		}
	}
}
