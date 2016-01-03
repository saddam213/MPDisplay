using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;

namespace SkinEditor.Helpers
{
    public static class FileSystemHelper
    {
        public static string OpenFolderDialog(string startDirectory)
        {
            var dialog = new FolderBrowserDialog();
            return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : string.Empty;
        }

        /// <summary>
        /// OpenFileDialog
        /// </summary>
        /// <param name="startDirectory">The start directory.</param>
        /// <param name="filter">The filter. Example: "Png Images (*.png)|*.png"</param>
        /// <returns></returns>
        public static string OpenFileDialog(string startDirectory, string filter)
        {
             // Filter Example = "Png Images (*.png)|*.png";
            var dialog = new OpenFileDialog
            {
                Filter = filter,
                RestoreDirectory = true,
                ShowReadOnly = true,
                InitialDirectory = startDirectory
            };
            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : string.Empty;
        }

        public static void TryDelete(string file)
        {
            try
            {
                File.Delete(file);
            }
            catch
            {
                // ignored
            }
        }

        public static void TryDelete(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                TryDelete(file);
            }
        }

        public static bool CreateDirectory(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
                return true;
            }
            catch
            {
                // ignored
            }
            return false;
        }

        public static bool DirecoryExists(string dir)
        {
            return Directory.Exists(dir);
        }

        public static void CreateIfNotExists(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public static IEnumerable<string> GetFiles(string dir, string extensions)
        {
            if (!Directory.Exists(dir)) yield break;

            foreach (var item in Directory.GetFiles(dir, extensions))
            {
                yield return item;
            }
        }

        public static bool IsValidFilePath(string value)
        {
            return !value.Any(c => Path.GetInvalidPathChars().Contains(c));
        }

        public static bool IsValidFileName(string value)
        {
            return !value.Any(c => Path.GetInvalidFileNameChars().Contains(c));
        }

        public static bool IsValidFullFileName(string value)
        {
            return !value.Any(c => Path.GetInvalidFileNameChars().Concat(Path.GetInvalidPathChars()).Contains(c));
        }
    }

    public class FilePathValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value != null && value.GetType() != typeof(string))
                return new ValidationResult(false, "Input value was of the wrong type, expected a text");
            var filePath = value as string;
            //check for empty/null file path:
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrWhiteSpace(filePath))
            {
                return !AllowEmptyPath ? new ValidationResult(false, $"The {Message} may not be empty.") : new ValidationResult(true, null);
            }

            if (IsFileNameOnly)
            {
                if (Path.GetInvalidFileNameChars().Any(x => filePath.Contains(x)))
                {
                    return new ValidationResult(false,
                        $"The characters {GetPrinatbleInvalidChars(Path.GetInvalidPathChars())} are not permitted in a {Message}.");
                }
            }
            else if (IsFilePathOnly)
            {
                if (Path.GetInvalidPathChars().Any(x => filePath.Contains(x)))
                    return new ValidationResult(false,
                        $"The characters {GetPrinatbleInvalidChars(Path.GetInvalidPathChars())} are not permitted in a {Message}.");
            }
            else
            {

                //null & empty has been handled above, now check for pure whitespace:
                if (string.IsNullOrWhiteSpace(filePath))
                    return new ValidationResult(false, $"The {Message} cannot consist only of whitespace.");
                //check the path:
                if (Path.GetInvalidPathChars().Any(x => filePath.Contains(x)))
                    return new ValidationResult(false,
                        $"The characters {GetPrinatbleInvalidChars(Path.GetInvalidPathChars())} are not permitted in a {Message}.");
                //check the filename (if one can be isolated out):
                try
                {
                    var fileName = Path.GetFileName(filePath);
                    if (Path.GetInvalidFileNameChars().Any(x => fileName.Contains(x)))
                        throw new ArgumentException(
                            $"The characters {GetPrinatbleInvalidChars(Path.GetInvalidFileNameChars())} are not permitted in a {Message}.");
                }
                catch (ArgumentException e) { return new ValidationResult(false, e.Message); }

            }
            return new ValidationResult(true, null);
        }
        /// <summary>
        /// Gets and sets a flag indicating whether an empty path forms an error condition or not.
        /// </summary>
        public bool AllowEmptyPath { get; set; }

        public string Message { get; set; } = "file path";

        public bool IsFileNameOnly { get; set; }

        public bool IsFilePathOnly { get; set; }
        

        private static string GetPrinatbleInvalidChars(IEnumerable<char> chars)
        {
            var invalidChars = string.Join("", chars.Where(x => !Char.IsWhiteSpace(x)));
            return invalidChars;
        }
    }

}
