// ---------------------------------------------------------  
// PlayerInput.cs  
// InputSystemからの入力受け取り
// 作成日:  2/13
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

public class PlayerInput : MonoBehaviour
{
    #region 変数

    [Tooltip("移動入力受け取り")]
    private Vector3 _velocity = default;
    [Tooltip("移動入力変換")]
    private readonly ReactiveProperty<Vector3> _isMove = new ReactiveProperty<Vector3>(default);
    [Tooltip("走り入力受け取り")]
    private readonly ReactiveProperty<bool> _isRun = new ReactiveProperty<bool>(false);

    //Unit型はそれ自身は特に意味を持たない
    //メッセージの内容に意味はなく、イベント通知のタイミングのみが重要な時に利用できる
    [Tooltip("回避入力受け取り")]
    private readonly Subject<Unit> _isDodge = new Subject<Unit>();
    [Tooltip("攻撃入力受け取り")]
    private readonly Subject<Unit> _isAttack = new Subject<Unit>();

    #endregion

    #region プロパティ

    public IReadOnlyReactiveProperty<Vector3> IsMove => _isMove;
    public IReadOnlyReactiveProperty<bool> IsRun => _isRun;
    public Subject<Unit> IsAttack => _isAttack;
    public Subject<Unit> IsDodge => _isDodge;

    #endregion

    #region メソッド

    /// <summary>
    /// 移動入力検知時
    /// </summary>
    /// <param name="context">移動入力方向</param>
    public void Move(InputAction.CallbackContext context)
    {
        // MoveActionの入力値を取得
        _velocity = context.ReadValue<Vector2>();
        //受け取った入力を3Dアクション用に変換
        _isMove.Value = new Vector3(_velocity.x, 0, _velocity.y);
    }

    /// <summary>
    /// ダッシュ入力検知時
    /// </summary>
    /// <param name="context"></param>
    public void Run(InputAction.CallbackContext context)
    {
        // 押されたとき
        if (context.started)
        {
            _isRun.Value = true;
        }
        //離れたとき
        else if (context.canceled)
        {
            _isRun.Value = false;
        }
    }

    /// <summary>
    /// 攻撃入力検知時
    /// </summary>
    /// <param name="context"></param>
    public void Attack(InputAction.CallbackContext context)
    {
        //押されたとき
        if (context.started)
        {
            //攻撃入力を通知する
            IsAttack.OnNext(Unit.Default);
        }
    }

    /// <summary>
    /// 回避入力検知時
    /// </summary>
    /// <param name="context"></param>
    public void Dodge(InputAction.CallbackContext context)
    {
        //押されたとき
        if (context.started)
        {
            //ジャンプ入力を通知する
            IsDodge.OnNext(Unit.Default);
        }
    }

    #endregion
}
