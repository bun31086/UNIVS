// ---------------------------------------------------------  
// EnemyUIView.cs  
// エネミーHPバーを変化させるスクリプト
// 作成日:  2/27
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIView : MonoBehaviour
{

    #region 変数  

    [Tooltip("HPバー")]
    private Image _hpImage = default;
    [Tooltip("0～1の値にするために使用")]
    private const int CONST_HUNDRED = 100;

    #endregion

    #region メソッド  

    /// <summary>  
    /// 更新前処理  
    /// </summary>  
    private void Start()
    {
        _hpImage = this.GetComponent<Image>();
    }

    /// <summary>
    /// HPバーの長さを変化させる
    /// </summary>
    /// <param name="nowHp">現在の敵HP</param>
    public void HpBarChange(float nowHp)
    {
        if(_hpImage != null)
        {
            //FillAmountが0～1の値のため、0～1になるように100で割る
            _hpImage.fillAmount = nowHp / CONST_HUNDRED;
        }
    }

    #endregion
}
