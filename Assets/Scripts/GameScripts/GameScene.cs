// ---------------------------------------------------------  
// SceneManager.cs  
// シーン遷移を管理するスクリプト
// 作成日:  3/4
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;
using System;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class GameScene : MonoBehaviour
{

    #region 変数  

    private string _sceneName = default;
    [SerializeField] private GameObject _player = default;
    [SerializeField] private GameObject _enemy = default;
    private PlayerManager _playerManager = default;
    private EnemyManager _enemyManager = default;
    private bool _isPlayerDeath = false;
    private bool _isEnemyDeath = false;
    [SerializeField] private GameObject _fadeObject = default;
    private Image _fadeImage = default;
    private float _alpha = default;
    private const int CONST_MAX_ALPHA = 1;
    private const float CONST_HALF_TIME = 0.5f;
    private Vector2 _vector2 = default;

    private enum SelectType
    {
        Stage = 0,
        Exit = 1,
        Title = 2,
    }
    private SelectType _select = SelectType.Stage;

    private readonly ReactiveProperty<int> _selectInt = new ReactiveProperty<int>(0);

    [SerializeField] private GameObject[] _buttonObjects = default;
    [SerializeField] private List<GameObject> _buttonList = new List<GameObject>(); //int型のListを定義
    private List<float> _buttonPos = new List<float>();
    [SerializeField] private int _buttonCount = default;
    private GameObject _arrowObj = default;
    private const int CONST_ARROW_POS = 200;

    #endregion

    #region プロパティ  

    #endregion

    #region メソッド  

    /// <summary>  
    /// 初期化処理  
    /// </summary>  
    void Awake()
    {
        _isPlayerDeath = false;
        _isEnemyDeath = false;
    }

    /// <summary>  
    /// 更新前処理  
    /// </summary>  
    void Start()
    {
        //現在のシーンを調べる
        _sceneName = SceneManager.GetActiveScene().name;
        //現在のシーンがStageSceneだったら
        if (_sceneName == "StageScene")
        {
            _playerManager = _player.GetComponent<PlayerManager>();
            _enemyManager = _enemy.GetComponent<EnemyManager>();

            //敵HPの値が変化したとき
            _enemyManager.EnemyStatus.EnemyHP
              //HPが０だったら
              .Where(enemyHP => enemyHP == 0)
              .Subscribe(_ =>
              {
                  Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ =>
                  {
                      //プレイヤー死亡フラグをONにする
                      _isEnemyDeath = true;
                  })
                  .AddTo(this);
              })
              //オブジェクトが破棄されたとき、イベントメッセージの購読を終了する
              .AddTo(this);

            //プレイヤーHPの値が変化したとき
            _playerManager.PlayerStatus.PlayerHP
              //HPが０だったら
              .Where(playerHP => playerHP == 0)
              .Subscribe(_ =>
              {
                  Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ =>
                  {
                      //プレイヤー死亡フラグをONにする
                      _isPlayerDeath = true;
                  })
                  .AddTo(this);
              })
              //オブジェクトが破棄されたとき、イベントメッセージの購読を終了する
              .AddTo(this);

            _fadeImage = _fadeObject.GetComponent<Image>();
            _alpha = 0;
            _fadeImage.color = new Color(0, 0, 0, _alpha);
        }
        //ステージシーン以外なら
        else
        {
            //シーン上にあるボタンを左から順にリストに格納する
            ButtonListIn();
            //そのシーンの矢印オブジェクトを取得
            _arrowObj = GameObject.FindGameObjectWithTag("Arrow");
            //矢印の座標変更の通知が来たとき
            _selectInt
                .Subscribe(selectInt =>
                {
                //矢印の座標を変更する
                _arrowObj.transform.position = new Vector3(_buttonList[selectInt].transform.position.x - CONST_ARROW_POS, _arrowObj.transform.position.y, _arrowObj.transform.position.z);
                })
                .AddTo(this);
        }
    }

    /// <summary>  
    /// 更新処理  
    /// </summary>  
    void Update()
    {
        Scene();
    }

    /// <summary>
    /// シーン移動処理
    /// </summary>
    private void Scene()
    {
        switch (_sceneName)
        {
            //タイトルシーンの処理
            case "TitleScene":
                break;
            //ステージシーンの処理
            case "StageScene":
                //プレイヤーの体力が0になったとき
                if (_isPlayerDeath)
                {
                    //現在のアルファ値がマックス値を超えてないとき
                    if (_alpha < CONST_MAX_ALPHA)
                    {
                        _alpha += Time.deltaTime * CONST_HALF_TIME;
                    }
                    else
                    {
                        //ゲームオーバーシーンに飛ばす
                        SceneManager.LoadScene("GameOverScene");
                    }
                    //白フェードイン
                    _fadeImage.color = new Color(0, 0, 0, _alpha);
                }
                //敵の体力が0になったとき
                else if (_isEnemyDeath)
                {
                    //現在のアルファ値がマックス値を超えてないとき
                    if (_alpha < CONST_MAX_ALPHA)
                    {
                        _alpha += Time.deltaTime * CONST_HALF_TIME;
                    }
                    else
                    {
                        //ゲームクリアシーンに飛ばす
                        SceneManager.LoadScene("GameClearScene");
                    }
                    //黒フェードイン
                    _fadeImage.color = new Color(1, 1, 1, _alpha);
                }

                break;
        }
    }


    /// <summary>
    /// シーン上にあるボタンを左から順にリストに格納する
    /// </summary>
    private void ButtonListIn()
    {
        // タグが同じオブジェクトを全て取得する
        _buttonObjects = GameObject.FindGameObjectsWithTag("Button");
        //インデックス初期化
        int index = 0;
        //x座標を配列に格納
        foreach (GameObject buttonObj in _buttonObjects)
        {
            //x座標を配列に入れる
            _buttonPos.Add(buttonObj.transform.position.x);
            //カウントアップ
            index++;
        }
        //座標を入れ終わったらソートする
        _buttonPos.Sort();
        //ソートされた座標と一致するオブジェクトを配列に格納
        foreach (float buttonPos in _buttonPos)
        {
            foreach (GameObject buttonObj in _buttonObjects)
            {
                //配列に格納されている座標とボタンオブジェクトの座標が一致したら
                if (buttonObj.transform.position.x == buttonPos)
                {
                    //配列に格納する
                    _buttonList.Add(buttonObj);
                }
            }
        }
        //今回あるボタンの数を格納
        _buttonCount = _buttonList.Count - 1;
    }

    #region InputSystem用

    /// <summary>
    /// 矢印の左右移動入力
    /// </summary>
    /// <param name="context"></param>
    public void LeftRight(InputAction.CallbackContext context)
    {
        // 押されたとき
        if (context.started)
        {
            // MoveActionの入力値を取得
            _vector2 = context.ReadValue<Vector2>();

            //左入力かつ一番左のボタンに矢印がないとき
            if (_vector2.x == -1 && _selectInt.Value > 0)
            {
                //選択を左に一個ずらす
                _selectInt.Value -= 1;

            }
            //右入力かつ一番右のボタンに矢印がないとき
            else if (_vector2.x == 1 && _selectInt.Value < _buttonCount)
            {
                //選択を右に一個ずらす
                _selectInt.Value += 1;
            }
        }
    }

    /// <summary>
    /// 決定ボタン入力
    /// </summary>
    /// <param name="context"></param>
    public void Decision(InputAction.CallbackContext context)
    {
        // 押されたとき
        if (context.started)
        {
            //押されたときのボタンの名前を格納
            string buttonName = _buttonList[_selectInt.Value].name;
            //もし"ExitButton"以外なら
            if (buttonName != "ExitButton")
            {
                //そのボタン名と同じシーンに移動
                SceneManager.LoadScene(buttonName);
            }
            //もし"ExitButton"なら
            else
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
            Application.Quit();//ゲームプレイ終了
#endif
            }
        }
    }
    #endregion

    #region ボタン用
    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
            Application.Quit();//ゲームプレイ終了
#endif
    }

    public void Stage()
    {
        SceneManager.LoadScene("StageScene");
    }

    public void Title()
    {
        SceneManager.LoadScene("TitleScene");
    }
    #endregion
    #endregion
}