using System;
using System.IO;
using Boo.Lang;
using UnityEditor;
using UnityEngine;

namespace Scripts
{
  public class AnimalsHouse : ScriptableObject
  {
    private const string AssetPath = "Assets";
    
    public List<AnimalInfo> Animals;

    private static AnimalsHouse _instance;

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
      var instance = CreateInstance<AnimalsHouse>();
      var dirPath = AssetPath;
      if (!Directory.Exists(dirPath))
      {
        Directory.CreateDirectory(dirPath);
        AssetDatabase.ImportAsset(dirPath);
      }

      var p = AssetFullPath;
      if (instance && !string.IsNullOrEmpty(p))
        AssetDatabase.CreateAsset(instance, p);
      
      return instance;
    }
    
    public static void Update(AnimalsHouse newValue)
    {
      _instance = newValue;
      
      MarkAsDirty();
    }

    private static void MarkAsDirty() =>
      EditorUtility.SetDirty(Instance);
  }
}