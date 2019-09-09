//----------------------------------------------
// Friday Engine 2015-2019 Fu Cong QQ: 353204643
//----------------------------------------------

using System.IO;

namespace F2DEngine
{
    public static class FEPath
    {
        private static bool m_IsWindosType;
        static FEPath()
        {
            var t = Path.GetDirectoryName("FridayEngine/Path/Test");
            m_IsWindosType = t.IndexOf("\\") != -1;
            
        }

        public static void Delete(string path)
        {
            Directory.Delete(path, true);
        }

        public static bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        public static string[] GetDirectories(string path)
        {
            var files = Directory.GetDirectories(path);
            return AutoStandardPaths(files);
        }

        private static string[] AutoStandardPaths(string[] files)
        {
            if (m_IsWindosType)
            {
                return StandardPaths(files);
            }
            return files;
        }

        public  static string[] StandardPaths(string[]files)
        {
            if (files != null)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    files[i] = StandardPath(files[i]);
                }
            }
            return files;
        }

        public static string GetFileName(string name)
        {
            return Path.GetFileName(name);
        }

        private static string AutoStandardPath(string path)
        {
            if(m_IsWindosType)
            {
                return StandardPath(path);
            }
            return path;
        }

        public static string StandardPath(string path)
        {
            if (path != null)
            {
                return path.Replace("\\", "/");
            }
            return path;
        }

        public static string GetDirectoryName(string path)
        {
            return StandardPath(Path.GetDirectoryName(path));
        }

        public static string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public static DirectoryInfo CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                return Directory.CreateDirectory(path);
            }
            return null;
        }


        public static string[] GetFiles(string path, string searchPattern = null)
        {
            string[] files = null;
            if(string.IsNullOrEmpty(searchPattern))
            {
                files = Directory.GetFiles(path);
            }
            else
            {
                files = Directory.GetFiles(path, searchPattern);
            }
            return AutoStandardPaths(files);
        }

        public static string[] GetFileSystemEntries(string path)
        {
            string[] files = Directory.GetFileSystemEntries(path);
            return AutoStandardPaths(files);
        }

        public static string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }
    }
}
