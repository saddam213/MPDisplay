using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;
using GUISkinFramework.Editors;
using MPDisplay.Common.Controls.PropertyGrid;

// ReSharper disable InconsistentNaming

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlInclude(typeof(XmlButton))]
    [XmlInclude(typeof(XmlGroup))]
    [XmlInclude(typeof(XmlLabel))]
    [XmlInclude(typeof(XmlImage))]
    [XmlInclude(typeof(XmlProgressBar))]
    [XmlInclude(typeof(XmlGuide))]
    [XmlInclude(typeof(XmlList))]
    [XmlInclude(typeof(XmlEqualizer))]
    [XmlInclude(typeof(XmlRectangle))]
    [XmlInclude(typeof(XmlBrush))]
    [XmlInclude(typeof(XmlAnimation))]
    [XmlType(TypeName = "Control")]
    public class XmlControl : INotifyPropertyChanged
    {
        private string _name;
        private int _id;
        private string _description;
        private int _width;
        private int _height;
        private int _posX;
        private int _posY;
        private int _posZ;
        private int _center3DZ;
        private int _center3DY;
        private int _center3DX;
        private int _pos3DZ;
        private int _pos3DY;
        private int _pos3DX;
        private ObservableCollection<XmlAnimation> _animations = new ObservableCollection<XmlAnimation>();
        private ObservableCollection<int> _mediaPortalFocusControls = new ObservableCollection<int>();
        private bool _designerVisible = true;

        public XmlControl()
        {
            this.SetDefaultValues();
        }

        [XmlIgnore]
        [Browsable(false)]
        public int WindowId { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public string DisplayName => Name;

        [XmlIgnore]
        [Browsable(false)]
        public bool DesignerVisible {
            get { return _designerVisible; }
            set { _designerVisible = value; NotifyPropertyChanged("DesignerVisible"); }
        }

        [PropertyOrder(0)]
        [EditorCategory("Control", 0)]
        public string Name
        {
            get { return _name; }
            set { _name = value; NotifyPropertyChanged("Name"); }
        }

        [PropertyOrder(1)]
        [EditorCategory("Control", 0)]
        public int Id
        {
            get { return _id; }
            set { _id = value; NotifyPropertyChanged("Id"); }
        }
        
        [PropertyOrder(2)]
        [EditorCategory("Control", 0)]
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
        [XmlElement(ElementName = "VisibleCondition")]
        public string XmlVisibleCondition { get; set; }
        

        [DefaultValue(true)]
        [PropertyOrder(20)]
        [EditorCategory("Visibility", 9)]
        public bool IsWindowOpenVisible { get; set; }

  


        [XmlArray(ElementName = "Animations")]
        [Editor(typeof(AnimationEditor), typeof(ITypeEditor))]
        public ObservableCollection<XmlAnimation> Animations
        {
            get { return _animations; }
            set { _animations = value; NotifyPropertyChanged("Animations"); }
        }

        [XmlArray(ElementName = "MediaPortalFocusControls")]
        [XmlArrayItem(Type=typeof(int), ElementName="MediaPortalControlId")]
        public ObservableCollection<int> MediaPortalFocusControls
        {
            get { return _mediaPortalFocusControls; }
            set { _mediaPortalFocusControls = value; NotifyPropertyChanged("MediaPortalFocusControls"); }
        }

        public virtual void ApplyStyle(XmlStyleCollection style)
        {
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
      
   

        public virtual void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
