// ---------------------------------------------------------  
// PlayerMove.cs  
// プレイヤー移動管理スクリプト
// 作成日:  2/13
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;
using System.Collections;

public class PlayerMove
{

    #region 変数  

    private GameObject _player = default;
    private Camera _camera = default;
    private Transform _playerTransform = default;
    private Transform _cameraTransform = default;
    private Rigidbody _rigidbody = default;
    private Vector3 _cameraForward = default;
    private Vector3 _moveForward = default;
    private Vector3 _xzGet = new Vector3(1, 0, 1);
    private float _moveSpeed = default;
    private float _time = default; 
    private bool _isDash = true;
    //True時Startメソッド実行
    private bool _isFirst = true;

    #region 定数
    private const int CONST_DIRECTION_CAHNGE = 20;
    private const int CONST_PLAYER_IDLE = 0;
    private const int CONST_PLAYER_WALK = 1;
    private const int CONST_PLAYER_RUN = 2;
    private const int CONST_MAX_STAMINA = 100;
    private const int CONST_MIN_STAMINA = 0;
    private const int CONST_RUN_SPEED = 6;
    private const int CONST_WALK_SPEED = 3;
    private const int CONST_STAMINA_CHANGE_MANY = 20;
    private const int CONST_STAMINA_CHANGE_FEW = 10;
    private const float CONST_WAIT_TIME = 5f;
    #endregion

    #endregion

    #region プロパティ  

    #endregion

    #region メソッド  

    /// <summary>
    /// Moveメソッドが呼び込まれた時、最初の一度だけ呼ばれる
    /// </summary>
    public void Start()
    {
        //プレイヤーオブジェクト取得
        _player = GameObject.Find("Player");
        //カメラオブジェクト取得
        _camera = Camera.main;
        //プレイヤーコンポーネント取得（Transform,Rigidbody）
        _playerTransform = _player.transform;
        _rigidbody = _player.GetComponent<Rigidbody>();
        //カメラコンポーネント取得（Transform）
        _cameraTransform = _camera.transform;
        //移動スピード初期化
        _moveSpeed = CONST_WALK_SPEED;
    }

    /// <summary>
    /// プレイヤー移動処理
    /// </summary>
    /// <param name="vector3">移動方向</param>
    /// <param name="isRun">走っているか</param>
    /// <param name="stamina">スタミナ</param>
    /// <returns></returns>
    public int Move(Vector3 vector3, bool isRun ,ref float stamina)
    {
        //最初の一度だけStartメソッドを実行する
        if (_isFirst)
        {        
            _isFirst = false;
            Start();
        }

        //もし走っているなら
        if (isRun && _isDash && vector3 != Vector3.zero)
        {
            //移動スピードを変更
            _moveSpeed = CONST_RUN_SPEED;
            //スタミナを減らす
            stamina -= CONST_STAMINA_CHANGE_MANY * Time.deltaTime;
            //スタミナが０を下回ったとき
            if (stamina <= CONST_MIN_STAMINA)
            {
                //スタミナを０にする
                stamina = CONST_MIN_STAMINA;
                //走り不可能にする
                _isDash = false;
            }
        }
        //歩いている又は止まっている
        else
        {
            //移動スピードを変更
            _moveSpeed = CONST_WALK_SPEED;
            //スタミナがなくなった場合
            if (!_isDash)
            {
                //CONST_WAIT_TIME秒間走れなくする
                _time += Time.deltaTime;
                if (stamina < CONST_MAX_STAMINA)
                {
                    stamina += CONST_STAMINA_CHANGE_MANY * Time.deltaTime;
                }
                else
                {
                    stamina = CONST_MAX_STAMINA;
                }
                if (_time > CONST_WAIT_TIME)
                {
                    //時間を初期化
                    _time = 0;
                    //走り可能にする
                    _isDash = true;
                }
            }
            
            if (stamina < CONST_MAX_STAMINA && _isDash)
            {
                //スタミナを増やす
                stamina += CONST_STAMINA_CHANGE_FEW * Time.deltaTime;
            }
        }

        // カメラの方向からX-Z平面の単位ベクトルを取得
        _cameraForward = Vector3.Scale(_cameraTransform.forward, _xzGet).normalized;

        // 方向キーの入力値とカメラの向きから移動方向を決定
        _moveForward = (_cameraForward * vector3.z) + (_cameraTransform.right * vector3.x);


        // 移動方向にスピードを掛ける。ジャンプや落下があるため、Y軸方向の速度ベクトルを足す。
        _rigidbody.velocity = (_moveForward * _moveSpeed) + new Vector3(0, _rigidbody.velocity.y, 0);


        // プレイヤーの向きが進行方向を向いていないなら
        if (_moveForward != Vector3.zero)
        {
            //プレイヤーの向きを進行方向に変える
            _playerTransform.rotation = Quaternion.RotateTowards(_playerTransform.rotation, Quaternion.LookRotation(_moveForward), CONST_DIRECTION_CAHNGE);
        }

        //走り、歩き、停止状態の返り値
        //止まっているなら
        if (vector3 == Vector3.zero)
        {
            return CONST_PLAYER_IDLE;
        }
        //歩いているなら
        else if (!isRun || !_isDash)
        {
            return CONST_PLAYER_WALK;
        }
        //走っているなら
        else
        {
            return CONST_PLAYER_RUN;
        }
    }

    #endregion
}
