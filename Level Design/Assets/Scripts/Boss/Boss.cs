using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Boss : Rewind, IPlaySound
{
    FiniteStateMachine _fsm;
    [SerializeField] LevelManager _lvlManager;

    [Header("Hands")]
    [SerializeField] BossHands _handPrefab;
    [SerializeField][Min(2)] int _handAmount;
    ObjectPool<BossHands> _handPool;
    Factory<BossHands> _handFactory;
    BossHands[] _hands;
    [SerializeField] Transform[] _proyectileSpawnTransform;
    [SerializeField] Transform _sweepLimitRight, _sweepLimitLeft, _sweepLimitFront, _sweepLimitBack, _disablerSpawnTransform, _prepareSlamRTransform, _prepareSlamLTransform,
        _idleRTransform, _idleLTransform, _secondPhaseRTransform, _secondPhaseLTransform;
    [SerializeField] float _slamPrepareSpeed, _slamSpeed, _sweepPrepareSpeed, _sweepSpeed, _retractSpeed, _slamPrepareTime, _sweepPrepareTime, _recoverTime,
        _spawnProyectileSpeed, _spawnPrepareTime, _spawnPrepareSpeed, _handOffset;
    Transform[] _prepareSlamTransform, _idleTransform, _secondPhaseTransform;

    [Header("First Phase")]
    [SerializeField] Proyectile _proyectile;
    ObjectPool<Proyectile> _proyectilePool;
    Factory<Proyectile> _proyectileFactory;

    [SerializeField] GameObject[] _shield;
    [SerializeField] ShieldEnergy _shieldEnergy;
    [SerializeField] float _energySpeed;
    int _currentShieldState = 0;
    [SerializeField] int[] _tilesThreshold;

    public List<Renderer> tiles;
    private List<Renderer> tilesDestroy=new List<Renderer>();
    [SerializeField] Material _damagedTileMat;
    [SerializeField] Material _nonDamagedTileMat;
    [SerializeField] float _tileDestroyDelay, _tileExplodeStartingInterval, _explosionRadius,_hookTimeToDisable, _restTime;
    [SerializeField] int _tilesToSecondPhase;

    [Header("Second phase")]
    [SerializeField] GameObject[] _secondPhasePaths;
    [SerializeField] Transform _secondPhasePos;
    [SerializeField] Obstacle _obstacle;
    ObjectPool<Obstacle> _obstaclePool;
    Factory<Obstacle> _obstacleFactory;

    [SerializeField] float _2ndPhaseTransitionSpeed, _2ndPhaseTransitionTime, _2ndPhaseObstacleInterval, _2ndPhaseRetreatSpawnInterval;
    [SerializeField] Vector3[] _2ndPhaseObstacleScales, _2ndPhaseObstacle1Pos, _2ndPhaseObstacle2Pos, _2ndPhaseObstacle3Pos;

    [Header("Third phase")]
    [SerializeField] Transform _thirdPhasePos;
    [SerializeField] Obstacle _wall;
    ObjectPool<Obstacle> _wallPool;
    Factory<Obstacle> _wallFactory;
    [SerializeField] Transform[] _initialWallSpawns;

    [SerializeField] float _3rdPhaseTransitionSpeed, _3rdPhaseTransitionTime, _3rdPhaseObstacleInterval, _3rdPhaseRetreatSpawnInterval,
        _wallSpawnInterval, _wallMinY, _wallMaxY, _wallYOffset;
    [SerializeField] Vector3[] _3rdPhaseObstacleScales, _3rdPhaseObstacle1Pos, _3rdPhaseObstacle2Pos, _3rdPhaseObstacle3Pos;

    [SerializeField] float _handsToHeadDestroyWait, _levelChangeDelay;

    AudioSource _audioSource;

    [Header("Audio clips")]
    [SerializeField] AudioClip _slam;
    [SerializeField] AudioClip _sweeping;
    [SerializeField] AudioClip _slamming;
    [SerializeField] AudioClip _thirdPhase;
    [SerializeField] AudioClip _death;
    [SerializeField] AudioClip _audioClip5;
    [SerializeField] AudioClip _explosion;
    [SerializeField] AudioClip _beep;
    [HideInInspector] public Vector3 playerPos { get; private set; }
    [HideInInspector] public bool takingAction { get; private set; }
    public bool _loading { get; private set; }
    [HideInInspector] public bool transitioning;
    BossStates currentState;

    public enum BossStates
    {
        Waiting,
        FirstPhase,
        SecondPhase,
        ThirdPhase
    }

    void Start()
    {
        _proyectileFactory = new Factory<Proyectile>(_proyectile);
        _proyectilePool = new ObjectPool<Proyectile>(_proyectileFactory.GetObject, Proyectile.TurnOff, Proyectile.TurnOn, _proyectileSpawnTransform.Length);

        _obstacleFactory = new Factory<Obstacle>(_obstacle);
        _obstaclePool = new ObjectPool<Obstacle>(_obstacleFactory.GetObject, Obstacle.TurnOff, Obstacle.TurnOn, 14);

        _wallFactory = new Factory<Obstacle>(_wall);
        _wallPool = new ObjectPool<Obstacle>(_wallFactory.GetObject, Obstacle.TurnOff, Obstacle.TurnOn, 10);

        _handFactory = new Factory<BossHands>(_handPrefab);
        _handPool = new ObjectPool<BossHands>(_handFactory.GetObject, BossHands.TurnOff, BossHands.TurnOn, _handAmount);

        _hands = new BossHands[_handAmount];
        _prepareSlamTransform = new Transform[_handAmount];
        _idleTransform = new Transform[_handAmount];
        _secondPhaseTransform = new Transform[_handAmount];

        int lCounter = 0, rCounter = 0;
        for (int i = 0; i < _hands.Length; i++)
        {
            if (i % 2 == 0)
            {
                Vector3 offset = new Vector3(_handOffset * rCounter, 0, 0);

                var slam = Instantiate(_prepareSlamRTransform, _prepareSlamRTransform.position, _prepareSlamRTransform.rotation);
                _prepareSlamTransform[i] = slam;
                _prepareSlamTransform[i].transform.position -= offset;

                var idle = Instantiate(_idleRTransform, _idleRTransform.position, _idleRTransform.rotation);
                _idleTransform[i] = idle;
                _idleTransform[i].transform.position -= offset;

                var secondPhase = Instantiate(_secondPhaseRTransform, _secondPhaseRTransform.position, _secondPhaseRTransform.rotation);
                _secondPhaseTransform[i] = secondPhase;
                _secondPhaseTransform[i].transform.position -= offset;

                _hands[i] = _handPool.Get();
                _hands[i].Initialize(_handPool, _idleTransform[i]);

                rCounter++;
            }
            else
            {
                Vector3 offset = new Vector3(_handOffset * lCounter, 0, 0);

                var slam = Instantiate(_prepareSlamLTransform, _prepareSlamLTransform.position, _prepareSlamLTransform.rotation);
                _prepareSlamTransform[i] = slam;
                _prepareSlamTransform[i].transform.position += offset;

                var idle = Instantiate(_idleLTransform, _idleLTransform.position, _idleLTransform.rotation);
                _idleTransform[i] = idle;
                _idleTransform[i].transform.position += offset;

                var secondPhase = Instantiate(_secondPhaseLTransform, _secondPhaseLTransform.position, _secondPhaseLTransform.rotation);
                _secondPhaseTransform[i] = secondPhase;
                _secondPhaseTransform[i].transform.position += offset;

                _hands[i] = _handPool.Get();
                _hands[i].Initialize(_handPool, _idleTransform[i]);

                lCounter++;
            }

            GameManager.instance.rewinds2.Add(_hands[i]);
        }

        _fsm = new FiniteStateMachine();
        _audioSource = GetComponent<AudioSource>();

        var secondPhaseDictionary = new Dictionary<Vector3, Vector3[]>
        {
            [_2ndPhaseObstacleScales[0]] = _2ndPhaseObstacle1Pos,
            [_2ndPhaseObstacleScales[1]] = _2ndPhaseObstacle2Pos,
            [_2ndPhaseObstacleScales[2]] = _2ndPhaseObstacle3Pos
        };

        var thirdPhaseDictionary = new Dictionary<Vector3, Vector3[]>
        {
            [_3rdPhaseObstacleScales[0]] = _3rdPhaseObstacle1Pos,
            [_3rdPhaseObstacleScales[1]] = _3rdPhaseObstacle2Pos,
            [_3rdPhaseObstacleScales[2]] = _3rdPhaseObstacle3Pos
        };

        _fsm.AddState(BossStates.Waiting, new WaitingState(this));
        _fsm.AddState(BossStates.FirstPhase, new FirstPhaseState(this, UseFirstPhaseAction, _hookTimeToDisable, _tilesToSecondPhase, _restTime));
        _fsm.AddState(BossStates.SecondPhase, new SecondPhaseState(this, _2ndPhaseTransitionTime, _2ndPhaseObstacleInterval, _2ndPhaseRetreatSpawnInterval, secondPhaseDictionary,
            _secondPhaseTransform[0].position.z - 7.5f));
        _fsm.AddState(BossStates.ThirdPhase, new ThirdPhaseState(this, _3rdPhaseTransitionTime, _wallSpawnInterval, _3rdPhaseObstacleInterval, _3rdPhaseRetreatSpawnInterval,
            thirdPhaseDictionary, _thirdPhasePos.position.z - 19.5f, _thirdPhasePos.position.z - 7.5f, _initialWallSpawns));
        _fsm.ChangeState(BossStates.Waiting);
        currentState = BossStates.Waiting;
    }

    void Update()
    {
        playerPos = GameManager.instance.player.transform.position;

        _fsm.Update();
    }

    public IEnumerator FistSlam(int handIndex)
    {
        int index;

        if (_hands[handIndex].busy)
        {
            index = PickFreeHand();
        }
        else
        {
            index = handIndex;
        }

        _hands[index].busy = true;
        takingAction = true;

        _hands[index].ChangeHandState(BossHands.HandStates.Closed);

        StartCoroutine(_hands[index].MoveAndRotate(_prepareSlamTransform[index], _slamPrepareSpeed, true));

        //moviendo mano hacia arriba

        while (_hands[index].moving)
        {
            yield return null;
        }

        yield return new WaitForSeconds(_slamPrepareTime);

        //mano lista para atacar
        _hands[index].PlaySound(_slamming, false);

        StartCoroutine(_hands[index].MoveAndRotate(new Vector3(playerPos.x, 49, playerPos.z), Quaternion.identity, _slamSpeed));

        // moviendo mano hacia player

        while (_hands[index].moving)
        {
            yield return null;
        }

        //mano golpea piso

        _hands[index].PlaySound(_slam, false);

        foreach (var item in tiles)
        {
            if (Vector3.Distance(item.transform.position, _hands[index].transform.position) <= 8)
            {
                StartCoroutine(DestroyTile(item));
            }
        }

        yield return new WaitForSeconds(_recoverTime);

        _hands[index].ChangeHandState(BossHands.HandStates.Idle);

        //empieza a volver

        StartCoroutine(_hands[index].MoveAndRotate(_idleTransform[index], _retractSpeed, true));

        while (_hands[index].moving)
        {
            yield return null;
        }

        // de nuevo en idle

        _hands[index].busy = false;
        takingAction = false;
    }

    public IEnumerator HandSweep(int handIndex)
    {
        int index;

        if (_hands[handIndex].busy)
        {
            index = PickFreeHand();
        }
        else
        {
            index = handIndex;
        }

        _hands[index].busy = true;
        takingAction = true;
        bool right = index % 2 == 0 ? true : false;
        float xStart, xEnd, zEnd;

        if (right)
        {
            xStart = _sweepLimitRight.position.x;
            xEnd = _sweepLimitLeft.position.x;
        }
        else
        {
            xStart = _sweepLimitLeft.position.x;
            xEnd = _sweepLimitRight.position.x;
        }

        _hands[index].ChangeHandState(BossHands.HandStates.Open);

        StartCoroutine(_hands[index].MoveAndRotate(new Vector3(xStart, 49, playerPos.z + 3f), Quaternion.identity, _sweepPrepareSpeed));

        //moviendo hacia al lado del player

        while (_hands[index].moving)
        {
            yield return null;
        }

        yield return new WaitForSeconds(_sweepPrepareTime);

        zEnd = _hands[index].transform.position.z < playerPos.z ? _sweepLimitBack.position.z : _sweepLimitFront.position.z;

        StartCoroutine(_hands[index].Sweep(playerPos, xEnd, zEnd, _sweepSpeed));

        //haciendo sweep
        _hands[index].PlaySound(_sweeping, true);

        while (_hands[index].moving)
        {
            
            yield return null;
        }

        _hands[index].StopSound();

        yield return new WaitForSeconds(_recoverTime);

        _hands[index].ChangeHandState(BossHands.HandStates.Idle);

        //volviendo

        StartCoroutine(_hands[index].MoveAndRotate(_idleTransform[index], _retractSpeed, true));

        while (_hands[index].moving)
        {
            yield return null;
        }

        _hands[index].busy = false;
        takingAction = false;
    }

    public IEnumerator SpawnProyectiles(int handIndex)
    {
        _hands[handIndex].busy = true;
        takingAction = true;
        bool right = handIndex % 2 == 0 ? true : false;

        Vector3 startPos = right ? _proyectileSpawnTransform[0].position : _proyectileSpawnTransform[_proyectileSpawnTransform.Length - 1].position;
        startPos += Vector3.forward * 4;
        Quaternion rotation = right ? _proyectileSpawnTransform[0].rotation : _proyectileSpawnTransform[_proyectileSpawnTransform.Length - 1].rotation;

        _hands[handIndex].ChangeHandState(BossHands.HandStates.Open);

        StartCoroutine(_hands[handIndex].MoveAndRotate(startPos, rotation, _spawnPrepareSpeed));

        // yendo a primer pos de spawn

        while (_hands[handIndex].moving)
        {
            yield return null;
        }

        yield return new WaitForSeconds(_spawnPrepareTime);

        //recorriendo las pos de spawn

        if (right)
        {
            for (int i = 0; i < _proyectileSpawnTransform.Length; i++)
            {
                Vector3 goal = _proyectileSpawnTransform[i].position + Vector3.forward * 4;
                //PlaySound(_audioClip5, false);
                StartCoroutine(_hands[handIndex].MoveAndRotate(goal, _spawnProyectileSpeed));

                while (_hands[handIndex].moving)
                {
                    yield return null;
                }

                var proyectile = _proyectilePool.Get();
                proyectile.Initialize(_proyectilePool, this);
                proyectile.transform.position = _proyectileSpawnTransform[i].position;
            }
        }
        else
        {
            for (int i = _proyectileSpawnTransform.Length - 1; i >= 0; i--)
            {
                Vector3 goal = _proyectileSpawnTransform[i].position + Vector3.forward * 4;
                //PlaySound(_audioClip5, false);
                StartCoroutine(_hands[handIndex].MoveAndRotate(goal, _spawnProyectileSpeed));

                while (_hands[handIndex].moving)
                {
                    yield return null;
                }

                var proyectile = _proyectilePool.Get();
                proyectile.Initialize(_proyectilePool, this);
                proyectile.transform.position = _proyectileSpawnTransform[i].position;
            }
        }

        _hands[handIndex].ChangeHandState(BossHands.HandStates.Idle);

        StartCoroutine(_hands[handIndex].MoveAndRotate(_idleTransform[handIndex], _retractSpeed, true));

        //volviendo

        while (_hands[handIndex].moving)
        {
            yield return null;
        }

        yield return new WaitForSeconds(_recoverTime * 2);

        _hands[handIndex].busy = false;
        takingAction = false;
    }

    public IEnumerator DisableHook(int handIndex)
    {
        _hands[handIndex].busy = true;
        bool right = handIndex % 2 == 0 ? true : false;

        Vector3 startPos = _disablerSpawnTransform.position - Vector3.up * 3;
        Quaternion rotation = right ? _disablerSpawnTransform.rotation : Quaternion.Inverse(_disablerSpawnTransform.rotation);

        _hands[handIndex].ChangeHandState(BossHands.HandStates.Open);

        StartCoroutine(_hands[handIndex].MoveAndRotate(startPos, rotation, _spawnPrepareSpeed));

        //moviendo a posicion de spawn

        while (_hands[handIndex].moving)
        {
            yield return null;
        }

        var disabler = _lvlManager.SpawnDisabler(_disablerSpawnTransform.position);
        disabler.transform.forward = -Vector3.forward;

        yield return new WaitForSeconds(4);

        StartCoroutine(_hands[handIndex].MoveAndRotate(disabler.transform.position, _spawnPrepareSpeed));

        //moviendo al disabler

        while (_hands[handIndex].moving)
        {
            yield return null;
        }

        // cierra el puño y muere disabler

        _hands[handIndex].ChangeHandState(BossHands.HandStates.Closed);

        disabler.Die();

        StartCoroutine(_hands[handIndex].MoveAndRotate(_idleTransform[handIndex], _retractSpeed, true));

        //volviendo

        while (_hands[handIndex].moving)
        {
            yield return null;
        }

        _hands[handIndex].ChangeHandState(BossHands.HandStates.Idle);

        _hands[handIndex].busy = false;
    }

    public void ExplodeRemainingTiles()
    {
        foreach (var item in tiles)
        {
            StartCoroutine(ExplodeTile(item));
        }
    }

    public void UseFirstPhaseAction()
    {
        int action = Random.Range(0, 3);

        switch (action)
        {
            case 0:
                StartCoroutine(FistSlam(PickHandBySide()));
                break;
            case 1:
                StartCoroutine(HandSweep(PickHandBySide()));
                break;
            case 2:
                StartCoroutine(SpawnProyectiles(PickFreeHand()));
                break;
            default:
                break;
        }
    }

    public int PickHandBySide()
    {
        int index;

        if (playerPos.x <= transform.position.x)
        {
            do
            {
                index = Random.Range(0, _hands.Length);
            } while (index % 2 != 0);
        }
        else
        {
            do
            {
                index = Random.Range(0, _hands.Length);
            } while (index % 2 == 0);
        }
        
        return index;
    }

    public int PickFreeHand()
    {
        int index;

        do
        {
            index = Random.Range(0, _hands.Length);
        } while (_hands[index].busy == true);

        return index;
    }

    public IEnumerator DestroyTile(Renderer tile)
    {
        tile.material = _damagedTileMat;

        yield return new WaitForSeconds(_tileDestroyDelay);

        tiles.Remove(tile);
        //spawneo particulas
        //Destroy(tile.gameObject);
        tile.gameObject.SetActive(false);
        tilesDestroy.Add(tile);
        if (tiles.Count <= _tilesThreshold[_currentShieldState])
        {
            ChangeShield(false);
        }
    }

    public IEnumerator ExplodeTile(Renderer tile)
    {
        
        float time = _tileExplodeStartingInterval;

        while (time > 0.03f)
        {
            tile.material = _damagedTileMat;
            // beep
            PlaySound(_beep, false);
            yield return new WaitForSeconds(0.1f);

            tile.material = _nonDamagedTileMat;

            yield return new WaitForSeconds(time);
            time *= 0.75f;
        }
        // explosion
        PlaySound(_explosion, false);
        if (Vector3.Distance(tile.transform.position, playerPos) <= _explosionRadius)
        {
            GameManager.instance.player.Knockback(tile.transform.position.x, tile.transform.position.z);
        }

        tiles.Remove(tile);
        //spawneo particulas
        Destroy(tile.gameObject);

        if (tiles.Count <= _tilesThreshold[_currentShieldState])
        {
            ChangeShield(false);
        }
    }

    void ChangeShield(bool grow)
    {
        _shield[_currentShieldState].SetActive(false);

        _currentShieldState += grow ? 1 : -1;

        if (_currentShieldState < 0)
        {
            _currentShieldState = 0;
            return;
        }
        else if (_currentShieldState >= _shield.Length)
            _currentShieldState = _shield.Length - 1;

        _shield[_currentShieldState].SetActive(true);
    }

    public IEnumerator FirstPhaseTransition()
    {
        takingAction = true;
        Save();
        foreach (var item in _hands)
        {
            item.Save();
        }
        List<ShieldEnergy> energies = new List<ShieldEnergy>();

        for (int i = 0; i < tiles.Count; i++)
        {
            var energy = Instantiate(_shieldEnergy, tiles[i].transform.position, Quaternion.identity);
            energy.currentGoal = tiles[i].transform.position + Vector3.up * 2;
            energy.finalGoal = _shield[0].transform.position - Vector3.up * 13;
            energy.speed = _energySpeed * 0.1f;
            energies.Add(energy);
        }

        yield return new WaitForSeconds(4);

        foreach (var item in energies)
        {
            item.currentGoal = item.finalGoal;
            item.speed = _energySpeed;
        }

        yield return new WaitForSeconds(1.9f);

        _shield[0].SetActive(true);

        yield return new WaitForSeconds(1.4f);

        ChangeShield(true);

        yield return new WaitForSeconds(1.4f);

        ChangeShield(true);

        yield return new WaitForSeconds(1.4f);

        ChangeShield(true);

        yield return new WaitForSeconds(1);

        _lvlManager.ChangeMusic(1);
        takingAction = false;
        currentState = BossStates.FirstPhase;
    }

    public IEnumerator SecondPhaseTransition()
    {
        yield return new WaitForSeconds(_2ndPhaseTransitionTime);

        for (int i = 0; i < _hands.Length; i++)
        {
            _hands[i].ChangeHandState(BossHands.HandStates.Open);

            StartCoroutine(_hands[i].MoveAndRotate(_secondPhaseTransform[i], _2ndPhaseTransitionSpeed, true));
        }
        
        while (transform.position != _secondPhasePos.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, _secondPhasePos.position, Time.deltaTime * _2ndPhaseTransitionSpeed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _secondPhasePos.rotation, Time.deltaTime * 20);

            yield return null;
        }

        for (int i = 0; i < _secondPhasePaths.Length; i++)
        {
            _secondPhasePaths[i].SetActive(true);
        }
        currentState = BossStates.SecondPhase;
        Save();
        foreach (var item in _hands)
        {
            item.Save();
        }
    }

    public void SpawnObstacle(int keyNumber, Dictionary<Vector3, Vector3[]> obsDictionary, float zValue, float startDelay = 1)
    {
        Vector3 scale = obsDictionary.ElementAt(keyNumber).Key;
        Vector3 pos = obsDictionary[scale][Random.Range(0, obsDictionary[scale].Length)];
        pos += Vector3.forward * zValue;

        var obs = _obstaclePool.Get();
        obs.transform.localScale = scale;
        obs.transform.position = pos;
        obs.startDelay = startDelay;
        obs.Initialize(_obstaclePool);
    }

    public IEnumerator ThirdPhaseTransition()
    {
        PlaySound(_thirdPhase, false);
        for (int i = 0; i < _hands.Length; i++)
        {
            _handPool.RefillStock(_hands[i]);
        }

        while (transform.position != _thirdPhasePos.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, _thirdPhasePos.position, Time.deltaTime * _3rdPhaseTransitionSpeed);

            yield return null;
        }
        currentState = BossStates.ThirdPhase;
        Save();
        foreach(var item in _hands)
        {
            item.Save();
        }
    }

    public void SpawnWall(float zValue)
    {
        var pos = new Vector3(transform.position.x, Random.Range(_wallMinY, _wallMaxY), zValue);

        var wall = _wallPool.Get();
        wall.transform.position = pos;
        wall.Initialize(_wallPool, _wall.transform.localScale.y);

        var topWallPos = pos + Vector3.up * _wallYOffset;

        var topWall = _wallPool.Get();
        topWall.transform.position = topWallPos;
        topWall.Initialize(_wallPool, topWall.transform.localScale.y);
    }

    public IEnumerator Die()
    {
        // alguna animacion o movimiento?

        yield return new WaitForSeconds(2f);

        // animacion o particula?
        var children = GetComponentsInChildren<Renderer>();
        foreach (var item in children)
        {
            item.enabled = false;
        }
        PlaySound(_death, false);

        yield return new WaitForSeconds(_levelChangeDelay);

        _lvlManager.BeginPart2(transform.position);
        gameObject.SetActive(false);
    }

    public override void Save()
    {
        if (_loading)
            return;
        _mementoState.Rec(currentState,transform.position,transform.rotation);
    }

    public override void Load()
    {
        foreach(var item in _shield)
        {
            item.SetActive(false);
        }
        _currentShieldState = 0;
        StopAllCoroutines();
        takingAction = false;
        if (currentState == BossStates.FirstPhase)
        {
            _lvlManager.ChangeMusic(0);
            ActiveAllPlatforms();
            foreach (var item in _hands)
                item.ChangeHandState(BossHands.HandStates.Idle);
            currentState = BossStates.Waiting;
        }
            
        if (_mementoState.IsRemember())
        {
            StartCoroutine(CoroutineLoad());
        }
    }
    IEnumerator CoroutineLoad()
    {
        var WaitForSeconds = new WaitForSeconds(0.01f);
        _loading = true;
        //_myRB.constraints = RigidbodyConstraints.FreezeAll;
        while (_mementoState.IsRemember())
        {
            var data = _mementoState.Remember();
            //_loading = true;
            //Pongo en el array la pos que se donde lo puse lo que quiero
            _fsm.RestartState((BossStates)data.parameters[0]);
            transform.position = (Vector3)data.parameters[1];
            transform.rotation = (Quaternion)data.parameters[2];
            yield return WaitForSeconds;
        }
        //_myRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        _loading = false;
    }
    public void ActiveAllPlatforms()
    {
        foreach(var item in tilesDestroy)
        {
            tiles.Add(item);
        }
        foreach (var item in tiles)
        {
            item.gameObject.SetActive(true);
            item.material = _nonDamagedTileMat;
        }
        tilesDestroy.Clear();
    }
    public void PlaySound(AudioClip clip, bool loop)
    {
        _audioSource.clip = clip;
        _audioSource.loop = loop;
        _audioSource.Play();
    }
    public void StopSound()
    {
        _audioSource.Stop();
    }
}
