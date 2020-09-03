using UnityEngine;

public class Wind : MonoBehaviour {

    public float coefficient;   // 空気抵抗係数
    public Vector2 windPower;   // 風力
    private CapsuleCollider2D capsuleCol;
    float windTimer;
    public float duration;      // 発生時間

    void Start() {
        capsuleCol = GetComponent<CapsuleCollider2D>();
        capsuleCol.enabled = false;
    }

    /// <summary>
    /// 上昇気流発生
    /// </summary>
    public void Updraft() {
        capsuleCol.enabled = true;
        windTimer = duration;
        // 回転方向ランダム
        int value = Random.Range(0, 2);
        // 回転方向設定
        windPower.x = value == 0 ? windPower.x *= -1 : windPower.x *= 1;
    }

    /// <summary>
    /// 衝突判定を風に見立てる
    /// </summary>
    /// <param name="col"></param>
    void OnTriggerStay2D(Collider2D col) {
        if (col.TryGetComponent(out Rigidbody2D rb)) {
            Debug.Log(rb);
            // 相対速度計算
            Vector2 relativeVelocity = windPower - rb.velocity;

            // 空気抵抗を与える
            rb.AddForce(coefficient * relativeVelocity);
        }
    }

    void Update() {
        // 回転中のみTimerをカウント
        windTimer -= Time.deltaTime;

        // 0になったらカウント終了
        if (windTimer <= 0 && capsuleCol.enabled) {
            windTimer = 0;
            // 回転判定をなくす
            capsuleCol.enabled = false;
            // 再度ボタンを押せるようにする
            UIManager.instance.StopUpdraft();
        }
    }
}
