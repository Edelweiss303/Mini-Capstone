using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockRotationGameController : MonoBehaviour
{
    public static LockRotationGameController Instance;
    public List<CircleLock> Locks;
    public CircleLock ActiveLock;
    public KeySquare CurrentCheckingKey;
    public Text InvalidText1, InvalidText2;


    private int currentActiveLockIndex = 0;
    private bool initialized = false;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)
        {
            ActiveLock = Locks[currentActiveLockIndex];
            ActiveLock.Active = true;
            setInvalids();
            initialized = true;
        }
    }

    private void randomizeElements()
    {
        foreach(CircleLock Lock in Locks)
        {
            Lock.randomizeKeys();
            Lock.chooseSolutionKey();
        }
    }
    
    public void tryUnlocking()
    {
        if(CurrentCheckingKey == ActiveLock.SolutionKey)
        {
            ActiveLock.Active = false;
            currentActiveLockIndex++;
            if(currentActiveLockIndex >= Locks.Count)
            {
                currentActiveLockIndex = 0;

                //Reset rotations and lock values
                
                randomizeElements();
                foreach(CircleLock Lock in Locks)
                {
                    Lock.resetAngles();
                }
            }

            ActiveLock = Locks[currentActiveLockIndex];
            ActiveLock.Active = true;
            setInvalids();
        }
    }

    private void setInvalids()
    {
        string invalids1, invalids2;
        ActiveLock.getInvalids(out invalids1, out invalids2);
        InvalidText1.text = invalids1;
        InvalidText2.text = invalids2;
    }
}
