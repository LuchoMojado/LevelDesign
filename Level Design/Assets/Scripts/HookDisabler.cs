using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookDisabler : Entity
{
    [SerializeField] float _disableDuration, _disableCooldown, _disableRadius;
    [SerializeField] LayerMask _ghostHookableLayer;
    [SerializeField] Material _normalHookable, _disabledHookable;

    float _timer;
    bool _disable = true;
    List<Renderer> _hookables = new List<Renderer>();

    void Start()
    {
        var hookableCols = Physics.OverlapSphere(transform.position, _disableRadius, _ghostHookableLayer);

        foreach (var item in hookableCols)
        {
            _hookables.Add(item.gameObject.GetComponent<Renderer>());
        }
    }

    void Update()
    {
        if (_timer <= 0)
        {
            ActivateAbility(ref _disable);

            _timer = _disable ? _disableCooldown : _disableDuration;
        }
        else
        {
            _timer -= Time.deltaTime;
        }
    }

    void ActivateAbility(ref bool disable)
    {
        foreach (var item in _hookables)
        {
            ChangeHookable(item, disable);
        }

        disable = !disable;
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

    public override void Die()
    {
        if (!_disable)
        {
            ActivateAbility(ref _disable);
        }

        Destroy(gameObject);
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
