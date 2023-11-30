using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Rewind : MonoBehaviour
{
    public MementoState _mementoState;
    private void Awake()
    {
        _mementoState = new MementoState();
        // para guaradr lo que sea _mementoState.Rec(x);
        // Para ver si ya tiene algo _mementoState.IsRemember();
    }
    //Guardar player, objetos agarrables y enemigos
    public abstract void Save();
    public abstract void Load();
}
