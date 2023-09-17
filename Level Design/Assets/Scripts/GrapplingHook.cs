using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [SerializeField] GameObject _hook;
    LineRenderer _lineR;
    Transform _return;

    [HideInInspector] public bool grappled;

    private void Awake()
    {
        _lineR = GetComponent<LineRenderer>();
        _return = GetComponentInChildren<Transform>();
    }

    private void Update()
    {
        if (_lineR.enabled == true)
        {
            _lineR.SetPosition(0, _return.position);
            _lineR.SetPosition(1, _hook.transform.position);
        }
        
    }

    public IEnumerator Grapple(float speed, RaycastHit hit)
    {
        _lineR.enabled = true;
        grappled = true;

        float time = 0;

        while (time < speed)
        {
            time += Time.deltaTime;

            _hook.transform.position = Vector3.Lerp(_hook.transform.position, hit.point, time);

            yield return null;
        }

        _hook.transform.SetParent(null);
    }

    public IEnumerator Ungrapple(float speed)
    {
        float time = 0;

        while (time < speed)
        {
            time += Time.deltaTime;

            _hook.transform.position = Vector3.Lerp(_hook.transform.position, _return.position, time);

            yield return null;
        }

        _hook.transform.SetParent(transform);
        //_hook.transform.position = _return.position;

        grappled = false;
        _lineR.enabled = false;
    }
}
