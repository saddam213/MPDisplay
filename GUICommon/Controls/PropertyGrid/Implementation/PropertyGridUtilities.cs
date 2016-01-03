using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    internal class PropertyGridUtilities
    {
        internal static T GetAttribute<T>(PropertyDescriptor property) where T : Attribute
        {
            return property.Attributes.OfType<T>().FirstOrDefault();
        }

        internal static PropertyItemCollection GetAlphabetizedProperties(List<PropertyItem> propertyItems)
        {
            var propertyCollection = new PropertyItemCollection(propertyItems);
            propertyCollection.SortBy("DisplayName", ListSortDirection.Ascending);
            return propertyCollection;
        }

        internal static PropertyItemCollection GetCategorizedProperties(List<PropertyItem> propertyItems)
        {
            var propertyCollection = new PropertyItemCollection(propertyItems);
            propertyCollection.GroupBy("Category");
            propertyCollection.SortBy("CategoryOrder", ListSortDirection.Ascending);
            propertyCollection.SortBy("PropertyOrder", ListSortDirection.Ascending);
            propertyCollection.SortBy("DisplayName", ListSortDirection.Ascending);
            return propertyCollection;
        }


        internal static PropertyDescriptorCollection GetPropertyDescriptors(object instance)
        {
            PropertyDescriptorCollection descriptors;

            var tc = TypeDescriptor.GetConverter(instance);
            if (!tc.GetPropertiesSupported())
            {
                var descriptor = instance as ICustomTypeDescriptor;
                descriptors = descriptor != null ? descriptor.GetProperties() : TypeDescriptor.GetProperties(instance.GetType());
            }
            else
            {
                descriptors = tc.GetProperties(instance);
            }

            return descriptors;
        }

        internal static PropertyItem CreatePropertyItem(PropertyDescriptor property, object instance, PropertyGrid grid, string bindingPath, int level)
        {
            var item = CreatePropertyItem(property, instance, grid, bindingPath);
            item.Level = level;
            return item;
        }

        internal static PropertyItem CreatePropertyItem(PropertyDescriptor property, object instance, PropertyGrid grid, string bindingPath)
        {
            var propertyItem = new PropertyItem(instance, property, grid, bindingPath);

            var binding = new Binding(bindingPath)
            {
                Source = instance,
                ValidatesOnExceptions = true,
                ValidatesOnDataErrors = true,
                Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay
            };
            propertyItem.SetBinding(PropertyItem.ValueProperty, binding);

            propertyItem.Editor = GetTypeEditor(propertyItem, grid.EditorDefinitions);

            return propertyItem;
        }

        internal static FrameworkElement GetTypeEditor(PropertyItem propertyItem, EditorDefinitionCollection editorDefinitions)
        {
            //first check for an attribute editor
            //now look for a custom editor based on editor definitions

            //guess we have to use the default editor
            var editor = (GetAttibuteEditor(propertyItem) ?? GetCustomEditor(propertyItem, editorDefinitions)) ?? CreateDefaultEditor(propertyItem);

            return editor;
        }

        internal static FrameworkElement GetAttibuteEditor(PropertyItem propertyItem)
        {
            FrameworkElement editor = null;

            var itemsSourceAttribute = GetAttribute<ItemsSourceAttribute>(propertyItem.PropertyDescriptor);
            if (itemsSourceAttribute != null)
                editor = new ItemsSourceAttributeEditor(itemsSourceAttribute).ResolveEditor(propertyItem);

            var editorAttribute = GetAttribute<EditorAttribute>(propertyItem.PropertyDescriptor);
            if (editorAttribute == null) return editor;

            var type = Type.GetType(editorAttribute.EditorTypeName);
            if (type == null) return editor;

            var instance = Activator.CreateInstance(type);
            if (instance is ITypeEditor) editor = (instance as ITypeEditor).ResolveEditor(propertyItem);

            return editor;
        }

        internal static FrameworkElement GetCustomEditor(PropertyItem propertyItem, EditorDefinitionCollection customTypeEditors)
        {
            FrameworkElement editor = null;

            //check for custom editor
            if (customTypeEditors.Count <= 0) return null;

            //first check if the custom editor is type based
            var customEditor = customTypeEditors[propertyItem.PropertyType] ?? customTypeEditors[propertyItem.Name];

            if (customEditor == null) return null;
            if (customEditor.EditorTemplate != null)
                editor = customEditor.EditorTemplate.LoadContent() as FrameworkElement;

            return editor;
        }

        internal static FrameworkElement CreateDefaultEditor(PropertyItem propertyItem)
        {
            ITypeEditor editor;

            if (propertyItem.IsReadOnly)
                editor = new TextBlockEditor();
            else if (propertyItem.PropertyType == typeof(bool) || propertyItem.PropertyType == typeof(bool?))
                editor = new CheckBoxEditor();
            else if (propertyItem.PropertyType == typeof(decimal) || propertyItem.PropertyType == typeof(decimal?))
                editor = new DecimalUpDownEditor();
            else if (propertyItem.PropertyType == typeof(double) || propertyItem.PropertyType == typeof(double?))
                editor = new DoubleUpDownEditor();
            else if (propertyItem.PropertyType == typeof(int) || propertyItem.PropertyType == typeof(int?))
                editor = new IntegerUpDownEditor();
            else if (propertyItem.PropertyType == typeof(Color))
                editor = new ColorEditor();
            else if (propertyItem.PropertyType.IsEnum)
                editor = new EnumComboBoxEditor();
            else if (propertyItem.PropertyType == typeof(FontFamily) || propertyItem.PropertyType == typeof(FontWeight) || propertyItem.PropertyType == typeof(FontStyle) || propertyItem.PropertyType == typeof(FontStretch))
                editor = new FontComboBoxEditor();
            else if (propertyItem.PropertyType.IsGenericType)
            {
                if (propertyItem.PropertyType.GetInterface("IList") != null)
                {
                    var t = propertyItem.PropertyType.GetGenericArguments()[0];
                    if (!t.IsPrimitive && !(t == typeof(String)))
                        editor = new CollectionEditor();
                        //editor = new MPDisplay.Common.Controls.PropertyGrid.PrimitiveTypeCollectionEditor();
                    else
                        editor = new PrimitiveTypeCollectionEditor();
                }
                else
                    editor = new TextBlockEditor();
            }
            else
                editor = new TextBoxEditor();

            return editor.ResolveEditor(propertyItem);
        }
    }
}
