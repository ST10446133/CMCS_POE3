// Small helper class to share file content-type logic between controllers
public static class LecturerControllerHelper
{
    public static string GetContentTypeFromFileName(string fileName)
    {
        var ext = Path.GetExtension(fileName)?.ToLowerInvariant() ??
        string.Empty;
        return ext switch
        {
        ".pdf" => "application/pdf", ".docx" => "application/vnd.openxmlformats officedocument.wordprocessingml.document",
        ".xlsx" => "application/vnd.openxmlformats officedocument.spreadsheetml.sheet",
        _ => "application/octet-stream",
        };
    }
}