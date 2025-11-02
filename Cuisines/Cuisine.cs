using Godot;
using System;
using System.Collections.Generic;
[GlobalClass]
public partial class Cuisine : Resource
{
    [Export] public int BaseScore = 10;
    public virtual string CuisineName { get; }
    public Texture2D CuisineTexture
    {
        get
        {
            if (field == null)
                field = ResourceLoader.Load<Texture2D>(TexturePath);
            return field;
        }
    }
    protected virtual string TexturePath { get; }
    protected static Dictionary<string, Cuisine> CuisineDictionary = new();
    public virtual void OnDelivered(float multiplier)
    {
        GameData.Instance.Score += (int)(BaseScore + GameData.Instance.TimePassed / 60 * 5 * multiplier);
    }
    static Cuisine()
    {
        var cuisineTypes = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in cuisineTypes)
            foreach (var type in assembly.GetTypes())
                if (type.IsSubclassOf(typeof(Cuisine)) && !type.IsAbstract)
                {
                    Cuisine instance = (Cuisine)Activator.CreateInstance(type);
                }
    }
    public static Cuisine GetCuisineByName(string name)
    {
        if (CuisineDictionary.ContainsKey(name))
            return CuisineDictionary[name];

        GD.PushError($"Cuisine: No cuisine found with name {name}!");
        return null;
    }
    public Cuisine() => CuisineDictionary[CuisineName] = this;
    public static Cuisine GetRandomCuisine()
    {
        var values = new List<Cuisine>(CuisineDictionary.Values);
        if (values.Count == 0)
        {
            GD.PushError("Cuisine: No cuisines available in dictionary!");
            return null;
        }
        var randomIndex = GD.Randi() % values.Count;
        return values[(int)randomIndex];
    }
}
