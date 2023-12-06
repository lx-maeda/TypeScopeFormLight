using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TypeScopeFormLight;

public static class ParseClassDeclaration
{
    /// <summary>
    /// クラスから各メンバーを取得してIEnumerable<ClassData>として返す
    /// メンバーはIndexer, Property, Fieldのみ
    /// </summary>
    /// <param name="classDeclaration"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static IEnumerable<ClassData> GetClassDataFromMembers(this ClassDeclarationSyntax classDeclaration, bool isPublicOnly)
    {
        // classDeclaration.Membersが空の場合は空のIEnumerable<ClassData>を返す
        if (!classDeclaration.Members.Any()) return Enumerable.Empty<ClassData>();

        // classDeclaration.Membersをソートする Indexer > Property > Field > その他
        var members = classDeclaration.Members.OrderBy(member =>
            member is IndexerDeclarationSyntax ? 1 :
            member is PropertyDeclarationSyntax ? 2 :
            member is FieldDeclarationSyntax ? 3 : 4);

        var classDatas = new List<ClassData>();

        foreach (var member in members.Where(x => IsValidMember(x, isPublicOnly)))
        {
            var classData = new ClassData()
            {
                クラス名 = classDeclaration.Identifier.Text,
                種別 = member switch
                {
                    IndexerDeclarationSyntax => "Indexer",
                    PropertyDeclarationSyntax => "Property",
                    FieldDeclarationSyntax => "Field",
                    _ => throw new NotImplementedException(),
                },
                修飾子 = member.Modifiers.First().ValueText,
                型名 = member switch
                {
                    IndexerDeclarationSyntax indexer => indexer.Type.ToString(),
                    PropertyDeclarationSyntax property => property.Type.ToString(),
                    FieldDeclarationSyntax field => field.Declaration.Type.ToString(),
                    _ => string.Empty,
                },
                名称 = member switch
                {
                    IndexerDeclarationSyntax indexer => "this",
                    PropertyDeclarationSyntax property => property.Identifier.Text,
                    FieldDeclarationSyntax field => field.Declaration.Variables.First().Identifier.Text,
                    _ => string.Empty,
                },
                コメント = member.GetComment(),
            };
            classDatas.Add(classData);
        }

        return classDatas;
    }

    /// <summary>
    /// クラスメンバーが特定の基準に基づいて有効かどうかを判定する
    /// ・メンバーがインデクサー、プロパティまたはフィールドであること
    /// ・publicのみチェックボックスにチェックがある場合、publicであること
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    private static bool IsValidMember(MemberDeclarationSyntax member, bool isPublicOnly)
    {
        if (member is IndexerDeclarationSyntax or PropertyDeclarationSyntax or FieldDeclarationSyntax)
        {
            // publicのみチェックボックスにチェックがある場合、public以外は表示しない
            // isPublicOnly が true の場合に限り、メンバーが public であるかどうかをチェックしています。
            return !isPublicOnly || member.Modifiers.Any(x => x.IsKind(SyntaxKind.PublicKeyword));
        }
        return false;
    }

    /// <summary>
    /// クラスメンバーのコメントを取得する
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    private static string GetComment(this MemberDeclarationSyntax member)
    {
        var leadingTrivia = member.GetLeadingTrivia();

        // 単一行コメントを取得する
        var commentList = leadingTrivia.Where(x => x.IsKind(SyntaxKind.SingleLineCommentTrivia)).ToList();
        var comment1 = commentList.Count == 0 ? string.Empty : string.Join(", ", commentList);

        // XMLドキュメントコメントを取得する
        var triviaList = leadingTrivia
                        .Where(x => x.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                        .Select(trvia => trvia.GetStructure() as DocumentationCommentTriviaSyntax)
                        .FirstOrDefault();
        var comment2 = triviaList?.DescendantTokens()
             .Where(token => token.IsKind(SyntaxKind.XmlTextLiteralToken))
             .FirstOrDefault(token => !string.IsNullOrWhiteSpace(token.ValueText))
             .ValueText ?? string.Empty;

        // 末尾コメントを取得する
        var trailingTrivia = member.GetTrailingTrivia();
        var comment3 = trailingTrivia
            .FirstOrDefault(trivia => trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
                                  trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)).ToString();

        //  単一行コメント > XMLドキュメントコメント > 末尾コメント の順に優先してコメントを取得する
        string comment = !string.IsNullOrEmpty(comment1) ? comment1
                       : !string.IsNullOrEmpty(comment2) ? comment2
                       : comment3;

        // コメントの不要箇所を削除する
        comment = comment.Replace("///", string.Empty).Replace("//", string.Empty)
                         .Replace("/*", string.Empty)
                         .Replace("*/", string.Empty)
                         .Trim();

        return comment;
    }
}
