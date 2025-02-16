﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Sounds;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]

public class Player : MonoBehaviour
{
    [SerializeField] private Animator _Animator;
    [SerializeField] private Transform _PlayerInCircle;
    [SerializeField] private Transform _PlayerOutCircle;
    [SerializeField] private CanvasGroup _PlayerCanvasGroup;


    [SerializeField] private Vector2 _deadAnimX;
    [SerializeField] private Vector2 _deadAnimY;
    [SerializeField] private float _deadGravityScale;

    private Tween _tweener;

    private float _Timer = 1.0f;

    [SerializeField] private Transform m_MoveTarget;

    private bool _isDied = false;

    private bool _isGeteItem = false;
    public bool IsGeteItem { get => _isGeteItem; set => _isGeteItem = value; }

    private void Start()
    {
        _isDied = false;
        //回転を無限ループさせる
        _PlayerInCircle.transform.DOLocalRotate(new Vector3(0, 0, 360f), 5f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
        _PlayerOutCircle.transform.DOLocalRotate(new Vector3(0, 0, -360f), 5f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
        DOTween.Sequence()
            .Append(_PlayerOutCircle.transform.DOScale(new Vector3(1.5f, 1.5f, 1f), 0.2f).SetEase(Ease.Linear))
            .Append(_PlayerOutCircle.transform.DOScale(new Vector3(1.0f, 1.0f, 1f), 0.2f).SetEase(Ease.Linear))
            .AppendInterval(0.25f)
            .SetLoops(-1, LoopType.Yoyo);

    }

    private void Reset()
    {
        _Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_isDied)
            return;

        if (_tweener == null && m_MoveTarget)
        {
            transform.position = m_MoveTarget.position;
        }

        if (transform.position.x < 150f)
        {
            PlayerDiedAnime(() => GameManager.Instance.ChangeState(GameState.Result), false);
        }

        if (_tweener != null)
        {
            return;
        }

        _Timer -= Time.deltaTime;
        if (_Timer <= 0)
        {
            MainManager.I.MapManager.ChangeVisibleAllTiles(false);
        }
    }

    public void Move(Transform target, Action onComplete)
    {
        if (_isDied)
        {
            return;
        }

        if (!_isGeteItem)
        {
            MainManager.I.MapManager.ChangeVisibleAllTiles(true);
        }

        // 移動中なので、何もさせない
        if (_tweener != null)
        {
            return;
        }

        _PlayerOutCircle.transform.DOScale(new Vector3(2f, 2f, 1f), 0.1f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Linear);

        m_MoveTarget = target;
        _tweener = transform.DOMove(target.position, 0.2f);
        _tweener.onComplete = () =>
        {
            _tweener = null;
            onComplete?.Invoke();
            _Timer = 1.0f;
        };
    }

    /// <summary>
    /// 死亡時演出処理
    /// </summary>
    public void PlayerDiedAnime(System.Action compriteCallback, bool isTile)
    {
        if (_isDied)
            return;

        _isDied = true;
        MainManager.I.MapManager.ChangeVisibleAllTiles(false);
        SoundManager.Request(1, SoundGroupID.SE);
        _PlayerInCircle.DOPause();
        _PlayerOutCircle.DOPause();

        if (isTile)
        {
            var rigid1 = _PlayerInCircle.GetComponent<Rigidbody2D>();
            var rigid2 = _PlayerOutCircle.GetComponent<Rigidbody2D>();
            rigid1.gravityScale = _deadGravityScale;
            rigid2.gravityScale = _deadGravityScale;
            var power1 = UnityEngine.Random.Range(_deadAnimX.x, _deadAnimX.y);
            var power2 = UnityEngine.Random.Range(_deadAnimX.x, _deadAnimX.y);
            var addForce1 = new Vector2(power1, UnityEngine.Random.Range(_deadAnimY.x, _deadAnimY.y));
            var addForce2 = new Vector2(-power2, UnityEngine.Random.Range(_deadAnimY.x, _deadAnimY.y));
            rigid1.AddForce(addForce1);
            rigid2.AddForce(addForce2);
            _PlayerCanvasGroup.DOFade(0f, 2.0f).SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    compriteCallback();
                });
        }
        else
        {
            _PlayerCanvasGroup.DOFade(0f, 1.0f).SetEase(Ease.OutSine);
            transform.DOScale(20.0f, 1.0f).SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    compriteCallback();
                });
        }
    }
}
