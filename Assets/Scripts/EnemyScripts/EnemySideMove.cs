// ---------------------------------------------------------  
// EnemySideMove.cs  
// 敵の横移動スクリプト
// 作成日:  2/26
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;

public class EnemySideMove
{


    #region 変数  

    private Transform _enemyTransform = default;
    private Rigidbody _enemyRigidbody = default;
    [Tooltip("横移動スピード")]
    private const float CONST_MOVE_SPEED = 2;


    /// <summary>
    /// コンストラクタ
    /// </summary>
    public EnemySideMove(Transform enemyTransform,Rigidbody enemyRigidbody)
    {
        _enemyTransform = enemyTransform;
        _enemyRigidbody = enemyRigidbody;
    }
    #endregion

    #region プロパティ  

    #endregion

    #region メソッド  

    /// <summary>
    /// 横移動
    /// </summary>
    /// <param name="moveDirection">移動する向き</param>
    public void SideMove(int moveDirection)
    {
        switch (moveDirection)
        {
            //左移動
            case 1:
                //物理挙動で移動
                _enemyRigidbody.velocity = (-_enemyTransform.right * CONST_MOVE_SPEED) + new Vector3(0, _enemyRigidbody.velocity.y, 0);
                break;
            //右移動
            case 2:
                //物理挙動で移動
                _enemyRigidbody.velocity = (_enemyTransform.right * CONST_MOVE_SPEED) + new Vector3(0, _enemyRigidbody.velocity.y, 0);
                break;
        }
    }  
    #endregion
}
