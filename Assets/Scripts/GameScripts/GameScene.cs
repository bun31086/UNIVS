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
    private int _selectInt = default;

    [SerializeField]private GameObject[] _buttonObjects = default;
    [SerializeField] private float[] _buttonPositions = default;
    private List<GameObject> _buttonList = new List<GameObject>(); //int型のListを定義
    private GameObject _mostLeftButton = default;
    private List<float> _buttonPos = new List<float>();
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

        ButtonListIn();
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
                //何かのキー又はボタンが押されたらステージシーンに遷移する
                //if (Input.anyKey)
                //{
                //    SceneManager.LoadScene("StageScene");
                //}
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

    public void LeftRight(InputAction.CallbackContext context)
    {        
        // 押されたとき
        if (context.started)
        {
            // MoveActionの入力値を取得
            _vector2 = context.ReadValue<Vector2>();

            //左入力だったら
            if (_vector2.x == -1)
            {
                //選択を左に一個ずらす
                if (_selectInt > 0)
                {
                    _selectInt -= 1;
                }
                /* 現在のシーンを読み込んで、
                 * シーンに合わせてボタンの種類を出す
                 */

            }
            //右入力だったら
            else if (_vector2.x == 1)
            {
                //選択を右に一個ずらす
                if (_selectInt < 3)
                {
                    _selectInt += 1;
                }
            }
        }
    }


    private void ButtonListIn()
    {
        // タグが同じオブジェクトを全て取得する
        _buttonObjects = GameObject.FindGameObjectsWithTag("Button");
        //インデックス初期化
        int index = 0;
        //ポジションを格納する
        //_buttonPos.AddRange();

        //ソートする
        _buttonPos.Sort();


        //_buttonObjectsに入っているオブジェクトで座標が左にあるものからリストに入れていく
        foreach (GameObject mostLeftButton in _buttonObjects)
        {

            //比較対象がないか
            if (_mostLeftButton == null)
            {
                //比較対象を代入
                _mostLeftButton = mostLeftButton;
                print(_mostLeftButton.transform.position.x);

            }
            //比較対象より左に位置しているか
            else if (_mostLeftButton.transform.position.x > mostLeftButton.transform.position.x)
            {
                _mostLeftButton = mostLeftButton;
                print(_mostLeftButton.transform.position.x);
            }
        }
        //リストに一番左にあるボタンを追加
        _buttonList.Add(_mostLeftButton);
        _buttonList.Sort();
    }

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
