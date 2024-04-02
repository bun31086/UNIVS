// ---------------------------------------------------------  
// PlayerDodge.cs  
// プレイヤー回避スクリプト
// 作成日:  2/28
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;
public class PlayerDodge
{

    #region 変数  

    private Rigidbody _playerRigidbody = default;
    private Transform _transform = default;
    [Tooltip("回避時加える力")]
    private const int CONST_DODGE_POWER = 10000;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="playerRigidbody">プレイヤーのリジッドボディー</param>
    /// <param name="transform">プレイヤーのトランスフォーム</param>
    public PlayerDodge(Rigidbody playerRigidbody, Transform transform)
    {
        _playerRigidbody = playerRigidbody;
        _transform = transform;
    }

    #endregion

    #region メソッド  

    /// <summary>  
    /// 回避処理
    /// </summary>  
    public void Dodge()
    {
        _playerRigidbody.AddForce(_transform.forward * CONST_DODGE_POWER);
    }

    #endregion
}
