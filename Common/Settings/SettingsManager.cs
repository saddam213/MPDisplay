using Common.Helpers;

namespace Common.Settings
{
    public static class SettingsManager
    {
        /// <summary>
        /// Saves the specified settings object.
        /// </summary>
        /// <typeparam name="T">the settings object type</typeparam>
        /// <param name="obj">The settings obj.</param>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public static bool Save<T>(T obj, string filename) where T : SettingsBase
        {
            return SerializationHelper.Serialize(obj, filename);
        }

        /// <summary>
        /// Loads the specified settings object from file.
        /// </summary>
        /// <typeparam name="T">the settings object type</typeparam>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public static T Load<T>(string filename) where T : SettingsBase
        {
            return SerializationHelper.Deserialize<T>(filename);
        }
    }
}
