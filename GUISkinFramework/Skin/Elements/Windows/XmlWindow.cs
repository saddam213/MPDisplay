using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;
using GUISkinFramework.Editors;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlInclude(typeof(XmlMPDWindow))]
    [XmlInclude(typeof(XmlMPDDialog))]
    [XmlInclude(typeof(XmlMPWindow))]
    [XmlInclude(typeof(XmlPlayerWindow))]
    [XmlInclude(typeof(XmlMPDialog))]
    [XmlInclude(typeof(XmlControl))]
    [XmlType("Window")]
    public class XmlWindow : IXmlControlHost, INotifyPropertyChanged
    {
        private string _name;
        private int _id;
        private string _description;
        private XmlBrush _backgroundBrush;
        private int _defaultMediaPortalFocusedControlId;
        private bool _designerVisible = true;
        private bool _disableSlideshow;
        private bool _disableDarkening;

        public XmlWindow()
        {
            this.SetDefaultValues();
        }

        [XmlIgnore]
        [Browsable(false)]
        public string DisplayName => Name;

        [XmlIgnore]
        [Browsable(false)]
        public virtual string DisplayType => "Window";

        [XmlIgnore]
        [Browsable(false)]
        public bool DesignerVisible
        {
            get { return _designerVisible; }
            set { _designerVisible = value; NotifyPropertyChanged("DesignerVisible"); }
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

        [PropertyOrder(11)]
        [DefaultValue(false)]
        [EditorCategory("ScreenSaver", 1)]
        public bool DisableSlideshow
        {
            get { return _disableSlideshow; }
            set { _disableSlideshow = value; NotifyPropertyChanged("DisableSlideshow"); }
        }

        [PropertyOrder(12)]
        [DefaultValue(false)]
        [EditorCategory("ScreenSaver", 1)]
        public bool DisableDarkening
        {
            get { return _disableDarkening; }
            set { _disableDarkening = value; NotifyPropertyChanged("DisableDarkening"); }
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

        [DefaultValue(-1)]
        public int DefaultMediaPortalFocusedControlId
        {
            get { return _defaultMediaPortalFocusedControlId; }
            set { _defaultMediaPortalFocusedControlId = value; NotifyPropertyChanged("DefaultMediaPortalFocusedControlId"); }
        }


        public virtual void ApplyStyle(XmlStyleCollection style)
        {
            BackgroundBrush = style.GetStyle(BackgroundBrush);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
