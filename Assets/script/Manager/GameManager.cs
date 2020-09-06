using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
	public Eto etoPrefab;

	private Eto firstEto;
	private Eto lastEto;
	private EtoType? currentEtoType;
	public List<Eto> removeEtoList = new List<Eto>();

	// 生成されている干支
	private List<Eto> etoList = new List<Eto>();

	public Transform etoSetTran;

	public int linkCount = 0;

	public float etoDistance = 1.0f;

	public float maxRotateAngle = 40.0f;

	// 干支12種類のList(干支ボタン生成とゲームに登場する干支の種類を決める際に使用する)
	public List<EtoDetail> etoDetailList = new List<EtoDetail>();

	[Header("今回のゲームに登場する干支")]
	public List<EtoDetail> selectedEtoList = new List<EtoDetail>();

	public enum GameState {
		Select,
		Ready,
		Play,
		Result
    }
	public GameState gameState = GameState.Select;

	public UIManager uiManager;

	public ResultPopUp resultPopUp;
	public Transform canvasTran;

	public int eraseEtoCount = 0;

    void Start() {
		//StartCoroutine(CreateEtos(generateBallCount));
	}

	void Update() {
		if (gameState != GameState.Play) {
			return;
		}

		// 干支をつなげる処理
		if (Input.GetMouseButtonDown(0) && firstEto == null) {
			OnStartDrag();
		} else if (Input.GetMouseButtonUp(0)) {
			OnEndDrag();
		} else if (firstEto != null) {
			OnDragging();
		}

		// ゲームの残り時間のカウント処理
		GameData.instance.gameTime -= Time.deltaTime;

		if (GameData.instance.gameTime <= 0) {
			GameData.instance.gameTime = 0;
			// ゲーム終了
			StartCoroutine(GameUp());
		}
		// 残り時間の表示更新
		uiManager.UpdateDisplayGameTime(GameData.instance.gameTime);
	}

	/// <summary>
	/// 干支を最初にドラッグした際の処理
	/// </summary>
	private void OnStartDrag() {
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

		// 干支がつながっている数を初期化
		linkCount = 0;
		
		if (hit.collider != null) {			
			if(hit.collider.gameObject.TryGetComponent(out Eto dragEto)) { 
				firstEto = dragEto;
				lastEto = dragEto;
				currentEtoType = dragEto.etoType;

				dragEto.isSelected = true;
				dragEto.num = linkCount;

				removeEtoList = new List<Eto>();
				AddSelectedBallList(dragEto);
			}
		}
	}

	private void OnDragging() {
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out Eto dragEto)) {
            if (currentEtoType == null) {
				return;
            }

			if (dragEto.etoType == currentEtoType && lastEto != dragEto && !dragEto.isSelected) {
				float distance = Vector2.Distance(dragEto.transform.position, lastEto.transform.position);
				if (distance < etoDistance) {
					dragEto.isSelected = true;

					lastEto = dragEto;

					linkCount++;
					dragEto.num = linkCount;
					AddSelectedBallList(dragEto);
					
				}
			}
			Debug.Log(dragEto.etoType);

			if (removeEtoList.Count > 1) {
				Debug.Log(dragEto.num);
				if (removeEtoList[linkCount - 1] != lastEto && removeEtoList[linkCount - 1].num == dragEto.num && dragEto.isSelected) {
					
					// 選択中のボールを取り除く 
					RemoveSelectedBallList(lastEto);
					lastEto.GetComponent<Eto>().isSelected = false;

					// 最後のボールの情報を、前のボールに戻す
					lastEto = dragEto;
					linkCount--;
				}
			}
		}
	}

	private void OnEndDrag() {
		if (removeEtoList.Count >= 3) {
			// 選択されている干支を消す
			for (int i = 0; i < removeEtoList.Count; i++) {
				// 干支リストから取り除く
				etoList.Remove(removeEtoList[i]);
				// 干支を削除
				Destroy(removeEtoList[i].gameObject);
			}

			// スコアと消した干支の数の加算
			AddScores(currentEtoType, removeEtoList.Count);

			//// スキルポイント加算
			//uiManager.AddSkillPoint(removeEtoList.Count);

			//// 消した数を加算
			//eraseEtoCount += removeEtoList.Count;

   //         // スコア加算処理
   //         if (currentEtoType == GameData.instance.etoType) {
			//	// 選択している干支の場合にはスコアボーナス
			//	uiManager.AddScore(Mathf.CeilToInt(GameData.instance.etoPoint * removeEtoList.Count * GameData.instance.etoRate), true);
			//} else {
			//	// それ以外
			//	uiManager.AddScore(GameData.instance.etoPoint * removeEtoList.Count, false);
			//}
			
			// TODO ４つ以上消えていたら、ボーナス

			// 消した干支の数だけ新しい干支をランダムに生成
			StartCoroutine(CreateEtos(removeEtoList.Count));
			removeEtoList.Clear();
		} else {
			for (int i = 0; i < removeEtoList.Count; i++) {
				// 選んだ数か2個以下の場合　各干支のboolを解除する
				removeEtoList[i].isSelected = false;　
				ChangeBallColorAlpha(removeEtoList[i], 1.0f);
			}
		}
		firstEto = null;
		lastEto = null;
		currentEtoType = null;
	}

	/// <summary>
	/// ゲームの準備
	/// </summary>
	public IEnumerator PreparateGame() {
		// ステートを準備中に変更
		gameState = GameState.Ready;

		// ゲームに登場させる干支の種類を設定する
		yield return StartCoroutine(SetUpEtoTypes(GameData.instance.etoTypeCount));

		yield return StartCoroutine(SetUpSkill(GameData.instance.skillType));

        // 初回分の干支を生成
		StartCoroutine(CreateEtos(GameData.instance.createEtoCount));
	}

	/// <summary>
	/// 干支を生成
	/// </summary>
	/// <param name="count"></param>
	/// <returns></returns>
	public IEnumerator CreateEtos(int count) {
		// 生成中はシャッフルボタンを押せないようにする
		uiManager.InactiveWind(false);

        for (int i = 0; i < count; i++) {
            // Ballプレファブを生成し、Ballクラスに代入
            Eto eto = Instantiate(etoPrefab, etoSetTran, false);
            eto.transform.rotation = Quaternion.AngleAxis(Random.Range(-maxRotateAngle, maxRotateAngle), Vector3.forward);
            eto.transform.localPosition = new Vector2(Random.Range(-400.0f, 400.0f), 1400f);

            // 今回のゲームに登場する干支の中から、ランダムな干支を１つ選択
            int randomVelue = Random.Range(0, selectedEtoList.Count);

            // Ballの初期設定
            eto.SetUpBall(selectedEtoList[randomVelue].etoType, selectedEtoList[randomVelue].imgEto.sprite);

            // ballListに追加
            etoList.Add(eto);

            yield return new WaitForSeconds(0.05f);
        }
		uiManager.InactiveWind(true);

		// ステートが準備中のときだけゲーム開始に変更
        if (gameState == GameState.Ready) {
			gameState = GameState.Play;
        }
	}

	/// <summary>
	/// ゲームに登場させる干支の種類を設定する
	/// </summary>
	/// <param name="typeCount"></param>
	/// <returns></returns>
	private IEnumerator SetUpEtoTypes(int typeCount) {
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
	/// 選択されたスキルをボタンに登録
	/// </summary>
	/// <param name="skillType"></param>
	/// <returns></returns>
	private IEnumerator SetUpSkill(SkillType skillType) {
		


		yield return StartCoroutine(uiManager.SetUpSkillButton(GetSkill(skillType)));
    }

	/// <summary>
	/// スキルボタンに登録するスキルのメソッドを取得して戻す
	/// </summary>
	public UnityAction GetSkill(SkillType chooseSkillType) {
		switch (chooseSkillType) {
			case SkillType.DeleteMaxBall:
				return DeleteMaxBalls;
		}
		return null;
	}

	/// <summary>
	/// つながった干支を削除リストに追加
	/// </summary>
	/// <param name="obj"></param>
	void AddSelectedBallList(Eto dragEto) {
		removeEtoList.Add(dragEto);
		ChangeBallColorAlpha(dragEto, 0.5f);
	}

	/// <summary>
	/// 前の干支に戻った際に削除リストから削除
	/// </summary>
	/// <param name="obj"></param>
	private void RemoveSelectedBallList(Eto dragEto) {
		removeEtoList.Remove(dragEto);
		ChangeBallColorAlpha(dragEto, 1.0f);
        if (dragEto.isSelected) {
			dragEto.isSelected = false;
		}
	}

	/// <summary>
	/// 干支のアルファを変更
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="transparency"></param>
	private void ChangeBallColorAlpha(Eto dragEto, float transparency) {
		dragEto.imgEto.color = new Color(dragEto.imgEto.color.r, dragEto.imgEto.color.g, dragEto.imgEto.color.b, transparency);
	}

	/// <summary>
	/// ゲーム終了ステートに変更
	/// </summary>
	public IEnumerator GameUp() {
		// シャッフルを止める
		uiManager.InactiveWind(false);

		// ボタンを押せなくする
		uiManager.InActiveButtons();

		gameState = GameState.Result;
		yield return new WaitForSeconds(1.5f);

		yield return StartCoroutine(MoveResultPopUp());
	}

	/// <summary>
	/// リザルドポップアップを画面内にスライドイン表示
	/// </summary>
	/// <returns></returns>
	private IEnumerator MoveResultPopUp() {
		resultPopUp.gameObject.SetActive(true);

		Sequence sequence = DOTween.Sequence();
		sequence.Append(resultPopUp.transform.DOMoveY(0, 1.0f)).SetEase(Ease.Linear);
		sequence.Append(resultPopUp.transform.DOPunchPosition(new Vector3(0, 0, 0), 0.5f))
			.OnComplete(() => {
				// ゲーム結果表示
				resultPopUp.DisplayResult(eraseEtoCount);
			}
		);

		yield return new WaitForSeconds(1.0f);
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


	/// <summary>
	/// 最も数の多い干支をまとめて削除する
	/// </summary>
	public void DeleteMaxBalls() {
		// Dictinaryを初期化。干支のタイプを数を代入できるようにする
		Dictionary<EtoType, int> dictionary = new Dictionary<EtoType, int>();

		// リストの中から干支のタイプごとにDictionaryの要素を作成(ここで５つの干支タイプごとにいくつ数があるかわかる)
        foreach (Eto eto in etoList) {
            if (dictionary.ContainsKey(eto.etoType)) {
				// すでにある要素の場合には数のカウントを加算
				dictionary[eto.etoType]++; 
            } else {
				// まだ作られていない干支のタイプの場合には新しく要素を作り、カウントを１する
				dictionary.Add(eto.etoType, 1);
            }
        }

		// Debug
        foreach (KeyValuePair<EtoType, int> keyValuePair in dictionary) {
			Debug.Log("干支 : " + keyValuePair.Key + " 数 : " + keyValuePair.Value);
        }

		// Dictionaryを検索し、最も数の多い干支のタイプを見つけて、消す干支のタイプと数を決定
		EtoType maxEtoType = dictionary.OrderByDescending(x => x.Value).First().Key;
		int removeNum = dictionary.OrderByDescending(x => x.Value).First().Value;
		
		Debug.Log("消す干支 : " + maxEtoType + " 数 : " + removeNum);
		
        // 対象の干支を破壊
        for (int i = 0; i < etoList.Count; i++) {
            if (etoList[i].etoType == maxEtoType) {
				Destroy(etoList[i].gameObject);
            }
        }

		// etoListから対象の干支データを削除
		etoList.RemoveAll(x => x.etoType == maxEtoType);

		// 点数と消した干支の加算
		AddScores(maxEtoType, removeNum);

		// 破壊した干支の数だけ干支を生成
		StartCoroutine(CreateEtos(removeNum));
	}

	/// <summary>
	/// スコアと消した干支の数を加算
	/// </summary>
	private void AddScores(EtoType? etoType, int count) {
		// スキルポイント加算
		uiManager.AddSkillPoint(count);

		// 消した数を加算
		eraseEtoCount += count;

		// スコア加算処理
		if (etoType == GameData.instance.etoType) {
			// 選択している干支の場合にはスコアボーナス
			uiManager.UpdateDisplayScore(Mathf.CeilToInt(GameData.instance.etoPoint * count * GameData.instance.etoRate), true);
		} else {
			// それ以外
			uiManager.UpdateDisplayScore(GameData.instance.etoPoint * count, false);
		}
	}
}
