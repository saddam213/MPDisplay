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

        public GUIVisibleCondition(int windowId, string xmlString)
        {
            _xmlString = xmlString;
           _condition = GUIVisibilityManager.CreateVisibleCondition(true, windowId, xmlString);
        }

        public GUIVisibleCondition(string xmlString)
        {
            _xmlString = xmlString;
            _condition = GUIVisibilityManager.CreateVisibleCondition(false, 0, xmlString);
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
