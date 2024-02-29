// ---------------------------------------------------------  
// EnemyUIPresenter.cs  
// エネミーのViewとModelを仲介するスクリプト
// 作成日:  2/27
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;
using UniRx;

public class EnemyUIPresenter : MonoBehaviour
{

    #region 変数  

    [SerializeField, Tooltip("敵のHPをもっているオブジェクト")]
    private GameObject _enemyModel = default;
    [SerializeField, Tooltip("敵のUI")]
    private GameObject _enemyView = default;
    [Tooltip("敵のUIにアタッチするスクリプト(View)")]
    private EnemyUIView _enemyUIView = default;
    [Tooltip("敵オブジェクトにアタッチするスクリプト(Model)")]
    private EnemyManager _enemyManager = default;

    #endregion

    #region メソッド  


    /// <summary>  
    /// 更新前処理  
    /// </summary>  
    void Start()
    {
        _enemyManager = _enemyModel.GetComponent<EnemyManager>();
        _enemyUIView = _enemyView.GetComponent<EnemyUIView>();

        //敵HPの値が変化したとき
        _enemyManager.EnemyStatus.EnemyHP
          .Subscribe(playerHP =>
          {
              //Viewに反映するように現在のHPを送る
              _enemyUIView.HpBarChange(playerHP);
          })
          //オブジェクトが破棄されたとき、イベントメッセージの購読を終了する
          .AddTo(this);
    }
    #endregion
}
