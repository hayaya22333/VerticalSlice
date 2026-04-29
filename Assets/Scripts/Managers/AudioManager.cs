using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource pistol;
    [SerializeField] AudioSource levelUp;
    [SerializeField] List<AudioSource> kills;
    void Start()
    {
        Locator.Instance.Player.Shoot += HandleShoot;
        Locator.Instance.Player.KilledEnemy += HandleKill;
        Locator.Instance.Player.LevelUp += HandleLevelUp;
    }

    void HandleShoot()
    {
        pistol.Play();
    }

    void HandleKill(int killCnt)
    {
        if (killCnt > 5)
        {
            killCnt = 5;
        }
        kills[killCnt-1].Play();
    }

    void HandleLevelUp(int level)
    {
        levelUp.Play();
    }

}
