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

    private GameObject _player = default;
    private Rigidbody _rigidbody = default;
    private Transform _transform = default;
    private const float CONST_RAY_LENGTH = 0.05f;
    private LayerMask _groundLayer = 1 << 8;
    private bool _isFirst = true;

    #endregion

    #region メソッド  

    /// <summary>  
    /// 初期化処理  
    /// </summary>  
    public void Start()
    {
        _player = GameObject.Find("Player");
        _rigidbody = _player.GetComponent<Rigidbody>();
        _transform = _player.transform;
    }

    /// <summary>
    /// ジャンプ挙動
    /// </summary>
    public void Jump()
    {
        //最初の一度だけStartメソッドを実行する
        if (_isFirst)
        {
            _isFirst = false;
            Start();
        }
        //力を加える
        _rigidbody.AddForce(new Vector3(0, 15000, 0));
    }

    /// <summary>
    /// 接地判定
    /// </summary>
    public bool GroundCheck()
    {
        //最初の一度だけStartメソッドを実行する
        if (_isFirst)
        {
            _isFirst = false;
            Start();
        }
        Vector3 rayPosition = _transform.position + new Vector3(0.0f, 0.0f, 0.0f);
        Ray jumpRay = new Ray(rayPosition, Vector3.down);
        bool isGround = Physics.Raycast(jumpRay, CONST_RAY_LENGTH, _groundLayer);

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
