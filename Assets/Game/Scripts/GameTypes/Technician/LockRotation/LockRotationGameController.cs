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
                TechnicianController.Instance.ResetHeat();
                return;
            }
            else
            {
                AudioManager.Instance.PlaySound("Technician_Success");
            }

            ActiveLock = Locks[currentActiveLockIndex];
            ActiveLock.Active = true;
            setInvalids();
        }
        else if(CurrentCheckingKey != null)
        {
            TechnicianController.Instance.TakeDamage(0.5f);
            AudioManager.Instance.PlaySound("Technician_Error");
        }
    }

    private void setInvalids()
    {
        string invalids1, invalids2;
        ActiveLock.getInvalids(out invalids1, out invalids2);
        GameNetwork.Instance.ToPlayerQueue.Add("g:MiniGameLRSetLockInvalids:" + invalids1);
        GameNetwork.Instance.ToPlayerQueue.Add("p:MiniGameLRSetLockInvalids:" + invalids2);
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
