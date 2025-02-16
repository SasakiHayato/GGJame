﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [Header("タイル関係（生成する場合のみ設定が必要）")]
    [SerializeField, Tooltip("マップの子オブジェクトとして生成されるタイル")]
    GameObject[] _tilePrefub;
    [SerializeField, Tooltip("安全地帯のタイル")]
    GameObject _safeTile;
    [SerializeField, Tooltip("タイルの生成上限")]
    int _tileLimit;

    /// <summary>現在、このMapの子オブジェクトになっているタイル</summary>
    List<GameObject> _currentMapTile = new List<GameObject>();
    [Header("タグ")]

    [SerializeField, Tooltip("Destroyされる座標")]
    float _destroyTransformX = -100f;

    private void Start()
    {
        if (_tilePrefub != null)
        {
            InstansTile();
        }
    }
    void Update()
    {
        Move();
        DestroySelf();
    }
    void Move()
    {
        if (!MapManager.I.CanStageMove)
        {
            return;
        }

        this.transform.Translate(-(MapManager.I._speed) * Time.deltaTime, 0, 0);
    }

    /// <summary>
    /// 自分の子オブジェクトにタイルを生成する関数
    /// </summary>
    void InstansTile()
    {
        int _whiteIndex = Random.Range(0, _tileLimit);
        for (int i = 0; i < _tileLimit; ++i)
        {
            if (i != _whiteIndex)
            {
                int random = Random.Range(0, _tilePrefub.Length);
                //生成したタイルをMapManagerのListに追加する
                var go = Instantiate(_tilePrefub[random], this.transform);
                MapManager.I.TileControll(go, true);
                //※問題あり、このクラスのListにも追加
                _currentMapTile.Add(go);
            }
            else
            {
                //安全地帯を絶対に生成する
                var go = Instantiate(_safeTile, this.transform);
                MapManager.I.TileControll(go, true);
                _currentMapTile.Add(go);
            }
        }

    }

    void DestroySelf()
    {
        if (transform.position.x < _destroyTransformX)
        {
            foreach (var i in _currentMapTile)
            {
                //MapmnagerのListから削除
                MapManager.I.TileControll(i, false);
            }
            Destroy(this.gameObject);
        }        
    }
}
