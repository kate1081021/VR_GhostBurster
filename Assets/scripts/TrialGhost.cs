using UnityEngine;
using System.Collections;

public class TrialGhost : MonoBehaviour
{
    // --- 状態管理 ---
    private bool isDefeated = false;

    // --- コンポーネント ---
    private Animator animator;

    // --- 設定 ---
    [Header("アニメーション設定")]
    public float dissolveDuration = 2.0f;
    public float respawnTime = 3.0f;
    public Transform armatureTransform;

    [Header("撃破設定")]
    public int lightsRequired = 1;

    private int lightsHittingCount = 0;

    // ▼▼▼ 追加：マネージャーへの参照 ▼▼▼
    private TutorialManager tutorialManager;
    // ▲▲▲ 追加 ▲▲▲

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError(this.name + " の子オブジェクトに Animator が見つかりません！");
        }

        if (armatureTransform == null)
        {
            armatureTransform = animator.transform;
        }

        // ▼▼▼ 追加：シーン内の TutorialManager を探す ▼▼▼
        // プレハブからシーン上のオブジェクトは直接設定できないため、Findで探します
        tutorialManager = FindObjectOfType<TutorialManager>();
        if (tutorialManager == null)
        {
            Debug.LogWarning("シーン上に TutorialManager が見つかりません！");
        }
        // ▲▲▲ 追加 ▲▲▲

        StartCoroutine(Respawn());
    }

    public void OnLightEnter()
    {
        if (isDefeated) return;
        lightsHittingCount++;
        if (lightsHittingCount >= lightsRequired)
        {
            Defeated();
        }
    }

    public void OnLightExit()
    {
        if (isDefeated) return;
        if (lightsHittingCount > 0)
        {
            lightsHittingCount--;
        }
    }

    private void Defeated()
    {
        if (isDefeated) return;
        isDefeated = true;
        lightsHittingCount = 0;

        Debug.Log(this.name + " (お試し用) が倒されました");

        // ▼▼▼ 追加：マネージャーに報告する ▼▼▼
        if (tutorialManager != null)
        {
            tutorialManager.OnGhostDefeated();
        }
        // ▲▲▲ 追加 ▲▲▲

        if (animator != null)
        {
            animator.SetTrigger("die");
        }

        StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(dissolveDuration);
        if (armatureTransform != null) armatureTransform.localScale = Vector3.zero;
        yield return new WaitForSeconds(respawnTime);
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        if (armatureTransform != null) armatureTransform.localScale = Vector3.zero;
        if (animator != null) animator.Play("New State", 0, 0f);
        yield return null;
        isDefeated = false;
        lightsHittingCount = 0;
        if (animator != null) animator.SetTrigger("fadein");
    }
}