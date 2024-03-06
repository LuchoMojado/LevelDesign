using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : Rewind
{
    [SerializeField] float _additiveGrow, _multiplicativeGrow, _changeGrowthTime;
    bool _loading;

    private void Start()
    {
        Save();
    }

    void Update()
    {
        if (_changeGrowthTime > 0)
        {
            _changeGrowthTime -= Time.deltaTime;

            transform.localScale += Vector3.one * _additiveGrow * Time.deltaTime;
        }
        else
        {
            transform.localScale += transform.localScale * _multiplicativeGrow * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.Die();
        }
    }

    public override void Save()
    {
        if (_loading)
            return;
        _mementoState.Rec(transform.localScale, _changeGrowthTime);
    }

    public override void Load()
    {
        if (_mementoState.IsRemember())
        {
            var data = _mementoState.Remember(false);
            transform.localScale = (Vector3)data.parameters[0];
            _changeGrowthTime = (float)data.parameters[1];
            //StartCoroutine(CoroutineLoad());
        }
    }

    IEnumerator CoroutineLoad()
    {
        var WaitForSeconds = new WaitForSeconds(0.01f);
        _loading = true;
        
        while (_mementoState.IsRemember())
        {
            var data = _mementoState.Remember(false);
            transform.localScale = (Vector3)data.parameters[0];
            _changeGrowthTime = (float)data.parameters[1];
            yield return WaitForSeconds;
        }

        _loading = false;
    }
}
