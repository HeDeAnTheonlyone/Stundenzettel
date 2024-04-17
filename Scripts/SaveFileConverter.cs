using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


/// FIX This isn't used yet


public static class SaveFileConverter
{
    public static string Stringify(TimeSheet timeSheet)
    {

        


        return "";
    }



    private static string FileBuilder<T>(T obj, int indentation = 0)
    {
        var strBuilder = new StringBuilder();
        var propertyList = typeof(T).GetProperties();

        foreach (var property in propertyList)
        {
            if (typeof(IList).IsAssignableFrom(property.PropertyType))
                foreach (var prop in (Array)property.GetValue(obj))
                    FileBuilder(prop, indentation + 1);
            else
                strBuilder.AppendLine($"{Padding(indentation)}{nameof(property)}:{property.GetValue(obj)}");
        }
        
        return strBuilder.ToString();
    }



    private static string Padding(int space)
    {
        var str = new StringBuilder();

        for (int i = 0; i < space; i++)
            str.Append("\t");

        return str.ToString();
    }



    public static TimeSheet ParseString(string saveData)
    {
        return new TimeSheet(new DateOnly(), new List<TimeSpanEntry>());
    }
}