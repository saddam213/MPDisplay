 using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GUIFramework.Managers;

namespace GUIFramework.GUI
{
    public class GUIVisibleCondition
    {
        private Func<bool> _condition = null;
        private string _xmlString = string.Empty;

        public GUIVisibleCondition(GUIControl control)
        {
            _xmlString = control.BaseXml.VisibleCondition;
            _condition = GUIVisibilityManager.CreateVisibleCondition(control, _xmlString);
        }

        public GUIVisibleCondition(GUIWindow window)
        {
            _xmlString = window.BaseXml.VisibleCondition;
            _condition = GUIVisibilityManager.CreateVisibleCondition(window, _xmlString);
        }

        public GUIVisibleCondition(GUIDialog dialog)
        {
            _xmlString = dialog.BaseXml.VisibleCondition;
            _condition = GUIVisibilityManager.CreateVisibleCondition(dialog, _xmlString);
        }

        public string XmlString
        {
            get { return _xmlString; }
        }

        public bool HasCondition
        {
            get { return _condition != null; }
        }

        public bool ShouldBeVisible()
        {
            if (HasCondition)
            {
                return _condition();
            }
            return true;
        }

        public override string ToString()
        {
            return XmlString;
        }
    }
}
