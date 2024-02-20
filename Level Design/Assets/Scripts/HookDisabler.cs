using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookDisabler : Entity
{
    ObjectPool<HookDisabler> _objectPool;
    float _firstDisableDelay, _disableDuration, _disableCooldown, _disableRadius;
    [SerializeField] float _disableDelay;
    [SerializeField] LayerMask _ghostHookableLayer;
    [SerializeField] Material _normalHookable, _disabledHookable;

    /*float _timer;
    bool _disable = true;*/
    List<Renderer> _hookables = new List<Renderer>();

    IEnumerator DisableCycle()
    {
        yield return new WaitForSeconds(_disableDelay + _firstDisableDelay);

        foreach (var item in _hookables)
        {
            ChangeHookable(item, true);
        }

        yield return new WaitForSeconds(_disableDuration);

        foreach (var item in _hookables)
        {
            ChangeHookable(item, false);
        }

        yield return new WaitForSeconds(_disableCooldown);

        StartCoroutine(DisableCycle());
    }

    void ChangeHookable(Renderer hookable, bool disable)
    {
        if (disable)
        {
            hookable.gameObject.layer = 0;
            hookable.material = _disabledHookable;
        }
        else
        {
            hookable.gameObject.layer = 12;
            hookable.material = _normalHookable;
        }
        
    }

    public void Initialize(ObjectPool<HookDisabler> op, float radius = 50, float duration = 2.5f, float firstDelay = 0, float cooldown = 6)
    {
        _objectPool = op;
        _disableRadius = radius;
        _firstDisableDelay = firstDelay;
        _disableDuration = duration;
        _disableCooldown = cooldown;

        var hookableCols = Physics.OverlapSphere(transform.position, _disableRadius, _ghostHookableLayer);

        foreach (var item in hookableCols)
        {
            _hookables.Add(item.gameObject.GetComponent<Renderer>());
        }

        StartCoroutine(DisableCycle());

        _firstDisableDelay = 0;
    }

    public static void TurnOff(HookDisabler x)
    {
        x.gameObject.SetActive(false);
    }
    public static void TurnOn(HookDisabler x)
    {
        x.gameObject.SetActive(true);
    }

    public override void Die()
    {
        foreach (var item in _hookables)
        {
            ChangeHookable(item, false);
        }

        _objectPool.RefillStock(this);
    }

    public override void Load()
    {
        throw new System.NotImplementedException();
    }

    public override void Save()
    {
        throw new System.NotImplementedException();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _disableRadius);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Die();
    }
}
