using UnityEngine;

public class ghostcoming : MonoBehaviour
{
    [Header("追跡設定")]
    // インスペクターで追いかける対象（プレイヤー）を設定
    public Transform playerTransform;
    // 追いかける速度
    public float speed = 3.0f;

    [Header("浮遊設定")]
    // 上下に動く幅（中心からの距離）
    public float floatHeight = 0.5f;
    // 上下に動く速さ
    public float floatSpeed = 1.0f;

    void Update()
    {
        // playerTransformが設定されていない場合は何もしない
        if (playerTransform == null)
        {
            return;
        }

        // --- 1. 追跡処理 ---
        // まず、プレイヤーの位置に向かって移動する次の座標を計算
        Vector3 nextPosition = Vector3.MoveTowards(
            transform.position,
            playerTransform.position,
            speed * Time.deltaTime
        );

        // --- 2. 浮遊処理 ---
        // Sin関数を使って、時間の経過と共に上下するオフセット値 (-floatHeight ～ +floatHeight) を計算
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        // --- 3. 統合 ---
        // 追跡して計算されたY座標に、さらに浮遊オフセットを加算
        nextPosition.y += yOffset;

        // 最終的な位置をオブジェクトに適用
        transform.position = nextPosition;
    }
}
