using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [SerializeField] GameObject _hook;
    LineRenderer _lineR;
    ConfigurableJoint _playerJoint;
    [SerializeField] Transform _return;
    SoftJointLimit limitConfig = new SoftJointLimit();
    public Chain chainPrefab;
    public Factory<Chain> factory;
    ObjectPool<Chain> myPool;
    public List<Chain> allChains = new List<Chain>();
    
    public ConfigurableJoint chainJoint;

    [HideInInspector] public bool shot, grappled;
    [HideInInspector] public GameObject grappledTo;

    private void Awake()
    {
        _lineR = GetComponent<LineRenderer>();
    }
    bool listo = true;

    public void Start()
    {
        factory = new Factory<Chain>(chainPrefab);
        myPool = new ObjectPool<Chain>(factory.GetObject, Chain.TurnOff, Chain.TurnOn, 30);
    }

    private void LateUpdate()
    {
        _lineR.SetPosition(0, _return.position);
        _lineR.SetPosition(1, _hook.transform.position);
    }

    public IEnumerator Grapple(float speed, RaycastHit hit, GameObject player)
    {
        _lineR.enabled = true;
        shot = true;
        _hook.transform.SetParent(null);
        float time = 0;
        float distance = (_hook.transform.position - hit.point).magnitude;

        while (time < distance * speed)
        {
            time += Time.deltaTime;

            _hook.transform.position = Vector3.Lerp(_hook.transform.position, hit.point, time / (distance * speed));

            if (_hook.transform.position == hit.point)
            {
                float jointLimit = (player.transform.position - hit.point).magnitude + 0.6f;
                _hook.transform.rotation = Quaternion.Euler(hit.normal + new Vector3(90, 0, 0));
                grappled = true;
                grappledTo = hit.collider.gameObject;
                limitConfig.limit = jointLimit;
                _playerJoint = player.AddComponent<ConfigurableJoint>();
                JointSetUp(_playerJoint, hit.point, limitConfig);
                player.GetComponent<Player>().joint = _playerJoint;
                yield break;
            }

            yield return null;
        }

        /*float jointLimit = (player.transform.position - hit.point).magnitude + 0.6f;
        _hook.transform.rotation = Quaternion.Euler(hit.normal + new Vector3(90,0,0));
        grappled = true;
        limitConfig.limit = jointLimit;
        _playerJoint = player.AddComponent<ConfigurableJoint>();
        JointSetUp(_playerJoint, hit.point, limitConfig);*/
    }

    public IEnumerator Ungrapple(float speed)
    {
        if (!shot) yield break;
        float time = 0;
        Vector3 oldPos = _hook.transform.position;
        float distance = (_hook.transform.position - _return.position).magnitude;
        grappled = false;
        grappledTo = null;
        Destroy(_playerJoint);

        while (time < distance * speed * 0.5f)
        {
            time += Time.deltaTime;

            _hook.transform.position = Vector3.Lerp(oldPos, _return.position, time / (distance * speed * 0.5f));
            
            yield return null;
        }

        _hook.transform.position = _return.position;
        _hook.transform.rotation = _return.rotation;
        _hook.transform.SetParent(transform);
        shot = false;
        _lineR.enabled = false;
    }

    public SoftJointLimit ChangeJointDistance(float dir, float maxRange)
    {
        float previewResult = limitConfig.limit + dir * Time.deltaTime;

        if (previewResult > 0.5f && previewResult <= maxRange)
        {
            limitConfig.limit = previewResult;
        }
        else if (previewResult >= maxRange && previewResult < limitConfig.limit)
        {
            limitConfig.limit = previewResult;
        }
        else if (previewResult < 0.5f && previewResult > limitConfig.limit)
        {
            limitConfig.limit = previewResult;
        }

        return limitConfig;
    }

    void JointSetUp(ConfigurableJoint joint, Vector3 grapplePoint, SoftJointLimit limit)
    {
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePoint;
        joint.linearLimit = limit;
        joint.enableCollision = true;
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;
    }

    /*public void SpawnChain(Vector3 newP)
    {
        var x = myPool.Get();
        x.Initialize(myPool);
        allChains.Add(x);
        int lastChain = allChains.Count;
        if (lastChain > 1)
        {
            int newPoint = lastChain - 2;
            x.transform.position = newP;
            x.transform.up = _hook.transform.position;
        }
        else
        {
            x.gameObject.GetComponent<ConfigurableJoint>().connectedBody = _hook.GetComponent<Rigidbody>();
            x.transform.position = _hook.transform.position;
            x.transform.up = _hook.transform.position;
            //x.transform.forward = transform.position;
        }
        //desde donde choca la cadena poner el primer punto, y si es mayor la distancia a tanto a comparacion del return spawnee otra 
    }*/
}
