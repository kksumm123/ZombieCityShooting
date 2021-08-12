using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomAudioPlayer : MonoBehaviour
{
    [System.Serializable]
    public class AudioInfo
    {
        public AudioClip clip;
        public float ratio;
        public float minVolume = 1f;
        public float maxVolume = 1f;
    }
    [SerializeField] List<AudioInfo> audios;
    [SerializeField] float maxRandomTime = 30;
    [SerializeField] float gMinVolume = 0.5f;
    [SerializeField] float gMaxVolume = 1f;
    IEnumerator Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(0, maxRandomTime));

                var info = audios.OrderBy(x => Random.Range(0, x.ratio)).Last();
                audioSource.volume = Random.Range(info.minVolume * gMinVolume, info.maxVolume * gMaxVolume);
                audioSource.clip = info.clip;
                audioSource.Play();

                yield return new WaitForSeconds(audioSource.clip.length);
            }
        }
    }
}
