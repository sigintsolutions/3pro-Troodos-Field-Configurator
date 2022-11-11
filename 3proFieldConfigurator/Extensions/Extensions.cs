using System.ComponentModel;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace _3proFieldConfigurator.Extensions;

public static class Extensions
{
    public static void InvokeIfRequired(this ISynchronizeInvoke obj, MethodInvoker action)
    {
        // From http://stackoverflow.com/questions/2367718/automating-the-invokerequired-code-pattern
        if (obj.InvokeRequired)
        {
            obj.Invoke(action, Array.Empty<object>());
        }
        else
        {
            action();
        }
    }

    //This is a extension class of enum
    public static string? GetEnumDisplayName(this Enum enumType)
    {
        return enumType.GetType().GetMember(enumType.ToString())
                       .First()
                       .GetCustomAttribute<DisplayAttribute>()
                       .Name ?? string.Empty;
    }

    //public static int ToInt(this Enum enumType)
    //{
    //    return Convert.ToInt32(enumType);
    //}

    public static string ValueAsString(this Enum enumType)
    {
        return enumType.ToString("D");
    }

    public static void VisibleAndEnable(this ToolStripMenuItem toolStripItem, bool status)
    {
        toolStripItem.Visible = status;
        toolStripItem.Enabled = status;
    }

    public static string BooleanValuesToZeroOrOneAsString(this bool value)
    {
        return Convert.ToInt32(value).ToString();
    }

    public static void VisibleAndEnable(this Control control, bool status)
    {
        control.Visible = status;
        control.Enabled = status;
    }

    public static void EnabledAndChecked(this CheckBox ckbox, bool status)
    {
        ckbox.Checked = status;
        ckbox.Enabled = status;
    }

    public static void VisibledEnabledAndChecked(this CheckBox ckbox, bool status)
    {
        ckbox.Checked = status;
        ckbox.Enabled = status;
        ckbox.Visible = status;
    }

    public static string FirstCharToUpper(this string input) =>
    input switch
    {
        null => throw new ArgumentNullException(nameof(input)),
        "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
        _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
    };

    public static void UpdateIndexColumnNumbering(this DataGridView dgv)
    {
        foreach (DataGridViewRow row in dgv.Rows)
        {
            row.HeaderCell.Value = string.Format("{0}", row.Index + 1);
        }
    }

    public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
    {
        foreach (var i in ie)
        {
            action(i);
        }
    }

    public static void SetNumLimits(this NumericUpDown num, int digits, decimal min, decimal max)
    {
        num.Minimum = min;
        num.Maximum = max;
        num.DecimalPlaces = digits;
    }

    public static long ToUnixTime(this DateTime dt)
    {
        return ((DateTimeOffset)dt).ToUnixTimeSeconds();
    }

    public static long ToUnixTime_ms(this DateTime dt)
    {
        return ((DateTimeOffset)dt).ToUnixTimeMilliseconds();
    }
}
