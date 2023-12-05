using System.Text;

namespace TypeScopeFormLight;

public static class ExportUtil
{
    public static void ToMarkDown(string path, IEnumerable<MetaData> records)
    {
        try
        {
            foreach (MetaData record in records)
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
            }

            using var writer = new StreamWriter(path, false, Encoding.UTF8);
            writer.WriteLine("``` mermaid");
            writer.WriteLine("erDiagram");
            writer.WriteLine(@$"  ""{records.First().クラス名}"" {{");
            foreach (MetaData record in records)
            {
                writer.WriteLine($"    {record.型名} {record.名称} \"{record.コメント}\"");
            }
            writer.WriteLine("  }");
            writer.WriteLine("```");
        }
        catch (Exception)
        {
            throw;
        }
    }
}
