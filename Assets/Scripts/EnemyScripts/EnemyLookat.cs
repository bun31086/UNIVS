// ---------------------------------------------------------  
// EnemyLookAt.cs  
// 対象のオブジェクトの方向を向くスクリプト
// 作成日:  2/26
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;

public class EnemyLookat
{

    #region 変数  

    [Tooltip("補完スピード")]
    private float _complementSpeed = default;
    [Tooltip("敵のtransformコンポーネント")]
    private Transform _enemyTransform = default;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public EnemyLookat(Transform enemyTransform)
    {
        _enemyTransform = enemyTransform;
        //向きの補完スピードを決める
        _complementSpeed = 0.05f;
    }

    #endregion

    #region メソッド  

    /// <summary>
    /// プレイヤーの方向を向く
    /// </summary>
    /// <param name="playerPosition">プレイヤーの座標</param>
    /// <param name="enemyPosition">敵の座標</param>
    public void Lookat(Vector3 playerPosition,Vector3 enemyPosition)
    {
        //プレイヤー方向のベクトルを計算
        Vector3 relativePos = playerPosition - enemyPosition;
        //方向を回転情報に変換
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        //現在の回転情報と、ターゲット方向の回転情報を補完する
        _enemyTransform.rotation = Quaternion.Slerp(_enemyTransform.rotation, rotation, _complementSpeed);
    }
  
    #endregion
}
