using UnityEngine;

public class Wind : MonoBehaviour {

    public float coefficient;   // 空気抵抗係数
    public Vector2 velocity;    // 風速

    void OnTriggerStay2D(Collider2D col) {
        if (col.TryGetComponent(out Rigidbody2D rb)) {
            Debug.Log("Wind");

            // 相対速度計算
            Vector2 relativeVelocity = velocity - rb.velocity;

            // 空気抵抗を与える
            rb.AddForce(coefficient * relativeVelocity);
        }
    }
}
