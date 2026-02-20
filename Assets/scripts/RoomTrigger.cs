using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    // この部屋（トリガー）に入ったら起動させたい敵のリスト
    // インスペクターで、この部屋にいる敵オブジェクトをすべて設定する
    [SerializeField]
    private Enemy[] enemiesInRoom;

    // プレイヤーが入ってきたら一度だけ実行するためのフラグ
    private bool hasTriggered = false;

    // このトリガー (Is Trigger = On) に他のコライダーが入ってきた時
    void OnTriggerEnter(Collider other)
    {
        // まだトリガーが作動しておらず、入ってきたのが "Player" タグのオブジェクトなら
        if (!hasTriggered && other.CompareTag("Player"))
        {
            // 1. フラグを立て、二度実行されないようにする
            hasTriggered = true;
            Debug.Log("プレイヤーが " + gameObject.name + " に入った！ 敵を起動します。");

            // 2. リスト（enemiesInRoom）に登録されているすべての敵をループ処理
            foreach (Enemy enemy in enemiesInRoom)
            {
                // 敵がnullでない（存在している）場合
                if (enemy != null)
                {
                    // 3. 敵の StartChasing() メソッドを呼び出す
                    enemy.StartChasing();
                }
            }
        }
    }

    // (参考) プレイヤーが部屋から出たら追跡をやめさせたい場合
    /*
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // プレイヤーが部屋を出たらフラグをリセット（再度入室したらまた襲う場合）
            hasTriggered = false; 
            Debug.Log("プレイヤーが部屋から出た。");
            
            // 敵に追跡をやめる命令を送る（Enemy.cs に StopChasing() メソッドを別途作る必要がある）
            foreach (Enemy enemy in enemiesInRoom)
            {
                if (enemy != null)
                {
                    // enemy.StopChasing(); 
                }
            }
        }
    }
    */
}