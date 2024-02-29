// ---------------------------------------------------------  
// PlayerAnimation.cs  
// プレイヤーアニメーション管理スクリプト
// 作成日:  2/16
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;
using System.Collections;

public class PlayerAnimation
{

    #region 変数  
    private GameObject _player = default;
    private Animator _playerAnimator = default;

    private bool _isFirst = true;

    #endregion

    #region プロパティ  

    #endregion

    #region メソッド  


    /// <summary>  
    /// 更新前処理  
    /// </summary>  
    void Start()
    {
        //プレイヤーオブジェクト取得
        _player = GameObject.Find("Player");
        //プレイヤーコンポーネント取得（Animator）
        _playerAnimator = _player.GetComponent<Animator>();
    }

    public void Animation(int state)
    {
        //最初の一度だけStartメソッドを実行する
        if (_isFirst)
        {
            _isFirst = false;
            Start();
        }

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
                //_playerAnimator.SetBool("isRun", false);
                //_playerAnimator.SetBool("isWalk", false);
                //_playerAnimator.SetBool("isIdle", false);
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