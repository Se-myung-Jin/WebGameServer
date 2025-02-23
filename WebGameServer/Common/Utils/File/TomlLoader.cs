namespace Common;

public static class TomlLoader
{
    public static T Load<T>(string fileName) where T : class, new()
    {
        var findFile = Path.GetFileName(fileName);
        var directory = Path.GetDirectoryName(fileName);

        var findPath = FileUtils.SearchFromApplicationDirectory(findFile, directory);
        if (string.IsNullOrWhiteSpace(findPath))
        {
            throw new Exception($"TomlLoader File Not Found.");
        }

        try
        {
            return Tomlet.TomletMain.To<T>(File.ReadAllText(findPath));
        }
        catch (Exception ex)
        {

        }

        return null;
    }
}
