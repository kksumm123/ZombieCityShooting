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
    }
    [SerializeField] List<AudioInfo> audios;
    [SerializeField] float maxRandomTime = 30;

    IEnumerator Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            while (true)
            {
                audioSource.clip = audios.OrderBy(x => Random.Range(0, x.ratio))
                                         .Last().clip;
                audioSource.Play();

                yield return new WaitForSeconds(audioSource.clip.length + Random.Range(0, maxRandomTime));
            }
        }
    }
}
