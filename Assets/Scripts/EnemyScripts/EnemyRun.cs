// ---------------------------------------------------------  
// EnemyRun.cs  
// 敵走り動作スクリプト
// 作成日:  2/22
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;

public class EnemyRun
{

    #region 変数  

    private Transform _enemyTransform = default;
    private Rigidbody _enemyRigidbody = default;
    [Tooltip("移動スピード")]
    private const float CONST_MOVE_SPEED = 10;


    /// <summary>
    /// コンストラクタ(インスタンス化されたときに一回実行される)
    /// </summary>
    public EnemyRun(Transform enemyTransform,Rigidbody enemyRigidbody)
    {
        _enemyTransform = enemyTransform;
        _enemyRigidbody = enemyRigidbody;
    }


    #endregion

    #region メソッド  

    /// <summary>
    /// 敵走り処理
    /// </summary>
    public void Run(Vector3 playerPosition)
    {
        //物理挙動で移動
        _enemyRigidbody.velocity = (_enemyTransform.forward * CONST_MOVE_SPEED) + new Vector3(0, _enemyRigidbody.velocity.y, 0);
    }
    #endregion
}
