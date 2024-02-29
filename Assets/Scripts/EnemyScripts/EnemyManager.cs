// ---------------------------------------------------------  
// EnemyManager.cs  
// 敵全体管理スクリプト
// 作成日:  2/22
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;
using UniRx;

public class EnemyManager : MonoBehaviour,IDamageable
{
    #region 変数

    #region スクリプト系

    [Tooltip("敵走り挙動スクリプト")]
    private EnemyRun _enemyRun = default;
    [Tooltip("プレイヤーの方向に敵を向かせるスクリプト")]
    private EnemyLookat _enemyLookat = default;
    [Tooltip("敵アニメーション管理スクリプト")]
    private EnemyAnimation _enemyAnimation = default;
    [Tooltip("攻撃ヒット受け取りスクリプト(BoxCollider)")]
    private EnemyTrigger _enemyTriggerBox = default;
    [Tooltip("攻撃ヒット受け取りスクリプト(CapselCollider)")]
    private EnemyTrigger _enemyTriggerBall = default;
    [Tooltip("敵横移動スクリプト")]
    private EnemySideMove _enemySideMove = default;
    [Tooltip("敵ステータス管理スクリプト")]
    private EnemyStatus _enemyStatus = new EnemyStatus();
    [Tooltip("索敵スクリプト")]
    private EnemySearch _enemySearch = new EnemySearch();
    [Tooltip("プレイヤー状態管理スクリプト")]
    private readonly ReactiveProperty<EnemyState> _enemyState = new ReactiveProperty<EnemyState>(EnemyState.Idle);
    //private EnemyStun _enemyStun = new EnemyStun(); 未実装
    #endregion

    #region 定数
    [Tooltip("敵の索敵範囲")]
    private const int CONST_SEARCH_RANGE = 30;
    [Tooltip("敵の攻撃可能範囲")]
    private const int CONST_ATTACK_RANGE = 4;

    #endregion

    [Tooltip("敵とプレイヤーの距離")]
    private float _distance = default;
    [Tooltip("プレイヤーの座標")]
    private Vector3 _playerPosition = default;
    [Tooltip("敵の座標")]
    private Vector3 _enemyPosition = default;

    private Transform _playerTransform = default;
    private Transform _enemyTransform = default;
    private Rigidbody _enemyRigidbody = default;
    private Animator _enemyAnimator = default;

    [SerializeField,Tooltip("プレイヤーオブジェクト")]
    private GameObject _player;
    [SerializeField,Tooltip("敵の攻撃範囲オブジェクト(箱)")]
    private GameObject _triggerBoxObject = default;
    [SerializeField, Tooltip("敵の攻撃範囲オブジェクト(球)")]
    private GameObject _triggerBallObject = default;
    [Tooltip("攻撃しているか")]
    private bool _isAttack = false;
    [Tooltip("攻撃し始めたか")]
    private bool _isAttackFirst = false;
    [Tooltip("横移動しているか")]
    private bool _isSideMove = false;
    [Tooltip("横移動し始めたか")]
    private bool _isSideMoveFirst = false;
    [Tooltip("左手の当たり判定")]
    private BoxCollider _boxCollider = default;
    [Tooltip("右手の当たり判定")]
    private CapsuleCollider _capsuleCollider = default;
    [Tooltip("1→左 2→右に移動する")]
    private int _moveDirection = default;


    #endregion

    #region プロパティ

    /// <summary>
    /// EnemyUIPresenterにEnemyHPを渡すため
    /// </summary>
    public EnemyStatus EnemyStatus { get => _enemyStatus; set => _enemyStatus = value; }

    #endregion

    #region メソッド  

    /// <summary>  
    /// 初期化処理  
    /// </summary>  
    void Awake()
    {
        _enemyTriggerBox = _triggerBoxObject.GetComponent<EnemyTrigger>();
        _enemyTriggerBall = _triggerBallObject.GetComponent<EnemyTrigger>();
        _enemyAnimator = this.GetComponent<Animator>();
        _enemyAnimation = new EnemyAnimation(_enemyAnimator);

        //HPを初期化する
        EnemyStatus.EnemyHP.Value = EnemyStatus.EnemyMaxHp;

        #region イベント
        EnemyStatus.EnemyHP
            //敵のHPが0になったとき
            .Where(_ => EnemyStatus.EnemyHP.Value <= 0)
            .Subscribe(_ =>
            {
                //状態を死亡状態にする
                _enemyState.Value = EnemyState.Death;
                //アニメーションを変更する
                _enemyAnimation.Animation((int)_enemyState.Value, 0);
                //当たり判定をなくす
                _capsuleCollider.enabled = false;
                _boxCollider.enabled = false;
            }).AddTo(this);
        _enemyState
            //攻撃状態、死亡状態、横移動以外のとき
            .Where(_ => _enemyState.Value != EnemyState.Attack && _enemyState.Value != EnemyState.Death && _enemyState.Value != EnemyState.SlowMove)
            .Subscribe(_ =>
            {
                //アニメーションを変更する
                _enemyAnimation.Animation((int)_enemyState.Value, 0);
            }).AddTo(this);
        //攻撃が当たった時（BoxCollider）
        _enemyTriggerBox.IsHit
            .Subscribe(_ =>
            {
                //もしIDamageableを持っていたら
                if (_enemyTriggerBox.HitObject.TryGetComponent(out IDamageable damageable))
                {
                    //当たったオブジェクトのインターフェースを参照し、ダメージを与える
                    _enemyTriggerBox.HitObject.GetComponent<IDamageable>().DamageHit(EnemyStatus.EnemyPower);
                }
            });
        //攻撃が当たった時（CapselCollider）
        _enemyTriggerBall.IsHit
            .Subscribe(_ =>
            {
                //もしIDamageableを持っていたら
                if (_enemyTriggerBall.HitObject.TryGetComponent(out IDamageable damageable))
                {
                    //当たったオブジェクトのインターフェースを参照し、ダメージを与える
                    _enemyTriggerBall.HitObject.GetComponent<IDamageable>().DamageHit(EnemyStatus.EnemyPower);
                }
            });

        #endregion
    }

    /// <summary>  
    /// 更新前処理  
    /// </summary>  
    void Start()
    {
        _playerTransform = _player.transform;
        _enemyTransform = this.transform;
        _enemyRigidbody = this.GetComponent<Rigidbody>();
        _boxCollider = _triggerBoxObject.GetComponent<BoxCollider>();
        _capsuleCollider = _triggerBallObject.GetComponent<CapsuleCollider>();
        #region インスタンス化
        _enemyRun = new EnemyRun(_enemyTransform, _enemyRigidbody);
        _enemyLookat = new EnemyLookat(_enemyTransform);
        _enemySideMove = new EnemySideMove(_enemyTransform, _enemyRigidbody);
        #endregion

    }

    /// <summary>  
    /// 更新処理  
    /// </summary>  
    void Update()
    {
        UpdateManager();
    }

    /// <summary>
    /// 更新処理（物理演算）
    /// </summary>
    private void FixedUpdate()
    {
        FixedManager();
    }

    /// <summary>
    /// Update基本動作
    /// </summary>
    private void UpdateManager()
    {
        //死亡していたら
        if (_enemyState.Value == EnemyState.Death)
        {
            //処理を中断
            return;
        }

        //攻撃状態かスタン状態か死亡状態以外なら
        if (_enemyState.Value != EnemyState.Attack && _enemyState.Value != EnemyState.Stun && _enemyState.Value != EnemyState.Death)
        {
            //プレイヤーの座標を調べる
            _playerPosition = _playerTransform.position;
            //敵の座標を調べる
            _enemyPosition = _enemyTransform.position;
            //プレイヤーと敵の距離を調べる
            _distance = _enemySearch.Search(_playerPosition, _enemyPosition);
        }

        switch (_enemyState.Value)
        {
            //待機状態
            case EnemyState.Idle:          
                //索敵範囲内に入ったらRunする
                if (_distance <= CONST_SEARCH_RANGE)
                {
                    _enemyState.Value = EnemyState.Run;
                }
                break;
            //ダッシュ状態
            case EnemyState.Run:
                //常にプレイヤーの方を向く
                _enemyLookat.Lookat(_playerPosition, _enemyPosition);
                //攻撃範囲内に入ったら攻撃を開始
                if (_distance <= CONST_ATTACK_RANGE)
                {
                    //次の行動は攻撃複数種類からランダムで選出され実行する
                    _enemyState.Value = EnemyState.Attack;
                }
                break;
            //攻撃状態
            case EnemyState.Attack:
                //プレイヤーの方を向く
                _enemyLookat.Lookat(_playerPosition, _enemyPosition);
                // 攻撃に遷移してから最初の一度のみ実行
                if (!_isAttackFirst)
                {                
                    //攻撃フラグをオンにする
                    _isAttackFirst = true;
                    _isAttack = true;
                    //攻撃の種類をランダムに一つ決める
                    int attackRandom = Random.Range(1, 6);
                    //決められた攻撃のアニメーションを実行
                    _enemyAnimation.Animation((int)_enemyState.Value, attackRandom);
                }
                //攻撃終了後に次の行動を実行する
                if (!_isAttack)
                {
                    _isAttackFirst = false;
                    //次の行動は（待機状態1、攻撃複数種類2、横移動3）からランダムで選出され実行する
                    int stateRandom = Random.Range(1, 4);
                    _enemyState.Value = (EnemyState)stateRandom;
                }
                break;
            //横移動状態
            case EnemyState.SlowMove:
                //攻撃判定エリアを消す
                _boxCollider.enabled = false;
                _capsuleCollider.enabled = false;
                //プレイヤーの方を向く
                _enemyLookat.Lookat(_playerPosition, _enemyPosition);

                // 横移動に遷移してから最初の一度のみ実行
                if (!_isSideMoveFirst)
                {
                    //横移動フラグを立てる
                    _isSideMoveFirst = true;
                    _isSideMove = true;
                    //ランダムで右移動か左移動か決める
                    int leftRight = Random.Range(1, 3);
                    //決まったアニメーションを実行
                    _enemyAnimation.Animation((int)_enemyState.Value, leftRight);
                    //FiexdUpdateに連携する
                    _moveDirection = leftRight;
                }
                //終了次第、次の行動を開始
                if (!_isSideMove)
                {
                    _isSideMoveFirst = false;
                    //次の行動は攻撃複数種類か、待機状態からランダムで選出され実行する
                    int stateRandom = Random.Range(1, 3);
                    _enemyState.Value = (EnemyState)stateRandom;
                    //Fiexdにアニメーション終了を伝える
                    _moveDirection = default;
                }
                break;
            //スタン状態（未実装）
            case EnemyState.Stun:
                //攻撃されるたびに蓄積されていく
                //蓄積ポイントが一定値に達するとスタンする
                //蓄積ポイントは常に少しずつ減っていく
                //スタンアニメーションを実行し、アニメーション終了次第、待機状態にうつる
                break;
        }
    }

    /// <summary>
    /// FixedUpdate基本動作
    /// </summary>
    private void FixedManager()
    {
        switch (_enemyState.Value)
        {
            //走り状態
            case EnemyState.Run:            
                //プレイヤーの近くまで走る
                _enemyRun.Run(_playerPosition);
                break;
            //ゆっくり横移動
            case EnemyState.SlowMove:       
                //横移動する方向が決まっていたら
                if (_moveDirection != default)
                {
                    //決められた方向に移動する
                    _enemySideMove.SideMove(_moveDirection);
                }
                break;
        }
    }

    #region アニメータに呼び出されるメソッド

    /// <summary>
    /// 攻撃終了時にアニメーターから呼び出されるメソッド
    /// </summary>
    public void AttackEnd()
    {
        //攻撃終了を通知
        _isAttack = false;
    }
    /// <summary>
    /// 横移動終了時にアニメーターから呼び出されるメソッド
    /// </summary>
    public void SideMoveEnd()
    {
        //横移動終了を通知
        _isSideMove = false;
    }
    /// <summary>
    /// 攻撃開始時にアニメーターから呼び出されるメソッド
    /// </summary>
    public void AttackStart(int attackType)
    {
        //攻撃アニメーションの種類に対応した当たり判定を出す
        switch (attackType)
        {
            case 1:
                _capsuleCollider.enabled = true;
                break;
            case 2:
                _capsuleCollider.enabled = true;
                break;
            case 3:
                _boxCollider.enabled = true;
                break;
            case 4:
                _capsuleCollider.enabled = true;
                break;
            case 5:
                _boxCollider.enabled = true;
                _capsuleCollider.enabled = true;
                break;
        }
    }
    #endregion

    /// <summary>
    /// 敵の体力をダメージ分減らす
    /// </summary>
    /// <param name="damage">送られてきたダメージ量</param>
    public void DamageHit(int damage)
    {
        EnemyStatus.EnemyHP.Value -= damage;
    }

    #endregion
}
