using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GUIFramework.GUI
{
    public interface IControlHost
    {
        int Id { get; set; }

        List<GUIControl> Controls { get; set; }

        void CreateControls();
    }
}
