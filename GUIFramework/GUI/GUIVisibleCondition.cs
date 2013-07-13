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
        private MethodInfo _condition = null;
        private string _xmlString = string.Empty;

        public GUIVisibleCondition(int windowId, string xmlString)
        {
            _condition = GUIVisibilityManager.CreateVisibleCondition(windowId, xmlString);
        }

        public string XmlString
        {
            get { return _xmlString; }
        }

        public bool HasCondition
        {
            get { return _condition != null; }
        }

        public bool ShouldBeVisible(bool currentVisibility)
        {
            if (HasCondition)
            {
                return (bool)_condition.Invoke(null, null);
            }
            return currentVisibility;
        }

        public override string ToString()
        {
            return XmlString;
        }
    }
}
