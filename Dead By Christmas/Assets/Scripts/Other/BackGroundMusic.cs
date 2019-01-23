using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BackGroundMusic : MonoBehaviour {
    public AudioClipInfo[] songs;
    public string objectTag;
    [HideInInspector]
    public bool indestructible;
    public AudioSource source;

    public void Awake()
    {
        GameObject[] musicPlayers = GameObject.FindGameObjectsWithTag(objectTag);
        if(musicPlayers.Length != 1)
        {
            foreach (GameObject obj in musicPlayers)
                if (!obj.GetComponent<BackGroundMusic>().indestructible)
                    Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            indestructible = true;
            StartCoroutine(PlayMusic());
        }
    }

    public IEnumerator PlayMusic()
    {
        int index = Random.Range(0, songs.Length);
        source.PlayOneShot(songs[index].song);
        yield return new WaitForSeconds(songs[index].time);
        StartCoroutine(PlayMusic());
    }
}

[System.Serializable]
public class AudioClipInfo
{
    public AudioClip song;
    public float time;
}