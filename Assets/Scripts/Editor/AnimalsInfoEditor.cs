using Boo.Lang;
using Editor;
using Scripts;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimalsInfoEditor : EditorWindow
{
  private static AnimalsHouse _animalsHouse;
  private VisualElement _animals;
  private VisualElement _animalInfo;

  private Label _typeLabel;
  private Image _mainIcon;
  private TextField _type;
  private TextField _sound;
  private FloatField _maxFoodPoints;
  private FloatField _maxSleepPoints;

  private AnimalInfo _current = new AnimalInfo();
  private AnimalEditor _lastSelected;

  [MenuItem("AnimalsInfoEditor/Open _%#T")]
  public static void StartSettings()
  {
    if (AnimalsHouse.Instance == null)
      CreateInstance<AnimalsHouse>();

    _animalsHouse = AnimalsHouse.Instance;

    var window = GetWindow<AnimalsInfoEditor>();
    window.titleContent = new GUIContent("AnimalsInfoEditor");
  }

  public void OnEnable()
  {
    // Each editor window contains a root VisualElement object
    var root = rootVisualElement;
    root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/AnimalsInfoEditor.uss"));

    // Loads and clones our VisualTree (eg. our UXML structure) inside the root.
    var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/AnimalsInfoEditor.uxml");
    visualTree.CloneTree(root);

    SetupControls();
  }
  
  private void SetupControls()
  {
    var topLine = rootVisualElement.Query<Image>("TopLine").First();
    topLine.image = Resources.Load<Texture2D>("Elements/Title");
    topLine.scaleMode = ScaleMode.ScaleToFit;
    
    var btnAddAnimal = rootVisualElement.Query<Button>("AddNewButton").First();
    btnAddAnimal.clickable.clicked += AddAnimal;
    
    var btnSave = rootVisualElement.Query<Button>("SaveButton").First();
    btnSave.clickable.clicked += Save;
    
    _animals = rootVisualElement.Query<VisualElement>("AnimalsList").First();
    
    _animalInfo = rootVisualElement.Query<VisualElement>("AnimalInfo").First();
    _animalInfo.visible = false;
    
    var infoBackground = rootVisualElement.Query<Image>("InfoBackground").First();
    infoBackground.image = Resources.Load<Texture2D>("Elements/Info");
    infoBackground.scaleMode = ScaleMode.StretchToFill;

    _typeLabel = rootVisualElement.Query<Label>("AnimalType").First();
    
    _type = rootVisualElement.Query<TextField>("Type").First();
    _type.RegisterValueChangedCallback(e => _current.Type = e.newValue);
    
    _sound = rootVisualElement.Query<TextField>("SoundText").First();
    _sound.RegisterValueChangedCallback(e => _current.Sound = e.newValue);
    
    _maxFoodPoints = rootVisualElement.Query<FloatField>("MaxFoodPoints").First();
    _maxFoodPoints.RegisterValueChangedCallback(e => _current.MaxFoodCount = e.newValue);
    
    _maxSleepPoints = rootVisualElement.Query<FloatField>("MaxSleepPoints").First();
    _maxSleepPoints.RegisterValueChangedCallback(e => _current.MaxSleepSeconds = e.newValue);
    
    _mainIcon = rootVisualElement.Query<Image>("MainIcon").First();
    
    UpdateAnimalsList();
  }

  private void UpdateAnimalsList()
  {
    if (_animalsHouse.Animals == null)
    {
      _animalsHouse.Animals = new List<AnimalInfo>();
      return;
    }

    foreach (var animalInfo in _animalsHouse.Animals)
    {
      var animalEditor = new AnimalEditor(this, animalInfo);
      _animals.Add(animalEditor);
    }
  }

  public void ShowDetailInfo(AnimalEditor editor)
  {
    _animalInfo.visible = true;
    
    UpdateLastSelected();
    
    var icon = Resources.Load<Texture2D>($"Icons/{editor.AnimalInfo.Type}");
    
    _typeLabel.text = editor.AnimalInfo.Type ?? string.Empty;
    _type.value = _current.Type = editor.AnimalInfo.Type ?? string.Empty;
    _sound.value = _current.Sound = editor.AnimalInfo.Sound ?? string.Empty;
    _maxFoodPoints.value = _current.MaxFoodCount = editor.AnimalInfo.MaxFoodCount;
    _maxSleepPoints.value = _current.MaxSleepSeconds = editor.AnimalInfo.MaxSleepSeconds;
    
    _mainIcon.image = icon;

    _lastSelected = editor;
  }

  private void UpdateLastSelected()
  {
    if (_lastSelected == null)
      return;
    
    _lastSelected.AnimalInfo.Type = _current.Type;
    _lastSelected.AnimalInfo.Sound = _current.Sound;
    _lastSelected.AnimalInfo.MaxFoodCount = _current.MaxFoodCount;
    _lastSelected.AnimalInfo.MaxSleepSeconds = _current.MaxSleepSeconds;
    
    _lastSelected.UpdateVisualData();
  }

  private void AddAnimal()
  {
    var animalInfo = new AnimalInfo();
    if (_animalsHouse.Animals == null)
      _animalsHouse.Animals = new List<AnimalInfo> { animalInfo };
    else
      _animalsHouse.Animals.Add(animalInfo);
    
    var animalEditor = new AnimalEditor(this, animalInfo);
    _animals.Add(animalEditor);
  }

  private void Save()
  {
    UpdateLastSelected();
    
    AnimalsHouse.Update(_animalsHouse);
    
    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();
  }
}