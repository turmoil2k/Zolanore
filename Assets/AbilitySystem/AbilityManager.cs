using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    #region Singleton LevelSystem Instance
    public static AbilityManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Another Level System Script?? bug check");
            return;
        }

        instance = this;
    }
    #endregion

    //change ability styles here i think

    public AbilityExecuter[] meleeAbilityExecs;

    //public AbilityExecuter[] magicAbilityExecs;

    //public AbilityExecuter[] rangedAbilityExecs;

    public delegate void GCDSTART();
    public static event GCDSTART onGCD;

    private void Start()
    {
        //meleeAbilityExecs = GetComponents<AbilityExecuter>();   //universal abilites maybe
    }

    public void StartGCD()
    {
        onGCD.Invoke();
    }
}