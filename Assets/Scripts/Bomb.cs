using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Bomb : MonoBehaviour
{
    private Button btnBomb;
    private float radius;

    [SerializeField]
    private bool onGizmos;

    public void SetUpBomb(GameManager gameManager, float bombRadius) {

        if (TryGetComponent(out btnBomb)) {
            btnBomb.onClick.AddListener(() => OnClickBomb(gameManager, bombRadius));
        }
        else {
            Debug.Log("ボムのボタンが取得出来ません");
        }

        // OnDrawGizmos 用
        radius = bombRadius;
    }
    
    /// <summary>
    /// ボムをタップした際の処理
    /// </summary>
    public void OnClickBomb(GameManager gameManager, float bombRadius)
    {
        // TODO SE
       
        // List<Eto> eraseEtos = new List<Eto>();
        // Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, bombRadius);
        //
        // foreach (Collider2D collider in colliders)
        // {
        //     if (collider.TryGetComponent(out Eto eto))
        //     {
        //         eraseEtos.Add(eto);
        //     }
        // }

        // LINQ を利用した場合
        List<Eto> eraseEtos = Physics2D.OverlapCircleAll(transform.position, bombRadius)
            .Select(collider => collider.GetComponent<Eto>())
            .Where(eto => eto != null)
            .ToList();

        if(gameManager != null)
        gameManager.AddRangeEraseEtolList(eraseEtos);
        
        Destroy(gameObject);
    }


    private void OnDrawGizmos() {

        if (!onGizmos) {
            return;
        }
        
        // コライダーの可視化
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void Start() {
        SetUpBomb(null, 1.0f);
    }
}
