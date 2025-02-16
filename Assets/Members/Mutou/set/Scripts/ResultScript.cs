﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultScript : MonoBehaviour
{
    //ゲームマネージャーから受け取ったスコアの変数に応じて、表示するテキストを変更
    [SerializeField, Tooltip("リザルト表示時に表示されるテキスト")] Text _resultText = default;
    [SerializeField, Tooltip("テキストを分岐させる時の値")] Vector2Int _judgeNumber = default;
    [SerializeField, Tooltip("表示させるイメージ")] Image[] _popUpImage = default;
    [SerializeField] RankingManage _ranking = default;
    [SerializeField] Animator _animCanvas = default;

    [SerializeField] Button _backButton = default;
    [SerializeField] Button _restartButton = default;

    [SerializeField] Transform _restartFadeTransform = default;

    private bool isBlock = false;

    private void Start()
    {
        isBlock = false;
        _restartButton.onClick.AddListener(ChangeRestart);
        _backButton.onClick.AddListener(ChangeTitle);
    }

    /// <summary>
    /// リザルトのパネルを表示させる関数
    /// </summary>
    public void ViewResult()
    {
        if (_resultText)
        {
            var score = GameManager.Instance.Score;
            _resultText.text = "Score:" + score.ToString();

            if (0 <= score && score < _judgeNumber.x)//ここで表示させるテキストをスコアによって変化させる
            {
                _popUpImage[0].gameObject.SetActive(true);
            }
            else if (_judgeNumber.x <= score && score < _judgeNumber.y)
            {
                _popUpImage[1].gameObject.SetActive(true);
            }
            else
            {
                _popUpImage[2].gameObject.SetActive(true);
            }
        }
    }
    public void GetRankingData()
    {
        _animCanvas.SetBool("Ranking", _ranking.IsRankIn());
        Debug.Log(_animCanvas.GetBool("Ranking"));
    }
    public void BackToResult()
    {
        _animCanvas.SetBool("Ranking", false);
    }

    private void ChangeTitle()
    {
        Debug.Log("ChangeTitle");
        if (isBlock)
            return;
        isBlock = true;
        GameManager.Instance.ChangeState(GameState.Title);
    }

    private void ChangeRestart()
    {
        Debug.Log("ChangeRestart");
        if (isBlock)
            return;
        isBlock = true;
        DOTween.Sequence().Append(_restartFadeTransform.DOScale(new Vector3(0f, 0f, 0f), 1).SetEase(Ease.Linear))
            .OnComplete(() =>
            {
                GameManager.Instance.ChangeState(GameState.InGame);
            });
    }
}
