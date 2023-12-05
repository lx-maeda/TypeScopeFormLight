namespace TypeScopeFormLight;

public class SampleClass
{
    // コメント1
    public int MyProperty1 { get; set; }

    /// <summary>
    /// コメント2
    /// </summary>
    public int MyProperty2 { get; set; }

    public int MyProperty3 { get; set; } // コメント3

    // コメント4
    // コメント5
    public int MyProperty4 { get; set; }

    // コメント6
    private int MyProperty5 { get; set; }

    /// <summary>
    /// コメント7
    /// </summary>
    private int MyProperty6 { get; set; }

    private int MyProperty7 { get; set; } // コメント8

    // コメント9
    // コメント10
    private int MyProperty8 { get; set; }


    // コメント11
    public int Field1;

    /// <summary>
    /// コメント12
    /// </summary>
    public int Field2;

    public int Field3; // コメント13

    // コメント14
    // コメント15
    public int Field4;

    // コメント16
    private int Field5;

    /// <summary>
    /// コメント17
    /// </summary>
    private int Field6;

    private int Field7; // コメント18

    // コメント19
    // コメント20
    private int Field8;

    public string StrProperty { get; set; } = string.Empty;

    public int IntProperty { get; set; } = 0;

    public int? NullableIntProperty { get; set; } = null;
}
