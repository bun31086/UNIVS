// ---------------------------------------------------------  
// PlayerInvincble.cs  
// プレイヤー無敵スクリプト
// 作成日:  2/22
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;
using System.Collections;

public class PlayerInvincble
{

    #region 変数  

    private GameObject _player = default;
    private const string CONST_HIT_TAG = "Player";
    private const string CONST_NOTHIT_TAG = "InvinciblePlayer";
    private bool _isFirst = true;
    #endregion

    #region プロパティ  

    #endregion

    #region メソッド  

    /// <summary>  
    /// 更新前処理  
    /// </summary>  
    private void Start ()
     {
        _player = GameObject.Find("Player");
     }

    public void Invincible(bool isDodge, bool isDeath,bool isDamaged)
    {
        if (_isFirst)
        {
            _isFirst = false;
            Start();
        }

        if (isDodge || isDeath || isDamaged)
        {
            _player.tag = CONST_NOTHIT_TAG;
        }
        else
        {
            _player.tag = CONST_HIT_TAG;
        }
    }

    #endregion
}
