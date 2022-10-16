using UnityEngine;

[CreateAssetMenu(fileName = "Достижение")]
public class Achievement : ScriptableObject
{
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _name;
    [SerializeField] private string _uuid;
    [SerializeField] private string _codeName;
    [SerializeField] private string _description;
    [SerializeField] private bool _hasMeta;

    public Sprite Icon => _icon;
    public string Name => _name;
    public string UUID => _uuid;
    public string CodeName => _codeName;
    public string Description => _description;
    public bool HasMeta => _hasMeta;
}