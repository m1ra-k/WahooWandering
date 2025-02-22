// BURGER

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteCache", menuName = "ScriptableObjects/SpriteCache")]
public class SpriteCache : ScriptableObject
{
    public Dictionary<string, Sprite> sprites = new();

    void OnEnable() 
    {
        Debug.Log("Loading sprites for the first time...");

        LoadSprite("Art/Sprites/NPC/", Enum.GetNames(typeof(NPCSpriteEnum)));
        LoadSprite("Art/Sprites/Speaker/", Enum.GetNames(typeof(SpeakerSpriteEnum)));
        LoadSprite("Art/Sprites/BG/", Enum.GetNames(typeof(BGSpriteEnum)));
        LoadSprite("Art/Sprites/CG/", Enum.GetNames(typeof(CGSpriteEnum)));
        LoadSprite("Art/Sprites/UI/", Enum.GetNames(typeof(UISpriteEnum)));
    }

    void LoadSprite(string path, string[] spriteNames) 
    {
        foreach (string spriteName in spriteNames) 
        {
            sprites[spriteName] = Resources.Load<Sprite>(path + spriteName);
        }
    }
}
