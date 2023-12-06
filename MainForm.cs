using System.Windows.Forms;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TypeScopeFormLight;

public partial class MainForm : Form
{
    // const
    private const string DEFAULT_READ_DIR_PATH = @"C:\Tmp";
    private const string DEFAULT_SAVE_DIR_PATH = @"C:\Tmp";

    // クラス名とメンバーの情報を格納するクラス
    private readonly List<MetaData> _metaDatas = [];

    // クラスを格納するリスト
    private List<ClassDeclarationSyntax> _classes = [];

    public MainForm()
    {
        InitializeComponent();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        // ディレクトリの存在チェック
        if (Directory.Exists(DEFAULT_READ_DIR_PATH))
        {
            TextBoxフォルダ選択Read.Text = DEFAULT_READ_DIR_PATH;
        }

        if (Directory.Exists(DEFAULT_SAVE_DIR_PATH))
        {
            TextBoxフォルダ選択Save.Text = DEFAULT_SAVE_DIR_PATH;
        }

        // DatagridView1(左)の初期化
        List<DataGridViewColumn> classColumns =
        [
            new DataGridViewTextBoxColumn()
            {
                HeaderText = "クラス名",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            },
        ];

        DataGridView1.Columns.AddRange(classColumns.ToArray());

        // DatagridView2(右)の初期化
        List<DataGridViewColumn> dataColumns =
        [
            new DataGridViewTextBoxColumn()
            {
                HeaderText = "",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                ReadOnly = true,
            },
            new DataGridViewTextBoxColumn()
            {
                HeaderText = "修飾子",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                ReadOnly = true,
            },
            new DataGridViewTextBoxColumn()
            {
                HeaderText = "型名",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                ReadOnly = true,
            },
            new DataGridViewTextBoxColumn()
            {
                HeaderText = "変数名",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                ReadOnly = true,
                MinimumWidth = 200,

            },
            new DataGridViewTextBoxColumn()
            {
                HeaderText = "コメント",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 200,
            },
            new DataGridViewCheckBoxColumn()
            {
                HeaderText = "選択",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                TrueValue = true,
            },
        ];

        DataGridView2.Columns.AddRange(dataColumns.ToArray());
    }

    private void Buttonフォルダ選択Read_Click(object sender, EventArgs e)
    {
        toolStripLabel1.Text = "読込中...";

        using var fd = new FolderBrowserDialog
        {
            SelectedPath = DEFAULT_READ_DIR_PATH,
            Description = "読み込むフォルダを選択してください。",
        };
        if (fd.ShowDialog() != DialogResult.OK) return;

        if (!Directory.Exists(fd.SelectedPath))
        {
            toolStripLabel1.Text = $"{fd.SelectedPath}フォルダが存在しません。";
            return;
        }

        DataGridView1.Rows.Clear();
        TextBoxフォルダ選択Read.Text = fd.SelectedPath;
        _classes = FileUtil.ExtractClassDeclarationsFromCsFiles(fd.SelectedPath);

        // クラス名を取得してDataGridView1に追加
        foreach (var classDeclaration in _classes)
        {
            var className = classDeclaration.Identifier.Text;
            DataGridView1.Rows.Add(className);
        }

        ShowMetaDatas();

        label1.Text = $"{_classes.Count} 件";
        toolStripLabel1.Text = "読込完了";
    }

    private void Buttonフォルダ選択Save_Click(object sender, EventArgs e)
    {
        using var fd = new FolderBrowserDialog
        {
            SelectedPath = DEFAULT_SAVE_DIR_PATH,
            Description = "保存するフォルダを選択してください。"
        };

        if (fd.ShowDialog() != DialogResult.OK) return;

        TextBoxフォルダ選択Save.Text = fd.SelectedPath;
    }

    private void ButtonMD出力_Click(object sender, EventArgs e)
    {
        FilterMetaData();

        if (_metaDatas.Count == 0)
        {
            toolStripLabel1.Text = "出力対象がありません。";
            return;
        }

        if (!Directory.Exists(TextBoxフォルダ選択Save.Text))
        {
            toolStripLabel1.Text = $"{TextBoxフォルダ選択Save.Text}フォルダが存在しません。";
            return;
        }

        var path = $"{TextBoxフォルダ選択Save.Text}\\{_metaDatas[0].クラス名}.md";
        FileUtil.ExportToFile(path, _metaDatas);
        toolStripLabel1.Text = $"{path} - 出力完了";
    }


    private void CheckBoxPublicのみ_CheckedChanged(object sender, EventArgs e)
    {
        ShowMetaDatas();
    }

    private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        // DataGridView1のクラス名をクリックしたら、DataGridView2にメンバーを表示する
        ShowMetaDatas();
    }

    private void ShowMetaDatas()
    {
        DataGridView2.Rows.Clear();

        int currentIndex = DataGridView1.CurrentCell?.RowIndex ?? -1;
        if (currentIndex < 0 || _classes.Count <= currentIndex) return;

        var classDeclaration = _classes[currentIndex];

        SetMetaDatas(classDeclaration);

        foreach (var data in _metaDatas)
        {
            DataGridView2.Rows.Add(data.種別, data.修飾子, data.型名, data.名称, data.コメント);
        }
        UpdateDataGridView2();
    }

    private void SetMetaDatas(ClassDeclarationSyntax classDeclaration)
    {
        if (classDeclaration is null) return;

        // classDeclaration.Membersをソートする Indexer > Property > Field > その他
        var sortedMembers = classDeclaration.Members.OrderBy(member =>
            member is IndexerDeclarationSyntax ? 1 :
            member is PropertyDeclarationSyntax ? 2 :
            member is FieldDeclarationSyntax ? 3 : 4);

        _metaDatas.Clear();
        AddRangeMetaDatas(sortedMembers);

        void AddRangeMetaDatas(IEnumerable<MemberDeclarationSyntax>? members)
        {
            if (members is null || !members.Any()) return;

            foreach (var member in members.Where(IsValidMember))
            {
                if (member is null) break;

                // アクセス修飾子
                var modifier = member.Modifiers.First().ValueText;
                // 型名
                var type = member switch
                {
                    IndexerDeclarationSyntax indexer => indexer.Type.ToString(),
                    PropertyDeclarationSyntax property => property.Type.ToString(),
                    FieldDeclarationSyntax field => field.Declaration.Type.ToString(),
                    _ => string.Empty,
                };
                // 変数名
                var name = member switch
                {
                    IndexerDeclarationSyntax indexer => "this",
                    PropertyDeclarationSyntax property => property.Identifier.Text,
                    FieldDeclarationSyntax field => field.Declaration.Variables.First().Identifier.Text,
                    _ => string.Empty,
                };
                // コメント
                var comment = GetComment(member);

                // MetaDataを作成
                var metaData = new MetaData()
                {
                    クラス名 = classDeclaration.Identifier.Text,
                    種別 = member switch
                    {
                        IndexerDeclarationSyntax => Member.Indexer.ToString(),
                        PropertyDeclarationSyntax => Member.Property.ToString(),
                        FieldDeclarationSyntax => Member.Field.ToString(),
                        _ => throw new NotImplementedException(),
                    },
                    修飾子 = modifier,
                    型名 = type,
                    名称 = name,
                    コメント = comment,
                };
                _metaDatas.Add(metaData);
            }
        }
    }

    /// <summary>
    /// DataGridView2の表示を更新する
    /// </summary>
    private void UpdateDataGridView2()
    {
        // 何もなかったらreturn
        if (DataGridView2.Rows.Count == 0) return;

        foreach (DataGridViewRow row in DataGridView2.Rows)
        {
            // dataGridView2のrowの1行目に入っている種別を取得
            var firstColStr = (string)row.Cells[0].Value;
            row.DefaultCellStyle.BackColor = firstColStr switch
            {
                "Indexer" => Color.LightYellow, // インデクサー
                "Property" => Color.LightGreen, // プロパティ
                "Field" => Color.LightBlue, // フィールド
                _ => Color.White,
            };

            // 2行目がprivateなら文字を赤に変更
            if (row.Cells[1].Value.ToString() == "private")
            {
                row.DefaultCellStyle.ForeColor = Color.Red;
            }

            // checkboxの初期値をtrueにする
            row.Cells[row.Cells.Count - 1].Value = true;
        }
    }

    /// <summary>
    /// memberが特定の基準に基づいて有効かどうかを判定する
    /// ・memberがインデクサー、プロパティ、またはフィールドであること
    /// ・publicのみチェックボックスにチェックがある場合、publicであること
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    private bool IsValidMember(MemberDeclarationSyntax member)
    {
        if (member is IndexerDeclarationSyntax or PropertyDeclarationSyntax or FieldDeclarationSyntax)
        {
            // publicのみチェックボックスにチェックがある場合、public以外は表示しない
            return !CheckBoxPublicのみ.Checked || member.Modifiers.Any(x => x.IsKind(SyntaxKind.PublicKeyword));
        }
        return false;
    }

    /// <summary>
    /// memberのコメントを取得する
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    private static string GetComment(MemberDeclarationSyntax member)
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

    /// <summary>
    /// metaDatasを以下の条件でフィルタリングする
    /// ・選択がチェックされてない行を除去する
    /// </summary>
    private void FilterMetaData()
    {
        foreach (DataGridViewRow row in DataGridView2.Rows)
        {
            // 選択されていない行はmetaDatasから削除
            if ((bool)row.Cells[row.Cells.Count - 1].Value == false)
            {
                var meta = _metaDatas.FirstOrDefault(x => x.名称 == row.Cells[3].Value.ToString());
                if (meta is not null)
                {
                    _metaDatas.Remove(meta);
                }
                continue;
            };
        }
    }

    private enum Member
    {
        Indexer,
        Property,
        Field,
        Method,
    }
}
