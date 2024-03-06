// ---------------------------------------------------------  
// PlayerTrigger.cs  
// プレイヤーの攻撃トリガー処理
// 作成日:  2/16
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;
using UniRx;

public class PlayerTrigger : MonoBehaviour
{

    #region 変数  
    private const string CONST_ENEMY_TAG = "Enemy";

    //Unit型はそれ自身は特に意味を持たない
    //メッセージの内容に意味はなく、イベント通知のタイミングのみが重要な時に利用できる
    private readonly Subject<Unit> _isHit = new Subject<Unit>();

    private GameObject _hitObject = default;

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
     void Start ()
     {
        //GameObjectを破棄されたときに自動でDisposeされる
        //Disposeされた後の処理は飛び出されなくなる
        _isHit.AddTo(this);
    }

    /// <summary>
    /// 攻撃ヒット判定
    /// </summary>
    /// <param name="other">当たったオブジェクト</param>
    private void OnTriggerEnter(Collider other)
    {
        //敵以外は判定しない
        if(other.gameObject.tag != CONST_ENEMY_TAG)
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
