// ---------------------------------------------------------  
// PlayerUIView.cs  
// プレイヤーHPバーを変化させるスクリプト
// 作成日:  2/27
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIView : MonoBehaviour
{

    #region 変数  

    [SerializeField,Tooltip("HPバーオブジェクト")]
    private GameObject _hpObj = default;
    [SerializeField,Tooltip("スタミナバーオブジェクト")] 
    private GameObject _staminaObj = default;
    [Tooltip("HPバーオブジェクトのImageコンポーネント")]  
    private Image _hpImage = default;
    [Tooltip("スタミナバーオブジェクトのImageコンポーネント")] 
    private Image _staminaImage = default;
    [Tooltip("0～1の値にするために使用")]
    private const int CONST_HUNDRED = 100;

    #endregion

    #region メソッド  

    /// <summary>  
    /// 更新前処理  
    /// </summary>  
    private void Start()
    {

        _hpImage = _hpObj.GetComponent<Image>();
        _staminaImage = _staminaObj.GetComponent<Image>();
    }

    /// <summary>
    /// HPバーの長さを変化させる
    /// </summary>
    /// <param name="nowHp">現在のプレイヤーHP</param>
    public void HpBarChange(float nowHp)
    {
        if (_hpImage != null)
        {
            //FillAmountが0～1の値のため、0～1になるように100で割る
            _hpImage.fillAmount = nowHp / CONST_HUNDRED;
        }
    }

    /// <summary>
    /// スタミナバーの長さを変化させる
    /// </summary>
    /// <param name="stamina">現在のプレイヤースタミナ量</param>
    public void StaminaBarChange(float stamina)
    {
        if (_staminaImage != null)
        {
            //FillAmountが0～1の値のため、0～1になるように100で割る
            _staminaImage.fillAmount = stamina / CONST_HUNDRED;
        }
    }

    #endregion
}