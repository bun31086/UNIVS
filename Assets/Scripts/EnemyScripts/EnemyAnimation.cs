// ---------------------------------------------------------  
// EnemyAnimation.cs  
// 敵アニメーション管理スクリプト
// 作成日:  2/22
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;

public class EnemyAnimation
{

    #region 変数  

    private Animator _enemyAnimator = default;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="enemyAnimator">敵のアニメータコンポーネント</param>
    public EnemyAnimation(Animator enemyAnimator)
    {
        _enemyAnimator = enemyAnimator;
    }

    #endregion

    #region メソッド  
    
    /// <summary>
    /// アニメーション変更
    /// </summary>
    /// <param name="state">敵のステート</param>
    /// <param name="attackInt">左右どちらに移動するか（１→左,２→右）</param>
    public void Animation(int state,int attackInt)
    {
        switch (state)
        {
            //走り状態
            case 0:         
                _enemyAnimator.SetBool("isRun", true);
                _enemyAnimator.SetBool("isIdle", false);
                _enemyAnimator.SetInteger("AttackNumber", 0);
                _enemyAnimator.SetInteger("SideNumber", 0);
                break;
            //待機状態
            case 1:         
                _enemyAnimator.SetBool("isIdle", true);
                _enemyAnimator.SetBool("isRun", false);
                _enemyAnimator.SetInteger("AttackNumber", 0);
                _enemyAnimator.SetInteger("SideNumber", 0);
                break;
            //攻撃状態
            case 2:      
                //送られてきたattackIntに対応した攻撃アニメーションを再生
                _enemyAnimator.SetInteger("AttackNumber", attackInt);
                _enemyAnimator.SetInteger("SideNumber", 0);
                _enemyAnimator.SetBool("isRun", false);
                _enemyAnimator.SetBool("isIdle", false);

                break;
            //横移動
            case 3:         
                //送られてきたattackIntに対応した横移動アニメーションを再生（１→左,２→右）
                _enemyAnimator.SetInteger("SideNumber", attackInt);
                _enemyAnimator.SetInteger("AttackNumber", 0);
                _enemyAnimator.SetBool("isRun", false);
                _enemyAnimator.SetBool("isIdle", false);
                break;
            //スタン
            case 4:   
                _enemyAnimator.SetTrigger("isStun");
                break;
            //死亡状態
            case 5:         
                _enemyAnimator.SetBool("isDeath", true);
                _enemyAnimator.SetBool("isRun", false);
                _enemyAnimator.SetBool("isIdle", false);
                _enemyAnimator.SetInteger("AttackNumber", 0);
                _enemyAnimator.SetInteger("SideNumber", 0);
                break;
        }
    }

    #endregion
}