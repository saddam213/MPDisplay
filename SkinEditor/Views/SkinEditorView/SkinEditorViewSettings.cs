using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using GUISkinFramework;
using GUISkinFramework.Styles;

namespace SkinEditor.Views
{
    public class SkinEditorViewSettings : EditorViewModelSettings
    {
        private bool _showGrid = false;
        private int _gridSize = 10;
        private string _gridColor = "Black";
        private bool _snapToGrid = false;
        private string _resizeSizeInfoColor = "Red";
        private string _resizeHiglightColor = "#FF0074FF";
        private bool _dragMoveResize = true;

        private XmlStyleCollection _designerStyle = new XmlStyleCollection();

        [XmlIgnore]
        public XmlStyleCollection DesignerStyle
        {
            get { return _designerStyle; }
            set { _designerStyle = value; }
        }

        public override void InitializeSettings()
        {
            base.InitializeSettings();
            if (File.Exists(Environment.CurrentDirectory + "\\DesignerStyle.xml"))
            {
                var styleCollection = XmlManager.Deserialize<XmlStyleCollection>(Environment.CurrentDirectory + "\\DesignerStyle.xml");
                if (styleCollection != null)
                {
                   DesignerStyle = styleCollection;
                   DesignerStyle.InitializeStyleCollection();
                }
            }
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



        public Rect GridSizeRect
        {
            get { return new Rect(0, 0, Math.Max(GridSize, 0), Math.Max(GridSize, 0)); }
        }
    }
}
