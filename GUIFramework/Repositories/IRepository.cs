using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUISkinFramework;
using Common.Settings;
using Common;

namespace GUIFramework.Managers
{
    public interface IRepository
    {
        GUISettings Settings { get; set; }
        XmlSkinInfo SkinInfo { get; set; }
        void Initialize(GUISettings settings, XmlSkinInfo skininfo);
        void ClearRepository();
        void ResetRepository();
    }




    public class DataRepository<K, V>
    {
        private Dictionary<K, V> _repository = new Dictionary<K, V>();
        private Func<V, V, bool> _valueEquals;

        public DataRepository()
        {
        }

        public DataRepository(Func<V, V, bool> valueEquals)
        {
            this._valueEquals = valueEquals;
        }

        public bool AddOrUpdate(K key, V value)
        {
            lock (_repository)
            {
            if (key != null)
            {
                V exists;
                if (!_repository.TryGetValue(key, out exists))
                {
                  
                        _repository.Add(key, value);
                 
                    return true;
                }
                else
                {
                    if (_valueEquals == null || !_valueEquals(value, exists))
                    {
                     
                            _repository[key] = value;
                      
                        return true;
                    }
                }
            }
            return false;
        }
        }

        public V GetValue(K key)
        {
            return _repository.GetValueOrDefault(key, default(V));
        }

        public V GetValueOrDefault(K key, V defaultValue)
        {
            lock (_repository)
            {
                V exists;
                if (_repository.TryGetValue(key, out exists))
                {
                    return exists;
                }
                return defaultValue;
            }
        }

        public void ClearRepository()
        {
            lock (_repository)
            {
                _repository.Clear();
            }
        }
    }
}
