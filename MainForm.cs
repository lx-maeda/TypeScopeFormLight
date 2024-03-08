using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TypeScopeFormLight;

public partial class MainForm : Form
{
    // const
    private const string DEFAULT_READ_DIR_PATH = @"C:\Tmp";
    private const string DEFAULT_SAVE_DIR_PATH = @"C:\Tmp";

    // �N���X���ƃ����o�[�̏����i�[����N���X
    private readonly List<ClassData> _classDatas = [];

    // �N���X���i�[���郊�X�g
    private List<ClassDeclarationSyntax> _classes = [];

    public MainForm()
    {
        InitializeComponent();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        // �f�B���N�g���̑��݃`�F�b�N
        if (Directory.Exists(DEFAULT_READ_DIR_PATH))
        {
            TextBox�t�H���_�I��Read.Text = DEFAULT_READ_DIR_PATH;
        }

        if (Directory.Exists(DEFAULT_SAVE_DIR_PATH))
        {
            TextBox�t�H���_�I��Save.Text = DEFAULT_SAVE_DIR_PATH;
        }

        // DatagridView1(��)�̏�����
        List<DataGridViewColumn> classColumns =
        [
            new DataGridViewTextBoxColumn()
            {
                HeaderText = "�N���X��",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            },
        ];

        DataGridView1.Columns.AddRange(classColumns.ToArray());

        // DatagridView2(�E)�̏�����
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
                HeaderText = "�C���q",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                ReadOnly = true,
            },
            new DataGridViewTextBoxColumn()
            {
                HeaderText = "�^��",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                ReadOnly = true,
            },
            new DataGridViewTextBoxColumn()
            {
                HeaderText = "�ϐ���",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                ReadOnly = true,
                MinimumWidth = 200,

            },
            new DataGridViewTextBoxColumn()
            {
                HeaderText = "�R�����g",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 200,
            },
            new DataGridViewCheckBoxColumn()
            {
                HeaderText = "�I��",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                TrueValue = true,
            },
        ];

        DataGridView2.Columns.AddRange(dataColumns.ToArray());
    }

    private void Button�t�H���_�I��Read_Click(object sender, EventArgs e)
    {
        toolStripLabel1.Text = "�Ǎ���...";

        using var fd = new FolderBrowserDialog
        {
            SelectedPath = DEFAULT_READ_DIR_PATH,
            Description = "�ǂݍ��ރt�H���_��I�����Ă��������B",
        };
        if (fd.ShowDialog() != DialogResult.OK) return;

        if (!Directory.Exists(fd.SelectedPath))
        {
            toolStripLabel1.Text = $"{fd.SelectedPath}�t�H���_�����݂��܂���B";
            return;
        }

        DataGridView1.Rows.Clear();
        TextBox�t�H���_�I��Read.Text = fd.SelectedPath;
        _classes = FileUtil.GetClassDeclarationsFromCsFiles(fd.SelectedPath);

        // �N���X�����擾����DataGridView1�ɒǉ�
        foreach (var classDeclaration in _classes)
        {
            var className = classDeclaration.Identifier.Text;
            DataGridView1.Rows.Add(className);
        }

        ShowClassDatas();

        label1.Text = $"{_classes.Count} ��";
        toolStripLabel1.Text = "�Ǎ�����";
    }

    private void Button�t�H���_�I��Save_Click(object sender, EventArgs e)
    {
        using var fd = new FolderBrowserDialog
        {
            SelectedPath = DEFAULT_SAVE_DIR_PATH,
            Description = "�ۑ�����t�H���_��I�����Ă��������B"
        };

        if (fd.ShowDialog() != DialogResult.OK) return;

        TextBox�t�H���_�I��Save.Text = fd.SelectedPath;
    }

    private void ButtonMD�o��_Click(object sender, EventArgs e)
    {
        FilterClassInfo();

        if (_classDatas.Count == 0)
        {
            toolStripLabel1.Text = "�o�͑Ώۂ�����܂���B";
            return;
        }

        if (!Directory.Exists(TextBox�t�H���_�I��Save.Text))
        {
            toolStripLabel1.Text = $"{TextBox�t�H���_�I��Save.Text}�t�H���_�����݂��܂���B";
            return;
        }

        var path = $"{TextBox�t�H���_�I��Save.Text}\\{_classDatas[0].�N���X��}.md";
        FileUtil.ExportToMarkdown(path, _classDatas.);
        toolStripLabel1.Text = $"{path} - �o�͊���";
    }


    private void CheckBoxPublic�̂�_CheckedChanged(object sender, EventArgs e)
    {
        ShowClassDatas();
    }

    private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        ShowClassDatas();
    }

    /// <summary>
    /// DataGridView2�ɃN���X�̃����o�[����\������
    /// </summary>
    private void ShowClassDatas()
    {
        int currentIndex = DataGridView1.CurrentCell?.RowIndex ?? -1;
        if (currentIndex < 0 || _classes.Count <= currentIndex) return;

        var classDeclaration = _classes[currentIndex];
        DataGridView2.Rows.Clear();
        foreach (var data in classDeclaration.GetClassDataFromMembers(CheckBoxPublic�̂�.Checked))
        {
            DataGridView2.Rows.Add(data.���, data.�C���q, data.�^��, data.����, data.�R�����g);
        }
        UpdateDataGridView2();
    }

    /// <summary>
    /// DataGridView2�̕\�����X�V����
    /// </summary>
    private void UpdateDataGridView2()
    {
        // �����Ȃ�������return
        if (DataGridView2.Rows.Count == 0) return;

        foreach (DataGridViewRow row in DataGridView2.Rows)
        {
            // dataGridView2�̎�ʂɉ����ăZ���w�i�F��ύX
            var firstColStr = (string)row.Cells[0].Value;
            row.DefaultCellStyle.BackColor = firstColStr switch
            {
                "Indexer" => Color.LightYellow, // �C���f�N�T�[
                "Property" => Color.LightGreen, // �v���p�e�B
                "Field" => Color.LightBlue, // �t�B�[���h
                _ => Color.White,
            };

            // �A�N�Z�X�C���q��private�Ȃ當����ԂɕύX
            if (row.Cells[1].Value.ToString() == "private")
            {
                row.DefaultCellStyle.ForeColor = Color.Red;
            }

            // checkbox�̏����l��true�ɂ���
            row.Cells[row.Cells.Count - 1].Value = true;
        }
    }

    /// <summary>
    /// classDatas���ȉ��̏����Ńt�B���^�����O����
    /// �E�I�����`�F�b�N����ĂȂ��s����������
    /// </summary>
    private void FilterClassInfo()
    {
        foreach (DataGridViewRow row in DataGridView2.Rows)
        {
            // �I������Ă��Ȃ��s��classDatas����폜
            if ((bool)row.Cells[row.Cells.Count - 1].Value == false)
            {
                var classData = _classDatas.FirstOrDefault(x => x.���� == row.Cells[3].Value.ToString());
                if (classData is not null)
                {
                    _classDatas.Remove(classData);
                }
                continue;
            };
        }
    }
}
