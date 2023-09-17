using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [SerializeField] GameObject _hook;
    LineRenderer _lineR;
    [SerializeField] Transform _return;

    [HideInInspector] public bool grappled;

    private void Awake()
    {
        _lineR = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        
    }

    private void LateUpdate()
    {
        _lineR.SetPosition(0, _return.position);
        _lineR.SetPosition(1, _hook.transform.position);
    }

    public IEnumerator Grapple(float speed, RaycastHit hit)
    {
        _lineR.enabled = true;
        grappled = true;
        _hook.transform.SetParent(null);
        float time = 0;
        float distance = (_hook.transform.position - hit.point).magnitude;

        while (time < distance * speed)
        {
            time += Time.deltaTime;

            _hook.transform.position = Vector3.Lerp(_hook.transform.position, hit.point, time / (distance * speed));

            yield return null;
        }

        _hook.transform.rotation = Quaternion.Euler(hit.normal + new Vector3(90,0,0));
    }

    public IEnumerator Ungrapple(float speed)
    {
        float time = 0;
        Vector3 oldPos = _hook.transform.position;
        float distance = (_hook.transform.position - _return.position).magnitude;

        while (time < distance * speed)
        {
            time += Time.deltaTime;

            _hook.transform.position = Vector3.Lerp(oldPos, _return.position, time / (distance * speed));
            print(time);
            yield return null;
        }
        print("si");

        _hook.transform.position = _return.position;
        _hook.transform.rotation = _return.rotation;
        _hook.transform.SetParent(transform);
        grappled = false;
        _lineR.enabled = false;
    }
}
