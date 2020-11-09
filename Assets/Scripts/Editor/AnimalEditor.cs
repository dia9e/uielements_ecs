using Scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
  public class AnimalEditor : VisualElement
  {
    private AnimalInfo _animalInfo;
    private AnimalsInfoEditor _animalsInfoEditor;

    private Button _button;

    public AnimalInfo AnimalInfo => _animalInfo;

    public AnimalEditor(AnimalsInfoEditor animalsInfoEditor, AnimalInfo animalInfo)
    {
      _animalInfo = animalInfo;
      _animalsInfoEditor = animalsInfoEditor;

      var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/AnimalInfoTemplate.uxml");
      visualTree.CloneTree(this);

      _button = this.Query<Button>(classes: "animal-icon");
      _button.clickable.clicked += () => _animalsInfoEditor.ShowDetailInfo(this);
      UpdateVisualData();
      
      var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/AnimalsInfoEditor.uss");
      styleSheets.Add(stylesheet);
    }

    public void UpdateVisualData()
    {
      var buttonIcon = _button.Q(_button.name);
      var iconAsset = Resources.Load<Texture2D>($"Icons/{_animalInfo.Type}");
      
      buttonIcon.style.backgroundImage = iconAsset;
      _button.tooltip = _animalInfo.Type;
    }
  }
}