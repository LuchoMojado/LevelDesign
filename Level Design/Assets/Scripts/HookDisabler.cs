using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookDisabler : Entity
{
    ObjectPool<HookDisabler> _objectPool;
    float _firstDisableDelay, _disableDuration, _disableCooldown, _disableRadius;
    [SerializeField] LayerMask _ghostHookableLayer;
    [SerializeField] Material _normalHookable, _disabledHookable, _warningHookable;

    enum Actions
    {
        Activate,
        Warn,
        Disable
    }

    List<Renderer> _hookables = new List<Renderer>();

    IEnumerator DisableCycle()
    {
        yield return new WaitForSeconds(_firstDisableDelay);

        foreach (var item in _hookables)
        {
            ChangeHookable(item, Actions.Warn);
        }

        yield return new WaitForSeconds(1);

        foreach (var item in _hookables)
        {
            ChangeHookable(item, Actions.Disable);
        }

        yield return new WaitForSeconds(_disableDuration);

        foreach (var item in _hookables)
        {
            ChangeHookable(item, Actions.Activate);
        }

        yield return new WaitForSeconds(_disableCooldown);

        StartCoroutine(DisableCycle());
    }

    void ChangeHookable(Renderer hookable, Actions action)
    {
        switch (action)
        {
            case Actions.Activate:
                hookable.material = _normalHookable;
                hookable.gameObject.layer = 12;
                break;
            case Actions.Warn:
                hookable.material = _warningHookable;
                break;
            case Actions.Disable:
                hookable.material = _disabledHookable;
                hookable.gameObject.layer = 0;
                break;
            default:
                break;
        }

    }

    public void Initialize(ObjectPool<HookDisabler> op, float radius, float duration, float firstDelay, float cooldown)
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
            ChangeHookable(item, Actions.Activate);
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
