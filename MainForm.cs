using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TypeScopeFormLight;

public partial class MainForm : Form
{
    // const
    private const string DEFAULT_READ_DIR_PATH = @"C:\Tmp";
    private const string DEFAULT_SAVE_DIR_PATH = @"C:\Tmp";

    // クラス名とメンバーの情報を格納するクラス
    private readonly List<ClassData> _classDatas = [];

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
        _classes = FileUtil.GetClassDeclarationsFromCsFiles(fd.SelectedPath);

        // クラス名を取得してDataGridView1に追加
        foreach (var classDeclaration in _classes)
        {
            var className = classDeclaration.Identifier.Text;
            DataGridView1.Rows.Add(className);
        }

        ShowClassDatas();

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
        FilterClassInfo();

        if (_classDatas.Count == 0)
        {
            toolStripLabel1.Text = "出力対象がありません。";
            return;
        }

        if (!Directory.Exists(TextBoxフォルダ選択Save.Text))
        {
            toolStripLabel1.Text = $"{TextBoxフォルダ選択Save.Text}フォルダが存在しません。";
            return;
        }

        var path = $"{TextBoxフォルダ選択Save.Text}\\{_classDatas[0].クラス名}.md";
        FileUtil.ExportToMarkdown(path, _classDatas.);
        toolStripLabel1.Text = $"{path} - 出力完了";
    }


    private void CheckBoxPublicのみ_CheckedChanged(object sender, EventArgs e)
    {
        ShowClassDatas();
    }

    private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        ShowClassDatas();
    }

    /// <summary>
    /// DataGridView2にクラスのメンバー情報を表示する
    /// </summary>
    private void ShowClassDatas()
    {
        int currentIndex = DataGridView1.CurrentCell?.RowIndex ?? -1;
        if (currentIndex < 0 || _classes.Count <= currentIndex) return;

        var classDeclaration = _classes[currentIndex];
        DataGridView2.Rows.Clear();
        foreach (var data in classDeclaration.GetClassDataFromMembers(CheckBoxPublicのみ.Checked))
        {
            DataGridView2.Rows.Add(data.種別, data.修飾子, data.型名, data.名称, data.コメント);
        }
        UpdateDataGridView2();
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
            // dataGridView2の種別に応じてセル背景色を変更
            var firstColStr = (string)row.Cells[0].Value;
            row.DefaultCellStyle.BackColor = firstColStr switch
            {
                "Indexer" => Color.LightYellow, // インデクサー
                "Property" => Color.LightGreen, // プロパティ
                "Field" => Color.LightBlue, // フィールド
                _ => Color.White,
            };

            // アクセス修飾子がprivateなら文字を赤に変更
            if (row.Cells[1].Value.ToString() == "private")
            {
                row.DefaultCellStyle.ForeColor = Color.Red;
            }

            // checkboxの初期値をtrueにする
            row.Cells[row.Cells.Count - 1].Value = true;
        }
    }

    /// <summary>
    /// classDatasを以下の条件でフィルタリングする
    /// ・選択がチェックされてない行を除去する
    /// </summary>
    private void FilterClassInfo()
    {
        foreach (DataGridViewRow row in DataGridView2.Rows)
        {
            // 選択されていない行はclassDatasから削除
            if ((bool)row.Cells[row.Cells.Count - 1].Value == false)
            {
                var classData = _classDatas.FirstOrDefault(x => x.名称 == row.Cells[3].Value.ToString());
                if (classData is not null)
                {
                    _classDatas.Remove(classData);
                }
                continue;
            };
        }
    }
}
