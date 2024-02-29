// ---------------------------------------------------------  
// EnemySearch.cs  
// プレイヤーとの距離を伝えるスクリプト
// 作成日:  2/22
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;

public class EnemySearch
{

    #region 変数  

    [Tooltip("プレイヤーと敵との距離")]
    private float _distance = default;

    #endregion


    #region メソッド  

    /// <summary>  
    /// 敵とプレイヤーの距離を測る
    /// </summary>  
    public float Search(Vector3 playerPosition, Vector3 enemyPosition)
     {
        //距離を測る
        _distance = Vector3.Distance(enemyPosition, playerPosition);

        //距離を返す
        return _distance;
    }

    #endregion
}
