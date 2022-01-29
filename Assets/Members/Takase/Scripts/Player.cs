﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]

public class Player : MonoBehaviour
{
    [SerializeField] private Animator _Animator;
    [SerializeField] private Transform _PlayerInCircle;
    [SerializeField] private Transform _PlayerOutCircle;

    private Tween _tweener;

    private float _Timer = 1.0f;

    private void Start()
    {
        //回転を無限ループさせる
        _PlayerInCircle.transform.DOLocalRotate(new Vector3(0, 0, 360f), 5f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
        _PlayerOutCircle.transform.DOLocalRotate(new Vector3(0, 0, -360f), 5f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
        _PlayerOutCircle.transform.DOScale(new Vector3(1.5f, 1.5f, 1f), 0.2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }

    private void Reset()
    {
        _Animator = GetComponent<Animator>();
    }

    private void Update()
    {

        if (transform.position.x < -100f)
        {
            GameManager.Instance.ChangeState(GameState.Result);
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
        MainManager.I.MapManager.ChangeVisibleAllTiles(true);

        // 移動中なので、何もさせない
        if (_tweener != null)
        {
            return;
        }

        _PlayerOutCircle.transform.DOScale(new Vector3(2f, 2f, 1f), 0.1f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Linear);
        transform.parent = target.transform;
        _tweener = transform.DOLocalMove(Vector3.zero, 0.2f);
        _tweener.onComplete = () =>
        {
            _tweener = null;
            onComplete?.Invoke();
            _Timer = 1.0f;
        };
    }
}
