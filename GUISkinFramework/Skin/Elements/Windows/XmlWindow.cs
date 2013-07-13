using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GUISkinFramework.Animations;
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Controls;
using GUISkinFramework.Dialogs;
using GUISkinFramework.Editor.PropertyEditors;
using MPDisplay.Common.Controls.PropertyGrid.Attributes;
using MPDisplay.Common.Controls.PropertyGrid.Editors;
using GUISkinFramework.Skin;
using GUISkinFramework.Styles;

namespace GUISkinFramework.Windows
{
    [Serializable]
    [XmlInclude(typeof(XmlMPDWindow))]
    [XmlInclude(typeof(XmlMPDDialog))]
    [XmlInclude(typeof(XmlMPWindow))]
    [XmlInclude(typeof(XmlMPDialog))]
    [XmlInclude(typeof(XmlControl))]
    [XmlType("Window")]
    public class XmlWindow : IXmlControlHost, INotifyPropertyChanged
    {
        private string _name;
        private int _id;
        private string _description;
        //private int _pos3DX;
        //private int _pos3DY;
        //private int _pos3DZ;
        //private int _center3DX;
        //private int _center3DY;
        //private int _center3DZ;
        private XmlBrush _backgroundBrush;

        public XmlWindow()
        {
            this.SetDefaultValues();
        }

        [XmlIgnore]
        [Browsable(false)]
        public string DisplayName
        {
            get { return Name; }
        }

        [XmlIgnore]
        [Browsable(false)]
        public virtual string DisplayType
        {
            get { return "Window"; }
        }


        public bool IsDefault { get; set; }

      //  [XmlIgnore]
        [Browsable(false)]
        public int Width { get; set; }

       // [XmlIgnore]
        [Browsable(false)]
        public int Height { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public int PosX { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public int PosY { get; set; }



        [PropertyOrder(0)]
        [EditorCategory("Window", 0)]
        public string Name
        {
            get { return _name; }
            set { _name = value; NotifyPropertyChanged("Name"); NotifyPropertyChanged("DisplayName"); }
        }
        

        [PropertyOrder(1)]
        [EditorCategory("Window", 0)]
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                if (Controls != null)
                {
                    foreach (var control in Controls)
                    {
                        control.WindowId = _id;
                    }
                }
                NotifyPropertyChanged("Id");
            }
        }
        

        [PropertyOrder(2)]
        [EditorCategory("Window", 0)]
        public string Description
        {
            get { return _description; }
            set { _description = value; NotifyPropertyChanged("Description"); }
        }
        
        //[DefaultValue(0)]
        //[PropertyOrder(25)]
        //[EditorCategory("Layout", 2)]
        //public int Pos3DX
        //{
        //    get { return _pos3DX; }
        //    set { _pos3DX = value; NotifyPropertyChanged("Pos3DX"); }
        //}

        //[DefaultValue(0)]
        //[PropertyOrder(26)]
        //[EditorCategory("Layout", 2)]
        //public int Pos3DY
        //{
        //    get { return _pos3DY; }
        //    set { _pos3DY = value; NotifyPropertyChanged("Pos3DY"); }
        //}

        //[DefaultValue(0)]
        //[PropertyOrder(27)]
        //[EditorCategory("Layout", 2)]
        //public int Pos3DZ
        //{
        //    get { return _pos3DZ; }
        //    set { _pos3DZ = value; NotifyPropertyChanged("Pos3DZ"); }
        //}

        //[DefaultValue(0)]
        //[PropertyOrder(28)]
        //[EditorCategory("Layout", 2)]
        //public int Center3DX
        //{
        //    get { return _center3DX; }
        //    set { _center3DX = value; NotifyPropertyChanged("Center3DX"); }
        //}

        //[DefaultValue(0)]
        //[PropertyOrder(29)]
        //[EditorCategory("Layout", 2)]
        //public int Center3DY
        //{
        //    get { return _center3DY; }
        //    set { _center3DY = value; NotifyPropertyChanged("Center3DY"); }
        //}

        //[DefaultValue(0)]
        //[PropertyOrder(30)]
        //[EditorCategory("Layout", 2)]
        //public int Center3DZ
        //{
        //    get { return _center3DZ; }
        //    set { _center3DZ = value; NotifyPropertyChanged("Center3DZ"); }
        //}

        [PropertyOrder(62)]
        [EditorCategory("Appearance", 3)]
        // [DefaultValue(XmlBrushStyle.Default)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush BackgroundBrush
        {
            get { return _backgroundBrush; }
            set { _backgroundBrush = value; NotifyPropertyChanged("BackgroundBrush"); }
        }



      


        [XmlArray(ElementName = "WindowAnimations")]
        [Editor(typeof(AnimationEditor), typeof(ITypeEditor))]
        public ObservableCollection<XmlAnimation> Animations { get; set; }

        [Browsable(false)]
        [XmlArray(ElementName = "WindowControls")]
        public ObservableCollection<XmlControl> Controls  { get; set; }

        [XmlIgnore]
        [DefaultValue("")]
        [PropertyOrder(20)]
        [EditorCategory("Visibility", 9)]
        [Editor(typeof(VisibleConditionEditor), typeof(ITypeEditor))]
        public string VisibleCondition
        {
            get { return XmlVisibleCondition.Replace(" ++ ", " && "); }
            set { XmlVisibleCondition = value.Replace(" && ", " ++ "); }
        }
        [Browsable(false)]
        [DefaultValue("")]
        [XmlElement(ElementName = "VisibleCondition")]
        public string XmlVisibleCondition { get; set; }


        public virtual void ApplyStyle(XmlStyleCollection style)
        {
            BackgroundBrush = style.GetStyle<XmlBrush>(BackgroundBrush);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

      
    }
}
