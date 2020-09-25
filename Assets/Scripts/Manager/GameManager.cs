using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine.Events;

public class GameManager : MonoBehaviour   // 干支の選択に戻ります。
{
	public Eto etoPrefab;

	public Transform etoSetTran;

	public float maxRotateAngle = 40.0f;

	[SerializeField, Header("生成された干支のリスト")]
	private List<Eto> etoList = new List<Eto>();

	//[SerializeField, Header("干支の画像データ")]
	//private Sprite[] etoSprites;

	private Eto firstSelectEto;
	private Eto lastSelectEto;
	private EtoType? currentEtoType;
    public List<Eto> eraseEtoList = new List<Eto>();

	public int linkCount = 0;

	public float etoDistance = 1.0f;

	[SerializeField]
	private UIManager uiManager;

	/// <summary>
    /// ゲームの進行状況
    /// </summary>
	public enum GameState
	{
		Select,
		Ready,
		Play,
		Result
	}

	[Header("現在のゲームの進行状況")]
	public GameState gameState = GameState.Select;

	private float timer;

	[SerializeField]
	private ResultPopUp resultPopUp;

	[SerializeField, Header("今回のゲームで生成する干支の種類")]
	private List<GameData.EtoData> selectedEtoDataList = new List<GameData.EtoData>();

	//追加
	[SerializeField, Header("干支の削除演出エフェクトのプレファブ")]
	private GameObject eraseEffectPrefab;

	[SerializeField]
	private EtoSelectPopUp etoSelectPopUp;

	IEnumerator Start() {
		StartCoroutine(TransitionManager.instance.FadePanel(0.0f));

		SoundManager.Instance.PlayBGM(SoundManager.Enum_BGM.Select);

		// スコアなどを初期化
		GameData.instance.InitGame();

		// ステートを干支選択中に変更
		gameState = GameState.Select;

		// UIManagerの初期設定
		yield return StartCoroutine(uiManager.Initialize());

		// 干支データのリストが作成されてなければ
		if (GameData.instance.etoDataList.Count == 0) {
			//Sprite[] etoSprites = new Sprite[(int)currentEtoType.Count];
			// 干支の画像を読みこむ。この処理が終了するまで、次の処理へはいかないようにする
			//yield return StartCoroutine(LoadEtoSprites());

			// 干支データのリストを作成。この処理が終了するまで、次の処理へはいかないようにする
			yield return StartCoroutine(GameData.instance.InitEtoDataList());
		}

		// 干支の選択ポップアップに干支選択ボタンを生成
		yield return StartCoroutine(etoSelectPopUp.CreateEtoButtons(this));

		// 生成する干支をランダムで選択。この処理が終了するまで、次の処理へはいかないようにする
		//yield return StartCoroutine(SetUpEtoTypes(GameData.instance.etoTypeCount));

		// 残り時間の表示
		//uiManager.UpdateDisplayGameTime(GameData.instance.gameTime);

		// 引数で指定した数の干支を生成する
		//StartCoroutine(CreateEtos(GameData.instance.createEtoCount));
	}

	/// <summary>
	/// 干支の画像を読み込んで配列から使用できるようにする
	/// </summary>
	private IEnumerator LoadEtoSprites()
	{
		// 配列の初期化(12個の画像が入るようにSprite型の配列を12個用意する)
		//etoSprites = new Sprite[(int)EtoType.Count];

		 // Resources.LoadAllを行い、分割されている干支の画像を順番にすべて読み込んで配列に代入
		//etoSprites = Resources.LoadAll<Sprite>("Sprites/eto");

		// １つのファイルを１２分割していない場合には、以下の処理を使います。
		//for (int i = 0; i < etoSprites.Length; i++)
		//{
		//	etoSprites[i] = Resources.Load<Sprite>("Sprites/eto_" + i);
		//}

		yield break;
	}


	void Update() {
		if (gameState != GameState.Play) {
			return;
		}

		// 干支をつなげる処理
		if (Input.GetMouseButtonDown(0) && firstSelectEto == null) {
			OnStartDrag();
		} else if (Input.GetMouseButtonUp(0)) {
			OnEndDrag();
		} else if (firstSelectEto != null) {
			OnDragging();
		}

		// ゲームの残り時間のカウント処理
		timer += Time.deltaTime;

		if (timer >= 1) {
			timer = 0;
			GameData.instance.gameTime--;

			if (GameData.instance.gameTime <= 0)
			{
				GameData.instance.gameTime = 0;
				// ゲーム終了
				StartCoroutine(GameUp());
			}
			// 残り時間の表示更新
			uiManager.UpdateDisplayGameTime(GameData.instance.gameTime);
		}
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
				firstSelectEto = dragEto;
				lastSelectEto = dragEto;
				currentEtoType = dragEto.etoType;

				dragEto.isSelected = true;
				dragEto.num = linkCount;

				eraseEtoList = new List<Eto>();
				AddEraseEtolList(dragEto);
			}
		}
	}

	/// <summary>
    /// 干支のドラッグ中（スワイプ）処理
    /// </summary>
	private void OnDragging() {
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out Eto dragEto)) {
            if (currentEtoType == null) {
				return;
            }

			if (dragEto.etoType == currentEtoType && lastSelectEto != dragEto && !dragEto.isSelected) {
				float distance = Vector2.Distance(dragEto.transform.position, lastSelectEto.transform.position);
				if (distance < etoDistance) {
					dragEto.isSelected = true;

					lastSelectEto = dragEto;

					linkCount++;
					dragEto.num = linkCount;
					AddEraseEtolList(dragEto);
					
				}
			}
			//Debug.Log(dragEto.etoType);

			if (eraseEtoList.Count > 1) {
				//Debug.Log(dragEto.num);
				if (eraseEtoList[linkCount - 1] != lastSelectEto && eraseEtoList[linkCount - 1].num == dragEto.num && dragEto.isSelected) {
					
					// 選択中のボールを取り除く 
					RemoveEraseEtoList(lastSelectEto);
					lastSelectEto.GetComponent<Eto>().isSelected = false;

					// 最後のボールの情報を、前のボールに戻す
					lastSelectEto = dragEto;
					linkCount--;
				}
			}
		}
	}

	/// <summary>
    /// 干支のドラッグをやめた（指を画面から離した）際の処理
    /// </summary>
	private void OnEndDrag() {
		if (eraseEtoList.Count >= 3) {
			// 選択されている干支を消す
			for (int i = 0; i < eraseEtoList.Count; i++) {
				// 干支リストから取り除く
				etoList.Remove(eraseEtoList[i]);

				// エフェクト生成
				GameObject effect = Instantiate(eraseEffectPrefab, eraseEtoList[i].gameObject.transform);
				effect.transform.SetParent(etoSetTran);

				// 干支を削除
				Destroy(eraseEtoList[i].gameObject);
			}

			// スコアと消した干支の数の加算
			AddScores(currentEtoType, eraseEtoList.Count);

			//// スキルポイント加算
			//uiManager.AddSkillPoint(removeEtoList.Count);
			
			// TODO ４つ以上消えていたら、ボーナス

			// 消した干支の数だけ新しい干支をランダムに生成
			StartCoroutine(CreateEtos(eraseEtoList.Count));
			eraseEtoList.Clear();
		} else {
			for (int i = 0; i < eraseEtoList.Count; i++) {
				// 選んだ数か2個以下の場合　各干支のboolを解除する
				eraseEtoList[i].isSelected = false;　
				ChangeEtoAlpha(eraseEtoList[i], 1.0f);
			}
		}
		firstSelectEto = null;
		lastSelectEto = null;
		currentEtoType = null;
	}

	/// <summary>
	/// ゲームの準備
	/// </summary>
	public IEnumerator PreparateGame() {
		// ステートを準備中に変更
		gameState = GameState.Ready;

		// 残り時間の表示
		uiManager.UpdateDisplayGameTime(GameData.instance.gameTime);

		// ゲームに登場させる干支の種類を設定する
		yield return StartCoroutine(SetUpEtoTypes(GameData.instance.etoTypeCount));

		yield return StartCoroutine(SetUpSkill(GameData.instance.skillType));

		// 引数で指定した数の干支を生成する
		StartCoroutine(CreateEtos(GameData.instance.createEtoCount));

		SoundManager.Instance.PlayBGM(SoundManager.Enum_BGM.Game);
	}

	/// <summary>
	/// 干支を生成
	/// </summary>
	/// <param name="count"></param>
	/// <returns></returns>
	private IEnumerator CreateEtos(int count) {
		// 生成中はシャッフルボタンを押せないようにする
		uiManager.ActivateShuffleButton(false);

        for (int i = 0; i < count; i++) {
            // Ballプレファブを生成し、Ballクラスに代入
            Eto eto = Instantiate(etoPrefab, etoSetTran, false);
            eto.transform.rotation = Quaternion.AngleAxis(Random.Range(-maxRotateAngle, maxRotateAngle), Vector3.forward);
            eto.transform.localPosition = new Vector2(Random.Range(-400.0f, 400.0f), 1400f);

			// 今回のゲームに登場する干支の中から、ランダムな干支を１つ選択
			int randomValue = Random.Range(0, selectedEtoDataList.Count);

            // 干支の初期設定
            eto.SetUpEto(selectedEtoDataList[randomValue].etoType, selectedEtoDataList[randomValue].sprite);

			etoList.Add(eto);

			yield return new WaitForSeconds(0.03f);
        }

		// シャッフルボタンを押せるようにする
		uiManager.ActivateShuffleButton(true);

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

		// 新しくリストを用意して初期化に合わせてetoDataListを複製して、干支の候補リストとする
		List<GameData.EtoData> candidateEtoDataList = new List<GameData.EtoData>(GameData.instance.etoDataList);

		// 選択中の干支を探して生成する干支のリストに追加
		GameData.EtoData myEto = candidateEtoDataList.Find((x) => x.etoType == GameData.instance.selectedEtoData.etoType);
		selectedEtoDataList.Add(myEto);
		candidateEtoDataList.Remove(myEto);
		typeCount--;

		// それ以外の干支を指定数だけをランダムに選ぶ
		while (typeCount > 0)
		{
			// ランダムに数字を選ぶ
			int randomValue = Random.Range(0, candidateEtoDataList.Count);

			// 生成する干支のリストに追加
			selectedEtoDataList.Add(candidateEtoDataList[randomValue]);

			// 選択範囲のリストから削除
			candidateEtoDataList.Remove(candidateEtoDataList[randomValue]);
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
	/// <param name="dragEto"></param>
	void AddEraseEtolList(Eto dragEto) {
		eraseEtoList.Add(dragEto);
		ChangeEtoAlpha(dragEto, 0.5f);
	}

	/// <summary>
	/// 前の干支に戻った際に削除リストから削除
	/// </summary>
	/// <param name="dragEto"></param>
	private void RemoveEraseEtoList(Eto dragEto) {
		eraseEtoList.Remove(dragEto);
		ChangeEtoAlpha(dragEto, 1.0f);
        if (dragEto.isSelected) {
			dragEto.isSelected = false;
		}
	}

	/// <summary>
	/// 干支のアルファを変更
	/// </summary>
	/// <param name="dragEto"></param>
	/// <param name="alphaValue"></param>
	private void ChangeEtoAlpha(Eto dragEto, float alphaValue) {
		dragEto.imgEto.color = new Color(dragEto.imgEto.color.r, dragEto.imgEto.color.g, dragEto.imgEto.color.b, alphaValue);
	}

	/// <summary>
	/// ゲーム終了ステートに変更
	/// </summary>
	private IEnumerator GameUp() {
		// シャッフルボタンを非活性化して押せなくする
		uiManager.ActivateShuffleButton(false);

		// ボタンを押せなくする
		//uiManager.InActiveButtons();

		gameState = GameState.Result;
		yield return new WaitForSeconds(1.5f);

		yield return StartCoroutine(MoveResultPopUp());
	}

	/// <summary>
	/// リザルドポップアップを画面内にスライドイン表示
	/// </summary>
	/// <returns></returns>
	private IEnumerator MoveResultPopUp() {
		// 
		//resultPopUp.gameObject.SetActive(true);

		resultPopUp.transform.DOMoveY(0, 1.0f).SetEase(Ease.Linear).OnComplete(() => {
			// ゲーム結果表示
			resultPopUp.DisplayResult(GameData.instance.score, GameData.instance.eraseEtoCount);
			//Debug.Log("リザルト内容を表示します");
		});

		// SEを鳴らすまでの待機時間
		yield return new WaitForSeconds(0.5f);

		// ドラムロールのSE再生
		SoundManager.Instance.PlaySE(SoundManager.Enum_SE.Result);

		// SEが流れ終わってBGMを切り替えるまでの待機時間
		yield return new WaitForSeconds(2.5f);

		SoundManager.Instance.PlayBGM(SoundManager.Enum_BGM.Result);
    }

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
	/// <param name="etoType">消した干支の種類</param>
	/// <param name="count">消した干支の数</param>
	private void AddScores(EtoType? etoType, int count) {
		// スキルポイント加算
		uiManager.AddSkillPoint(count);

		// スコアを加算
		bool isChooseEto = false;
		if (etoType == GameData.instance.selectedEtoData.etoType) {
			// 選択している干支の場合にはスコアを多く加算　etoPoint * 消した干支の数 * etoRate
			GameData.instance.score += Mathf.CeilToInt(GameData.instance.etoPoint * count * GameData.instance.etoRate);
			isChooseEto = true;
		} else {
			// それ以外は etoPoint * 消した干支の数 を加算
			GameData.instance.score += GameData.instance.etoPoint * count;
		}

		// 消した干支の数を加算
		GameData.instance.eraseEtoCount += count;

		// 画面に表示されているスコアの更新
		uiManager.UpdateDisplayScore(isChooseEto);
	}
}
