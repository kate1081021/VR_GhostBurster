using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

// 敵のタイプを定義する（インスペクターで選べるようになります）
public enum EnemyType
{
    Normal,     // ノーマル（片手即死）
    MediumBoss, // 中ボス（両手即死のみ）
    BigBoss     // 大ボス（両手1秒タイマー）
}

public class Enemy : MonoBehaviour
{
    // --- 状態管理 ---
    private bool isDefeated = false;
    private bool isChasing = false;

    // --- コンポーネント ---
    private Animator animator;

    // --- 敵タイプ設定 ---
    [Header("敵のタイプ設定")]
    [Tooltip("ゴーストの種類（Normal, MediumBoss, BigBoss）を選択")]
    public EnemyType enemyType = EnemyType.Normal;

    // --- 追跡設定 ---
    [Header("追跡設定")]
    [Tooltip("追いかける対象（自動検出されない場合は Playermovebox を設定）")]
    public Transform playerTransform;
    [Tooltip("追いかける基本速度")]
    public float speed = 1.0f; // 1.0f で固定
    [Tooltip("プレイヤーの方を向く回転速度")]
    public float rotationSpeed = 5.0f;

    // ▼▼▼ ボス専用の動き ▼▼▼
    [Header("ボス専用の動き（回避行動）")]
    [Tooltip("（ボス用）左右に揺れる速さ")]
    public float strafeSpeed = 2.0f;
    [Tooltip("（ボス用）左右に揺れる幅")]
    public float strafeDistance = 2.0f;
    [Tooltip("（ボス用）上下に揺れる速さ")]
    public float verticalSpeed = 2.0f;
    [Tooltip("（ボス用）上下に揺れる幅")]
    public float verticalDistance = 1.0f;
    // ▲▲▲ ボス専用の動き ▲▲▲

    // --- 撃破設定 ---
    [Header("撃破設定")]
    [Tooltip("倒れた時に消える（dissolve）アニメーションの長さ")]
    public float dissolveDuration = 2.0f;

    // --- ポイント設定 ---
    [Header("ポイント・消滅設定")]
    [Tooltip("1秒あたりに減少するポイント")]
    public float pointsDecayPerSecond = 10.0f;
    [Tooltip("プレイヤーにこの距離まで近づいたら消滅する")]
    public float despawnDistance = 0.1f;
    private float currentPoints; // 現在のスコア

    // --- 懐中電灯 撃破設定 ---
    [Header("懐中電灯 撃破設定")]
    [Tooltip("（大ボス用）2本のライトで倒すのに必要な時間")]
    [SerializeField]
    public float timeToDefeatWithTwoLights = 1.0f; // 大ボス用タイマー

    private int lightsHittingCount = 0; // 当たっているライトの数
    private float twoLightTimer = 0.0f; // 大ボス用タイマー

    public AudioSource audioSource;

    void Start()
    {
        // 子オブジェクトも含めて Animator を探す
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError(this.name + " の子オブジェクトに Animator が見つかりません！");
        }

        // プレイヤーを自動検出
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }
    }

    void Update()
    {
        if (isDefeated) return;

        // --- 懐中電灯タイマーの処理 ---
        // (大ボス用) 2本ライトのタイマー
        if (enemyType == EnemyType.BigBoss && lightsHittingCount >= 2)
        {
            twoLightTimer += Time.deltaTime;
            if (twoLightTimer >= timeToDefeatWithTwoLights)
            {
                Debug.Log("大ボス: 2本のライトで撃破");
                Defeated();
            }
        }

        // --- 追跡処理 ---
        if (isChasing && playerTransform != null)
        {
            // 1. 近すぎたら消滅
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= despawnDistance)
            {
                Vanish();
                return;
            }

            // 2. ポイントの時間経過 減算
            if (currentPoints > 0)
            {
                currentPoints -= pointsDecayPerSecond * Time.deltaTime;
                currentPoints = Mathf.Max(currentPoints, 0);
            }

            // 3. 追跡・回転
            Vector3 directionToPlayer = playerTransform.position - transform.position;
            directionToPlayer.y = 0;
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // 4. 移動
            Vector3 targetPosition = playerTransform.position;

            // ボスタイプ（中ボス・大ボス）の場合、ターゲット座標に回避行動を加える
            if (enemyType == EnemyType.MediumBoss || enemyType == EnemyType.BigBoss)
            {
                float horizontalOffset = Mathf.Sin(Time.time * strafeSpeed) * strafeDistance;
                float verticalOffset = Mathf.Cos(Time.time * verticalSpeed) * verticalDistance;
                Vector3 offset = (transform.right * horizontalOffset) + (transform.up * verticalOffset);
                targetPosition += offset;
            }

            // 最終的なターゲット（通常またはオフセット付き）に向かって移動
            Vector3 nextPosition = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            transform.position = nextPosition;
        }
    }

    // --- 外部から呼び出すための関数 ---

    /// <summary>
    /// 【懐中電灯(FlashlightHit.cs)から呼び出す用】
    /// ライトが当たり始めた時に呼ばれる
    /// </summary>
    public void OnLightEnter()
    {
        if (isDefeated) return;
        lightsHittingCount++;

        // 2本以上当たった場合
        if (lightsHittingCount >= 2)
        {
            if (enemyType == EnemyType.Normal)
            {
                Debug.Log("ノーマル: 2本で即時撃破");
                Defeated(); // ノーマルは即死
            }
            else if (enemyType == EnemyType.MediumBoss)
            {
                Debug.Log("中ボス: 2本で即時撃破");
                Defeated(); // 中ボスは即死
            }
            else if (enemyType == EnemyType.BigBoss)
            {
                twoLightTimer = 0.0f; // 大ボスのタイマースタート
                Debug.Log("大ボス: 2本タイマースタート");
            }
        }
        // 1本だけ当たった場合
        else if (lightsHittingCount == 1)
        {
            if (enemyType == EnemyType.Normal)
            {
                Debug.Log("ノーマル: 1本で即時撃破");
                Defeated(); // ノーマルは1本で即死
            }
        }
    }

    /// <summary>
    /// 【懐中電灯(FlashlightHit.cs)から呼び出す用】
    /// ライトが離れた時に呼ばれる
    /// </summary>
    public void OnLightExit()
    {
        if (isDefeated) return;
        if (lightsHittingCount > 0)
        {
            lightsHittingCount--;
        }
    }

    /// <summary>
    /// 【ドア(Door.cs)から呼び出す用】
    /// プレイヤーの追跡を開始する
    /// </summary>
    public void StartChasing()
    {
        if (isDefeated) return;
        isChasing = true;

        // タイプに応じてスコアを初期化
        switch (enemyType)
        {
            case EnemyType.Normal:
                currentPoints = 100;
                break;
            case EnemyType.MediumBoss:
                currentPoints = 300;
                break;
            case EnemyType.BigBoss:
                currentPoints = 1000;
                break;
        }

        Debug.Log(name + " が追跡開始！ 初期ポイント: " + currentPoints);

        if (animator != null)
        {
            animator.SetTrigger("fadein");
        }
    }

    /// <summary>
    /// 倒れる処理を開始する (ポイント獲得)
    /// </summary>
    public void Defeated()
    {
        if (isDefeated) return;
        isDefeated = true;
        isChasing = false;

        ResetTimers();
        Debug.Log(this.name + " は倒れた！ " + (int)currentPoints + "ポイント獲得！");

        // ▼▼▼ SE再生（ここは残す） ▼▼▼
        if (audioSource != null)
        {
            audioSource.Play(); // 倒れる音を再生
        }
        // ▲▲▲ SE再生 ▲▲▲

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore((int)currentPoints);
        }

        if (animator != null)
        {
            animator.SetTrigger("die");
        }
        StartCoroutine(DestroyAfterAnimation(dissolveDuration));
    }

    /// <summary>
    /// 近づきすぎて消滅する処理 (ポイントなし)
    /// </summary>
    private void Vanish()
    {
        if (isDefeated) return;
        isDefeated = true;
        isChasing = false;

        ResetTimers();
        Debug.Log(this.name + " は近すぎたため消滅しました (ポイントなし)");

        if (animator != null)
        {
            animator.SetTrigger("die");
        }
        StartCoroutine(DestroyAfterAnimation(dissolveDuration));
    }

    // タイマーをリセットする共通関数
    private void ResetTimers()
    {
        lightsHittingCount = 0;
        twoLightTimer = 0.0f;
    }

    // 破棄コルーチン
    private IEnumerator DestroyAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}