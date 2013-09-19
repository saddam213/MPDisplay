using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GUIFramework.GUI
{
    public class GUISurfaceElement : ContentControl, IControlHost, INotifyPropertyChanged
    {
        public int Id { get; set; }

        public bool IsOpen
        {
            get { return Visibility == System.Windows.Visibility.Visible; }
        }

        private List<GUIControl> _controls = new List<GUIControl>();

        public List<GUIControl> Controls
        {
            get { return _controls; }
            set { _controls = value; NotifyPropertyChanged(); }
        }
        

        public virtual void CreateControls()
        {
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName]string property = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
