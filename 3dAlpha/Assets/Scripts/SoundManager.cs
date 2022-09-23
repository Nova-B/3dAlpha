//SoundManager
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public AudioMixer mixer;//오디오 믹서 BGM과 효과음 구분
    public AudioSource bgSound;//빈 게임 오브젝트 -> soundobject 자신의 audiosource
    public AudioClip[] bglist;
    public static SoundManager instance;

    [Header("GunSound")]
    [SerializeField] AudioClip pistolShotClip;
    [SerializeField] AudioClip subMachinShotClip;
    [SerializeField] AudioClip rifleShotClip;
    [SerializeField] AudioClip pistolReloadClip;
    [SerializeField] AudioClip subMachineReloadClip;
    [SerializeField] AudioClip rifleReloadClip;

    [Header("Click")]
    [SerializeField] AudioClip weaonBtnClickClip;
    [SerializeField] AudioClip deWeaonBtnClickClip;
    [SerializeField] AudioClip openUIClickClip;

    [Header("Elevator")]
    [SerializeField] AudioClip startEleLiftUpClip;
    [SerializeField] AudioClip endEleLiftUpClip;



    private void Awake()    //싱글톤
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
            SceneManager.sceneLoaded += OnSceneLoaded;  //씬마다 다른 배경음 추가
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        for (int i = 0; i < bglist.Length; i++)
        {
            if (arg0.name == bglist[i].name)
            {
                BgSoundPlay(bglist[i]);
            }
        }
    }

    public void BGSoundVolume(float val) //다른 슬라이더에서 실행하는 함수
    {
        mixer.SetFloat("BGSoundVolume", Mathf.Log10(val) * 20);
    }

    public void SFXSoundVolume(float val) //다른 슬라이더에서 실행하는 함수
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(val) * 20);
    }

    public void SFXPlay(string sfxName, AudioClip clip)
    {
        GameObject go = new GameObject(sfxName + "Sound");//음향을 위한 겜오브젝트 생성(이름)
        AudioSource audiosource = go.AddComponent<AudioSource>();
        audiosource.clip = clip;
        audiosource.Play();
        audiosource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0]; //그룹설정
                                                                                //오디오 믹서에서 파라미터를 만드는 방법이 있다!
        Destroy(go, clip.length); //클립길이만큼 재생하고 삭제
    }

    public void BgSoundPlay(AudioClip clip)
    {
        bgSound.outputAudioMixerGroup = mixer.FindMatchingGroups("BGSound")[0];
        bgSound.clip = clip;
        bgSound.loop = true; //반복재생
        bgSound.volume = 0.1f;
        bgSound.Play();
    }

    public void GunShotSound(Gun.Kind kind)
    {
        switch(kind)
        {
            case Gun.Kind.Pistol:
                {
                    SFXPlay("Pistol", pistolShotClip);
                    break;
                }
            case Gun.Kind.Submachine:
                {
                    SFXPlay("Submachine", subMachinShotClip);
                    break;
                }

            case Gun.Kind.Rifle:
                {
                    SFXPlay("Rifle", rifleShotClip);
                    break;
                }
        }
    }
    public void GunReloadSound(Gun.Kind kind)
    {
        switch (kind)
        {
            case Gun.Kind.Pistol:
                {
                    SFXPlay("Pistol", pistolReloadClip);
                    break;
                }
            case Gun.Kind.Submachine:
                {
                    SFXPlay("Submachine", subMachineReloadClip);
                    break;
                }

            case Gun.Kind.Rifle:
                {
                    SFXPlay("Rifle", rifleReloadClip);
                    break;
                }
        }
    }

    public void WeaponBtnClickSound()
    {
        SFXPlay("WeaponClick", weaonBtnClickClip);
    }

    public void DeEquipWeaponSound()
    {
        SFXPlay("DeWeapon", deWeaonBtnClickClip);
    }

    public void OpenUISound()
    {
        SFXPlay("OpenUI", openUIClickClip);
    }

    public void StartEleLiftUpSound()
    {
        SFXPlay("Start LiftUp", startEleLiftUpClip);
    }

    public void EndEleLiftUpSound()
    {
        SFXPlay("End LiftUp", endEleLiftUpClip);
    }
}
