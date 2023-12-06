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

    // �N���X���ƃ����o�[�̏����i�[����N���X
    private readonly List<MetaData> _metaDatas = [];

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
        _classes = FileUtil.ExtractClassDeclarationsFromCsFiles(fd.SelectedPath);

        // �N���X�����擾����DataGridView1�ɒǉ�
        foreach (var classDeclaration in _classes)
        {
            var className = classDeclaration.Identifier.Text;
            DataGridView1.Rows.Add(className);
        }

        ShowMetaDatas();

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
        FilterMetaData();

        if (_metaDatas.Count == 0)
        {
            toolStripLabel1.Text = "�o�͑Ώۂ�����܂���B";
            return;
        }

        if (!Directory.Exists(TextBox�t�H���_�I��Save.Text))
        {
            toolStripLabel1.Text = $"{TextBox�t�H���_�I��Save.Text}�t�H���_�����݂��܂���B";
            return;
        }

        var path = $"{TextBox�t�H���_�I��Save.Text}\\{_metaDatas[0].�N���X��}.md";
        FileUtil.ExportToFile(path, _metaDatas);
        toolStripLabel1.Text = $"{path} - �o�͊���";
    }


    private void CheckBoxPublic�̂�_CheckedChanged(object sender, EventArgs e)
    {
        ShowMetaDatas();
    }

    private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        // DataGridView1�̃N���X�����N���b�N������ADataGridView2�Ƀ����o�[��\������
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
            DataGridView2.Rows.Add(data.���, data.�C���q, data.�^��, data.����, data.�R�����g);
        }
        UpdateDataGridView2();
    }

    private void SetMetaDatas(ClassDeclarationSyntax classDeclaration)
    {
        if (classDeclaration is null) return;

        // classDeclaration.Members���\�[�g���� Indexer > Property > Field > ���̑�
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

                // �A�N�Z�X�C���q
                var modifier = member.Modifiers.First().ValueText;
                // �^��
                var type = member switch
                {
                    IndexerDeclarationSyntax indexer => indexer.Type.ToString(),
                    PropertyDeclarationSyntax property => property.Type.ToString(),
                    FieldDeclarationSyntax field => field.Declaration.Type.ToString(),
                    _ => string.Empty,
                };
                // �ϐ���
                var name = member switch
                {
                    IndexerDeclarationSyntax indexer => "this",
                    PropertyDeclarationSyntax property => property.Identifier.Text,
                    FieldDeclarationSyntax field => field.Declaration.Variables.First().Identifier.Text,
                    _ => string.Empty,
                };
                // �R�����g
                var comment = GetComment(member);

                // MetaData���쐬
                var metaData = new MetaData()
                {
                    �N���X�� = classDeclaration.Identifier.Text,
                    ��� = member switch
                    {
                        IndexerDeclarationSyntax => Member.Indexer.ToString(),
                        PropertyDeclarationSyntax => Member.Property.ToString(),
                        FieldDeclarationSyntax => Member.Field.ToString(),
                        _ => throw new NotImplementedException(),
                    },
                    �C���q = modifier,
                    �^�� = type,
                    ���� = name,
                    �R�����g = comment,
                };
                _metaDatas.Add(metaData);
            }
        }
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
            // dataGridView2��row��1�s�ڂɓ����Ă����ʂ��擾
            var firstColStr = (string)row.Cells[0].Value;
            row.DefaultCellStyle.BackColor = firstColStr switch
            {
                "Indexer" => Color.LightYellow, // �C���f�N�T�[
                "Property" => Color.LightGreen, // �v���p�e�B
                "Field" => Color.LightBlue, // �t�B�[���h
                _ => Color.White,
            };

            // 2�s�ڂ�private�Ȃ當����ԂɕύX
            if (row.Cells[1].Value.ToString() == "private")
            {
                row.DefaultCellStyle.ForeColor = Color.Red;
            }

            // checkbox�̏����l��true�ɂ���
            row.Cells[row.Cells.Count - 1].Value = true;
        }
    }

    /// <summary>
    /// member������̊�Ɋ�Â��ėL�����ǂ����𔻒肷��
    /// �Emember���C���f�N�T�[�A�v���p�e�B�A�܂��̓t�B�[���h�ł��邱��
    /// �Epublic�̂݃`�F�b�N�{�b�N�X�Ƀ`�F�b�N������ꍇ�Apublic�ł��邱��
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    private bool IsValidMember(MemberDeclarationSyntax member)
    {
        if (member is IndexerDeclarationSyntax or PropertyDeclarationSyntax or FieldDeclarationSyntax)
        {
            // public�̂݃`�F�b�N�{�b�N�X�Ƀ`�F�b�N������ꍇ�Apublic�ȊO�͕\�����Ȃ�
            return !CheckBoxPublic�̂�.Checked || member.Modifiers.Any(x => x.IsKind(SyntaxKind.PublicKeyword));
        }
        return false;
    }

    /// <summary>
    /// member�̃R�����g���擾����
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    private static string GetComment(MemberDeclarationSyntax member)
    {
        var leadingTrivia = member.GetLeadingTrivia();

        // �P��s�R�����g���擾����
        var commentList = leadingTrivia.Where(x => x.IsKind(SyntaxKind.SingleLineCommentTrivia)).ToList();
        var comment1 = commentList.Count == 0 ? string.Empty : string.Join(", ", commentList);

        // XML�h�L�������g�R�����g���擾����
        var triviaList = leadingTrivia
                        .Where(x => x.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                        .Select(trvia => trvia.GetStructure() as DocumentationCommentTriviaSyntax)
                        .FirstOrDefault();
        var comment2 = triviaList?.DescendantTokens()
             .Where(token => token.IsKind(SyntaxKind.XmlTextLiteralToken))
             .FirstOrDefault(token => !string.IsNullOrWhiteSpace(token.ValueText))
             .ValueText ?? string.Empty;

        // �����R�����g���擾����
        var trailingTrivia = member.GetTrailingTrivia();
        var comment3 = trailingTrivia
            .FirstOrDefault(trivia => trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
                                  trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)).ToString();

        //  �P��s�R�����g > XML�h�L�������g�R�����g > �����R�����g �̏��ɗD�悵�ăR�����g���擾����
        string comment = !string.IsNullOrEmpty(comment1) ? comment1
                       : !string.IsNullOrEmpty(comment2) ? comment2
                       : comment3;

        // �R�����g�̕s�v�ӏ����폜����
        comment = comment.Replace("///", string.Empty).Replace("//", string.Empty)
                         .Replace("/*", string.Empty)
                         .Replace("*/", string.Empty)
                         .Trim();

        return comment;
    }

    /// <summary>
    /// metaDatas���ȉ��̏����Ńt�B���^�����O����
    /// �E�I�����`�F�b�N����ĂȂ��s����������
    /// </summary>
    private void FilterMetaData()
    {
        foreach (DataGridViewRow row in DataGridView2.Rows)
        {
            // �I������Ă��Ȃ��s��metaDatas����폜
            if ((bool)row.Cells[row.Cells.Count - 1].Value == false)
            {
                var meta = _metaDatas.FirstOrDefault(x => x.���� == row.Cells[3].Value.ToString());
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
