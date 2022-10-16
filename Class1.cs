using UnityEngine;

public class Skin : ScriptableObject
{
    [SerializeField] private Material m_material;
    [SerializeField] private int m_cost;

    public Material material => m_material;
    public int cost => m_cost;
    
}
