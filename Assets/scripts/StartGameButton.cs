using UnityEngine;
using UnityEngine.Playables; // Playable Director を使うために必要
using OVR; // OVRInput を使うために必要

public class StartGameButton : MonoBehaviour
{
    // インスペクターで、再生したいタイムライン（Playable Director）を設定
    public PlayableDirector gameTimeline;

    // ボタンが一度押されたかを記憶するフラグ
    private bool hasBeenPressed = false;

    // 現在、プレイヤーの手がボタンに触れているかどうかのフラグ
    private bool isHandTouching = false;

    // --- ステップ1：手が触れているかを監視 ---

    // 手（"PlayerHand"タグ）がコライダーに入ってきた時
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("light"))
        {
            isHandTouching = true;
            Debug.Log("手がボタンに触れた");
        }
    }

    // 手（"PlayerHand"タグ）がコライダーから離れた時
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("light"))
        {
            isHandTouching = false;
            Debug.Log("手がボタンから離れた");
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
                Debug.Log("ボタンに触れたままトリガーが押されました。ゲームスタート！");

                // 1. 一度だけ押せるようにフラグを立てる
                hasBeenPressed = true;

                // 2. タイムラインの再生を開始する
                if (gameTimeline != null)
                {
                    gameTimeline.Play();
                }

                // 3. （オプション）押されたボタンを非表示にする
                gameObject.SetActive(false);
            }
        }
    }
}