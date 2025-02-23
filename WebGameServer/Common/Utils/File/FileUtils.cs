namespace Common;

public static class FileUtils
{
    public static string SearchParentDirectory(string filePath, int retryParentDirectory = 5)
    {
        if (File.Exists(filePath))
        {
            return filePath;
        }

        string dir = Path.GetDirectoryName(filePath);
        string fileName = Path.GetFileName(filePath);

        for (int i = 1; i <= retryParentDirectory; i++)
        {
            dir = Path.Combine(dir, "..");

            dir = Path.GetFullPath(dir);

            filePath = Path.Combine(dir, fileName);

            if (File.Exists(filePath))
            {
                return filePath;
            }
        }

        return null;
    }

    public static string GetDirectory(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return null;
        }

        int index = filePath.LastIndexOfAny(new char[] { '/', '\\' });
        if (index < 0)
        {
            return string.Empty;
        }

        return filePath.Substring(0, index);
    }

    public static string SearchFromApplicationDirectory(string fileName, string directory, int retryParentDirectory = 10)
    {
        var findDirectory = SearchDirectory(directory);

        return SearchParentDirectory(Path.Combine(findDirectory, fileName), retryParentDirectory);
    }

    public static string SearchParentDirectory(string fileName, string startDir, int retryParentDirectory = 5)
    {
        var path = Path.Combine(startDir, fileName);
        if (File.Exists(path))
        {
            return path;
        }

        string dir = startDir;

        for (int i = 1; i <= retryParentDirectory; i++)
        {
            dir = Path.Combine(dir, "..");
            path = Path.Combine(dir, fileName);

            if (File.Exists(path))
            {
                return path;
            }
        }

        return null;
    }

    public static string SearchDirectory(string directoryName, int retryParentDirectory = 5)
    {
        string dir = Path.GetDirectoryName(AppContext.BaseDirectory);

        int tryCount = 0;
        do
        {
            ++tryCount;

            var path = Path.Combine(dir, directoryName);
            if (Directory.Exists(path))
            {
                return path;
            }

            dir = Path.Combine(dir, "..");

        } while (tryCount < retryParentDirectory);

        return null;
    }
}
