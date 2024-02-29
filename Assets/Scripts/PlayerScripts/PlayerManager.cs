// ---------------------------------------------------------  
// PlayerManager.cs  
// プレイヤーの管理スクリプト
// 作成日:  2/13
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;
using UniRx;
using System;

public class PlayerManager : MonoBehaviour,IDamageable
{

    #region 変数  

    #region スクリプト系
    //[Tooltip("プレイヤージャンプ挙動スクリプト")]
    //private PlayerJump _playerJump = new PlayerJump();
    [Tooltip("プレイヤー移動挙動スクリプト")]
    private PlayerMove _playerMove = new PlayerMove();
    [Tooltip("プレイヤーアニメーション管理スクリプト")]
    private PlayerAnimation _playerAnimation = new PlayerAnimation();
    [Tooltip("プレイヤー状態管理スクリプト")]
    private PlayerState _playerState = new PlayerState();
    [Tooltip("プレイヤーステータス管理スクリプト")]
    private PlayerStatus _playerStatus = new PlayerStatus();
    [SerializeField, Tooltip("操作受け取りスクリプト")]
    private PlayerInput _playerInput = default;
    [Tooltip("攻撃ヒット受け取りスクリプト")]
    private PlayerTrigger _playerTrigger = default;
    [Tooltip("無敵時スクリプト")]
    private PlayerInvincble _playerInvincble = new PlayerInvincble();
    [Tooltip("回避スクリプト")]
    private PlayerDodge _playerDodge = default;
    #endregion

    [SerializeField,Tooltip("攻撃エリアオブジェクト")]
    private GameObject _triggerObject = default;
    //[SerializeField,Tooltip("プレイヤーが接地しているか")]
    //private bool _isGround = false;
    [Tooltip("プレイヤーが走っているか")]
    private bool _isRun = false;
    [Tooltip("プレイヤーが攻撃しているか")]
    private bool _isAttack = false;
    [Tooltip("プレイヤーが死亡しているか")]
    private bool _isDeath = false;
    [Tooltip("プレイヤーがジャンプしているか")]
    private bool _isDodge = false;
    [Tooltip("プレイヤーがダメージを受けているか")]
    private bool _isDamaged = false;
    [SerializeField,Tooltip("移動入力方向")]
     private Vector3 _playerPosition = default;
    [Tooltip("プレイヤーの状態をInt型にしたもの")]
    private ReactiveProperty<int> _stateInt = new ReactiveProperty<int>();
    [Tooltip("通常時のプレイヤータグ")]
    private const string CONST_NOTINVINCBLE_TAG = "Player";
    private Rigidbody _playerRigidbody = default;
    private Transform _playerTransform = default;
    private const int CONST_DODGE_STAMINA = 20;
    #endregion

    #region プロパティ  

    /// <summary>
    /// playerUIPresenterにPlayerHPを渡すため
    /// </summary>
    public PlayerStatus PlayerStatus { get => _playerStatus; set => _playerStatus = value; }

    #endregion

    #region メソッド  

    /// <summary>  
    /// 初期化処理  
    /// </summary>  
    private void Awake()
    {
        //HPを初期化する
        PlayerStatus.PlayerHP.Value = PlayerStatus.PlayerMaxHp;

        _playerRigidbody = this.GetComponent<Rigidbody>();
        _playerTrigger = _triggerObject.GetComponent<PlayerTrigger>();
        _playerTransform = this.transform;
        _playerDodge = new PlayerDodge(_playerRigidbody, _playerTransform);

        _playerInput.IsMove
            .Subscribe(vector3 => _playerPosition = vector3);
        _playerInput.IsRun
            .Subscribe(isRun => _isRun = isRun);
        _playerInput.IsDodge
            //回避していない、攻撃していない、死亡していない、ダメージを受けていない、移動している、スタミナがあるとき
            .Where(_ => !_isDodge && !_isAttack && !_isDamaged && !_isDeath && _playerPosition != Vector3.zero && PlayerStatus.PlayerStamina.Value > 0)
            .Subscribe(_ =>
            {               
                //回避処理
                _isDodge = true;
                _playerDodge.Dodge();
                //スタミナを減らす
                PlayerStatus.PlayerStamina.Value -= CONST_DODGE_STAMINA;
                _stateInt.Value = (int)PlayerState.Dodge;
            }).AddTo(this);
        _playerInput.IsAttack
            //ジャンプしていないとき,ダメージを受けていないとき
            .Where(_ => !_isDodge && !_isDamaged)
            .Subscribe(_ =>
            {
                _isAttack = true;
                //動きを止める
                _playerRigidbody.velocity = Vector3.zero;
                //攻撃判定エリアを出す
                _triggerObject.GetComponent<BoxCollider>().enabled = true;

                //すでに攻撃状態だったら
                if (_stateInt.Value == (int)PlayerState.Attack1)
                {
                    _stateInt.Value = (int)PlayerState.Attack2;
                } else
                {
                    _stateInt.Value = (int)PlayerState.Attack1;
                }
            }).AddTo(this);
        _stateInt
            .Subscribe(_ =>
            {
                //プレイヤーの状態を変更する
                _playerState = (PlayerState)_stateInt.Value;
                //アニメーションも同期させる
                _playerAnimation.Animation((int)_playerState);
            }).AddTo(this);
        //攻撃が当たった時
        _playerTrigger.IsHit
            .Subscribe(_ =>
            {
                //もしIDamageableを持っていたら
                if (_playerTrigger.HitObject.TryGetComponent(out IDamageable damageable))
                {
                    //当たったオブジェクトのインターフェースを参照し、ダメージを与える
                    _playerTrigger.HitObject.GetComponent<IDamageable>().DamageHit(PlayerStatus.PlayerPower);
                }
            });
        PlayerStatus.PlayerHP
            //プレイヤーのHPが100の時以外、かつ0以上のとき
            .Where(playerHp => playerHp != 100 && playerHp >= 0)
            .Subscribe(_ =>
            {
                _isDamaged = true;
                //HPが減ったとき、被ダメアニメーションを起こす
                _stateInt.Value = (int)PlayerState.Knockback;
            }).AddTo(this);
        PlayerStatus.PlayerHP
            //もしHPが0以下になったら
            .Where(_ => PlayerStatus.PlayerHP.Value <= 0)
            .Subscribe(_ =>
            {
                //プレイヤーを死亡状態にする
                _stateInt.Value = (int)PlayerState.Death;
                _isDeath = true;
                //動かなくする
                _playerRigidbody.velocity = Vector3.zero;
            }).AddTo(this);
    }

    /// <summary>  
    /// 更新前処理  
    /// </summary>  
    private void Start()
    {

        //初期状態を待機状態にする
        _playerState = PlayerState.Idle;
        //プレイヤーを通常タグにする
        this.tag = CONST_NOTINVINCBLE_TAG;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        UpdateManager();
    }

    /// <summary>
    /// 更新処理（物理演算等）
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
        //回避しているとき,死んでいるとき,ダメージを受けているとき、無敵にする
        _playerInvincble.Invincible(_isDodge,_isDeath, _isDamaged);
    }

    /// <summary>
    /// FixedUpdate基本動作
    /// </summary>
    private void FixedManager()
    {
        //回避していないとき,攻撃していないとき,死亡していないとき,攻撃を受けていないとき
        if (!_isAttack && !_isDeath && !_isDodge && !_isDamaged)
        {
            //プロパティをrefのパラメータとして渡せないため、変数を使って仲介する
            float stamina = PlayerStatus.PlayerStamina.Value;
            //移動処理と移動状態確認
            _stateInt.Value = _playerMove.Move(_playerPosition, _isRun,ref stamina);
            //変更された場合、書き換える
            PlayerStatus.PlayerStamina.Value = stamina;
        }
    }

    #region アニメータに呼び出されるメソッド

    /// <summary>
    /// アイドルアニメーション開始時に呼び出されるメソッド
    /// </summary>
    public void IdleStart()
    {
        //回避を終了する
        _isDodge = false;
        //被ダメを終了する
        _isDamaged = false;
        //攻撃を終了する
        _isAttack = false;
        //攻撃判定エリアを消す
        _triggerObject.GetComponent<BoxCollider>().enabled = false;
    }

    /// <summary>
    /// 被ダメアニメーション開始時に呼び出されるメソッド
    /// </summary>
    public void DamagedStart()
    {
        //攻撃判定エリアを消す
        _triggerObject.GetComponent<BoxCollider>().enabled = false;
    }

    #endregion

    /// <summary>
    /// 敵の体力をダメージ分減らす
    /// </summary>
    /// <param name="damage">送られてきたダメージ量</param>
    public void DamageHit(int damage)
    {
        PlayerStatus.PlayerHP.Value -= damage;
    }

    #endregion
}