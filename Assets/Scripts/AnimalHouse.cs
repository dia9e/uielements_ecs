using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Scripts
{
  public class AnimalsHouse : ScriptableObject
  {
    private const string AssetPath = "Assets";
    
    private static AnimalsHouse _instance;
    
    public List<AnimalInfo> Animals;

    public static AnimalsHouse Instance =>
      _instance ?? LoadInstance();

    private static string AssetName => 
      typeof(AnimalsHouse).Name;

    private static string AssetFullPath => 
      Path.Combine(AssetPath, AssetName + ".asset");

    private static AnimalsHouse LoadInstance()
    {
      try
      {
        var path = AssetFullPath;
        var instance = AssetDatabase.LoadAssetAtPath(path, typeof (AnimalsHouse)) as AnimalsHouse;
        if (!instance)
        {
          instance = CreateInstance();
          Debug.Log("Created Editor Config instance at " + path, instance);
        }
        
        return instance;
      }
      catch (Exception e)
      {
        Debug.LogError("Failed to load instance of " + AssetName + "\r\n" + e);
        return null;
      }
    }
    
    private static AnimalsHouse CreateInstance()
    {
      var dirPath = AssetPath;
      if (!Directory.Exists(dirPath))
      {
        Directory.CreateDirectory(dirPath);
        AssetDatabase.ImportAsset(dirPath);
      }
      
      var instance = CreateInstance<AnimalsHouse>();
      instance.Animals = new List<AnimalInfo>();
      AssetDatabase.CreateAsset(instance, AssetFullPath);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();

      return instance;
    }
    
    public static void Update(AnimalsHouse newValue)
    {
      _instance = newValue;
      
      MarkAsDirty();
    }

    private static void MarkAsDirty() =>
      EditorUtility.SetDirty(_instance);
  }
}