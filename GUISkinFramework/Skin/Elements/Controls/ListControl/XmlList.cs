﻿using System;
using System.ComponentModel;
using System.Xml.Serialization;
using GUISkinFramework.Editors;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "List")]
  
    public class XmlList : XmlControl
    {
        private XmlListStyle _controlStyle;
        private XmlListLayout _listLayout;
        private XmlListItemStyle _horizontalItemStyle;
        private XmlListItemStyle _verticalItemStyle;
        private XmlListItemStyle _verticalIconItemStyle;
        private XmlListItemStyle _coverFlowItemStyle;
        private XmlListType _listType = XmlListType.None;

        [XmlElement("ListControlStyle")]
        [DefaultValue(null)]
        [PropertyOrder(100)]
        [EditorCategory("Appearance", 6)]
        [DisplayName("Style")]
        [Editor(typeof(StyleEditor), typeof(ITypeEditor))]
        public XmlListStyle ControlStyle
        {
            get { return _controlStyle; }
            set { _controlStyle = value;  NotifyPropertyChanged("ControlStyle"); }
        }

        [DefaultValue(XmlListType.None)]
        [PropertyOrder(99)]
        [EditorCategory("List", 7)]
        [DisplayName("List Type")]
        public XmlListType ListType
        {
            get { return _listType; }
            set { _listType = value; NotifyPropertyChanged("ListType"); }
        }
        
        [DefaultValue(null)]
        [PropertyOrder(100)]
        [EditorCategory("List", 7)]
        [DisplayName("Layout")]
        public XmlListLayout ListLayout
        {
            get { return _listLayout; }
            set { _listLayout = value; NotifyPropertyChanged("ListLayout"); }
        }

        [XmlElement("VerticalItemStyle")]
        [DefaultValue(null)]
        [PropertyOrder(1)]
        [EditorCategory("ListItems", 8)]
        [DisplayName("Vertical Style")]
        [Editor(typeof(StyleEditor), typeof(ITypeEditor))]
        public XmlListItemStyle VerticalItemStyle
        {
            get { return _verticalItemStyle; }
            set { _verticalItemStyle = value; NotifyPropertyChanged("VerticalItemStyle"); }
        }

        [XmlElement("VerticalIconItemStyle")]
        [DefaultValue(null)]
        [PropertyOrder(2)]
        [EditorCategory("ListItems", 8)]
        [DisplayName("VerticalIcon Style")]
        [Editor(typeof(StyleEditor), typeof(ITypeEditor))]
        public XmlListItemStyle VerticalIconItemStyle
        {
            get { return _verticalIconItemStyle; }
            set { _verticalIconItemStyle = value; NotifyPropertyChanged("VerticalIconItemStyle"); }
        }

        [XmlElement("HorizontalItemStyle")]
        [DefaultValue(null)]
        [PropertyOrder(3)]
        [EditorCategory("ListItems", 8)]
        [DisplayName("Horizontal Style")]
        [Editor(typeof(StyleEditor), typeof(ITypeEditor))]
        public XmlListItemStyle HorizontalItemStyle
        {
            get { return _horizontalItemStyle; }
            set { _horizontalItemStyle = value; NotifyPropertyChanged("HorizontalItemStyle"); }
        }

        [XmlElement("CoverFlowItemStyle")]
        [DefaultValue(null)]
        [PropertyOrder(4)]
        [EditorCategory("ListItems", 8)]
        [DisplayName("CoverFlow Style")]
        [Editor(typeof(StyleEditor), typeof(ITypeEditor))]
        public XmlListItemStyle CoverFlowItemStyle
        {
            get { return _coverFlowItemStyle; }
            set { _coverFlowItemStyle = value; NotifyPropertyChanged("CoverFlowItemStyle"); }
        }

        public override void ApplyStyle(XmlStyleCollection style)
        {
            base.ApplyStyle(style);
            ControlStyle = style.GetControlStyle(ControlStyle);
            VerticalItemStyle = style.GetControlStyle(VerticalItemStyle);
            VerticalIconItemStyle = style.GetControlStyle(VerticalIconItemStyle);
            HorizontalItemStyle = style.GetControlStyle(HorizontalItemStyle);
            CoverFlowItemStyle = style.GetControlStyle(CoverFlowItemStyle);
        }
    }

    public enum XmlListLayout
    {
        Auto = 0,
        Vertical = 1,
        Horizontal = 2,
        CoverFlow = 3,
        VerticalIcon = 4
    }
}
