using UnityEngine;
using TMPro; // TextMeshPro を使うために必要

public class ScoreManager : MonoBehaviour
{
    // このスクリプトにどこからでもアクセスできるようにする
    public static ScoreManager instance;

    // 現在の合計スコア
    public int totalScore { get; private set; }

    // ▼▼▼ 変更点 ▼▼▼
    [Header("UI設定")]
    [Tooltip("スコアを表示するTextMeshProのTextコンポーネント")]
    public TextMeshProUGUI scoreText; // Text ではなく TextMeshProUGUI
    // ▲▲▲ 変更点 ▲▲▲

    void Awake()
    {
        // Scene内に ScoreManager が1つだけ存在するように保証する
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 初期スコアは0
        totalScore = 0;
        UpdateScoreUI();
    }

    /// <summary>
    /// スコアを加算する
    /// </summary>
    public void AddScore(int points)
    {
        if (points <= 0) return;

        totalScore += points;
        UpdateScoreUI();

        Debug.Log(points + "ポイント獲得！ 合計スコア: " + totalScore);
    }

    /// <summary>
    /// （変更点）UIテキストを更新する
    /// </summary>
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "スコア: " + totalScore;
        }
    }
}