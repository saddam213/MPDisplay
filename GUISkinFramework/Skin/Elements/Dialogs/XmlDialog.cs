using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;
using GUISkinFramework.Editors;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlInclude(typeof(XmlMPDialog))]
    [XmlInclude(typeof(XmlMPDDialog))]
    [XmlInclude(typeof(XmlControl))]
    [XmlType("Dialog")]
    public class XmlDialog : IXmlControlHost, INotifyPropertyChanged
    {
        private string _name;
        private int _id;
        private string _description;
        private int _width;
        private int _height;
        private int _posX;
        private int _posY;
        private int _posZ;
        private int _pos3DX;
        private int _pos3DY;
        private int _pos3DZ;
        private int _center3DX;
        private int _center3DY;
        private int _center3DZ;
        private string _borderThickness;
        private string _borderCornerRadius;
        private XmlBrush _borderBrush;
        private XmlBrush _backgroundBrush;
        private bool _designerVisible = true;

        public XmlDialog()
        {
            this.SetDefaultValues();
        }


        [XmlIgnore]
        [Browsable(false)]
        public string DisplayName => Name;

        [XmlIgnore]
        [Browsable(false)]
        public virtual string DisplayType => "Dialog";

        [XmlIgnore]
        [Browsable(false)]
        public bool DesignerVisible
        {
            get { return _designerVisible; }
            set { _designerVisible = value; NotifyPropertyChanged("DesignerVisible"); }
        }

        [PropertyOrder(0)]
        [EditorCategory("Dialog", 0)]
        public string Name
        {
            get { return _name; }
            set { _name = value; NotifyPropertyChanged("Name"); NotifyPropertyChanged("DisplayName"); }
        }


        [PropertyOrder(1)]
        [EditorCategory("Dialog", 0)]
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
        [EditorCategory("Dialog", 0)]
        public string Description
        {
            get { return _description; }
            set { _description = value; NotifyPropertyChanged("Description"); }
        }



        [DefaultValue(0)]
        [PropertyOrder(20)]
        [EditorCategory("Layout", 2)]
     
        public int Width
        {
            get { return _width; }
            set { _width = value; NotifyPropertyChanged("Width"); }
        }

        [DefaultValue(0)]
        [PropertyOrder(21)]
        [EditorCategory("Layout", 2)]
       
        public int Height
        {
            get { return _height; }
            set { _height = value; NotifyPropertyChanged("Height"); }
        }

        [DefaultValue(0)]
        [PropertyOrder(22)]
        [EditorCategory("Layout", 2)]
      
        public int PosX
        {
            get { return _posX; }
            set { _posX = value; NotifyPropertyChanged("PosX"); }
        }

        [DefaultValue(0)]
        [PropertyOrder(23)]
        [EditorCategory("Layout", 2)]
      
        public int PosY
        {
            get { return _posY; }
            set { _posY = value; NotifyPropertyChanged("PosY"); }
        }

        [DefaultValue(0)]
        [PropertyOrder(24)]
        [EditorCategory("Layout", 2)]
        [Browsable(false)]
        public int PosZ
        {
            get { return _posZ; }
            set { _posZ = value; NotifyPropertyChanged("PosZ"); }
        }

        [DefaultValue(0)]
        [PropertyOrder(25)]
        [EditorCategory("Layout", 2)]
        public int Pos3DX
        {
            get { return _pos3DX; }
            set { _pos3DX = value; NotifyPropertyChanged("Pos3DX"); }
        }

        [DefaultValue(0)]
        [PropertyOrder(26)]
        [EditorCategory("Layout", 2)]
        public int Pos3DY
        {
            get { return _pos3DY; }
            set { _pos3DY = value; NotifyPropertyChanged("Pos3DY"); }
        }

        [DefaultValue(0)]
        [PropertyOrder(27)]
        [EditorCategory("Layout", 2)]
        public int Pos3DZ
        {
            get { return _pos3DZ; }
            set { _pos3DZ = value; NotifyPropertyChanged("Pos3DZ"); }
        }

        [DefaultValue(0)]
        [PropertyOrder(28)]
        [EditorCategory("Layout", 2)]
        public int Center3DX
        {
            get { return _center3DX; }
            set { _center3DX = value; NotifyPropertyChanged("Center3DX"); }
        }

        [DefaultValue(0)]
        [PropertyOrder(29)]
        [EditorCategory("Layout", 2)]
        public int Center3DY
        {
            get { return _center3DY; }
            set { _center3DY = value; NotifyPropertyChanged("Center3DY"); }
        }

        [DefaultValue(0)]
        [PropertyOrder(30)]
        [EditorCategory("Layout", 2)]
        public int Center3DZ
        {
            get { return _center3DZ; }
            set { _center3DZ = value; NotifyPropertyChanged("Center3DZ"); }
        }


        [PropertyOrder(60)]
        [EditorCategory("Appearance", 3)]
        [DefaultValue("0,0,0,0")]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string BorderThickness
        {
            get { return _borderThickness; }
            set { _borderThickness = value; NotifyPropertyChanged("BorderThickness"); }
        }

        [PropertyOrder(61)]
        [EditorCategory("Appearance", 3)]
        [DefaultValue("0,0,0,0")]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string CornerRadius
        {
            get { return _borderCornerRadius; }
            set { _borderCornerRadius = value; NotifyPropertyChanged("CornerRadius"); }
        }

        [PropertyOrder(62)]
        [EditorCategory("Appearance", 3)]
        // [DefaultValue(XmlBrushStyle.Default)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush BackgroundBrush
        {
            get { return _backgroundBrush; }
            set { _backgroundBrush = value; NotifyPropertyChanged("BackgroundBrush"); }
        }

        [PropertyOrder(63)]
        [EditorCategory("Appearance", 3)]
        // [DefaultValue(XmlBrushStyle.Default)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush BorderBrush
        {
            get { return _borderBrush; }
            set { _borderBrush = value; NotifyPropertyChanged("BorderBrush"); }
        }














        [XmlArray(ElementName = "DialogAnimations")]
        [Editor(typeof(AnimationEditor), typeof(ITypeEditor))]
        public ObservableCollection<XmlAnimation> Animations { get; set; }

        [XmlArray(ElementName = "DialogControls")]
        [Browsable(false)]
        public ObservableCollection<XmlControl> Controls { get; set; }

        [XmlIgnore]
        [DefaultValue("")]
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
            BackgroundBrush = style.GetStyle(BackgroundBrush);
            BorderBrush = style.GetStyle(BorderBrush);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}