using UnityEngine;
using UnityEngine.SceneManagement; // シーン管理のために必要
using OVR; // OVRInputのために必要

public class ResetSceneButton : MonoBehaviour
{
    // 現在、プレイヤーの手がボタンに触れているかどうかのフラグ
    private bool isHandTouching = false;

    // ボタンが一度押されたかを記憶するフラグ（重複実行防止）
    private bool hasBeenPressed = false;

    // --- ステップ1：手が触れているかを監視 ---

    // 手（"PlayerHand"タグ）がコライダーに入ってきた時
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("light"))
        {
            isHandTouching = true;
            Debug.Log("リセットボタンに手が触れました");
        }
    }

    // 手（"PlayerHand"タグ）がコライダーから離れた時
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("light"))
        {
            isHandTouching = false;
            Debug.Log("リセットボタンから手が離れました");
        }
    }

    // --- ステップ2：トリガー入力を監視 ---

    void Update()
    {
        // まだ押されておらず、かつ手がボタンに触れている場合
        if (!hasBeenPressed && isHandTouching)
        {
            // 左手 または 右手 の「人差し指トリガー」が "押された瞬間" かどうかをチェック
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch) ||
                OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                Debug.Log("リセットボタンがトリガーで押されました。シーンをリセットします。");

                // 1. 重複実行防止フラグを立てる
                hasBeenPressed = true;

                // 2. 現在のシーンをリセット（リロード）する関数を呼び出す
                ResetCurrentScene();
            }
        }
    }

    /// <summary>
    /// 現在のシーンをリセット（リロード）する関数
    /// </summary>
    private void ResetCurrentScene()
    {
        // 1. 現在アクティブなシーンの名前を取得します。
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 2. 取得した名前のシーンをリロードします。
        SceneManager.LoadScene(currentSceneName);
    }
}