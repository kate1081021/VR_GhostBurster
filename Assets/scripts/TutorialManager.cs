using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("クリア設定")]
    [Tooltip("ボタンを出すために必要な合計撃破数")]
    public int targetDefeatCount = 3;

    [Header("進行制御")]
    [Tooltip("条件達成時に出現させるスタートボタン")]
    public GameObject startButtonObject;

    // 現在の撃破数
    private int currentDefeatCount = 0;

    void Start()
    {
        // ゲーム開始時（リセット時）は必ず0からスタート
        currentDefeatCount = 0;

        // ボタンを隠す
        if (startButtonObject != null)
        {
            startButtonObject.SetActive(false);
        }
    }

    /// <summary>
    /// ゴーストから呼ばれる「倒された報告」を受け取る関数
    /// </summary>
    public void OnGhostDefeated()
    {
        currentDefeatCount++;
        Debug.Log("お試しゴースト撃破！ 現在の撃破数: " + currentDefeatCount);

        // 目標数に達したらボタンを表示
        if (currentDefeatCount >= targetDefeatCount)
        {
            if (startButtonObject != null)
            {
                startButtonObject.SetActive(true);
                Debug.Log("チュートリアルクリア！ スタートボタンが出現しました。");
            }
        }
    }
}