// ---------------------------------------------------------  
// PlayerInvincble.cs  
// プレイヤー無敵スクリプト
// 作成日:  2/22
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;

public class PlayerInvincble
{

    #region 変数  

    private GameObject _player = default;
    private bool _isFirst = true;
    [Tooltip("非無敵タグ")]
    private const string CONST_HIT_TAG = "Player";
    [Tooltip("無敵タグ")]
    private const string CONST_NOTHIT_TAG = "InvinciblePlayer";
    #endregion

    #region メソッド  

    /// <summary>  
    /// 更新前処理  
    /// </summary>  
    private void Start ()
     {
        _player = GameObject.Find("Player");
     }

    /// <summary>
    /// 無敵処理
    /// </summary>
    /// <param name="isDodge">回避している</param>
    /// <param name="isDeath">死んでいる</param>
    /// <param name="isDamaged">ダメージを受けている</param>
    public void Invincible(bool isDodge, bool isDeath,bool isDamaged)
    {
        //初期化
        if (_isFirst)
        {
            _isFirst = false;
            Start();
        }

        //もし回避している、死んでいる、ダメージを受けているなら
        if (isDodge || isDeath || isDamaged)
        {
            //無敵にする
            _player.tag = CONST_NOTHIT_TAG;
        }
        //それ以外なら
        else
        {
            //無敵を解除する
            _player.tag = CONST_HIT_TAG;
        }
    }

    #endregion
}
