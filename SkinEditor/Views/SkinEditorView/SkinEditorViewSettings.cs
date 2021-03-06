﻿using System;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using Common.Helpers;
using GUISkinFramework.Skin;
using SkinEditor.Themes;

namespace SkinEditor.Views
{
    public class SkinEditorViewSettings : EditorViewModelSettings
    {
        private bool _showGrid;
        private int _gridSize = 10;
        private string _gridColor = "Black";
        private bool _snapToGrid;
        private string _resizeSizeInfoColor = "Red";
        private string _resizeHiglightColor = "#FF0074FF";
        private bool _dragMoveResize = true;

        [XmlIgnore]
        public XmlStyleCollection DesignerStyle { get; set; } = new XmlStyleCollection();

        public override void InitializeSettings()
        {
            base.InitializeSettings();
            if (!File.Exists(Environment.CurrentDirectory + "\\DesignerStyle.xml")) return;

            var styleCollection = SerializationHelper.Deserialize<XmlStyleCollection>(Environment.CurrentDirectory + "\\DesignerStyle.xml");
            if (styleCollection == null) return;

            DesignerStyle = styleCollection;
            DesignerStyle.InitializeStyleCollection();
        }


        public bool ShowGrid
        {
            get { return _showGrid; }
            set { _showGrid = value; NotifyPropertyChanged("ShowGrid"); }
        }

        public int GridSize
        {
            get { return _gridSize; }
            set { _gridSize = value; NotifyPropertyChanged("GridSize"); NotifyPropertyChanged("GridSizeRect"); }
        }

        public string GridColor
        {
            get { return _gridColor; }
            set { _gridColor = value; NotifyPropertyChanged("GridColor"); }
        }

        public bool SnapToGrid
        {
            get { return _snapToGrid; }
            set { _snapToGrid = value; NotifyPropertyChanged("SnapToGrid"); }
        }




        public bool DragMoveResize
        {
            get { return _dragMoveResize; }
            set { _dragMoveResize = value; NotifyPropertyChanged("DragMoveResize"); }
        }


        public string ResizeSizeInfoColor
        {
            get { return _resizeSizeInfoColor; }
            set { _resizeSizeInfoColor = value; NotifyPropertyChanged("ResizeSizeInfoColor"); }
        }



        public string ResizeHiglightColor
        {
            get { return _resizeHiglightColor; }
            set { _resizeHiglightColor = value; NotifyPropertyChanged("ResizeHiglightColor"); }
        }

        public Rect GridSizeRect => new Rect(0, 0, Math.Max(GridSize, 0), Math.Max(GridSize, 0));
    }
}
