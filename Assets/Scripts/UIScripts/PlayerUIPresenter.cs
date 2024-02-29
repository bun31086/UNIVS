// ---------------------------------------------------------  
// PlayerUIPresenter.cs  
// プレイヤーのViewとModelを仲介するスクリプト
// 作成日:  2/27
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;
using UniRx;

public class PlayerUIPresenter : MonoBehaviour
{

    #region 変数  

    [SerializeField, Tooltip("プレイヤーのHPをもっているオブジェクト")]
    private GameObject _playerModel = default;
    [SerializeField, Tooltip("プレイヤーのUI")]
    private GameObject _playerView = default;
    [Tooltip("プレイヤーのUIにアタッチするスクリプト(View)")]
    private PlayerUIView _playerUIView = default;
    [Tooltip("敵オブジェクトにアタッチするスクリプト(Model)")]
    private PlayerManager _playerManager = default;

    #endregion

    #region メソッド  

  
     /// <summary>  
     /// 更新前処理  
     /// </summary>  
     void Start ()
     {
        _playerManager = _playerModel.GetComponent<PlayerManager>();
        _playerUIView = _playerView.GetComponent<PlayerUIView>();

        //プレイヤーHPの値が変化したとき
        _playerManager.PlayerStatus.PlayerHP
          .Subscribe(playerHP =>
          //Viewに反映するように現在のHPを送る
          _playerUIView.HpBarChange(playerHP)
          )
          //オブジェクトが破棄されたとき、イベントメッセージの購読を終了する
          .AddTo(this);
        //プレイヤースタミナの値が変化したとき
        _playerManager.PlayerStatus.PlayerStamina
            .Subscribe(playerStamina =>
            //Viewに反映するように現在のスタミナを送る
            _playerUIView.StaminaBarChange(playerStamina)
            )
            //オブジェクトが破棄されたとき、イベントメッセージの購読を終了する
            .AddTo(this);
    }
  
    #endregion
}
