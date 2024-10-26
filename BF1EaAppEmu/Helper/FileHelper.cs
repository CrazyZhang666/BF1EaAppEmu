namespace BF1EaAppEmu.Helper;

public static class FileHelper
{
    /// <summary>
    /// 创建文件夹
    /// </summary>
    public static void CreateDirectory(string dirPath)
    {
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);
    }

    /// <summary>
    /// 获取嵌入资源流（自动添加前缀）
    /// </summary>
    public static Stream GetEmbeddedResourceStream(string resPath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        return assembly.GetManifestResourceStream($"BF1EaAppEmu.Assets.Files.{resPath}");
    }

    /// <summary>
    /// 获取嵌入资源文本内容
    /// </summary>
    public static string GetEmbeddedResourceText(string resPath)
    {
        var stream = GetEmbeddedResourceStream(resPath);
        if (stream is null)
            return string.Empty;

        return new StreamReader(stream).ReadToEnd();
    }

    /// <summary>
    /// 从资源文件中抽取资源文件（默认覆盖源文件）
    /// </summary>
    public static void ExtractResFile(string resPath, string outputPath, bool isOverride = true)
    {
        // 如果输出文件存在，并且不覆盖文件，则退出
        if (!isOverride && File.Exists(outputPath))
            return;

        var stream = GetEmbeddedResourceStream(resPath);
        if (stream is null)
            return;

        BufferedStream inStream = null;
        FileStream outStream = null;

        try
        {
            inStream = new BufferedStream(stream);
            outStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);

            var buffer = new byte[1024];
            int length;

            while ((length = inStream.Read(buffer, 0, buffer.Length)) > 0)
                outStream.Write(buffer, 0, length);

            outStream.Flush();

            LoggerHelper.Info($"释放资源文件成功 {outputPath}");
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"释放资源文件异常 {outputPath}", ex);
        }
        finally
        {
            outStream?.Close();
            inStream?.Close();
        }
    }
}