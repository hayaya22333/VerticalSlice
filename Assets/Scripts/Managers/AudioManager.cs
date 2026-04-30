using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    PlayerController player;
    [SerializeField] AudioSource pistol;
    [SerializeField] AudioSource levelUp;
    [SerializeField] List<AudioSource> kills;
    [SerializeField] AudioSource hurt;

    void Start()
    {
        player = Locator.Instance.Player;
        player.Shoot += HandleShoot;
        player.KilledEnemy += HandleKill;
        player.LevelUp += HandleLevelUp;
        player.PlayerTookDamage += HandlePlayerDamage;
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

    void HandlePlayerDamage(int dmg)
    {
        hurt.Play();
    }

}
