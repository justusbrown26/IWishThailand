﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// Oversees all aspects of the story
/// </summary>
public class StoryManager : MonoBehaviour {

    public static StoryManager instance;

	private AudioSource audioSource; // plays the words audiofile
    private List<Word> words; // Contains all words in the story

	private void Awake()
	{
        audioSource = GetComponent<AudioSource>();
        if (instance == null) // singleton pattern, instance of StoryManager
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

	// Use this for initialization
	void Start () {

        words = new List<Word>();

        LoadSounds(); // Load the sounds from file

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Plays the sound of a word
    /// </summary>
    /// <param name="word">The word to be spoken</param>
    public void PlayWord(string word)
    {
        // finds the appropriate audioclip for the given word
        Word tmp = words.Find(w => w.sWord.Equals(word.ToLower()));
        if (tmp == null) return; // if recording doesnt exist return
        audioSource.clip = tmp.audioClip;

        if (audioSource != null && audioSource.clip != null)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    /// <summary>
    /// Finds all audio files and calls LoadFile() to load all the sound files
    /// </summary>
    void LoadSounds()
    {

        FileInfo[] files;
        string fileName = Application.dataPath + "/" + "Sounds/Words/";
        var info = new DirectoryInfo(fileName);

        files = info.GetFiles()
            .Where(f => Path.GetExtension(f.Name) == ".wav" || Path.GetExtension(f.Name) == ".ogg" || Path.GetExtension(f.Name) == ".mp3")
            .ToArray();


        foreach (var file in files)
        {
            StartCoroutine(LoadFile(file.FullName));
        }


    }

    /// <summary>
    /// Loads a single audio clip from file into the List words
    /// </summary>
    /// <param name="path">path of audiofile</param>
    /// <returns></returns>
    IEnumerator LoadFile(string path)
    {
        WWW www = new WWW("file://" + path);

        AudioClip clip = www.GetAudioClip(false);
        while (clip.loadState != AudioDataLoadState.Loaded)
            yield return www;

        clip.name = Path.GetFileName(path);
        words.Add(new Word(clip.name.Substring(0, clip.name.IndexOf("_")).ToLower() ,clip, true));
    }

}