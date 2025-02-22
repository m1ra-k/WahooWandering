using System;
using System.ComponentModel;

public enum CharacterEnum
{
    Tubby,
    Nana,
    Lacey,
    Yaning,
    [Description("Bloody Manager")]
    BloodyManager,
    [Description("Fries Manager")]
    FriesManager,
    [Description("Spaghetti Worker")]
    SpaghettiWorker,
    [Description("Jolly Worker")]
    JollyWorker,
    [Description("Rat Worker")]
    RatWorker,
    [Description("Kitty Employee")]
    KittyEmployee,
    [Description("Flashy Lady")]
    FlashyLady // to change | also adding more when names finalized
}

public static class EnumExtensions
{
    public static string GetParsedName(this Enum value)
    {
        var attribute = Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()), typeof(DescriptionAttribute)) as DescriptionAttribute;

        return attribute?.Description ?? value.ToString();
    }
}