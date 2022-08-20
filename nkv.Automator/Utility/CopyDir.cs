using System.IO;

namespace nkv.Automator.Utility
{
    internal static class CopyDir
    {
        public static void Copy(string sourceDirectory, string targetDirectory, string projectName, string existingProjectName)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget, projectName, existingProjectName);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target, string projectName, string existingProjectName)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {

                string targetFilename = target.FullName;
                targetFilename = targetFilename.Replace(existingProjectName, projectName);
                string filename = fi.Name.Replace(existingProjectName, projectName);
                fi.CopyTo(Path.Combine(targetFilename, filename), true);
                string path = Path.Combine(targetFilename, filename);
                string contents = File.ReadAllText(path);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                using (var txtFile = File.AppendText(path))
                {
                    contents = contents.Replace(existingProjectName, projectName.Trim());
                    txtFile.WriteLine(contents);
                }
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                if (diSourceSubDir.Name != ".vs")
                {
                    string diSourceSubDirString = diSourceSubDir.Name.Replace(existingProjectName, projectName);
                    DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDirString);
                    CopyAll(diSourceSubDir, nextTargetSubDir, projectName, existingProjectName);
                }
            }
        }

    }
}
