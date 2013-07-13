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

      

        private ObservableCollection<GUIControl> _controls = new ObservableCollection<GUIControl>();

        public ObservableCollection<GUIControl> Controls
        {
            get { return _controls; }
            set { _controls = value; NotifyPropertyChanged(); }
        }
        

        public virtual void CreateControls()
        {
        }

      

        public int FocusedControlId
        {
            get { return (int)GetValue(FocusedControlIdProperty); }
            set { SetValue(FocusedControlIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FocusedControlId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FocusedControlIdProperty =
            DependencyProperty.Register("FocusedControlId", typeof(int), typeof(GUISurfaceElement), new PropertyMetadata(0));


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
