using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace TypeScopeFormLight;

public static class FileUtil
{
    /// <summary>
    /// 指定されたパス内のすべてのC#ファイルからクラス宣言を読み取り、リストとして返します
    /// </summary>
    /// <param name="dirPath">クラス宣言を検索するディレクトリのパス</param>
    /// <returns>指定されたディレクトリ内のすべてのC#ファイルから見つかったクラス宣言のリスト</returns>
    public static List<ClassDeclarationSyntax> GetClassDeclarationsFromCsFiles(string dirPath)
    {
        // ディレクトリ内の全てのcsファイルのパスを取得
        var csFilePaths = Directory.EnumerateFiles(dirPath, "*.cs", SearchOption.AllDirectories);

        var classDeclarations = new List<ClassDeclarationSyntax>();

        foreach (var csFilePath in csFilePaths)
        {
            var declarationsInFile = ParseCsFile(csFilePath);
            classDeclarations.AddRange(declarationsInFile);
        }

        return classDeclarations;
    }

    /// <summary>
    /// 指定されたC#ファイルを解析し、その中のすべてのクラス宣言を取得します
    /// </summary>
    /// <param name="filePath">解析するC#ファイルのパス</param>
    /// <returns>指定されたファイル内のすべてのクラス宣言</returns>
    private static IEnumerable<ClassDeclarationSyntax> ParseCsFile(string filePath)
    {
        // ファイルの内容を読み込み
        var context = File.ReadAllText(filePath);
        // 構文木をパースし、クラス宣言を取得
        var tree = CSharpSyntaxTree.ParseText(context);
        var root = tree.GetCompilationUnitRoot();
        return root.DescendantNodes().OfType<ClassDeclarationSyntax>();
    }

    /// <summary>
    /// メタデータをCSV形式で出力する
    /// </summary>
    /// <param name="path"></param>
    /// <param name="records"></param>
    public static void ExportToFile(string path, IEnumerable<ClassData> records)
    {
        using var writer = new StreamWriter(path, false, Encoding.UTF8);
        writer.Write(records.GetMarkDownString());
    }

    /// <summary>
    /// メタデータをMarkDown形式の文字列に変換する
    /// </summary>
    /// <param name="records"></param>
    /// <returns></returns>
    private static string GetMarkDownString(this IEnumerable<ClassData> records)
    {
        if (records is null || !records.Any())
        {
            return string.Empty;
        }

        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine("``` mermaid");
        stringBuilder.AppendLine("erDiagram");
        stringBuilder.AppendLine(@$"  ""{records.First().クラス名}"" {{");

        foreach (ClassData record in records)
        {
            // mermaidの仕様上、<>は使用できないため、~に置換する
            if (record.型名.Contains('<') || record.型名.Contains('>'))
            {
                record.型名 = record.型名.Replace('<', '~').Replace('>', '~');
            }

            // 型名に名前空間が含まれている場合、名前空間を削除する
            int dotIndex = record.型名.IndexOf('.');
            if (dotIndex != -1)
            {
                record.型名 = record.型名[(dotIndex + 1)..];
            }

            stringBuilder.AppendLine($"    {record.型名} {record.名称} \"{record.コメント}\"");
        }

        stringBuilder.AppendLine("  }");
        stringBuilder.AppendLine("```");

        return stringBuilder.ToString();
    }
}
