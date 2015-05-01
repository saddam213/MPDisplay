using System;
using System.Collections.Generic;
using Common.Helpers;
using Common.Settings;
using GUISkinFramework.Skin;

namespace GUIFramework.Repositories
{
    public interface IRepository
    {
        GUISettings Settings { get; set; }
        XmlSkinInfo SkinInfo { get; set; }
        void Initialize(GUISettings settings, XmlSkinInfo skininfo);
        void ClearRepository();
        void ResetRepository();
    }




    public class DataRepository<TK, TV>
    {
        private Dictionary<TK, TV> _repository = new Dictionary<TK, TV>();
        private Func<TV, TV, bool> _valueEquals;

        public DataRepository()
        {
        }

        public DataRepository(Func<TV, TV, bool> valueEquals)
        {
            _valueEquals = valueEquals;
        }

        public bool AddOrUpdate(TK key, TV value)
        {
            lock (_repository)
            {
            if (key != null)
            {
                TV exists;
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

        public TV GetValue(TK key)
        {
            lock (_repository)
            {
                return _repository.GetValueOrDefault(key, default(TV));
            }
        }

        public TV GetValueOrDefault(TK key, TV defaultValue)
        {
            lock (_repository)
            {
                TV exists;
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
