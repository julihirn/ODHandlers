// '======================================
// DOCUMENT HANDLER
// ONE DESKTOP COMPONENTS 
// JULIAN HIRNIAK
// COPYRIGHT (C) 2016-2022 Julian Hirniak
// '======================================
using System.Drawing;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Xml.Linq;

namespace Handlers {
    public class DocumentHandler {
        // Interpreter Properties
        public static List<Variable> VAR = new List<Variable>();
        public static List<ParameterStructure> PARM = new List<ParameterStructure>();
        public static List<string> LIST = new List<string>();
        public static List<string> INST = new List<string>();
        public static List<string> LINES = new List<string>();
        public static List<Enum> ENUMERATION = new List<Enum>();
        public static List<int> LinestoIgnore = new List<int>();
        public static bool PostOutput = true;
        public static string InterpreterName = "SXINTER: ";
        private static string Address = "";
        private static bool EnteredParm = false;
        private static bool EnteredEnum = false;
        static int CurrentEnum = 0;
        private static List<Point> LEVELS = new List<Point>();

        private static bool Initalised = false;
        private static bool BEGCHK = false;
        private static bool INLIST = false;
        private static string ListIn = "";
        static int ErrorCount = 0;
        static int WarningCount = 0;
        static int CritialCount = 0;
        private static void Reporter(bool Report, string Message, StatusType Critically) {
            if (Critically == StatusType.Warning) {
                WarningCount++;

            }
            else if (Critically == StatusType.Error) {
                ErrorCount++;
                EncounteredErrors = true;
            }
            else if (Critically == StatusType.Critical) {
                CritialCount++;
                EncounteredErrors = true;
            }
            if (Report == true) {
                Handlers.ExceptionHandler.Print_Status(Message);
            }
        }
        public static void Open(string Adr, bool ReadProperties = false) {
            LINES.Clear();
            System.IO.StreamReader myStreamReader = null;
            try {
                // If Not My.Settings.mon_adr = "" Then

                myStreamReader = System.IO.File.OpenText(Adr);
                if (ReadProperties == false) {
                    Reporter(PostOutput, InterpreterName + "Reading file at: " + Handlers.StringHandler.STR_SFN(Adr, 3), StatusType.Information);
                }
                else {
                    Reporter(PostOutput, "Reading file at: " + Handlers.StringHandler.STR_SFN(Adr, 3), StatusType.Information);
                }
                Address = Adr;
                while (myStreamReader.Peek() > -1) {
                    string INPUTLINE = myStreamReader.ReadLine();
                    LINES.Add(INPUTLINE);
                }
            }
            catch {
                Reporter(PostOutput, InterpreterName + "Unable to correctly open file. The file is either corrupt or missing.", StatusType.Error);
            }
            finally {
                if (myStreamReader != null) {
                    myStreamReader.Dispose();
                }
            }
            BeginInterpret();
        }
        public static void BeginInterpret() {
            int Line = 1;
            VAR.Clear();
            PARM.Clear();
            LIST.Clear();
            INST.Clear();

            LEVELS.Clear();
            ENUMERATION.Clear();
            LinestoIgnore.Clear();
            INLIST = false;
            Initalised = false;
            BEGCHK = false;
            ListIn = "";
            EnteredParm = false;
            EnteredEnum = false;

            EncounteredErrors = false;
            WarningCount = 0;
            ErrorCount = 0;
            CritialCount = 0;
            string Value = "";
            if (LINES.Count > 0) {
                for (int i = 0; i < LINES.Count; i++) {
                    Value = CleanTabs(LINES[i]);
                    ClassRead(Value, i);
                }
                Line = 1;
                for (int i = 0; i < LINES.Count; i++) {
                    if (Ignore(i) == false) {
                        try {
                            Line = i + 1;
                            Value = CleanTabs(LINES[i]);
                            LineRead(Value, Line);
                        }
                        catch {
                            Reporter(PostOutput, InterpreterName + "Syntax Error at line " + Line.ToString() + ", '" + CleanTabs(LINES[i]) + "' is invalid.", StatusType.Error);
                            break;
                        }
                    }
                }
                if (Initalised == false) {
                    Reporter(PostOutput, InterpreterName + "Syntax Error at line " + Line.ToString() + ", document primary entry point skipped! Lines not created!", StatusType.Critical);
                }
                if ((EnteredParm == true) || (EnteredEnum == true)) {
                    Reporter(PostOutput, InterpreterName + "Syntax Warning: Parameter or enumeration was never exited, line: " + Line.ToString(), StatusType.Warning);
                }
                Reporter(PostOutput, InterpreterName + "--------------------", StatusType.Information);
                Reporter(PostOutput, "", StatusType.Information);
                Reporter(PostOutput, "  Warnings: " + WarningCount.ToString(), StatusType.Information);
                Reporter(PostOutput, "  Errors:   " + ErrorCount.ToString(), StatusType.Information);
                Reporter(PostOutput, "  Critical: " + CritialCount.ToString(), StatusType.Information);
                Reporter(PostOutput, "", StatusType.Information);
            }
        }
        private static string CleanTabs(string Input) {
            return Input.Replace(Convert.ToString(Constants.Tab), "");
        }
        static int InvaildCount = 0;
        private static void ClassRead(string Input, int Line) {
            string NSI = RemoveInLineComment(Input.Replace(" ", ""));

            if (IsType(NSI) == ObjectType.Type_Enum) {
                string Name = GetName(RemoveInLineComment(Input.TrimEnd(Constants.Space)));
                if (Name.EndsWith("{")) {
                    EnteredEnum = true;
                    Name = Name.Replace("{", "");
                    Enum En = new Enum {
                        Name = Name
                    };
                    ENUMERATION.Add(En);
                    CurrentEnum = ENUMERATION.Count - 1;
                    LinestoIgnore.Add(Line);
                }
            }
            else {
                if (EnteredEnum == true) {
                    LinestoIgnore.Add(Line);
                }
                if (IsComment(NSI) == false) {
                    if (EnteredEnum == true) {
                        if (RemoveInLineComment(NSI) == "}") {
                            EnteredEnum = false;
                        }
                        else {
                            try {
                                ENUMERATION[CurrentEnum].Members.Add(NSI);
                            }
                            catch {
                            }
                        }
                    }

                }
            }

        }
        private static string GetName(string Input) {
            try {
                string HolString = StringHandler.STR_SS(Input, Constants.Colon, 1);
                HolString = HolString.TrimEnd();

                return HolString;
            }
            catch {
                InvaildCount++;
                return "<InvaildDef>" + InvaildCount;
            }
        }
        private static string RemoveInLineComment(string Input) {
            int DashCount = 0;
            string Output = "";
            if (Input.Length > 0) {
                for (int i = 0; i < Input.Length; i++) {
                    if (Input[i] == Constants.Dash) {
                        DashCount += 1;
                        if (DashCount == 2) {
                            Output = Input.Remove(i - 1, Input.Length - i + 1);
                            break;
                        }
                    }
                    else {
                        Output = Input;
                        DashCount = 0;
                    }

                }
                return Output;
            }
            else {
                return "";
            }
        }
        private static bool Ignore(int Index) {
            bool Ret = false;
            if (LinestoIgnore.Count > 0) {
                for (int i = 0; i < LinestoIgnore.Count; i++) {
                    if (LinestoIgnore[i] == Index) {
                        Ret = true;
                        break;
                    }
                }
                return Ret;
            }
            else {
                return false;
            }
        }
        private static void LineRead(string Input, int Line) {
            string NSI = Input.Replace(" ", "");
            if ((NSI.ToLower() ?? "") == "begin,")
                BEGCHK = true;
            if (BEGCHK == true) {
                if (NSI.ToLower().StartsWith("createlines") && NSI.ToLower().EndsWith(","))
                    Initalised = true;
            }
            if (Initalised == true) {
                if (IsComment(NSI) == false) {
                    if (IsType(NSI) == ObjectType.Type_Def) {
                        try {
                            string ITRM = Input.TrimStart(' ');
                            var SS = Handlers.StringHandler.STR_MVSS(ITRM, ',');
                            if (SS.Count > 1) {
                                var ST = Handlers.StringHandler.STR_MVSS(SS.Value[1], ':');
                                if ((ST.Value[0].ToLower() ?? "") == "parm") {
                                    if (ST.Value[1].Replace(" ", "").EndsWith("{")) {
                                        if ((EnteredParm == true) || (EnteredEnum == true)) {
                                            Reporter(PostOutput, InterpreterName + "Syntax Warning: Parameter or enumeration was never exited, line: " + Line.ToString(), StatusType.Warning);
                                        }
                                        var prm = new ParameterStructure();
                                        {
                                            var withBlock = prm;
                                            withBlock.Name = ST.Value[1].Replace(" ", "").Remove(ST.Value[1].Replace(" ", "").Length - 1, 1);
                                        }
                                        PARM.Add(prm);
                                        EnteredParm = true;
                                    }
                                    else
                                        throw new InvalidCastException("Unable to cast parameter");
                                }
                                else if ((ST.Value[0].ToLower() ?? "") == "list") {
                                    string lstmthd = Handlers.StringHandler.STR_RCM(ST.Value[1].TrimEnd(' '));
                                    if (lstmthd.EndsWith("{")) {
                                        ListIn = ST.Value[1].Replace("{", "");
                                        INST.Add(ST.Value[1].Replace("{", ""));
                                        INLIST = true;
                                    }
                                    else
                                        throw new InvalidCastException("Unable to cast list");
                                }
                                else if (INLIST == false)
                                    DeclareVariable(ITRM);
                                else if (NSI.EndsWith("}")) {
                                    INLIST = false;
                                    ListIn = "";
                                }
                                else
                                    LIST.Add(ListIn + "," + ITRM);
                            }
                        }
                        catch {
                            Reporter(PostOutput, InterpreterName + "Syntax Error.", StatusType.Error);
                        }
                    }
                    else if ((int)IsType(NSI) == (int)ObjectType.Type_Op) {
                        string CUR = Handlers.StringHandler.STR_TRIMSTART(NSI, "op,").Replace(" ", "");
                        string Type_op = Handlers.StringHandler.STR_SS(CUR, ':', 0);
                        if (IsOp(CUR, Line) == (int)ControlFlowType.Type_If) {
                        }
                    }
                    else if ((int)IsType(NSI) == (int)ObjectType.Type_OpRet) {
                        if (LEVELS.Count >= 1)
                            LEVELS.RemoveAt(LEVELS.Count - 1);
                    }
                    else if (INLIST == true) {
                        string ITRM = Input.TrimStart(' ');
                        if (NSI.EndsWith("}")) {
                            if (INLIST == true)
                                INLIST = false;
                            //if (EnteredParm == true)
                            //    EnteredParm = false;
                            ListIn = "";
                        }
                        else
                            LIST.Add(ListIn + "," + ITRM);
                    }
                    else if (EnteredArray == true) {
                        string LineData = Input.TrimEnd(' ');
                        if (LineData.EndsWith("}")) { LineData = LineData.Remove(LineData.Length - 1, 1); }
                        ProcessArrayObjects(LineData);
                        if (NSI.EndsWith("}")) {
                            EnteredArray = false;
                        }
                    }
                    else if (EnteredParm == true) {
                        if (NSI.EndsWith("}")) {
                            EnteredParm = false;
                        }
                    }
                }
            }
        }
        private static ControlFlowType IsOp(string Input, int Line) {
            string BUILD = "";
            for (int I = 0, loopTo = Input.Length - 1; I <= loopTo; I++) {
                if (Input[I].ToString() == ":")
                    break;
                else
                    BUILD += Input[I].ToString();
            }
            var TOP = ControlFlowType.Type_Error;
            if ((BUILD.ToLower() ?? "") == "if")
                TOP = ControlFlowType.Type_If;
            else if ((BUILD.ToLower() ?? "") == "for")
                TOP = ControlFlowType.Type_For;
            else if ((BUILD.ToLower() ?? "") == "while")
                TOP = ControlFlowType.Type_While;
            else if ((BUILD.ToLower() ?? "") == "try")
                TOP = ControlFlowType.Type_Try;
            else
                TOP = ControlFlowType.Type_Error;
            if (LEVELS.Count >= 1) {
                if (LEVELS[LEVELS.Count - 1].X == OpTypetoInt(TOP) & LEVELS[LEVELS.Count - 1].Y == Line) {
                }
                else
                    LEVELS.Add(new Point(OpTypetoInt(TOP), Line));
            }
            else if (OpTypetoInt(TOP) != -1)
                LEVELS.Add(new Point(OpTypetoInt(TOP), Line));
            return TOP;
        }
        private static int OpTypetoInt(ControlFlowType TypeO) {
            if ((int)TypeO == (int)ControlFlowType.Type_Error)
                return -1;
            else
                return Convert.ToInt32((int)TypeO);
        }
        private static Variable CurrentArrayVariable = null;
        private static bool EnteredArray = false;
        private static string MissingArrayTypeMessage = "Missing array type, declaration was aborted";
        private static string MissingAssignmentMessage = "Variable or constant is either missing an assignment or is incorrectly defined";
        private static void DeclareVariable(string Input) {
            try {
                bool ArrayAlreadyEvaluated = false;
                STR_MVSSF SS = Handlers.StringHandler.STR_MVSS(Input, ',');
                STR_MVSSF ST = Handlers.StringHandler.STR_MVSS(StringHandler.CombineSpilt(SS, ',', 1), ':');
                STR_MVSSF SQ = Handlers.StringHandler.STR_MVSS(Handlers.StringHandler.STR_RSEQ(StringHandler.CombineSpilt(ST, ':', 1)), '=');
                Variable VAL = new Variable();

                {
                    var withBlock = VAL;
                    withBlock.Name = SQ.Value[0];
                    DataType Type = StringToDataType(ST.Value[0].ToLower());
                    string Data = RemoveInLineComment(SQ.Value[1]);
                    if (Type == DataType.INT) {//if ((ST.Value[0].ToLower() ?? "") == "int") {
                        withBlock.DataType = Handlers.DataType.INT;
                        if (ConversionHandler.IsNumeric(Data))
                            withBlock.Value = Convert.ToInt32(Data);
                        else {
                            withBlock.Value = 0;
                            Reporter(PostOutput, InterpreterName + @"Syntax Warning: " + MissingAssignmentMessage + ", defaulted to 0", StatusType.Warning);
                        }
                    }
                    else if (Type == DataType.LNG) {//if ((ST.Value[0].ToLower() ?? "") == "lng") {
                        withBlock.DataType = Handlers.DataType.LNG;
                        if (ConversionHandler.IsNumeric(Data))
                            withBlock.Value = Convert.ToInt64(Data);
                        else {
                            withBlock.Value = 0;// .LNG = 0
                            Reporter(PostOutput, InterpreterName + @"Syntax Warning: " + MissingAssignmentMessage + ", defaulted to 0", StatusType.Warning);
                        }
                    }
                    else if (Type == DataType.STR) {//if ((ST.Value[0].ToLower() ?? "") == "str") {
                        withBlock.DataType = Handlers.DataType.STR;
                        string HoldString = StringHandler.TrimStartFromFirstOccurance(Input, '=', false).TrimStart(' ').TrimStart(Constants.Tab);
                        if (IsString(HoldString) == true) {//SQ.Value[1]
                            string STRHOLD = Handlers.StringHandler.STR_RSE(HoldString);
                            withBlock.Value = STRHOLD; // .STR = STRHOLD
                        }
                        else {
                            withBlock.Value = "";// .STR = ""
                            Reporter(PostOutput, InterpreterName + @"Syntax Warning: " + MissingAssignmentMessage + @", defaulted to """, StatusType.Warning);
                        }
                    }
                    else if (Type == DataType.DEC) {//if ((ST.Value[0].ToLower() ?? "") == "dec") {
                        withBlock.DataType = Handlers.DataType.DEC;
                        if (ConversionHandler.IsNumeric(Data)) {
                            withBlock.Value = Convert.ToDecimal(Data); // .DEC = Convert.ToDecimal(SQ.Value(1))
                        }
                        else {
                            withBlock.Value = 0;// .DEC = 0
                            Reporter(PostOutput, InterpreterName + @"Syntax Warning: " + MissingAssignmentMessage + ", defaulted to 0", StatusType.Warning);
                        }
                    }
                    else if (Type == DataType.BOL) {//if ((ST.Value[0].ToLower() ?? "") == "bol") {
                        withBlock.DataType = Handlers.DataType.BOL;
                        if (((Data.ToLower() ?? "") == "true") || ((Data ?? "") == "1")) {
                            withBlock.Value = true; // .BOL = True
                        }
                        else if (((Data.ToLower() ?? "") == "false") || ((Data ?? "") == "0")) {
                            withBlock.Value = false; // .BOL = False
                        }
                        else {
                            withBlock.Value = false;// .BOL = False
                            Reporter(PostOutput, InterpreterName + @"Syntax Warning: " + MissingAssignmentMessage + ", defaulted to false", StatusType.Warning);
                        }
                    }
                    else if (Type == DataType.CHR) {//if ((ST.Value[0].ToLower() ?? "") == "chr") {
                        withBlock.DataType = Handlers.DataType.CHR;
                        if (IsString(Data) == true) {
                            string STRHOLD = Handlers.StringHandler.STR_RSE(SQ.Value[1]);
                            if (STRHOLD.Length >= 1)
                                withBlock.Value = STRHOLD[0];// .CHR = STRHOLD(0)
                        }
                        else {
                            Reporter(PostOutput, InterpreterName + @"Syntax Warning: " + MissingAssignmentMessage + ", defaulted to void", StatusType.Warning);
                        }
                    }
                    else if ((ST.Value[0].ToLower().StartsWith("e(")) && (ST.Value[0].EndsWith(")"))) {
                        string EnumName = ST.Value[0].Remove(ST.Value[0].Length - 1, 1);
                        EnumName = EnumName.Remove(0, 2);
                        withBlock.DataType = Handlers.DataType.INT;
                        int BoundingEnumeration = EnumerationToInteger(EnumName);
                        if (BoundingEnumeration >= 0) {
                            string STRHOLD = SQ.Value[1];
                            if (STRHOLD.Length >= 1) {
                                withBlock.Value = EnumerationValue(STRHOLD, BoundingEnumeration);
                            }
                            else {
                                withBlock.Value = 0;
                            }
                        }
                        else {
                            withBlock.Value = 0;
                            Reporter(PostOutput, InterpreterName + @"Syntax Warning: " + MissingAssignmentMessage + ", defaulted to 0", StatusType.Warning);
                        }
                    }
                    else if ((ST.Value[0].ToLower().StartsWith("a(")) && (ST.Value[0].EndsWith(")"))) {
                        string ArrayType = ST.Value[0].Remove(ST.Value[0].Length - 1, 1);
                        ArrayType = ArrayType.Remove(0, 2);
                        if (ArrayType != "") {
                            string STRHOLD = SQ.Value[1];
                            string HoldString = StringHandler.TrimStartFromFirstOccurance(Input, '=', false);
                            withBlock.DataType = StringToDataType(ArrayType);
                            withBlock.IsArray = true;
                            if (withBlock.DataType != DataType.NotVaild) {
                                if (HoldString.StartsWith("{")) {
                                    HoldString = HoldString.Remove(0, 1);
                                    CurrentArrayVariable = withBlock;
                                    if (HoldString.EndsWith("}")) {
                                        EnteredArray = false;
                                        HoldString = HoldString.Remove(HoldString.Length - 1, 1);
                                    }
                                    else {
                                        EnteredArray = true;
                                    }
                                    ProcessArrayObjects(HoldString);
                                    ArrayAlreadyEvaluated = true;
                                }
                                else {
                                    withBlock.IsArray = false;
                                    withBlock.DataType = DataType.VOID;
                                    Reporter(PostOutput, InterpreterName + @"Syntax Warning: Missing '{' for assignment, defaulted to type void", StatusType.Warning);
                                }
                            }
                            else {
                                withBlock.IsArray = false;
                                withBlock.DataType = DataType.VOID;
                                Reporter(PostOutput, InterpreterName + @"Syntax Warning: " + MissingAssignmentMessage + ", defaulted to type void", StatusType.Warning);
                            }
                        }
                        else {
                            withBlock.Value = 0;
                            Reporter(PostOutput, InterpreterName + @"Syntax Error: " + MissingArrayTypeMessage + ", not defined", StatusType.Error);
                        }
                    }
                    else {
                        throw new InvalidCastException("Syntax Error: Invalid data type! The type: '" + ST.Value[0].ToLower() + "' is not a recognized data type.");
                    }
                }
                if (EnteredArray == true) {
                    if (ArrayAlreadyEvaluated == false) {
                        ProcessArrayObjects(Input);
                    }
                }
                if (EnteredParm == false) {
                    CanDefine(VAL);
                    // VAR.Add(VAL);
                    //Reporter(PostOutput, InterpreterName + "Variable at DR0+" + VAR.Count.ToString(), StatusType.Information);
                }
                else if (PARM.Count > 0) {
                    PARM[PARM.Count - 1].VALUES.Add(VAL);
                    //Reporter(PostOutput, InterpreterName + "Variable at DPR1+" + PARM.Count.ToString(), StatusType.Information);
                }
                else {
                    throw new InvalidCastException("Critical Error: Parameter memory cannot be less than 1");
                }
            }
            catch (Exception ex) {
                Reporter(PostOutput, InterpreterName + "Syntax Error: Unable to declare variable or constant.", StatusType.Error);
                if (PARM.Count <= 0)
                    Reporter(PostOutput, InterpreterName + ex.Message, StatusType.Error);
            }
        }
        private static bool StringToBool(string Input) {
            if (Input.ToLower() == "false") { return false; }
            else if (Input.ToLower() == "0") { return false; }
            else if (Input.ToLower() == "1") { return true; }
            else if (Input.ToLower() == "true") { return true; }
            else { return false; }
        }
        private static void ProcessArrayObjects(string Input) {
            if (CurrentArrayVariable != null) {
                string Formatted = Input.Replace(Constants.Tab.ToString(), "");
                bool InString = false;
                string CurrentValue = "";
                bool JustReturnedFromString = false;
                bool JustStartedString = false;
                for (int i = 0; i < Formatted.Length; i++) {
                    if (Formatted[i] == Constants.DblQuote) {
                        if (InString == false) { InString = true; JustStartedString = true; }
                        else {
                            InString = false; JustReturnedFromString = true;
                        }
                    }
                    if (InString == false) {
                        if ((Formatted[i] != ' ') && (JustReturnedFromString == false)) {
                            CurrentValue += Formatted[i];
                            if (CurrentValue.EndsWith(",")) { CurrentValue = CurrentValue.Remove(CurrentValue.Length - 1, 1); }
                        }
                        if ((Formatted[i] == ',') || (i == Formatted.Length - 1)) {
                            JustReturnedFromString = false;
                            if (CurrentArrayVariable.DataType == DataType.BOL) {
                                CurrentArrayVariable.Value = StringToBool(CurrentValue); CurrentValue = "";
                            }
                            else if (CurrentArrayVariable.DataType == DataType.NotVaild) { CurrentValue = ""; }
                            else {
                                if ((CurrentArrayVariable.DataType == DataType.STR) && (CurrentValue == "")) {
                                    if (Formatted.Replace(" ", "") != "") {
                                        CurrentArrayVariable.Value = CurrentValue;
                                    }
                                }
                                else {
                                    if (CurrentValue != "") {
                                        CurrentArrayVariable.Value = CurrentValue;
                                    }
                                }
                                CurrentValue = "";
                            }
                        }
                    }
                    else {
                        if (JustStartedString == false) {
                            CurrentValue += Formatted[i];
                        }
                        else {
                            JustStartedString = false;
                        }
                    }

                }
            }
        }
        private static DataType StringToDataType(string Input) {
            if (Input.ToLower() == "int") { return DataType.INT; }
            else if (Input.ToLower() == "lng") { return DataType.LNG; }
            else if (Input.ToLower() == "str") { return DataType.STR; }
            else if (Input.ToLower() == "dec") { return DataType.DEC; }
            else if (Input.ToLower() == "bol") { return DataType.BOL; }
            else if (Input.ToLower() == "chr") { return DataType.CHR; }
            else if (Input.ToLower() == "bte") { return DataType.BTE; }
            else if (Input.ToLower() == "dbl") { return DataType.DBL; }
            else if (Input.ToLower() == "dte") { return DataType.DTE; }
            else if (Input.ToLower() == "sng") { return DataType.SNG; }
            else if (Input.ToLower() == "void") { return DataType.VOID; }
            return DataType.NotVaild;
        }
        private static void CanDefine(Variable Input) {
            bool NoReturn = true;
            foreach (Variable vr in VAR) {
                if (vr.Name == Input.Name) {
                    if (vr.DataType == Input.DataType) {
                        vr.Value = Input.Value;
                        NoReturn = false;
                        break;
                    }
                    else {
                        Reporter(PostOutput, InterpreterName + "Warning: Variable type mismatched! " + vr.DataType.ToString() + " != " + Input.DataType.ToString(), StatusType.Warning);
                        NoReturn = false;
                        break;
                    }
                }
            }
            if (NoReturn == true) {
                VAR.Add(Input);
                //Reporter(PostOutput, InterpreterName + "Variable at DR0+" + VAR.Count.ToString(), StatusType.Information);
            }
        }
        static bool EncounteredErrors = false;

        private static int EnumerationToInteger(string Name) {
            int ret = -1;
            for (int i = 0; i < ENUMERATION.Count; i++) {
                if (ENUMERATION[i].Name == Name) {
                    ret = i;
                    break;
                }
            }
            return ret;
        }
        private static int EnumerationValue(string Value, int Index) {
            if ((ENUMERATION.Count > 0) && (Index < ENUMERATION.Count)) {
                if (ENUMERATION[Index].Members.Count > 0) {
                    int ret = 0;
                    for (int i = 0; i < ENUMERATION[Index].Members.Count; i++) {
                        if (ENUMERATION[Index].Members[i] == Value) {
                            ret = i;
                            break;
                        }
                    }
                    return ret;
                }
                else {
                    return 0;
                }
            }
            else {
                return 0;
            }
        }
        private static bool IsString(string Input) {
            if (Input.StartsWith(Convert.ToString((char)34)) & Input.EndsWith(Convert.ToString((char)34)))
                return true;
            else
                return false;
        }
        private static ObjectType IsType(string input) {
            string BUILD = "";
            for (int I = 0, loopTo = input.Length - 1; I <= loopTo; I++) {
                if ((Convert.ToString(input[I]) == ",") || (Convert.ToString(input[I]) == ":"))
                    break;
                else
                    BUILD += Convert.ToString(input[I]);
            }
            if ((BUILD.ToLower() ?? "") == "def")
                return ObjectType.Type_Def;
            else if ((BUILD.ToLower() ?? "") == "op")
                return ObjectType.Type_Op;
            else if ((BUILD.ToLower() ?? "") == "class")
                return ObjectType.Type_Class;
            else if ((BUILD.ToLower() ?? "") == @"\e")
                return ObjectType.Type_OpRet;
            else if ((BUILD.ToLower() ?? "") == "func")
                return ObjectType.Type_Class;
            else if ((BUILD.ToLower() ?? "") == "enum")
                return ObjectType.Type_Enum;
            else
                return ObjectType.Type_Nothing;
        }
        private static bool IsComment(string input) {
            if (input.StartsWith("--"))
                return true;
            else
                return false;
        }
        public static object GetVariable(string Name, bool CheckType = false, DataType Type = DataType.STR) {
            try {
                if (CheckType == false) {
                    Variable config = VAR.Find(item => item.Name == Name);
                    return config.Value;
                }
                else {
                    Variable config = VAR.Find(item => item.Name == Name);
                    if (config != null) {
                        if (Type == config.DataType) {
                            return config.Value;
                        }
                        else {
                            return null;
                        }
                    }
                    else {
                        return null;
                    }
                }
            }
            catch {
                return null;
            }
        }
        public static object GetVariable(ParameterStructure ParStrc, string Name, bool CheckType = false, DataType Type = DataType.STR) {
            try {
                if (CheckType == false) {
                    Variable config = ParStrc.VALUES.Find(item => item.Name == Name);
                    return config.Value;
                }
                else {
                    Variable config = ParStrc.VALUES.Find(item => item.Name == Name);
                    if (config != null) {
                        if (Type == config.DataType) {
                            return config.Value;
                        }
                        else {
                            return null;
                        }
                    }
                    else {
                        return null;
                    }
                }
            }
            catch {
                return null;
            }
        }
        public static bool IsDefined(string VariableName, VariableStructureType CheckOnType = VariableStructureType.Variable) {
            bool Output = false;
            if ((CheckOnType == VariableStructureType.Variable) || (CheckOnType == VariableStructureType.All)) {
                foreach (Variable vr in VAR) {
                    if (vr.Name == VariableName) {
                        Output = true;
                        break;
                    }
                }
            }
            if ((CheckOnType == VariableStructureType.Parameter) || (CheckOnType == VariableStructureType.All)) {
                foreach (ParameterStructure pm in PARM) {
                    if (pm.Name == VariableName) {
                        Output = true;
                        break;
                    }
                }
            }
            return Output;
        }
        public static bool IsDefinedInParameter(string VariableName, ParameterStructure Parameter) {
            bool Output = false;
            foreach (Variable vr in Parameter.VALUES) {
                if (vr.Name == VariableName) {
                    Output = true;
                    break;
                }
            }
            return Output;
        }

        public static bool GetBooleanVariable(string Name, bool Default = false) {
            object obj = GetVariable(Name, true, DataType.BOL);
            if (obj != null) {
                return (bool)obj;
            }
            return Default;
        }
        public static bool GetBooleanVariable(ParameterStructure ParStrc, string Name, bool Default = false) {
            object obj = GetVariable(ParStrc, Name, true, DataType.BOL);
            if (obj != null) {
                return (bool)obj;
            }
            return Default;
        }
        public static int GetIntegerVariable(string Name, int Default = 0) {
            object obj = GetVariable(Name, true, DataType.INT);
            if (obj != null) {
                return (int)obj;
            }
            return Default;
        }
        public static int GetIntegerVariable(ParameterStructure ParStrc, string Name, int Default = 0) {
            object obj = GetVariable(ParStrc, Name, true, DataType.INT);
            if (obj != null) {
                return (int)obj;
            }
            return Default;
        }
        public static decimal GetDecimalVariable(string Name, decimal Default = 0) {
            object obj = GetVariable(Name, true, DataType.DEC);
            if (obj != null) {
                return (decimal)obj;
            }
            return Default;
        }
        public static decimal GetDecimalVariable(ParameterStructure ParStrc, string Name, decimal Default = 0) {
            object obj = GetVariable(ParStrc, Name, true, DataType.DEC);
            if (obj != null) {
                return (decimal)obj;
            }
            return Default;
        }
        public static string GetStringVariable(string Name, string Default = "") {
            object obj = GetVariable(Name, true, DataType.STR);
            if (obj != null) {
                return obj.ToString();
            }
            return Default;
        }
        public static string GetStringVariable(ParameterStructure ParStrc, string Name, string Default = "") {
            object obj = GetVariable(ParStrc, Name, true, DataType.STR);
            if (obj != null) {
                return obj.ToString();
            }
            return Default;
        }
        public static short GetSingleVariable(string Name, short Default = 0) {
            object obj = GetVariable(Name, true, DataType.SNG);
            if (obj != null) {
                return (short)obj;
            }
            return Default;
        }
        public static short GetSingleVariable(ParameterStructure ParStrc, string Name, short Default = 0) {
            object obj = GetVariable(ParStrc, Name, true, DataType.SNG);
            if (obj != null) {
                return (short)obj;
            }
            return Default;
        }
        public static byte GetByteVariable(string Name, byte Default = 0) {
            object obj = GetVariable(Name, true, DataType.BTE);
            if (obj != null) {
                return (byte)obj;
            }
            return Default;
        }
        public static byte GetByteVariable(ParameterStructure ParStrc, string Name, byte Default = 0) {
            object obj = GetVariable(ParStrc, Name, true, DataType.BTE);
            if (obj != null) {
                return (byte)obj;
            }
            return Default;
        }
        public static long GetLongVariable(string Name, long Default = 0) {
            object obj = GetVariable(Name, true, DataType.LNG);
            if (obj != null) {
                return (long)obj;
            }
            return Default;
        }
        public static long GetLongVariable(ParameterStructure ParStrc, string Name, long Default = 0) {
            object obj = GetVariable(ParStrc, Name, true, DataType.LNG);
            if (obj != null) {
                return (long)obj;
            }
            return Default;
        }
        public static double GetDoubleVariable(string Name, double Default = 0) {
            object obj = GetVariable(Name, true, DataType.DBL);
            if (obj != null) {
                return (double)obj;
            }
            return Default;
        }
        public static double GetDoubleVariable(ParameterStructure ParStrc, string Name, double Default = 0) {
            object obj = GetVariable(ParStrc, Name, true, DataType.DBL);
            if (obj != null) {
                return (double)obj;
            }
            return Default;
        }


        public static void Write(StreamWriter StrWriter, int Tabs, string Name, int Value) {
            string Assignment = "def,int:" + Name.Trim(' ').Trim('\t') + "=" + Value.ToString();
            StrWriter.WriteLine(StringHandler.AddTabs(Tabs, Assignment));
        }
        public static void Write(StreamWriter StrWriter, int Tabs, string Name, string Value) {
            string Assignment = "def,str:" + Name.Trim(' ').Trim('\t') + "=" + StringHandler.EncapsulateString(Value);
            StrWriter.WriteLine(StringHandler.AddTabs(Tabs, Assignment));
        }
        public static void WriteEntry(StreamWriter StrWriter, string EnumName) {
            string Assignment = EnumName.Trim(' ').Trim('\t');
            StrWriter.WriteLine("Begin,");
            StrWriter.WriteLine("Create Lines(" + Assignment + "), ");
        }
        public static void WriteComment(StreamWriter StrWriter, int Tabs, string Comment) {
            StrWriter.WriteLine(StringHandler.AddTabs(Tabs, "--" + Comment));
        }
        public static void Write(StreamWriter StrWriter, int Tabs, string Name, decimal Value) {
            string Assignment = "def,dec:" + Name.Trim(' ').Trim('\t') + "=" + Value.ToString();
            StrWriter.WriteLine(StringHandler.AddTabs(Tabs, Assignment));
        }
        public static void Write(StreamWriter StrWriter, int Tabs, string Name, long Value) {
            string Assignment = "def,lng:" + Name.Trim(' ').Trim('\t') + "=" + Value.ToString();
            StrWriter.WriteLine(StringHandler.AddTabs(Tabs, Assignment));
        }
        public static void Write(StreamWriter StrWriter, int Tabs, string Name, bool Value) {
            string Assignment = "def,bol:" + Name.Trim(' ').Trim('\t') + "=" + Value.ToString();
            StrWriter.WriteLine(StringHandler.AddTabs(Tabs, Assignment));
        }
        public static void Write(StreamWriter StrWriter, int Tabs, string Name, short Value) {
            string Assignment = "def,sng:" + Name.Trim(' ').Trim('\t') + "=" + Value.ToString();
            StrWriter.WriteLine(StringHandler.AddTabs(Tabs, Assignment));
        }
        public static void Write(StreamWriter StrWriter, int Tabs, string Name, byte Value) {
            string Assignment = "def,bte:" + Name.Trim(' ').Trim('\t') + "=" + Value.ToString();
            StrWriter.WriteLine(StringHandler.AddTabs(Tabs, Assignment));
        }
        public static void Write(StreamWriter StrWriter, int Tabs, string Name, double Value) {
            string Assignment = "def,dbl:" + Name.Trim(' ').Trim('\t') + "=" + Value.ToString();
            StrWriter.WriteLine(StringHandler.AddTabs(Tabs, Assignment));
        }
        public static void Write(StreamWriter StrWriter, int Tabs, string Name) {
            string Assignment = "def,parm:" + Name.Trim(' ').Trim('\t') + "{";
            StrWriter.WriteLine(StringHandler.AddTabs(Tabs, Assignment));
        }


    }
    public class ParameterStructure {
        public string Name;
        public List<Handlers.Variable> VALUES = new List<Variable>();
        public object GetVariable(string Name, bool CheckType = false, DataType Type = DataType.STR) {
            try {
                if (CheckType == false) {
                    Variable config = VALUES.Find(item => item.Name == Name);
                    if (config == null) {
                        return null;
                    }
                    return config.Value;
                }
                else {
                    Variable config = VALUES.Find(item => item.Name == Name);
                    if (config != null) {
                        if (Type == config.DataType) {
                            return config.Value;
                        }
                        else {
                            return null;
                        }
                    }
                    else {
                        return null;
                    }
                }
            }
            catch {
                return null;
            }
        }
    }
    public enum ObjectType {
        Type_Class = 0,
        Type_Def = 1,
        Type_Op = 2,
        Type_Func = 3,
        Type_Nothing = 4,
        Type_OpRet = 5,
        Type_Comment = 6,
        Type_Line = 7,              //Used for non empty lines
        Type_Enum = 8
    }
    public enum ControlFlowType {
        Type_If = 0,
        Type_While = 1,
        Type_For = 2,
        Type_Try = 3,
        Type_Error = 4
    }
    public enum TYPEOP {
        Type_If = 0,
        Type_While = 1,
        Type_For = 2,
        Type_Try = 3,
        Type_Error = 4
    }
    public enum TYPEOBJ {
        Type_Class = 0,
        Type_Def = 1,
        Type_Op = 2,
        Type_Func = 3,
        TYPE_Nothing = 4,
        Type_OpRet = 5,
        Type_Enum = 6
    }
    public enum VariableStructureType {
        Variable = 0x01,
        Parameter = 0x02,
        All = 0xFF
    }
    public class Variable {
        public string Name;
        public string Ref;
        public string Scope;
        public DataType DataType;
        private int INT = 0;
        private string STR = "";
        private char CHR;
        private decimal DEC = 0;
        // private DateTime DTE;
        private float SNG = 0;
        private double DBL = 0;
        private long LNG = 0;
        private bool BOL = false;
        private byte BTE;
        public bool IsArray = false;

        List<int> INT_LST = new List<int>();
        List<string> STR_LST = new List<string>();
        List<char> CHR_LST = new List<Char>();
        List<decimal> DEC_LST = new List<decimal>();
        List<float> SNG_LST = new List<float>();
        List<double> DBL_LST = new List<double>();
        List<long> LNG_LST = new List<long>();
        List<bool> BOL_LST = new List<bool>();
        List<byte> BTE_LST = new List<byte>();
        public object Value {
            get {
                if (DataType == DataType.INT) {
                    if (IsArray) {
                        return INT_LST;
                    }
                    else { return INT; }
                }
                else if (DataType == DataType.BOL) {
                    if (IsArray) {
                        return BOL_LST;
                    }
                    else { return BOL; }
                }
                else if (DataType == DataType.DEC) {
                    if (IsArray) {
                        return DEC_LST;
                    }
                    else { return DEC; }
                }
                else if (DataType == DataType.CHR) {
                    if (IsArray) {
                        return CHR_LST;
                    }
                    else { return CHR; }
                }
                else if (DataType == DataType.STR) {
                    if (IsArray) {
                        return STR_LST;
                    }
                    else { return STR; }
                }
                else if (DataType == DataType.LNG) {
                    if (IsArray) {
                        return LNG_LST;
                    }
                    else { return LNG; }
                }
                else if (DataType == DataType.SNG) {
                    if (IsArray) {
                        return SNG_LST;
                    }
                    else { return SNG; }
                }
                else if (DataType == DataType.DBL) {
                    if (IsArray) {
                        return DBL_LST;
                    }
                    else { return DBL; }
                }
                else if (DataType == DataType.VOID) {
                    return null;
                }
                else {
                    return null;
                }
            }
            set {
                if (DataType == DataType.INT) {
                    if (IsArray) {
                        INT_LST.Add(Convert.ToInt32(value));
                    }
                    else { INT = Convert.ToInt32(value); }
                }
                else if (DataType == DataType.BOL) {
                    if (IsArray) {
                        BOL_LST.Add(Convert.ToBoolean(value));
                    }
                    else { BOL = Convert.ToBoolean(value); }
                }
                else if (DataType == DataType.DEC) {
                    if (IsArray) {
                        DEC_LST.Add(Convert.ToDecimal(value));
                    }
                    else { DEC = Convert.ToDecimal(value); }
                }
                else if (DataType == DataType.CHR) {
                    if (IsArray) {
                        CHR_LST.Add(Convert.ToChar(value));
                    }
                    else { CHR = Convert.ToChar(value); }
                }
                else if (DataType == DataType.STR) {
                    if (IsArray) {
                        STR_LST.Add(Convert.ToString(value));
                    }
                    else { STR = Convert.ToString(value); }
                }
                else if (DataType == DataType.LNG) {
                    if (IsArray) {
                        LNG_LST.Add(Convert.ToInt64(value));
                    }
                    else { LNG = Convert.ToInt64(value); }
                }
                else if (DataType == DataType.SNG) {
                    if (IsArray) {
                        SNG_LST.Add(Convert.ToSingle(value));
                    }
                    else { SNG = Convert.ToSingle(value); }
                }
                else if (DataType == DataType.DBL) {
                    if (IsArray) {
                        DBL_LST.Add(Convert.ToDouble(value));
                    }
                    else { DBL = Convert.ToDouble(value); }
                }
                else if (DataType == DataType.BTE) {
                    if (IsArray) {
                        BTE_LST.Add(Convert.ToByte(value));
                    }
                    else { BTE = Convert.ToByte(value); }
                }
            }
        }
    }
    public class DEFINES {
        public string NAME = "";
        public Color COLOUR;
    }
    public class PROPS {
        public string NAME = "";
        public Color COLOUR;
    }
    // Public Class VARS
    // Public NAME As String = ""
    // Public TYPE As DataType
    // Public INT As Integer = 0
    // Public DEC As Decimal = 0
    // Public CHR As Char
    // Public STR As String = ""
    // Public BOL As Boolean = False
    // Public REF As String = ""
    // Public LNG As Long = 0
    // End Class
    public class VARS {
        public string Name;
        public string Ref;
        public DataType DType;
        private int INT = 0;
        private string STR = "";
        private char CHR;
        private decimal DEC = 0;
        //private DateTime DTE;
        private float SNG = 0;
        private double DBL = 0;
        private long LNG = 0;
        private bool BOL = false;
        private byte BTE;
        public object Value {
            get {
                if (DType == (int)DataType.INT)
                    return INT;
                else if ((int)DType == (int)DataType.BOL)
                    return BOL;
                else if ((int)DType == (int)DataType.DEC)
                    return DEC;
                else if ((int)DType == (int)DataType.CHR)
                    return CHR;
                else if ((int)DType == (int)DataType.STR)
                    return STR;
                else if ((int)DType == (int)DataType.LNG)
                    return LNG;
                else if ((int)DType == (int)DataType.SNG)
                    return SNG;
                else if ((int)DType == (int)DataType.DBL)
                    return DBL;
                else
                    return null;
            }
            set {
                if (DType == (int)DataType.INT)
                    INT = Convert.ToInt32(value);
                else if ((int)DType == (int)DataType.BOL)
                    BOL = Convert.ToBoolean(value);
                else if ((int)DType == (int)DataType.DEC)
                    DEC = Convert.ToDecimal(value);
                else if ((int)DType == (int)DataType.CHR)
                    CHR = Convert.ToChar(value);
                else if ((int)DType == (int)DataType.STR)
                    STR = Convert.ToString(value);
                else if ((int)DType == (int)DataType.LNG)
                    LNG = Convert.ToInt64(value);
                else if ((int)DType == (int)DataType.SNG)
                    SNG = Convert.ToSingle(value);
                else if ((int)DType == (int)DataType.DBL)
                    DBL = Convert.ToDouble(value);
                else if ((int)DType == (int)DataType.BTE)
                    BTE = Convert.ToByte(value);
            }
        }
    }

    public enum DataType {
        INT = 0,
        DEC = 1,
        STR = 2,
        CHR = 3,
        BOL = 4,
        LNG = 5,
        DBL = 6,
        SNG = 7,
        DTE = 8,
        BTE = 9,
        VOID = 10,
        NotVaild = 0XFF
    }
}
public class Enum {
    public string Name = "";
    public List<string> Members = new List<string>();
}

