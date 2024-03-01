// ---------------------------------------------------------  
// EnemySound.cs  
// エネミー効果音を再生するスクリプト
// 作成日:  3/1
// 作成者:  竹村綾人
// ---------------------------------------------------------  
using UnityEngine;
using System.Collections;

public class EnemySound : MonoBehaviour
{

    #region 変数  

    private AudioSource _audioSource = default;
    [SerializeField] private AudioClip _attackUp = default;
    [SerializeField] private AudioClip _attackDown = default;
    //[SerializeField] private AudioClip _attack3 = default;
    //[SerializeField] private AudioClip _attack4 = default;
    //[SerializeField] private AudioClip _dodge = default;
    //[SerializeField] private AudioClip _walk = default;
    //[SerializeField] private AudioClip _run = default;

    #endregion

    #region プロパティ  

    #endregion

    #region メソッド  

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }


    /// <summary>  
    /// 効果音再生
    /// </summary>  
    public void Sound(string animeType)
    {
        //今のアニメーションに対応したサウンドを設定
        switch (animeType)
        {
            case "AttackUp":
                _audioSource.clip = _attackUp;
                break;
            case "AttackDown":
                _audioSource.clip = _attackDown;
                break;

        }
        print("ANIME");
        //サウンドを再生する
        _audioSource.PlayOneShot(_audioSource.clip);
    }
    #endregion
}
