// ---------------------------------------------------------  
// PlayerStatus.cs  
// プレイヤーのステータス（体力、スタミナなど）のみを管理する
// 作成日:  2/15
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;
using System.Collections;
using UniRx;

public class PlayerStatus //: IDamageable
{

    #region 変数  

    private int _playerPower = 10;
    [Tooltip("プレイヤーのスタミナ")]
    private readonly ReactiveProperty<float> _playerStamina = new ReactiveProperty<float>(100);

    [Tooltip("プレイヤーのHP")]
    private readonly ReactiveProperty<float> _playerHp = new ReactiveProperty<float>(100);
    [Tooltip("プレイヤーのMAXHP")]
    private int _playerMaxHp = 100;

    #endregion

    #region プロパティ  
    public ReactiveProperty<float> PlayerHP => _playerHp;
    public ReactiveProperty<float> PlayerStamina => _playerStamina;
    public int PlayerPower { get => _playerPower; set => _playerPower = value; }
    public int PlayerMaxHp { get => _playerMaxHp; set => _playerMaxHp = value; }

    #endregion

    #region メソッド  

    /// <summary>
    /// プレイヤーの体力をダメージ分減らす
    /// </summary>
    /// <param name="damage">送られてきたダメージ量</param>
    //public void DamageHit(int damage)
    //{
    //    _playerHp -= damage;
    //}

    #endregion
}