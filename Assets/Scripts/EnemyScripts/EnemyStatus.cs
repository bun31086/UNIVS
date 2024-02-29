// ---------------------------------------------------------  
// EnemyStatus.cs  
// 敵ステータス管理スクリプト
// 作成日: 2/22 
// 作成者: 竹村綾人
// ---------------------------------------------------------  
using UnityEngine;
using UniRx;

public class EnemyStatus
{

    #region 変数  
    
    [Tooltip("敵の攻撃力")]
    private int _enemyPower = 20;
    [Tooltip("敵のHP")]
    private readonly ReactiveProperty<int> _enemyHp = new ReactiveProperty<int>(200);
    [Tooltip("敵のMAXHP")]
    private int _enemyMaxHp = 100;

    #endregion

    #region プロパティ  

    //他スクリプトからも参照可能にする
    public ReactiveProperty<int> EnemyHP => _enemyHp;

    public int EnemyPower
    {
        get => _enemyPower;
        set => _enemyPower = value;
    }
    public int EnemyMaxHp
    {
        get => _enemyMaxHp;
        set => _enemyMaxHp = value;
    }

    #endregion
}
