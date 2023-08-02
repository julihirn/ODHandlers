using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Handlers {
    public static class SXHandler {
        // private static int EntryPoint = 0;
        public static List<ShadowX.SXInterpreter> Interpreters = new List<ShadowX.SXInterpreter>();

        public static void Open(string File) {
            Interpreters.Add(new ShadowX.SXInterpreter(File));
            Interpreters[Interpreters.Count - 1].Open();
        }

      


        
       //public static SXValueType GetValueClass(SXVariable variable) {
       //    switch (variable.Type) {
       //        case SXDataType.Boolean:
       //            return SXValueType.Composite;
       //        case SXDataType.Byte:
       //            return SXValueType.Numerical;
       //        case SXDataType.Character:
       //            return SXValueType.String;
       //        case SXDataType.Date:
       //            return SXValueType.Composite;
       //        case SXDataType.Decimal:
       //            return SXValueType.Numerical;
       //        case SXDataType.Double:
       //            return SXValueType.Numerical;
       //        case SXDataType.Integer:
       //            return SXValueType.Numerical;
       //        case SXDataType.Long:
       //            return SXValueType.Numerical;
       //        case SXDataType.Single:
       //            return SXValueType.Numerical;
       //        case SXDataType.String:
       //            return SXValueType.String;
       //        default:
       //            return SXValueType.Void;
       //    }
       //}



        public static int IsComment(string Input) {
            // Output Modes:
            //          -1: No comment exists in line
            //           0: Line Comment
            //           N: Comment Starts from
            int InstanceCount = 0;
            int StartingIndex = -1;
            int SpaceCount = 0;
            char CommentCharacter = '-';
            char CurrentSelected = (char)0x00;
            bool ContainsNonCommentSpace = false;
            for (int i = 0; i < Input.Length; i++) {
                CurrentSelected = Input[i];
                if (CurrentSelected == CommentCharacter) {
                    InstanceCount += 1;
                    if (InstanceCount == 1) {
                        if (ContainsNonCommentSpace == false) {
                            StartingIndex = 0;
                        }
                        else {
                            StartingIndex = i;
                        }
                    }
                    if (i > 0) {
                        if (Input[i - 1] == CommentCharacter) {
                            return StartingIndex;
                        }
                    }
                }
                else {
                    InstanceCount = 0;
                }
                if (CurrentSelected == CommentCharacter) {
                    SpaceCount = 0;
                }
                else {
                    SpaceCount++;
                    if ((Input[i] != ' ') && (Input[i] != (char)0x09)) {
                        ContainsNonCommentSpace = true;
                    }
                }
            }
            return -1;                              //Define that there is no comment in the line
        }
        public static string RemoveComment(string Input) {
            int CommentCondition = IsComment(Input);
            int CommentLength = Input.Length - CommentCondition;
            if (CommentCondition < 0) {
                return Input;
            }
            else if (CommentCondition == 0) {
                return "";
            }
            else {
                return Input.Remove(CommentCondition, CommentLength);
            }
        }
        public static void Reporter(bool Report, string Message, StatusType Critically) {
            if (Report == true) {
                //Handlers.ExceptionHandler.Print_Status(Message);
            }
        }
    }
    public class SXInterpreter {
        private string name;
        public string Name { get => name; }
        private bool postOutput = true;
        public bool PostOutput { get => postOutput; set => postOutput = value; }

       
       
        
        public List<CodeListItem> CodeList = new List<CodeListItem>();

        bool EntryMark = false;
        bool IsVaild = false;
        public bool CheckVaild(CodeListItem Line, int Index) {
            string FormattedLine = StringHandler.STR_RS(Line.FormattedText).ToLower();
            bool Output = false;
            if (EntryMark == false) {
                if (FormattedLine == "begin,") { EntryMark = true; }
            }
            else {
                if ((FormattedLine.StartsWith("createlines(")) && (FormattedLine.EndsWith("),"))) {
                    name = FormattedLine.Remove(0, 12);
                    name = name.Replace("),", "");
                    Output = true;
                }
                else if (FormattedLine == "") { } //Ignore blank lines
                else {
                    EntryMark = false;
                    //SXHandler.ErrorList.Add(new ErrorListItem(StatusType.Warning, name, "Initial entry mark was ignored.", Index, 0));
                }
            }
            return Output;
        }
        public void Open(string Address, bool ReadProperties = false) {
            System.IO.StreamReader FileReader = null;
            try {
                FileReader = System.IO.File.OpenText(Address);
                if (ReadProperties == false) {
                    SXHandler.Reporter(postOutput, name + "Reading file at: " + Handlers.StringHandler.STR_SFN(Address, 3), StatusType.Information);
                }
                else {
                    SXHandler.Reporter(PostOutput, "Reading file at: " + Handlers.StringHandler.STR_SFN(Address, 3), StatusType.Information);
                }
                //Read document and store to memory
                int LineNumber = 0;
                while (FileReader.Peek() > -1) {
                    string CurrentLine = FileReader.ReadLine();
                    CodeList.Add(new CodeListItem(CurrentLine, LineNumber));
                }
                //fileName = Address;
            }
            catch {
                SXHandler.Reporter(PostOutput, name + "Unable to correctly open file. The file is either corrupt or missing.", StatusType.Error);
            }
            finally {
                if (FileReader != null) {
                    FileReader.Dispose();
                }
            }
        }

        public void PreProcessor() {
            for (int i = 0; i < CodeList.Count; i++) {
                if (IsVaild == true) {
                    for(int c = 0; c < CodeList[i].FormattedText.Length; c++) {

                    }
                }
                else {
                    IsVaild = CheckVaild(CodeList[i], i);
                }
            }
        }
        public void Run() {

        }
    }
    public class CodeListItem {
        public string Text = "";
        public string FormattedText = "";
        public int LineNumber;
        public int StartingIndex = 0;
        public LineType Type = LineType.Blank;
        
        public CodeListItem(string Line, int LineNumber = 0) {
            Text = Line;
            this.LineNumber = LineNumber;
            int LineClassification = Handlers.SXHandler.IsComment(Line);
            switch (LineClassification) {
                case -1:
                    if (GetFormatted().Length <= 0) {
                        Type = LineType.Blank;
                    }
                    else {
                        Type = LineType.Normal;
                    }
                    break;
                case 0:
                    if (GetFormatted().Length <= 0) {
                        Type = LineType.Blank;
                    }
                    else {
                        Type = LineType.Comment;
                    }
                    break;
                default:
                    Type = LineType.Normal;
                    break;
            }
            FormattedText = Handlers.SXHandler.RemoveComment(Text);
            StartingIndex = GetTrimmedDifference();
        }
        private int GetTrimmedDifference() {
            return Text.Length - Text.TrimStart(' ').TrimStart((char)0x09).Length;
        }
        private string GetFormatted() {
            return Text.Replace(Convert.ToString(Handlers.Constants.Tab), "").TrimEnd(' ').TrimStart(' ');
        }
        public override string ToString() {
            return FormattedText.TrimEnd(' ').TrimStart(' ').TrimStart((char)0x09).TrimEnd((char)0x09);
        }
        public override bool Equals(object obj) {
            if (obj is CodeListItem) {
                if (((CodeListItem)obj).Text == this.Text) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                return false;
            }
        }
        public override int GetHashCode() {
            int hashCode = -2021586956;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Text);
            hashCode = hashCode * -1521134295 + LineNumber.GetHashCode();
            return hashCode;
        }
    }
    //public class SXClass {
    //    public string Name;
    //    public TextLocation Location;
    //    public string File;
    //    public SXClass(string name, TextLocation location, string file) {
    //        Name = name;
    //        Location = location;
    //        File = file;
    //    }
    //}
    //public class SXStructuredParameter : SXObject {
    //    private string name;
    //    public string Name { get => name; set => name = value; }
    //    bool HasNestedParameterLists = false;
    //    List<SXVariable> Variables = new List<SXVariable>();
    //    private void SetVariable(string VariableName, SXDataType Type, object Value) {
    //        for (int i = 0; i < Variables.Count; i++) {
    //            if (Variables[i].Name == VariableName) {
    //                Variables[i] = new SXVariable(VariableName, Type, Value);
    //                return;
    //            }
    //        }
    //        Variables.Add(new SXVariable(VariableName, Type, Value));
    //    }
    //    private SXVariable GetVariable(string VariableName) {
    //        foreach (SXVariable SXVar in Variables) {
    //            if (SXVar.Name == VariableName) {
    //                return SXVar;
    //            }
    //        }
    //        return null;
    //    }
    //}
    //public class SXVariable : SXObject {
    //    private string name;
    //    public string Name { get => name; set => name = value; }
    //    public SXDataType Type { get => type; set => type = value; }
    //    private SXDataType type;
    //    private object data;
    //    public SXVariable(string name, SXDataType type, object data) {
    //        this.name = name;
    //        this.type = type;
    //        this.data = data;
    //    }
    //    public object Data {
    //        get {
    //            return data;
    //        }
    //        set {
    //            data = value;
    //        }
    //    }
    //    public override bool Equals(object obj) {
    //        if (obj.GetType() == typeof(SXVariable)) {
    //            if ((((SXVariable)obj).Name == this.name) && (((SXVariable)obj).Type == this.type)) {
    //                if (((SXVariable)obj).Data == this.data) {
    //                    return true;
    //                }
    //            }
    //        }
    //        return false;
    //    }
    //    public override int GetHashCode() {
    //        var hashCode = 1330260032;
    //        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
    //        hashCode = hashCode * -1521134295 + type.GetHashCode();
    //        hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(data);
    //        return hashCode;
    //    }
    //    public static SXVariable operator +(SXVariable a, SXVariable b) {
    //        SXValueType Atype = SXHandler.GetValueClass(a);
    //        SXValueType Btype = SXHandler.GetValueClass(b);
    //        if ((Atype == SXValueType.Numerical) && (Btype == SXValueType.Numerical)) {
    //            List<MathVariable> MathVariables = new List<MathVariable>();
    //            MathVariables.Add(new MathVariable("a", a));
    //            MathVariables.Add(new MathVariable("b", b));
    //            NumericalString VarOutput = MathHandler.EvaluateExpression("a+b", MathVariables);
    //            return new SXVariable("out", SXDataType.Decimal, decimal.Parse(VarOutput.ToString()));
    //        }
    //        else if ((Atype == SXValueType.Numerical) && (Btype != SXValueType.Numerical)) {
    //            if ((b.Type == SXDataType.String) || (b.Type == SXDataType.Character)) {
    //                return new SXVariable("out", SXDataType.String, a.ToString() + b.ToString());
    //            }
    //            else if (b.Type == SXDataType.Boolean) {
    //                int c = 0;
    //                if ((bool)b.Data == true) { c = 1; }
    //                List<MathVariable> MathVariables = new List<MathVariable>();
    //                MathVariables.Add(new MathVariable("a", a));
    //                MathVariables.Add(new MathVariable("b", c));
    //                NumericalString VarOutput = MathHandler.EvaluateExpression("a+b", MathVariables);
    //                return new SXVariable("out", SXDataType.Decimal, decimal.Parse(VarOutput.ToString()));
    //            }
    //        }
    //        else if ((Atype != SXValueType.Numerical) && (Btype == SXValueType.Numerical)) {
    //            if ((a.Type == SXDataType.String) || (a.Type == SXDataType.Character)) {
    //                return new SXVariable("out", SXDataType.String, a.ToString() + b.ToString());
    //            }
    //            else if (a.Type == SXDataType.Boolean) {
    //                int c = 0;
    //                if ((bool)a.Data == true) { c = 1; }
    //                List<MathVariable> MathVariables = new List<MathVariable>();
    //                MathVariables.Add(new MathVariable("a", c));
    //                MathVariables.Add(new MathVariable("b", b));
    //                NumericalString VarOutput = MathHandler.EvaluateExpression("a+b", MathVariables);
    //                return new SXVariable("out", SXDataType.Decimal, decimal.Parse(VarOutput.ToString()));
    //            }
    //        }
    //        else if ((a.Type == SXDataType.Boolean) && (b.Type == SXDataType.Boolean)) {
    //            return new SXVariable("out", SXDataType.Boolean, (bool)a.Data | (bool)b.Data);
    //        }
    //        else if ((Atype != SXValueType.String) && (Btype == SXValueType.String)) {
    //            return new SXVariable("out", SXDataType.String, a.ToString() + b.ToString());
    //        }
    //        else {
    //            return null;
    //        }
    //        return null;
    //    }
    //}
    public enum SXDataType {
        Void = 0x00,            //VOID
        Boolean = 0x01,         //BOL 
        Byte = 0x02,            //BTE 
        Integer = 0x04,         //INT 
        Long = 0x08,            //LNG 
        Single = 0x10,          //SNG 
        Double = 0x20,          //DBL 
        Decimal = 0x40,         //DEC 
        Character = 0x100,      //CHR  
        String = 0x200,         //STR 
        Date = 0x800000        //DTE 

    }
    public enum SXValueType {
        Void = 0x00,
        Numerical = 0x01,
        String = 0x02,
        Composite = 0x04,
        Object = 0x08
    }
    public class SXCommand {

    }


    public class TextLocation {
        public int Line;
        public int Column;
        public TextLocation(int line, int column) {
            Line = line;
            Column = column;
        }
        public Point GetLocation() {
            return new Point(Line, Column);
        }
        public override string ToString() {
            return "Line: " + Line + ", Column: " + Column;
        }
    }
    public class ErrorListItem {
        public StatusType Criticality;
        string Message = "";
        string InterpreterTrace = "";
        TextLocation Position;
        public ErrorListItem(StatusType criticality, string InterpreterTrace, string message, int Line, int Column) {
            Criticality = criticality;
            Message = message;
            Position = new TextLocation(Line, Column);
        }
        public override string ToString() {
            string Output = Criticality.ToString() + " - Trace: " + InterpreterTrace + ": " + Message + " @ " + Position.ToString();
            return Output;
        }
    }
    //public class Conditionals {
    //    public ConditionalType Type;
    //    public Scope Lines = new Scope();
    //}
    //public class Scope {
    //    public int Start;
    //    public int Stop;
    //}
    public enum ConditionalType {
        If = 0,
        For = 1,
        While = 2
    }
    public enum StatusType {
        Information = 0,
        Warning = 1,
        Error = 2,
        Critical = 3
    }
    public enum LineType {
        Normal = 0x00,
        Comment = 0x01,
        Blank = 0x02
    }
}
