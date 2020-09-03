using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class EtoManager : MonoBehaviour
{
	public Eto etoPrefab;
	private GameObject firstBall;
	private GameObject lastBall;
	private string currentName;
	public List<GameObject> selectedBallList = new List<GameObject>();

	private int point = 100;
	public bool isPlaying = true;

	// 生成されている干支
	private List<Eto> etoList = new List<Eto>();

	public Transform canvasTran;
	public int generateBallCount;

	public int linkCount = 0;

	// List
	public List<EtoDetail> etoDetailList = new List<EtoDetail>();

	[Header("今回のゲームに登場する干支")]
	public List<EtoDetail> selectedEtoList = new List<EtoDetail>();


	void Start() {
		//StartCoroutine(DropBall(generateBallCount));
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
			Eto ball = hitObj.GetComponent<Eto>();

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
			Eto ball = hitObj.GetComponent<Eto>();


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
				Debug.Log(hitObj.GetComponent<Eto>().num);
				if (selectedBallList[linkCount - 1] != lastBall && selectedBallList[linkCount - 1].GetComponent<Eto>().num == hitObj.GetComponent<Eto>().num && hitObj.GetComponent<Eto>().isSelected) {
					
					// 選択中のボールを取り除く 
					RemoveSelectedBallList(lastBall);
					lastBall.GetComponent<Eto>().isSelected = false;

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
			
			// 加算処理
			//scoreGUI.SendMessage("AddPoint", point * selectedBallList.Count);
			UIManager.instance.AddScore(point * selectedBallList.Count);

			StartCoroutine(CreateEtos(selectedBallList.Count));
			selectedBallList.Clear();
		} else {
			for (int i = 0; i < selectedBallList.Count; i++) {
				selectedBallList[i].GetComponent<Eto>().isSelected = false;　//選んだ数か2個以下の場合　各干支のboolを解除する
				ChangeBallColorAlpha(selectedBallList[i], 1.0f);
			}
		}
		firstBall = null;
		lastBall = null;
	}

	/// <summary>
	/// 干支を生成
	/// </summary>
	/// <param name="count"></param>
	/// <returns></returns>
	public IEnumerator CreateEtos(int count) {
		// 生成中はシャッフルボタンを押せないようにする
		UIManager.instance.InactiveWind(false);


		// ゲームに登場させる干支の種類を設定する
		if (selectedBallList.Count == 0) {
			yield return ChooseEtoType(GameData.instance.etoTypeCount);
		}

        for (int i = 0; i < count; i++) {
            // Ballプレファブを生成し、Ballクラスに代入
            Eto eto = Instantiate(etoPrefab, canvasTran, false);
            eto.transform.rotation = Quaternion.AngleAxis(Random.Range(-40, 40), Vector3.forward);
            eto.transform.position = new Vector2(Random.Range(-2.0f, 2.0f), 7f);

            // 今回のゲームに登場する干支の中から、ランダムな干支を１つ選択
            int randomVelue = Random.Range(0, selectedEtoList.Count);

            // Ballの初期設定
            eto.SetUpBall(selectedEtoList[randomVelue].etoType, selectedEtoList[randomVelue].imgEto.sprite);

            // ballListに追加
            etoList.Add(eto);

            yield return new WaitForSeconds(0.05f);
        }
        UIManager.instance.InactiveWind(true);
	}

	/// <summary>
	/// ゲームに登場させる干支の種類を設定する
	/// </summary>
	/// <param name="typeCount"></param>
	/// <returns></returns>
	private IEnumerator ChooseEtoType(int typeCount) {
		// 新しくリストを用意して初期化し、etoDeatilListを複製する
		List<EtoDetail> chooseEtoList = new List<EtoDetail>(etoDetailList);

		// 選択中の干支を選んでリストに追加する
		EtoDetail myEto = chooseEtoList.Find((x) => x.etoType == GameData.instance.etoType);
		selectedEtoList.Add(myEto);
		chooseEtoList.Remove(myEto);
		typeCount--;

        // それ以外の干支を指定数だけをランダムに選ぶ
        while (typeCount > 0) {
			// ランダムに数字を選ぶ
			int randomValue = Random.Range(0, chooseEtoList.Count);
			// リストに追加
			selectedEtoList.Add(chooseEtoList[randomValue]);
			// 選択範囲のリストから削除
			chooseEtoList.Remove(chooseEtoList[randomValue]);
			typeCount--;
			yield return null;
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
        if (obj.GetComponent<Eto>().isSelected) {
			obj.GetComponent<Eto>().isSelected = false;
		}
	}

	/// <summary>
	/// ボールのアルファを変更
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="transparency"></param>
	void ChangeBallColorAlpha(GameObject obj, float transparency) {
		Image imageEto = obj.GetComponent<Image>();

		imageEto.color = new Color(imageEto.color.r, imageEto.color.g, imageEto.color.b, transparency);
	}

	/// <summary>
	/// 特定の色のボールを違う色に変更する
	/// </summary>
	/// <param name="chooseBallColorNum">対象となるボールの番号</param>
	/// <param name="changeSprite">変更する色</param>
	//public void ChangeSpecificBalls(EtoType chooseEtoType, Sprite changeSprite) {
	//	// chooseEtoTypeと同じ干支を持つ干支を探し出して、そのイメージをchangeBallColorNumに変える

	//	// 干支変更の対象となる干支を入れるリストを用意
	//	List<Eto> chooseBallList = new List<Eto>();

	//	// 色の変更となるボールをすべて検索し、照合したボールをリストに代入する
	//	// ①Linqで書く場合
	//	chooseBallList = etoList.Where(x => x.etoType == chooseEtoType).ToList();

	//	// ②foreachで書く場合
	//	foreach (Eto ball in etoList) {
	//		if (ball.etoNum == chooseBallColorNum) {
	//			chooseBallList.Add(ball);
	//		}
	//	}

	//	// 対象となったボールの色を指定された色に変更する
	//	for (int i = 0; i < chooseBallList.Count; i++) {
	//		chooseBallList[i].ChangeBallColor(changeSprite);
	//	}
	//}

	/// <summary>
	/// 特定の色のボールの色をランダムに変更する
	/// </summary>
	//public void ChangeSpecificBallsToRandomColor(int chooseBallColorNum) { 
	//	// chooseBallColorNumという番号のボールを探しだして、ランダムな色に変える
	//	List<Eto> chooseBallList = new List<Eto>();

	//	// 色の変更となるボールをすべて検索し、照合したボールをリストに代入する
	//	chooseBallList = etoList.Where(X => X.etoNum == chooseBallColorNum).ToList();

	//	// foreachで書く場合
	//	foreach (Eto ball in etoList) {
	//		if (ball.etoNum == chooseBallColorNum) {
	//			chooseBallList.Add(ball);
	//		}
	//	}

	//	// 対象となったボールの色を指定された色に変更する
	//	for (int i = 0; i < chooseBallList.Count; i++) {
	//		chooseBallList[i].ChangeBallColor(selectedEtoList[Random.Range(0, selectedEtoList.Count)].imgEto.sprite);
	//	}
	//}

	/// <summary>
	/// すべてのボールをランダムな色に変更する
	/// </summary>
	public void ChangeRandomBalls() {
		for (int i = 0; i < etoList.Count; i++) {
			etoList[i].ChangeBallColor(selectedEtoList[Random.Range(0, selectedEtoList.Count)].imgEto.sprite);
		}
	}

	// 自分の干支以外で一番数の多い干支をすべて自分の干支に変える


}
