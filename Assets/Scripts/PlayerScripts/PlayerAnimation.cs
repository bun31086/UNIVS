// ---------------------------------------------------------  
// PlayerAnimation.cs  
// プレイヤーアニメーション管理スクリプト
// 作成日:  2/16
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;

public class PlayerAnimation
{

    #region 変数  

    private Animator _playerAnimator = default;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="playerAnimator">プレイヤーのアニメータ</param>
    public PlayerAnimation(Animator playerAnimator)
    {
        _playerAnimator = playerAnimator;
    }

    #endregion

    #region メソッド  

    /// <summary>
    /// アニメーション変更
    /// </summary>
    /// <param name="state">プレイヤーステート</param>
    public void Animation(int state)
    {
        switch (state)
        {
            case 0:     //待機
                _playerAnimator.SetBool("isIdle", true);
                _playerAnimator.SetBool("isWalk", false);
                _playerAnimator.SetBool("isRun", false);
                _playerAnimator.ResetTrigger("isAttack");
                _playerAnimator.ResetTrigger("isDodge");
                break;
            case 1:     //歩き
                _playerAnimator.SetBool("isWalk", true);
                _playerAnimator.SetBool("isRun", false);
                _playerAnimator.SetBool("isIdle", false);
                _playerAnimator.ResetTrigger("isAttack");
                break;
            case 2:     //走り
                _playerAnimator.SetBool("isRun", true);
                _playerAnimator.SetBool("isWalk", false);
                _playerAnimator.SetBool("isIdle", false);
                _playerAnimator.ResetTrigger("isAttack");
                break;
            case 3:     //回避
                _playerAnimator.SetBool("isRun", false);
                _playerAnimator.SetBool("isWalk", false);
                _playerAnimator.SetBool("isIdle", false);
                _playerAnimator.SetTrigger("isDodge");
                break;
            case 4:     //攻撃1
                _playerAnimator.SetBool("isRun", false);
                _playerAnimator.SetBool("isWalk", false);
                _playerAnimator.SetBool("isIdle", false);
                _playerAnimator.SetTrigger("isAttack");
                break;
            case 5:     //攻撃2
                _playerAnimator.SetBool("isRun", false);
                _playerAnimator.SetBool("isWalk", false);
                _playerAnimator.SetBool("isIdle", false);
                _playerAnimator.SetTrigger("isAttack");
                break;
            case 6:     //被ダメ
                _playerAnimator.SetTrigger("isHit");
                break;
            case 7:     //死亡
                _playerAnimator.SetBool("isRun", false);
                _playerAnimator.SetBool("isWalk", false);
                _playerAnimator.SetBool("isIdle", false);
                _playerAnimator.SetBool("isDeath", true);
                break;
        }
    }

    #endregion
}