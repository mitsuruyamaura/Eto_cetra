using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

// 音管理クラス
public class SoundManager : MonoBehaviour
{

    protected static SoundManager instance;

    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (SoundManager)FindObjectOfType(typeof(SoundManager));

                if (instance == null)
                {
                    Debug.LogError("SoundManager Instance Error");
                }
            }
            return instance;
        }
    }

    // 音楽管理
    public enum Enum_BGM : int
    {
        Select,
        Game,
        Result,
    }

    // 効果音管理
    public enum Enum_SE : int
    {
        Result,
        OK,
        Erase,
        Skill,
        Transition,
        Shuffle,
    }

    // クロスフェード時間
    public const float XFADE_TIME = 1.4f;

    // 音量
    public SoundVolume volume = new SoundVolume();

    // === AudioSource ===
    // BGM
    private AudioSource[] BGMsources = new AudioSource[2];
    // SE
    private AudioSource[] SEsources = new AudioSource[16];

    // === AudioClip ===
    // BGM
    public BGMDatas[] BGM;
    // SE
    public AudioClip[] SE;

    // SE用AudioMixer
    public AudioMixer audioMixer;

    bool isXFading;

    int currentBgmIndex = 999;

    [System.Serializable]
    public class BGMDatas
    {
        public AudioClip clip;
        public float loopTime;
        public float endTime;
    }

    void Awake()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("SoundManager");

        if (obj.Length > 1)
        {
            // 既に存在しているなら削除
            Destroy(gameObject);
        }
        else
        {
            // 音管理はシーン遷移では破棄させない
            DontDestroyOnLoad(gameObject);
        }

        // BGM AudioSource
        BGMsources[0] = gameObject.AddComponent<AudioSource>();
        BGMsources[1] = gameObject.AddComponent<AudioSource>();

        // SE AudioSource
        for (int i = 0; i < SEsources.Length; i++)
        {
            SEsources[i] = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // ミュート設定
        BGMsources[0].mute = volume.Mute;
        BGMsources[1].mute = volume.Mute;
        foreach (AudioSource source in SEsources)
        {
            source.mute = volume.Mute;
        }

        // ボリューム設定
        if (!isXFading)
        {
            BGMsources[0].volume = volume.BGM;
            BGMsources[1].volume = volume.BGM;
        }
        foreach (AudioSource source in SEsources)
        {
            source.volume = volume.SE;
        }

        // Loop処理
        if (currentBgmIndex != 999)
        {
            if (BGM[currentBgmIndex].loopTime > 0f)
            {
                if (!BGMsources[0].mute && BGMsources[0].isPlaying && BGMsources[0].clip != null)
                {
                    if (BGMsources[0].time >= BGM[currentBgmIndex].endTime)
                    {
                        BGMsources[0].time = BGM[currentBgmIndex].loopTime;
                    }
                }
                if (!BGMsources[1].mute && BGMsources[1].isPlaying && BGMsources[1].clip != null)
                {
                    if (BGMsources[1].time >= BGM[currentBgmIndex].endTime)
                    {
                        BGMsources[1].time = BGM[currentBgmIndex].loopTime;
                    }
                }
            }
        }
    }

    // ***** BGM再生 *****
    // BGM再生
    public void PlayBGM(Enum_BGM bgmNo, bool loopFlg = true)
    {
        int index = (int)bgmNo;
        currentBgmIndex = index;
        //if(PlayerPrefs.GetInt(Constant.BGM_FLG_NAME,1) == 1){
        if (0 > index || BGM.Length <= index)
        {
            return;
        }
        // 同じBGMの場合は何もしない
        if (BGMsources[0].clip != null && BGMsources[0].clip == BGM[index].clip)
        {
            return;
        }
        else if (BGMsources[1].clip != null && BGMsources[1].clip == BGM[index].clip)
        {
            return;
        }
        //volume.BGM = gameData.volumeBgm;
        // フェードでBGM開始
        if (BGMsources[0].clip == null && BGMsources[1].clip == null)
        {
            BGMsources[0].loop = loopFlg;
            BGMsources[0].clip = BGM[index].clip;
            BGMsources[0].Play();
            //BGMsources[0].DOFade(gameData.volumeBgm, XFADE_TIME);
        }
        else
        {
            // クロスフェード
            StartCoroutine(CrossfadeChangeBMG(index, loopFlg));
        }
    }

    private IEnumerator CrossfadeChangeBMG(int index, bool loopFlg)
    {
        isXFading = true;
        if (BGMsources[0].clip != null)
        {
            // 0がなっていて、1を新しい曲としてPlay
            BGMsources[1].volume = 0;
            BGMsources[1].clip = BGM[index].clip;
            BGMsources[1].loop = loopFlg;
            BGMsources[1].Play();
            BGMsources[0].DOFade(0, XFADE_TIME).SetEase(Ease.Linear);
            //BGMsources[1].DOFade(gameData.volumeBgm, XFADE_TIME).SetEase(Ease.Linear);
            yield return new WaitForSeconds(XFADE_TIME);
            BGMsources[0].Stop();
            BGMsources[0].clip = null;
        }
        else
        {
            // 1がなっていて、0を新しい曲としてPlay
            BGMsources[0].volume = 0;
            BGMsources[0].clip = BGM[index].clip;
            BGMsources[0].loop = loopFlg;
            BGMsources[0].Play();
            BGMsources[1].DOFade(0, XFADE_TIME).SetEase(Ease.Linear);
            //BGMsources[0].DOFade(gameData.volumeBgm, XFADE_TIME).SetEase(Ease.Linear);
            yield return new WaitForSeconds(XFADE_TIME);
            BGMsources[1].Stop();
            BGMsources[1].clip = null;
        }
        isXFading = false;
    }

    // BGM停止
    public void StopBGM()
    {
        BGMsources[0].Stop();
        BGMsources[1].Stop();
        BGMsources[0].clip = null;
        BGMsources[1].clip = null;
    }

    // ***** SE再生 *****
    // SE再生
    public void PlaySE(Enum_SE seNo)
    {
        int index = (int)seNo;
        //if(PlayerPrefs.GetInt(Constant.SE_FLG_NAME,1) == 1){
        if (0 > index || SE.Length <= index)
        {
            return;
        }

        // 再生中で無いAudioSouceで鳴らす
        foreach (AudioSource source in SEsources)
        {
            if (false == source.isPlaying)
            {
                source.clip = SE[index];
                //volume.SE = gameData.volumeSe;
                source.Play();
                return;
            }
        }
        //}
    }

    // SE停止
    public void StopSE()
    {
        // 全てのSE用のAudioSouceを停止する
        foreach (AudioSource source in SEsources)
        {
            source.Stop();
            source.clip = null;
        }
    }

    public void SetAudioMixerVolume(float vol)
    {
        if (vol == 0)
        {
            audioMixer.SetFloat("volumeSE", -80);
        }
        else
        {
            audioMixer.SetFloat("volumeSE", 0);
        }
    }

    public void MuteBGM()
    {
        BGMsources[0].Stop();
        BGMsources[1].Stop();
    }

    public void ResumeBGM()
    {
        BGMsources[0].Play();
        BGMsources[1].Play();
    }
}