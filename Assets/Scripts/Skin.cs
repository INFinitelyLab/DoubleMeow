using UnityEngine;

[CreateAssetMenu( fileName = "Скин" )]
public class Skin : ScriptableObject
{
    [SerializeField] private Material _material;
    [SerializeField] private string _codeName;
    [SerializeField] private string _name;
    [SerializeField] private string _uuid;
    [SerializeField] private int _cost;

    public Material Material => _material;
    public string CodeName => _codeName;
    public string Name => _name;
    public string UUID => _uuid;
    public int Cost => _cost;


    // Static

    private static Skin[] _skins;


    public static void Initialize()
    {
        if (_skins != null) return;

        _skins = Resources.LoadAll<Skin>("");

        Debug.Log("Всего скинов в игре : " + _skins.Length);
    }


    public static Skin Find(string codeName)
    {
        foreach( Skin skin in _skins )
        {
            if( skin.CodeName == codeName )
            {
                return skin;
            }
        }

        Debug.Log("Не найден скин с именем : " + codeName);

        return null;
    }


    public static void Unlock(string codeName)
    {
        Skin skin = Find(codeName);

        Secure secure = new Secure(skin.UUID, "UUID");

        if (Has(codeName) == false)
        {
            PlayerPrefsExtentions.SetSecure(codeName, secure);

            Debug.Log("Получено достижение: " + skin.Name);
        }
    }


    public static bool Has(string codeName)
    {
        if (PlayerPrefsExtentions.HasSecure(codeName))
        {
            return PlayerPrefsExtentions.GetSecure(codeName, "UUID") == Find(codeName).UUID;
        }

        return false;
    }
}