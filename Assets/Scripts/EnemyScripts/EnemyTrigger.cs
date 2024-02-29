// ---------------------------------------------------------  
// EnemyTrigger.cs  
// 攻撃ヒット確認用スクリプト
// 作成日:  2/22
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;
using UniRx;
public class EnemyTrigger : MonoBehaviour
{

    #region 変数  

    [Tooltip("当たったオブジェクト")]
    private GameObject _hitObject = default;

    //Unit型はそれ自身は特に意味を持たない
    //メッセージの内容に意味はなく、イベント通知のタイミングのみが重要な時に利用できる
    [Tooltip("当たったことを通知する")]
    private readonly Subject<Unit> _isHit = new Subject<Unit>();

    #endregion

    #region プロパティ

    public Subject<Unit> IsHit => _isHit;
    public GameObject HitObject
    {
        get => _hitObject;
        set => _hitObject = value;
    }

    #endregion

    #region メソッド  

    /// <summary>  
    /// 更新前処理  
    /// </summary>  
    void Start()
    {
        //GameObjectを破棄されたときに自動でDisposeされる
        //Disposeされた後の処理は飛び出されなくなる
        _isHit.AddTo(this);
    }

    /// <summary>
    /// トリガーに入ったとき
    /// </summary>
    /// <param name="other">当たったコライダー</param>
    private void OnTriggerEnter(Collider other)
    {
        //プレイヤー以外は判定しない
        if (!other.CompareTag("Player"))
        {
            return;
        }

        //当たったオブジェクトを格納
        HitObject = other.gameObject;

        //当たったということを通知する
        IsHit.OnNext(Unit.Default);
    }
    #endregion
}
