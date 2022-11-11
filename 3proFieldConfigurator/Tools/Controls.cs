using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3proFieldConfigurator.Tools;

public static class Control
{
    public static IEnumerable<System.Windows.Forms.Control> GetAllControlsOfSpecificType(System.Windows.Forms.Control control, Type type)
    {
        var controls = control.Controls.Cast<System.Windows.Forms.Control>();
        return controls.SelectMany(ctrls => GetAllControlsOfSpecificType(ctrls, type)).
            Concat(controls).Where(c => c.GetType() == type);
    }
}
