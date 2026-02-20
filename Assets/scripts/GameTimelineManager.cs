using UnityEngine;
using UnityEngine.Playables;
using TMPro;

// このスクリプトは gamedirector オブジェクトにアタッチします
[RequireComponent(typeof(PlayableDirector))]
public class GameTimelineManager : MonoBehaviour
{
    private PlayableDirector playableDirector;

    [Header("UI設定")]
    // (ScoreCanvas は削除)
    public TextMeshProUGUI scoreText; // 最終スコアの更新にだけ使う

    [Header("BGM設定")]
    public AudioSource mainBGMAudio;
    public AudioSource clearBGMAudio;

    public GameObject resetbutton;

    void Awake()
    {
        playableDirector = GetComponent<PlayableDirector>();
        resetbutton.SetActive(false);
    }

    void OnEnable()
    {
        playableDirector.stopped += OnTimelineFinished;
    }

    void OnDisable()
    {
        playableDirector.stopped -= OnTimelineFinished;
    }

    /// <summary>
    /// タイムラインの再生が停止した時（＝クリア時）に自動的に呼ばれる関数
    /// </summary>
    private void OnTimelineFinished(PlayableDirector director)
    {
        Debug.Log("タイムラインが終了しました。BGM切り替えと最終スコアの確認を行います。");

        // 1. （念のため）最終スコアをUIに反映
        int finalScore = 0;
        if (ScoreManager.instance != null)
        {
            finalScore = ScoreManager.instance.totalScore;
        }
        if (scoreText != null)
        {
            scoreText.text = "スコア: " + finalScore;
        }
        // (scoreCanvas.SetActive(true) の行を削除)

        // 2. BGMを切り替え
        if (mainBGMAudio != null)
        {
            mainBGMAudio.Stop();
        }
        if (clearBGMAudio != null)
        {
            clearBGMAudio.Play();
        }
        resetbutton.SetActive(true);
    }
}