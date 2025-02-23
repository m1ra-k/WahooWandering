using System;
using System.ComponentModel;

public enum CharacterEnum
{
    [Description("Joy")]
    MainCharacter,
    [Description("Room 53 Girl")]
    Room53Resident,
    [Description("Room 19 Girl")]
    Room19Resident,
    [Description("Room 07 Girl")]
    Room07Resident,
    [Description("Overworked Baker")]
    BodosWorker,
    [Description("Trolley Fanatic")]
    DecadesWorker,
    [Description("Fashionable Singer")]
    BandMember1,
    [Description("Emo Drummer")]
    BandMember2,
    [Description("Masked Guitarist")]
    BandMember3
}

public static class EnumExtensions
{
    public static string GetParsedName(this Enum value)
    {
        var attribute = Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()), typeof(DescriptionAttribute)) as DescriptionAttribute;

        return attribute?.Description ?? value.ToString();
    }
}