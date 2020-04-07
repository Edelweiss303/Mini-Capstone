using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockRotationGameController : Singleton<LockRotationGameController>
{
    public List<CircleLock> Locks;
    public CircleLock ActiveLock;
    public KeySquare CurrentCheckingKey;
    public Text InvalidText1, InvalidText2;
    private int currentActiveLockIndex = 0;
    private bool isInitialized = false;

    private void Start()
    {
        isInitialized = false;
    }

    private void Update()
    {
        if (!isInitialized)
        {
            isInitialized = true;
            resetGame();
            
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
                resetGame();
                return;
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
        GameNetwork.Instance.ToPlayerQueue.Add("g:MiniGameLRSetLockInvalids:" + invalids1);
        GameNetwork.Instance.ToPlayerQueue.Add("p:MiniGameLRSetLockInvalids:" + invalids2);
        //InvalidText1.text = invalids1;
        //InvalidText2.text = invalids2;
    }

    public void resetGame()
    {
        if (isInitialized)
        {
            currentActiveLockIndex = 0;
            randomizeElements();
            foreach (CircleLock Lock in Locks)
            {
                Lock.resetAngles();
                Lock.Active = false;
            }

            ActiveLock = Locks[currentActiveLockIndex];
            ActiveLock.Active = true;
            setInvalids();
        }

    }
}
