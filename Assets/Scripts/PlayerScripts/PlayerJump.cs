// ---------------------------------------------------------  
// PlayerJump.cs  
// プレイヤージャンプ挙動（今回は回避を使うため、使用しない）
// 作成日:  2/16
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;

public class PlayerJump
{

    #region 変数  

    private Rigidbody _rigidbody = default;
    private Transform _transform = default;
    private const float CONST_RAY_LENGTH = 0.05f;
    private LayerMask _groundLayer = 1 << 8;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="playerRigidbody">プレイヤーのリジッドボディー</param>
    /// <param name="playerTransform">プレイヤーのトランスフォーム</param>
    public PlayerJump(Rigidbody playerRigidbody,Transform playerTransform)
    {
        _rigidbody = playerRigidbody;
        _transform = playerTransform;
    }

    #endregion

    #region メソッド  

    /// <summary>
    /// ジャンプ挙動
    /// </summary>
    public void Jump()
    {
        //力を加える
        _rigidbody.AddForce(new Vector3(0, 15000, 0));
    }

    /// <summary>
    /// 接地判定
    /// </summary>
    public bool GroundCheck()
    {
        //Rayを出す位置を格納
        Vector3 rayPosition = _transform.position;
        //どの向きに出すか
        Ray jumpRay = new Ray(rayPosition, Vector3.down);
        //Raycastを出し、当たっているか調べる
        bool isGround = Physics.Raycast(jumpRay, CONST_RAY_LENGTH, _groundLayer);
        //boolを返す
        if (isGround)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    #endregion
}
