using UnityEngine;

// このスクリプトは、タイムラインと連携する
// メインのドアオブジェクト（例: doormaster0）にアタッチします。
public class Door : MonoBehaviour
{
    // === 敵のリストをフェーズごとに分ける ===

    [Header("フェーズ1の敵リスト")]
    [SerializeField]
    private Enemy[] phase1Enemies;
    private bool hasPhase1Triggered = false;

    [Header("フェーズ2の敵リスト")]
    [SerializeField]
    private Enemy[] phase2Enemies;
    private bool hasPhase2Triggered = false;

    [Header("フェーズ3の敵リスト")]
    [SerializeField]
    private Enemy[] phase3Enemies;
    private bool hasPhase3Triggered = false;

    [Header("フェーズ4の敵リスト")]
    [SerializeField]
    private Enemy[] phase4Enemies;
    private bool hasPhase4Triggered = false;

    [Header("フェーズ5の敵リスト")]
    [SerializeField]
    private Enemy[] phase5Enemies;
    private bool hasPhase5Triggered = false;

    [Header("フェーズ6の敵リスト")]
    [SerializeField]
    private Enemy[] phase6Enemies;
    private bool hasPhase6Triggered = false;


    void Start()
    {
        // ゲーム開始と同時にすべての敵を非表示にする
        DeactivateEnemyList(phase1Enemies);
        DeactivateEnemyList(phase2Enemies);
        DeactivateEnemyList(phase3Enemies);
        DeactivateEnemyList(phase4Enemies);
        DeactivateEnemyList(phase5Enemies);
        DeactivateEnemyList(phase6Enemies);
    }

    // 敵リストを非表示にする共通関数
    private void DeactivateEnemyList(Enemy[] enemies)
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.gameObject.SetActive(false);
            }
        }
    }

    // (共通処理) 指定された敵リストを起動する
    private void ActivateEnemyList(Enemy[] enemies)
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
            {
                // 1. オブジェクトを「出現（表示）」させる
                enemy.gameObject.SetActive(true);

                // 2. 「追跡開始」命令を送る
                enemy.StartChasing();
            }
        }
    }


    // --- 各フェーズを起動するための「スイッチ」（ public な関数） ---
    // これらをタイムラインのシグナルから呼び出します

    public void ActivatePhase1()
    {
        if (hasPhase1Triggered) return;
        hasPhase1Triggered = true;
        Debug.Log("フェーズ1 起動！");
        ActivateEnemyList(phase1Enemies);
    }

    public void ActivatePhase2()
    {
        if (hasPhase2Triggered) return;
        hasPhase2Triggered = true;
        Debug.Log("フェーズ2 起動！");
        ActivateEnemyList(phase2Enemies);
    }

    public void ActivatePhase3()
    {
        if (hasPhase3Triggered) return;
        hasPhase3Triggered = true;
        Debug.Log("フェーズ3 起動！");
        ActivateEnemyList(phase3Enemies);
    }

    public void ActivatePhase4()
    {
        if (hasPhase4Triggered) return;
        hasPhase4Triggered = true;
        Debug.Log("フェーズ4 起動！");
        ActivateEnemyList(phase4Enemies);
    }

    public void ActivatePhase5()
    {
        if (hasPhase5Triggered) return;
        hasPhase5Triggered = true;
        Debug.Log("フェーズ5 起動！");
        ActivateEnemyList(phase5Enemies);
    }

    public void ActivatePhase6()
    {
        if (hasPhase6Triggered) return;
        hasPhase6Triggered = true;
        Debug.Log("フェーズ6 起動！");
        ActivateEnemyList(phase6Enemies);
    }
}