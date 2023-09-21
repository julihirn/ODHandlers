// '======================================
// MATH HANDLER
// ONE DESKTOP COMPONENTS 
// JULIAN HIRNIAK
// COPYRIGHT (C) 2014-2023 J.J.HIRNIAK
// '======================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;
using System.Numerics;
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;
using System.Globalization;
using System.Drawing.Drawing2D;

namespace Handlers {
    public class MathHandler {
        public enum Operators {
            None = 0x00,
            Addition = 0x01,
            Multiplication = 0x02,
            Division = 0x03,
            Power = 0x04,
            Remainder = 0x05
        }
        public enum BooleanOperators {
            None = 0x00,
            Or = 0x01,
            And = 0x02,
            Xor = 0x03
        }
        #region Boolean Equation Evaluation
        public static bool EvaluateBooleanExpression(string Expression, List<BooleanVariable> Variables) {
            if (Variables == null) {
                Variables = new List<BooleanVariable>();
            }
            string Exp = FormatBooleanExpression(Expression);
            if (Exp.Contains("(")) {
                while (Exp.Contains("(")) {
                    List<MathTextLocation> EvaluatedBrackets = EvaluateBooleanBottomBrackets(Exp, Variables);
                    if (EvaluatedBrackets.Count > 0) {
                        EvaluatedBrackets.Sort((x, y) => x.StartIndex.CompareTo(y.StartIndex));
                        for (int i = EvaluatedBrackets.Count - 1; i >= 0; i--) {
                            Exp = Exp.Remove(EvaluatedBrackets[i].StartIndex, EvaluatedBrackets[i].Length);
                            Exp = Exp.Insert(EvaluatedBrackets[i].StartIndex, ConversionHandler.BoolToString((bool)EvaluatedBrackets[i].Value));
                        }
                    }
                    else { break; }
                }
                return EvaluateSmallBooleanExpression(Exp, Variables);
            }
            else {
                return EvaluateSmallBooleanExpression(Exp, Variables);
            }
        }
        private static List<MathTextLocation> EvaluateBooleanBottomBrackets(string Expression, List<BooleanVariable> Variables) {
            int CurrentDepth = 0;
            int MaxDepth = 0;
            List<MathTextLocation> Markers = new List<MathTextLocation>();
            List<MathTextLocation> Output = new List<MathTextLocation>();
            int MarkIn = 0;
            for (int i = 0; i < Expression.Length; i++) {
                char CEC = Expression[i];
                if (CEC == '(') { CurrentDepth++; MarkIn = i; }
                else if (CEC == ')') { Markers.Add(new MathTextLocation(MarkIn, i, CurrentDepth)); CurrentDepth--; }
                if (CurrentDepth >= MaxDepth) { MaxDepth = CurrentDepth; }
            }
            Markers.Sort((x, y) => x.Depth.CompareTo(y.Depth));
            for (int i = Markers.Count - 1; i >= 0; i--) {
                if (Markers[i].Depth == MaxDepth) {
                    string ExpCurrent = "";
                    for (int j = Markers[i].StartIndex + 1; j < Markers[i].EndIndex; j++) {
                        ExpCurrent += Expression[j];
                    }
                    Output.Add(new MathTextLocation(Markers[i].StartIndex, Markers[i].EndIndex, Markers[i].Depth, EvaluateSmallBooleanExpression(ExpCurrent, Variables, false)));
                }
                else { break; }
            }
            return Output;
        }
        private static bool EvaluateSmallBooleanExpression(string Expression, List<BooleanVariable> Variables, bool InitialValue = false) {
            BooleanOperators NextOperator = BooleanOperators.None;
            BooleanOperators CurrentOperator = BooleanOperators.None;
            bool OutputStr = InitialValue;
            string CurrentString = "";
            int CharCount = 0;
            for (int i = 0; i < Expression.Length; i++) {
                CurrentOperator = NextOperator;
                if (Expression[i] == '^') { NextOperator = BooleanOperators.Xor; CharCount = 0; }
                else if (Expression[i] == '&') { NextOperator = BooleanOperators.And; CharCount = 0; }
                else if (Expression[i] == '*') { NextOperator = BooleanOperators.And; CharCount = 0; }
                else if (Expression[i] == '+') { NextOperator = BooleanOperators.Or; CharCount = 0; }
                else if (Expression[i] == '|') { NextOperator = BooleanOperators.Or; CharCount = 0; }
                else {
                    CurrentString += Expression[i];
                    CharCount++;
                    if (i == Expression.Length - 1) {
                        CharCount = 0;
                    }
                }
                if (CharCount == 0) {
                    if (ConversionHandler.IsBoolean(CurrentString)) {
                        OutputStr = PerformBooleanOperation(CurrentOperator, CurrentString.Replace('−', '~').Replace('−', '~'), OutputStr);
                    }
                    else {
                        //IsAVariable
                        if (Variables.Count > 0) {
                            foreach (BooleanVariable var in Variables) {
                                if (var.Name == CurrentString) {
                                    OutputStr = PerformBooleanOperation(CurrentOperator, ConversionHandler.BoolToString((bool)var.Value), OutputStr);
                                    break;
                                }
                            }
                        }
                    }
                    CurrentString = "";
                }
            }
            return OutputStr;
        }

        public static bool PerformBooleanOperation(BooleanOperators Operation, string Input, bool InitialValue) {
            bool InputChng = ConversionHandler.StringToBool(Input);
            bool Output = InitialValue;
            try {
                if ((Operation == BooleanOperators.None) || (Operation == BooleanOperators.Or)) {
                    Output |= InputChng;
                }
                else if (Operation == BooleanOperators.And) {
                    Output &= InputChng;
                }
                else if (Operation == BooleanOperators.Xor) {
                    Output ^= InputChng;
                }
            }
            catch { Output = false; }
            return Output;
        }
        #endregion
        #region Decimal Real Equation Evaluation
        public static NumericalString EvaluateExpression(string Expression, List<MathVariable> Variables) {
            if (Variables == null) {
                Variables = new List<MathVariable>();
            }
            Variables.Add(new MathVariable("pi", "3.14159265358979323846264338327950288419716939937510582097494459230781640628620899862803482534211706798214"));
            Variables.Add(new MathVariable("e", Math.E));
            string Exp = FormatExpression(Expression);
            if (Exp.Contains("(")) {
                while (Exp.Contains("(")) {
                    List<MathTextLocation> EvaluatedBrackets = EvaluateBottomBrackets(Exp, Variables);
                    if (EvaluatedBrackets.Count > 0) {
                        EvaluatedBrackets.Sort((x, y) => x.StartIndex.CompareTo(y.StartIndex));
                        for (int i = EvaluatedBrackets.Count - 1; i >= 0; i--) {
                            Exp = Exp.Remove(EvaluatedBrackets[i].StartIndex, EvaluatedBrackets[i].Length);
                            Exp = Exp.Insert(EvaluatedBrackets[i].StartIndex, ((NumericalString)EvaluatedBrackets[i].Value).ToString());
                            //Exp = Exp.Insert(EvaluatedBrackets[i].StartIndex, String.Format("{0:F20}", EvaluatedBrackets[i].Value));
                        }
                    }
                    else {
                        break;
                    }
                }
                return (NumericalString)EvaluateSmallExpression(Exp, Variables);
            }
            else {
                return (NumericalString)EvaluateSmallExpression(Exp, Variables);
            }
        }
        private static List<MathTextLocation> EvaluateBottomBrackets(string Expression, List<MathVariable> Variables) {
            int CurrentDepth = 0;
            int MaxDepth = 0;
            List<MathTextLocation> Markers = new List<MathTextLocation>();
            List<MathTextLocation> Output = new List<MathTextLocation>();
            int MarkIn = 0;
            for (int i = 0; i < Expression.Length; i++) {
                char CEC = Expression[i];
                if (CEC == '(') { CurrentDepth++; MarkIn = i; }
                else if (CEC == ')') { Markers.Add(new MathTextLocation(MarkIn, i, CurrentDepth)); CurrentDepth--; }
                if (CurrentDepth >= MaxDepth) { MaxDepth = CurrentDepth; }
            }
            Markers.Sort((x, y) => x.Depth.CompareTo(y.Depth));
            for (int i = Markers.Count - 1; i >= 0; i--) {
                if (Markers[i].Depth == MaxDepth) {
                    string ExpCurrent = "";
                    for (int j = Markers[i].StartIndex + 1; j < Markers[i].EndIndex; j++) {
                        ExpCurrent += Expression[j];
                    }
                    Output.Add(new MathTextLocation(Markers[i].StartIndex, Markers[i].EndIndex, Markers[i].Depth, EvaluateSmallExpression(ExpCurrent, Variables, null)));
                }
                else { break; }
            }
            return Output;
        }
        private static object EvaluateSmallExpression(string Expression, List<MathVariable> Variables, object InitialValue = null) {
            Operators NextOperator = Operators.None;
            Operators CurrentOperator = Operators.None;

            NumericalString OutputStr = new NumericalString(InitialValue);
            //double Output = double.Parse(OutputStr.ToString());
            string CurrentString = "";
            int CharCount = 0;
            for (int i = 0; i < Expression.Length; i++) {
                CurrentOperator = NextOperator;
                if (Expression[i] == '^') { NextOperator = Operators.Power; CharCount = 0; }
                else if (Expression[i] == '/') { NextOperator = Operators.Division; CharCount = 0; }
                else if (Expression[i] == '*') { NextOperator = Operators.Multiplication; CharCount = 0; }
                else if (Expression[i] == '+') { NextOperator = Operators.Addition; CharCount = 0; }
                else if (Expression[i] == '%') { NextOperator = Operators.Remainder; CharCount = 0; }
                else {
                    CurrentString += Expression[i];
                    CharCount++;
                    if (i == Expression.Length - 1) {
                        CharCount = 0;
                    }
                }
                if (CharCount == 0) {
                    if (ConversionHandler.IsNumeric(CurrentString)) {
                        //OutputStr = PerformOperation(CurrentOperator, Convert.ToDouble(CurrentString.Replace('−', '-')), OutputStr);
                        OutputStr = PerformOperation(CurrentOperator, CurrentString.Replace('−', '-'), OutputStr);
                    }
                    else {
                        //IsAVariable
                        if (Variables.Count > 0) {
                            foreach (MathVariable var in Variables) {
                                if (var.Name == CurrentString) {
                                    OutputStr = PerformOperation(CurrentOperator, var.Value, OutputStr);
                                    break;
                                }
                            }
                        }
                    }
                    CurrentString = "";
                }
            }
            return OutputStr;
        }
        public static NumericalString PerformOperation(Operators Operation, object Input, object InitialValue) {
            NumericalString InputChng = new NumericalString(Input.ToString());
            NumericalString Output = new NumericalString(InitialValue);
            try {
                if ((Operation == Operators.None) || (Operation == Operators.Addition)) {
                    Output += InputChng;
                }
                else if (Operation == Operators.Multiplication) {
                    Output *= InputChng;
                }
                else if (Operation == Operators.Division) {
                    Output /= InputChng;
                }
                else if (Operation == Operators.Power) {
                    Output = Output ^ InputChng;
                }
                else if (Operation == Operators.Remainder) {
                    Output = Output % InputChng;
                }
            }
            catch { Output.Value = 0; }
            return Output;
        }
        public static double PerformOperationQuick(Operators Operation, double Input, double InitialValue) {
            double Output = InitialValue;
            try {
                if ((Operation == Operators.None) || (Operation == Operators.Addition)) {
                    Output += Input;
                }
                else if (Operation == Operators.Multiplication) {
                    Output *= Input;
                }
                else if (Operation == Operators.Division) {
                    Output /= Input;
                }
                else if (Operation == Operators.Power) {
                    Output = Math.Pow(Output, Input);
                }
            }
            catch { Output = 0; }
            if (Double.IsInfinity(Output)) {
                Output = 0;
            }
            return Output;
        }
        #endregion
        #region Equation Support Functions
        #region Support Functions
        public static bool IsNumericalDataType(object Input) {
            if (Input.GetType() == typeof(int)) { return true; }
            else if (Input.GetType() == typeof(uint)) { return true; }
            else if (Input.GetType() == typeof(byte)) { return true; }
            else if (Input.GetType() == typeof(sbyte)) { return true; }
            else if (Input.GetType() == typeof(short)) { return true; }
            else if (Input.GetType() == typeof(ushort)) { return true; }
            else if (Input.GetType() == typeof(long)) { return true; }
            else if (Input.GetType() == typeof(ulong)) { return true; }
            else if (Input.GetType() == typeof(float)) { return true; }
            else if (Input.GetType() == typeof(double)) { return true; }
            else if (Input.GetType() == typeof(decimal)) { return true; }
            else if (Input.GetType() == typeof(double)) { return true; }
            else { return false; }
        }
        #endregion 
        private static string FormatExpression(string Input) {
            string Exp = Input.Replace(" ", "").Replace(((char)0x09).ToString(), "");
            string Output = "";
            for (int i = 0; i < Exp.Length; i++) {
                char CEC = Exp[i];
                if (i > 0) {
                    if (CEC == '-') {
                        if ((Exp[i - 1] == '/') || (Exp[i - 1] == '*') || (Exp[i - 1] == '+') || (Exp[i - 1] == '^') || (Exp[i - 1] == '(')) {
                        }
                        else {
                            Output += "+";
                        }
                    }
                }
                Output += CEC;
            }
            return Output;
        }
        private static string FormatBooleanExpression(string Input) {
            if (Input == null) { Input = ""; }
            string Exp = Input.Replace(" ", "").Replace(((char)0x09).ToString(), "");
            string Output = "";
            for (int i = 0; i < Exp.Length; i++) {
                char CEC = Exp[i];
                if (i > 0) {
                    if (CEC == '~') {
                        if ((Exp[i - 1] == '|') || (Exp[i - 1] == '&') || (Exp[i - 1] == '*') || (Exp[i - 1] == '+') || (Exp[i - 1] == '^') || (Exp[i - 1] == '(')) {
                        }
                        else {
                            Output += "+";
                        }
                    }
                }
                Output += CEC;
            }
            return Output;
        }
        public static List<string> ExtractVariablesFromExpression(string Input) {
            string TempExpession = RemoveAllSymbols(Input).Replace(Constants.Tab.ToString(), "");
            STR_MVSSF SpiltValues = StringHandler.SpiltStringMutipleValues(TempExpession, ' ');
            List<string> Output = new List<string>();
            foreach (string Vr in SpiltValues.Value) {
                if (ConversionHandler.IsNumeric(Vr) == false) {
                    if (Vr.Length != 0) {
                        if (Output.Contains(Vr) == false) {
                            Output.Add(Vr);
                        }
                    }
                }
            }
            return Output;
        }

        #endregion

        public static void ModifyMathVariable(List<MathVariable> Variables, string VariableName, NumericalString Value) {
            foreach (MathVariable Var in Variables) {
                if (Var.Name == VariableName) {
                    Var.Value = Value;
                    break;
                }
            }
        }
        public static string SolveExpression(string Expression, string SolveFor) {
            if (IsVaildExpression(Expression) == true) {
                //  Equation 
                return "";
            }
            else {
                return "";
            }
        }
        private static string RemoveAllSymbols(string Expression) {
            string Output = Expression.Replace("+", " ");
            Output = Output.Replace("-", " ");
            Output = Output.Replace("*", " ");
            Output = Output.Replace("/", " ");
            Output = Output.Replace("^", " ");
            Output = Output.Replace("(", "");
            Output = Output.Replace(")", "");
            return Output;
        }
        //private static bool ContainsVariable(string Expression, string Variable, bool ContainsOnlyVariable = false) {
        //    string StringUnderTest = RemoveAllSymbols(Expression);
        //    if (StringUnderTest.Contains(Variable)) {
        //        if (ContainsOnlyVariable == true) {
        //            bool Result = false;
        //            bool InString = false;
        //            for (int i =0; i<StringUnderTest.Length; i++) {
        //                if (StringUnderTest[i] != ' ') {
        //                    for (int j=0; j<Variable.Length; j++) {
        //                        if (Variable[j] == StringUnderTest[i]) {

        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else { return true; }
        //    }
        //    else { return false;}
        //}
        #region Equation Decomposition
        private static bool IsVaildExpression(string Expression) {
            if (Expression.Contains("=")) {
                int EqualsSigns = 0;
                for (int i = 0; i < Expression.Length; i++) {
                    if (Expression[i] == '=') {
                        EqualsSigns++;
                    }
                }
                if (EqualsSigns == 1) { return true; }
                else { return false; }
            }
            else { return false; }
        }
        public static List<DualSideExpression> DecomposeEquation(string Expression, bool DecomposeOnBraces = false) {
            SubExpression = 0;
            List<DualSideExpression> Output = new List<DualSideExpression>();
            if (Expression == null) { return Output; }
            if (Expression == "") { return Output; }
            List<BracketedEquation> SubExpressions = new List<BracketedEquation>();
            string WorkingExpression = Expression;

            string DecomposeOn = "(";
            string RightBracket = ")";
            if (DecomposeOnBraces == true) {
                DecomposeOn = "{";
                RightBracket = "}";
            }
            while ((WorkingExpression.Contains(DecomposeOn)) && (WorkingExpression.Contains(RightBracket))) {
                BracketedEquation CurrentSubExpression = DecomposeBracketedEquation(WorkingExpression, DecomposeOnBraces);
                if (CurrentSubExpression != null) {
                    WorkingExpression = WorkingExpression.Remove(CurrentSubExpression.Location.StartIndex, CurrentSubExpression.Location.Length);
                    WorkingExpression = WorkingExpression.Insert(CurrentSubExpression.Location.StartIndex, CurrentSubExpression.Name);
                    SubExpressions.Add(CurrentSubExpression);
                }
            }
            foreach (BracketedEquation CurrentSubExpression in SubExpressions) {
                Output.Add(new DualSideExpression(CurrentSubExpression.Name, CurrentSubExpression.Location.Value.ToString().Replace(DecomposeOn, "").Replace(RightBracket, "")));
            }
            Output.Add(new DualSideExpression("OUT", WorkingExpression));
            return Output;
        }
        private static int GetExpressionMaximumBracketDepth(string Expression, bool DecomposeOnBraces = false) {
            int CurrentLevel = 0;
            int CurrentMax = 0;
            char LeftBracket = '(';
            char RightBracket = ')';

            if (DecomposeOnBraces == true) {
                LeftBracket = '{';
                RightBracket = '}';
            }
            for (int i = 0; i < Expression.Length; i++) {
                if (Expression[i] == LeftBracket) {
                    CurrentLevel++;
                    if (CurrentLevel > CurrentMax) { CurrentMax = CurrentLevel; }
                }
                else if (Expression[i] == RightBracket) { CurrentLevel--; }
            }
            return CurrentMax;
        }
        static int SubExpression = 0;
        private static BracketedEquation DecomposeBracketedEquation(string Expression, bool DecomposeOnBraces = false) {
            int CurrentLevel = 0;
            int CurrentMax = GetExpressionMaximumBracketDepth(Expression, DecomposeOnBraces);
            int StartIndex = -1;
            int EndIndex = -1;

            char LeftBracket = '(';
            char RightBracket = ')';

            if (DecomposeOnBraces == true) {
                LeftBracket = '{';
                RightBracket = '}';
            }
            for (int i = 0; i < Expression.Length; i++) {
                if (Expression[i] == LeftBracket) {
                    CurrentLevel++;
                    if (CurrentLevel == CurrentMax) { StartIndex = i; }
                }
                else if (Expression[i] == RightBracket) {
                    if (CurrentLevel == CurrentMax) { EndIndex = i; break; }
                    CurrentLevel--;
                }
            }
            if (StartIndex == EndIndex) { return null; }
            else {
                SubExpression++;
                MathTextLocation BracketExpression = new MathTextLocation(StartIndex, EndIndex, CurrentMax, StringHandler.ExtractStringBetweenIndices(Expression, StartIndex, EndIndex));
                BracketedEquation Output = new BracketedEquation(BracketExpression, "EXPSUBv" + SubExpression.ToString());
                return Output;
            }
        }
        private static bool IsMathsSymbol(char TestCharacter, bool AllowBracketing = false) {
            switch (TestCharacter) {
                case '+': return true;
                case '-': return true;
                case '*': return true;
                case '/': return true;
                case '(': return AllowBracketing;
                case ')': return AllowBracketing;
                default: return false;
            }
        }
        public static List<ExpressionFormat> DecomposeRquationToScriptTypes(string Expression) {
            List<DualSideExpression> Eq = MathHandler.DecomposeEquation(Expression, true);

            List<ExpressionFormat> Output = new List<ExpressionFormat>();

            ExpressionScriptType CurrentType = ExpressionScriptType.Normal;
            ExpressionScriptType LastType = ExpressionScriptType.Normal;
            if (Eq.Count == 0) { return Output; }
            string RunningString = "";
            int Length = Eq[Eq.Count - 1].Right.Length;
            for (int i = 0; i < Length; i++) {
                char TestCharacter = Eq[Eq.Count - 1].Right[i];
                bool PrintCharacter = true;

                if (TestCharacter == '_') {
                    PrintCharacter = false;
                    LastType = CurrentType;
                    if (RunningString.Trim(' ') != "") {
                        Output.Add(new ExpressionFormat(RunningString, CurrentType));
                        RunningString = "";
                    }
                    CurrentType = ExpressionScriptType.Subscript;
                }
                else if (TestCharacter == '^') {
                    PrintCharacter = false;
                    LastType = CurrentType;
                    if (RunningString.Trim(' ') != "") {
                        Output.Add(new ExpressionFormat(RunningString, CurrentType));
                        RunningString = "";
                    }
                    CurrentType = ExpressionScriptType.Superscript;
                }
                else { PrintCharacter = true; }
                if (PrintCharacter == true) {
                    if (IsMathsSymbol(TestCharacter, true)) {

                        if (RunningString.Trim(' ') != "") {
                            Output.Add(new ExpressionFormat(RunningString, CurrentType));
                            RunningString = "";
                        }
                        LastType = CurrentType;
                        CurrentType = ExpressionScriptType.Normal;
                        RunningString += TestCharacter;
                    }
                    else {
                        RunningString += TestCharacter;
                        if (i == Length - 1) {
                            Output.Add(new ExpressionFormat(RunningString, CurrentType));
                            RunningString = "";
                        }
                    }
                }
            }
            if (Eq.Count > 1) {
                for (int i = Output.Count - 1; i >= 0; i--) {
                    for (int j = 0; j < Eq.Count - 1; j++) {
                        if (Output[i].Text == Eq[j].Left) {
                            Output[i].Text = Eq[j].Right;
                        }
                        else {
                            if (Output[i].Text.Contains(Eq[j].Left)) {
                                string OldText = Output[i].Text;
                                Output[i].Text = OldText.Replace(Eq[j].Left, "");
                                List<ExpressionFormat> Temp = DecomposeRquationToScriptTypes(Eq[j].Right);
                                Output.InsertRange(i + 1, Temp);
                            }
                        }
                    }
                }
            }

            return Output;
        }
        #endregion
        /*
         *                to
         *          02  08  10  16
         *      02  --  y   y   y
         *      08  y   __  y   y 
         *      10      y   __  y
         *      16  y   y   y   __
         * 
         */
        #region To Base 2 Conversion Functions
        //To Binary
        public static string OctalToBinary(string Data) {
            string Temp = "";
            Data = Data.Replace(" ", "");
            Data = Data.Replace(Constants.Tab.ToString(), "");
            Data = Data.ToUpper();
            if (IsOct(Data)) {
                if (Data.Length >= 1) {
                    for (int i = 0; i < Data.Length; i++) {
                        Temp += OctValueToBinaryString(Data[i]);
                    }
                }
                else { return "0"; }
            }
            else { return "0"; }
            Temp = Temp.TrimStart('0');
            if (Temp.Length == 0) { Temp = "0"; }
            return Temp;
        }
        public static string HexadecimalToBinary(string Data) {
            string Temp = "";
            Data = Data.Replace(" ", "");
            Data = Data.Replace(Constants.Tab.ToString(), "");
            Data = Data.ToUpper();
            if (IsHex(Data)) {
                if (Data.Length >= 1) {
                    for (int i = 0; i < Data.Length; i++) {
                        Temp += HexValueToBinaryString(Data[i]);
                    }
                }
                else { return "0"; }
            }
            else { return "0"; }
            Temp = Temp.TrimStart('0');
            if (Temp.Length == 0) { Temp = "0"; }
            return Temp;
        }
        public static string DecimalToBinary(string Data, BinaryFormatFlags FormatFlags) {
            bool IsSigned = ((int)FormatFlags & 0xF0000) == 0x10000 ? true : false;
            bool IsDecimal = ((int)FormatFlags & 0x0F000) == 0x01000 ? true : false;
            NumericalString DecimalData = new NumericalString(Data);
            if (IsDecimal == false) {
                int WordLength = BinaryFormatFlagsToLength(FormatFlags);
                return DecimalIntegerToBinary(new NumericalString(Data), WordLength, IsSigned);
            }
            else {

            }
            return "0";
        }
        #endregion
        private static string DecimalIntegerToBinary(NumericalString DecimalData, int BitSize, bool IsSigned) {
            string NonSignedInt = DecimalData.Abs().GetIntergralComponent();
            if ((IsSigned == true) && (DecimalData.IsNegitive() == true)) {
                NumericalString Complement = EvaluateExpression("(2^" + BitSize.ToString() + ")+" + DecimalData.GetIntergralComponent(), null);
                NonSignedInt = Complement.GetIntergralComponent();
            }
            NumericalString NubString = new NumericalString(NonSignedInt);
            NumericalString Zero = new NumericalString(0);
            string BinaryResult = "";
            while (NubString != Zero) {
                NumericalString DivResult = NubString / 2;
                NumericalString ModResult = NubString % 2;
                NubString.Value = DivResult.GetIntergralComponent();
                BinaryResult = ModResult.GetIntergralComponent() + BinaryResult;
            }
            BinaryResult = BinaryResult.TrimStart('0');
            if ((IsSigned == true) && (DecimalData.IsNegitive() == true)) {
                //int ExtraDigits = BitSize - BinaryResult.Length;
                BinaryResult = StringHandler.PadWithCharacter(BinaryResult, BitSize, '1', false);
            }
            if (BinaryResult.Length == 0) {
                BinaryResult = "0";
            }
            return BinaryResult;
        }
        #region To Base 8 Conversion Functions
        //To Octal
        public static string BinaryToOctal(string Data) {
            string Temp = "";
            Data = Data.Replace(" ", "");
            Data = Data.Replace(Constants.Tab.ToString(), "");
            int ExtraZeros = 3 - (Data.Length % 3);
            if (ExtraZeros == 1) { Data = "0" + Data; }
            else if (ExtraZeros == 2) { Data = "00" + Data; }
            if (IsBin(Data)) {
                if ((Data.Length % 3) == 0) {
                    int Grouping = Data.Length / 3;
                    int Index = Data.Length - 1;
                    for (int i = 0; i < Grouping; i++) {
                        char A = Data[Index];
                        char B = Data[Index - 1];
                        char C = Data[Index - 2];
                        string BinaryGroup = C.ToString() + B.ToString() + A.ToString();
                        Temp = BinValueToOctalChar(BinaryGroup) + Temp;
                        Index -= 3;
                    }
                }
            }
            return Temp;
        }
        public static string DecimalToOctal(string Data, BinaryFormatFlags FormatFlags) {
            string BinaryData = DecimalToBinary(Data, FormatFlags);
            return BinaryToOctal(BinaryData);
        }
        public static string HexadecimalToOctal(string Data) {
            string OctalBinary = HexadecimalToBinary(Data);
            return BinaryToOctal(OctalBinary);
        }
        #endregion
        #region To Base 16 Conversion Functions
        //To Hexadecimal
        public static string OctalToHexadecimal(string Data) {
            string OctalBinary = OctalToBinary(Data);
            return BinaryToHexadecimal(OctalBinary);
        }
        public static string DecimalToHexadecimal(string Data, BinaryFormatFlags FormatFlags) {
            string BinaryData = DecimalToBinary(Data, FormatFlags);
            return BinaryToHexadecimal(BinaryData);
        }
        public static TernaryString DecimalToTriBases(string Data, BinaryFormatFlags FormatFlags) {
            string BinaryData = DecimalToBinary(Data, FormatFlags);
            string OctalData = BinaryToOctal(BinaryData);
            string HexData = BinaryToHexadecimal(BinaryData);
            TernaryString Output = new TernaryString(BinaryData, OctalData, HexData);
            return Output;
        }
        public static string BinaryToHexadecimal(string Data) {
            string Temp = "";
            Data = Data.Replace(" ", "");
            Data = Data.Replace(Constants.Tab.ToString(), "");
            int ExtraZeros = 4 - (Data.Length % 4);
            if (ExtraZeros == 1) { Data = "0" + Data; }
            else if (ExtraZeros == 2) { Data = "00" + Data; }
            else if (ExtraZeros == 3) { Data = "000" + Data; }
            if (IsBin(Data)) {
                if ((Data.Length % 4) == 0) {
                    int Grouping = Data.Length / 4;
                    int Index = Data.Length - 1;
                    for (int i = 0; i < Grouping; i++) {
                        char A = Data[Index];
                        char B = Data[Index - 1];
                        char C = Data[Index - 2];
                        char D = Data[Index - 3];
                        string BinaryGroup = D.ToString() + C.ToString() + B.ToString() + A.ToString();
                        Temp = BinValueToHexChar(BinaryGroup) + Temp;
                        Index -= 4;
                    }
                }
            }
            return Temp;
        }
        #endregion
        #region To Base 10 Conversion Functions
        //To Decimal
        public static NumericalString BinaryToDecimal(string Data, BinaryFormatFlags FormatFlags) {
            bool IsSigned = ((int)FormatFlags & 0xF0000) == 0x10000 ? true : false;
            bool IsDecimal = ((int)FormatFlags & 0x0F000) == 0x01000 ? true : false;
            NumericalString Result = new NumericalString("0");
            int ExpectedLength = BinaryFormatFlagsToLength(FormatFlags);
            Data = Data.Replace(" ", "");
            Data = Data.Replace(Constants.Tab.ToString(), "");
            Data = Data.TrimStart('0');
            if (Data.Length == 0) {
                Data = "0";
            }
            if (Data.Length > ExpectedLength) {
                return Result;
            }
            if (IsBin(Data)) {
                if (IsDecimal == false) {
                    if (IsSigned == false) {
                        string Temp = "";
                        int Exp = 0;
                        for (int i = Data.Length - 1; i >= 0; i--) {
                            if (Data[i] == '1') {
                                string SetData = "(2^(" + Exp.ToString() + "))+";
                                Temp += SetData;
                            }
                            Exp++;
                        }
                        if (Temp.EndsWith("+")) { Temp = Temp.TrimEnd('+'); }
                        Result = EvaluateExpression(Temp, null);
                    }
                    else {
                        string Temp = "";
                        int Exp = 0;
                        int End = 0;
                        if (ExpectedLength == Data.Length) {
                            End = 1;
                        }
                        for (int i = Data.Length - 1; i >= End; i--) {
                            if (Data[i] == '1') {
                                string SetData = "(2^(" + Exp.ToString() + "))+";
                                Temp += SetData;
                            }
                            Exp++;
                        }
                        if (Temp.EndsWith("+")) { Temp = Temp.TrimEnd('+'); }
                        Result = EvaluateExpression(Temp, null);
                        if (ExpectedLength == Data.Length) {
                            int ShortLength = ExpectedLength - 1;
                            if (Data[ShortLength] == '1') {
                                Temp = "((-2)^(" + ShortLength.ToString() + "))+" + Result.ToString();
                                Result = EvaluateExpression(Temp, null);
                            }
                        }
                    }
                }
                else {
                    if (FormatFlags == BinaryFormatFlags.Float) {
                        if (Data.Length < 32) {
                            Data = PadNumberWithZeros(Data, 32 - Data.Length);
                        }
                        bool DataIsSigned = Data[0] == '1' ? true : false;
                        string Exponent = Data.Substring(1, 8);//.Substring(23, 8);
                        NumericalString ExponentNum = BinaryToDecimal(Exponent, BinaryFormatFlags.Length8Bit);
                        string Value = EvaluateBinaryDecimalForFloats(Data, 9, Data.Length);
                        string Expression = "(2^(" + ExponentNum.ToString() + " - 127))*" + Value;
                        if (DataIsSigned == true) { Expression += "*(-1)"; }
                        Result = EvaluateExpression(Expression, null);
                    }
                    else if (FormatFlags == BinaryFormatFlags.Double) {
                        if (Data.Length < 64) {
                            Data = PadNumberWithZeros(Data, 64 - Data.Length);
                        }
                        bool DataIsSigned = Data[0] == '1' ? true : false;
                        string Exponent = Data.Substring(1, 11);//.Substring(23, 8);
                        NumericalString ExponentNum = BinaryToDecimal(Exponent, BinaryFormatFlags.Length16Bit);
                        string Value = EvaluateBinaryDecimalForFloats(Data, 12, Data.Length);
                        string Expression = "(2^(" + ExponentNum.ToString() + " - 1023))*" + Value;
                        if (DataIsSigned == true) { Expression += "*(-1)"; }
                        Result = EvaluateExpression(Expression, null);
                    }
                }
            }
            return Result;
        }
        public static NumericalString OctalToDecimal(string Data, BinaryFormatFlags FormatFlags) {
            string Temp = "";
            NumericalString Result = new NumericalString("0");
            Data = Data.Replace(" ", "");
            Data = Data.Replace(Constants.Tab.ToString(), "");
            Data = Data.ToUpper();
            if (IsOct(Data)) {
                for (int i = 0; i < Data.Length; i++) {
                    Temp += OctValueToBinaryString(Data[i]);
                }
                Result = BinaryToDecimal(Temp, FormatFlags);
            }
            return Result;
        }
        public static NumericalString HexadecimalToDecimal(string Data, BinaryFormatFlags FormatFlags) {
            string Temp = "";
            NumericalString Result = new NumericalString("0");
            Data = Data.Replace(" ", "");
            Data = Data.Replace(Constants.Tab.ToString(), "");
            Data = Data.ToUpper();
            if (IsHex(Data)) {
                for (int i = 0; i < Data.Length; i++) {
                    Temp += HexValueToBinaryString(Data[i]);
                }
                Result = BinaryToDecimal(Temp, FormatFlags);
            }
            return Result;
        }
        #endregion
        #region Private Base Convertion Support Functions
        private static string PadNumberWithZeros(string Input, int Quantity) {
            string Temp = "";
            for (int i = 0; i < Quantity; i++) {
                Temp += "0";
            }
            Temp += Input;
            return Temp;
        }
        private static string EvaluateBinaryDecimalForFloats(string Input, int Start, int End) {
            string Temp = "1+";
            int Exp = -1;
            for (int i = Start; i < End; i++) {
                if (Input[i] == '1') {
                    string SetData = "(2^(" + Exp.ToString() + "))+";
                    Temp += SetData;
                }
                Exp--;
            }
            if (Temp.EndsWith("+")) { Temp = Temp.TrimEnd('+'); }
            return EvaluateExpression(Temp, null).ToString();
        }
        private static string HexValueToBinaryString(char Input) {
            switch (Input) {
                case '0': return "0000";
                case '1': return "0001";
                case '2': return "0010";
                case '3': return "0011";
                case '4': return "0100";
                case '5': return "0101";
                case '6': return "0110";
                case '7': return "0111";
                case '8': return "1000";
                case '9': return "1001";
                case 'A': return "1010";
                case 'B': return "1011";
                case 'C': return "1100";
                case 'D': return "1101";
                case 'E': return "1110";
                case 'F': return "1111";
                default: return "0000";
            }
        }
        private static string OctValueToBinaryString(char Input) {
            switch (Input) {
                case '0': return "000";
                case '1': return "001";
                case '2': return "010";
                case '3': return "011";
                case '4': return "100";
                case '5': return "101";
                case '6': return "110";
                case '7': return "111";
                default: return "000";
            }
        }
        private static char BinValueToOctalChar(string Input) {
            if (Input == "000") { return '0'; }
            else if (Input == "001") { return '1'; }
            else if (Input == "010") { return '2'; }
            else if (Input == "011") { return '3'; }
            else if (Input == "100") { return '4'; }
            else if (Input == "101") { return '5'; }
            else if (Input == "110") { return '6'; }
            else if (Input == "111") { return '7'; }
            return '0';
        }
        private static char BinValueToHexChar(string Input) {
            if (Input == "0000") { return '0'; }
            else if (Input == "0001") { return '1'; }
            else if (Input == "0010") { return '2'; }
            else if (Input == "0011") { return '3'; }
            else if (Input == "0100") { return '4'; }
            else if (Input == "0101") { return '5'; }
            else if (Input == "0110") { return '6'; }
            else if (Input == "0111") { return '7'; }
            else if (Input == "1000") { return '8'; }
            else if (Input == "1001") { return '9'; }
            else if (Input == "1010") { return 'A'; }
            else if (Input == "1011") { return 'B'; }
            else if (Input == "1100") { return 'C'; }
            else if (Input == "1101") { return 'D'; }
            else if (Input == "1110") { return 'E'; }
            else if (Input == "1111") { return 'F'; }
            return '0';
        }
        public static bool IsHex(string Input) {
            bool isHex;
            for (int i = 0; i < Input.Length; i++) {
                char c = Input[i];
                isHex = ((c >= '0' && c <= '9') ||
                         (c >= 'a' && c <= 'f') ||
                         (c >= 'A' && c <= 'F'));

                if (!isHex) { return false; }
            }
            return true;
        }
        public static bool IsBin(string Input) {
            bool isHex;
            for (int i = 0; i < Input.Length; i++) {
                char c = Input[i];
                isHex = ((c >= '0' && c <= '1'));

                if (!isHex) { return false; }
            }
            return true;
        }
        public static bool IsOct(string Input) {
            bool isHex;
            for (int i = 0; i < Input.Length; i++) {
                char c = Input[i];
                isHex = ((c >= '0' && c <= '8'));

                if (!isHex) { return false; }
            }
            return true;
        }
        public static int HexCharToInt(char Input) {
            if (((int)Input - 0x30 >= 0) && ((int)Input - 0x30 <= 9)) {
                int Output = 0; int.TryParse(Input.ToString(), out Output);
                return Output;
            }
            else {
                char LowerChar = Input.ToString().ToLower()[0];
                if (((int)LowerChar - 0x61 >= 0) && ((int)Input - 0x61 <= 5)) {
                    int Output = 0; int.TryParse(Input.ToString(), out Output);
                    return Output + 10;
                }
            }
            return -1;
        }
        public static int BinaryFormatFlagsToLength(BinaryFormatFlags FormatFlags) {
            BinaryFormatFlags Flags = (BinaryFormatFlags)((int)FormatFlags & 0x00FFF);
            switch (Flags) {
                case BinaryFormatFlags.Length4Bit: return 4;
                case BinaryFormatFlags.Length8Bit: return 8;
                case BinaryFormatFlags.Length12Bit: return 12;
                case BinaryFormatFlags.Length16Bit: return 16;
                case BinaryFormatFlags.Length24Bit: return 24;
                case BinaryFormatFlags.Length32Bit: return 32;
                case BinaryFormatFlags.Length64Bit: return 64;
                case BinaryFormatFlags.Length128Bit: return 128;
                case BinaryFormatFlags.Length256Bit: return 256;
                default: return 0;
            }
        }

        #endregion
        public static DualNumericalString GetBinaryFormatRange(BinaryFormatFlags FormatFlags) {
            int BitCount = BinaryFormatFlagsToLength(FormatFlags);
            bool IsSigned = ((int)FormatFlags & 0xF0000) == 0x10000 ? true : false;

            string Expression = "(2^(" + BitCount.ToString() + "))";
            if (IsSigned == true) { Expression += "/2"; }
            NumericalString Maximum = EvaluateExpression(Expression, null);
            if (IsSigned == false) {
                return new DualNumericalString(new NumericalString(0), Maximum - 1);
            }
            else {
                return new DualNumericalString(Maximum * (-1), Maximum - 1);
            }
        }
    }
    public enum ExpressionScriptType {
        Normal = 0x00,
        Subscript = 0x01,
        Superscript = 0x02
    }
    public enum BinaryFormatFlags {
        Length4Bit = 0x00000,
        Length8Bit = 0x00001,
        Length12Bit = 0x00002,
        Length16Bit = 0x00004,
        Length24Bit = 0x00008,
        Length32Bit = 0x00010,
        Length64Bit = 0x00020,
        Length128Bit = 0x00040,
        Length256Bit = 0x00080,
        Signed = 0x10000,
        Float = 0x11010,
        Double = 0x11020,
    }
    public class DualNumericalString {
        public DualNumericalString(NumericalString A, NumericalString B) {
            this.a = A;
            this.b = B;
        }
        NumericalString a = new NumericalString();
        public NumericalString A {
            get { return a; }
            // set { a = value; }
        }
        NumericalString b = new NumericalString();
        public NumericalString B {
            get { return b; }
            //set { a = value; }
        }
    }
    public class TernaryString {
        public TernaryString(string A, string B, string C) {
            this.a = A;
            this.b = B;
            this.c = C;
        }
        string a = "";
        public string A {
            get { return a; }
            // set { a = value; }
        }
        string b = "";
        public string B {
            get { return b; }
            //set { a = value; }
        }
        string c = "";
        public string C {
            get { return c; }
            //set { a = value; }
        }
    }
    public class ExpressionFormat {
        public ExpressionScriptType ScriptType = ExpressionScriptType.Normal;
        public string Text = "";
        public ExpressionFormat(string Text, ExpressionScriptType Format) {
            this.Text = Text;
            this.ScriptType = Format;
        }
    }
    public class DualSideExpression {
        public string Left;
        public string Right;
        public DualSideExpression(string Left, string Right) {
            this.Left = Left;
            this.Right = Right;
        }
    }
    public class BracketedEquation {
        public MathTextLocation Location;
        public string Name = "";
        public BracketedEquation(MathTextLocation location, string name) {
            Location = location;
            Name = name;
        }
    }
    public class MathTextLocation {
        public int StartIndex;
        public int EndIndex;
        public int Depth;
        public object Value;
        public MathTextLocation(int startIndex, int endIndex, int depth, object value = null) {
            StartIndex = startIndex;
            EndIndex = endIndex;
            Depth = depth;
            Value = value;
        }
        public int Length {
            get {
                if (EndIndex < StartIndex) {
                    return 0;
                }
                else {
                    return EndIndex + 1 - StartIndex;
                }
            }
        }
        public override string ToString() {
            return "Text Location: " + StartIndex.ToString() + " - " + EndIndex.ToString() + ", Depth: " + Depth.ToString();
        }
    }
    public class Equation {
        public string A;
        public string B;
        public string SolveFor;
        public Equation(string Expression, string solveFor) {
            STR_MVSSF Spilt = StringHandler.SpiltStringMutipleValues(Expression, '=');
            if (Spilt.Count == 1) {
                A = Spilt.Value[0].Replace(" ", "").Replace(((char)0x09).ToString(), "");
                B = "0";
            }
            else if (Spilt.Count == 2) {
                A = Spilt.Value[0].Replace(" ", "").Replace(((char)0x09).ToString(), "");
                B = Spilt.Value[1].Replace(" ", "").Replace(((char)0x09).ToString(), "");
            }
            SolveFor = solveFor;
        }
        public bool IsSolved() {
            if (A == SolveFor) {
                return true;
            }
            if (B == SolveFor) {
                return true;
            }
            return false;
        }
    }
    public class MathVariable {
        public string Name;
        //ublic double Value;
        //public Type DataType = typeof(NumericalString);
        private NumericalString var_numstring = new NumericalString(null);
        //private int var_int;
        //private double var_dbl;
        public MathVariable(string name, object value) {
            Name = name;
            Value = value;
        }
        public object Value {
            get {
                //if (DataType == typeof(int)) { return var_int; }
                //else if (DataType == typeof(double)) { return var_dbl; }
                //else if (DataType == typeof(NumericalString)) { return var_numstring; }
                //else if (DataType == typeof(string)) { return var_numstring.ToString(); }
                //else { return null; }
                return var_numstring.ToString();
            }
            set {
                //if (DataType == typeof(int)) {
                //    if (value.GetType() == typeof(string)) {
                //        if (ConversionHandler.IsNumeric(value.ToString())) {
                //            var_int = int.Parse(value.ToString());
                //        }
                //    }
                //    else if (value.GetType() == typeof(int)) { var_int = (int)value; }
                //    else if (value.GetType() == typeof(double)) { var_int = (int)value; }
                //    else if (value.GetType() == typeof(NumericalString)) {
                //        var_int = int.Parse(((NumericalString)value).ToString());
                //    }
                //}
                //else if (DataType == typeof(double)) {
                //    if (value.GetType() == typeof(string)) {
                //        if (ConversionHandler.IsNumeric(value.ToString())) {
                //            var_dbl = double.Parse(value.ToString());
                //        }
                //    }
                //    else if (value.GetType() == typeof(int)) { var_dbl = (double)value; }
                //    else if (value.GetType() == typeof(double)) { var_dbl = (double)value; }
                //    else if (value.GetType() == typeof(NumericalString)) {
                //        var_dbl = double.Parse(((NumericalString)value).ToString());
                //    }
                //}
                //else if (DataType == typeof(NumericalString)) {
                if (value.GetType() == typeof(NumericalString)) {
                    var_numstring.Value = ((NumericalString)value).ToString();
                }
                else if (value.GetType() == typeof(double)) {
                    var_numstring.Value = ((double)value).ToString();
                }
                else {
                    var_numstring.Value = value;
                }
                //}
                //else if (DataType == typeof(string)) {
                //    var_numstring.Value = value;
                //}
                //else { }
            }
        }
    }
    //[TypeConverter(typeof(NumericalStringConverter))]
    [TypeConverter(typeof(NumericalStringConverter))]
    public class NumericalString : INotifyPropertyChanged {
        #region Class Initalisation
        string numericalvalue = "0";
        public static int Precision = 200;
        public NumericalString() {
            ChangeValue(0);
        }
        public NumericalString(int Input) {
            ChangeValue(Input);
        }
        public NumericalString(uint Input) {
            ChangeValue(Input);
        }
        public NumericalString(byte Input) {
            ChangeValue(Input);
        }
        public NumericalString(sbyte Input) {
            ChangeValue(Input);
        }
        public NumericalString(float Input) {
            ChangeValue(Input);
        }
        public NumericalString(double Input) {
            ChangeValue(Input);
        }
        public NumericalString(decimal Input) {
            ChangeValue(Input);
        }
        public NumericalString(long Input) {
            ChangeValue(Input);
        }
        public NumericalString(short Input) {
            ChangeValue(Input);
        }
        public NumericalString(ulong Input) {
            ChangeValue(Input);
        }
        public NumericalString(ushort Input) {
            ChangeValue(Input);
        }
        public NumericalString(object Input) {
            ChangeValue(Input);
        }
        public static explicit operator double(NumericalString a) {
            return Double.Parse(a.ToString());
        }
        #endregion
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //[Browsable(false)]
        public object Value {
            get {
                return numericalvalue;
            }
            set {
                ChangeValue(value);
                OnPropertyChanged("Value");
            }
        }
        public object DisplayValue {
            get { return Value; }
            set { Value = value; }
        }
        private void ChangeValue(object Input) {
            try {
                if (Input != null) {
                    Type T = Input.GetType();
                    if (T == typeof(string)) {
                        string FormattedString = Input.ToString().Replace(" ", "").Replace(Constants.Tab.ToString(), "");
                        if (ConversionHandler.IsNumeric(FormattedString)) {
                            numericalvalue = FormattedString;
                        }
                        else {
                            numericalvalue = "0";
                        }
                    }
                    else if (T == typeof(double)) {
                        numericalvalue = String.Format("{0:F15}", (double)Input);
                    }
                    else if (T == typeof(float)) {
                        numericalvalue = String.Format("{0:F15}", (float)Input);
                    }
                    else if (T == typeof(decimal)) {
                        numericalvalue = String.Format("{0:F15}", (decimal)Input);
                    }
                    else if (T == typeof(short)) {
                        numericalvalue = ((short)Input).ToString();
                    }
                    else if (T == typeof(int)) {
                        numericalvalue = ((int)Input).ToString();
                    }
                    else if (T == typeof(long)) {
                        numericalvalue = ((int)Input).ToString();
                    }
                    else if (T == typeof(ushort)) {
                        numericalvalue = ((ushort)Input).ToString();
                    }
                    else if (T == typeof(uint)) {
                        numericalvalue = ((uint)Input).ToString();
                    }
                    else if (T == typeof(ulong)) {
                        numericalvalue = ((uint)Input).ToString();
                    }
                    else if (T == typeof(byte)) {
                        numericalvalue = ((byte)Input).ToString();
                    }
                    else if (T == typeof(sbyte)) {
                        numericalvalue = ((sbyte)Input).ToString();
                    }
                    else if (T == typeof(NumericalString)) {
                        numericalvalue = ((NumericalString)Input).ToString();
                    }
                    else {
                        throw new InvalidCastException();
                    }
                }
                else {
                    numericalvalue = "0";
                }
            }
            catch {
                numericalvalue = "0";
            }
        }
        #region Mathematical Operators
        public static NumericalString operator +(NumericalString a, NumericalString b) {
            string Result = ProcessOperator(LocalOperator.Addition, a.ToString(), b.ToString());
            NumericalString c = new NumericalString(Result);
            return c;
        }
        public static NumericalString operator +(NumericalString a, object b) {
            string Result = ProcessOperator(LocalOperator.Addition, a.ToString(), new NumericalString(b).ToString());
            NumericalString c = new NumericalString(Result);
            return c;
        }
        public static NumericalString operator +(object a, NumericalString b) {
            string Result = ProcessOperator(LocalOperator.Addition, new NumericalString(a).ToString(), b.ToString());
            NumericalString c = new NumericalString(Result);
            return c;
        }
        public static NumericalString operator ++(NumericalString a) {
            string Result = ProcessOperator(LocalOperator.Addition, a.ToString(), "1");
            NumericalString c = new NumericalString(Result);
            a.ChangeValue(Result);
            return c;
        }
        public static NumericalString operator -(NumericalString a, NumericalString b) {
            string Result = ProcessOperator(LocalOperator.Addition, a.ToString(), b.Negate().ToString());
            NumericalString c = new NumericalString(Result);
            return c;
        }
        public static NumericalString operator -(object a, NumericalString b) {
            string Result = ProcessOperator(LocalOperator.Addition, new NumericalString(a).ToString(), b.Negate().ToString());
            NumericalString c = new NumericalString(Result);
            return c;
        }
        public static NumericalString operator -(NumericalString a, object b) {
            string Result = ProcessOperator(LocalOperator.Addition, a.ToString(), new NumericalString(b).Negate().ToString());
            NumericalString c = new NumericalString(Result);
            return c;
        }
        public static NumericalString operator --(NumericalString a) {
            string Result = ProcessOperator(LocalOperator.Addition, a.ToString(), "-1");
            NumericalString c = new NumericalString(Result);
            a.ChangeValue(Result);
            return c;
        }
        public static NumericalString operator *(NumericalString a, NumericalString b) {
            string Result = ProcessOperator(LocalOperator.Multiplication, a.ToString(), b.ToString());
            NumericalString c = new NumericalString(Result);
            return c;
        }
        public static NumericalString operator *(NumericalString a, object b) {
            string Result = ProcessOperator(LocalOperator.Multiplication, a.ToString(), new NumericalString(b).ToString());
            NumericalString c = new NumericalString(Result);
            return c;
        }
        public static NumericalString operator *(object a, NumericalString b) {
            string Result = ProcessOperator(LocalOperator.Multiplication, new NumericalString(a).ToString(), b.ToString());
            NumericalString c = new NumericalString(Result);
            return c;
        }
        public static NumericalString operator /(NumericalString a, NumericalString b) {
            string Result = ProcessOperator(LocalOperator.Division, a.ToString(), b.ToString());
            NumericalString c = new NumericalString(Result);
            return c;
        }
        public static NumericalString operator /(NumericalString a, object b) {
            string Result = ProcessOperator(LocalOperator.Division, a.ToString(), new NumericalString(b).ToString());
            NumericalString c = new NumericalString(Result);
            return c;
        }
        public static NumericalString operator /(object a, NumericalString b) {
            string Result = ProcessOperator(LocalOperator.Division, new NumericalString(a).ToString(), b.ToString());
            NumericalString c = new NumericalString(Result);
            return c;
        }
        public static NumericalString operator %(NumericalString a, NumericalString b) {
            string Result = ProcessOperator(LocalOperator.Remainder, a.ToString(), b.ToString());
            NumericalString c = new NumericalString(Result);
            return c;
        }
        public static NumericalString operator %(NumericalString a, object b) {
            string Result = ProcessOperator(LocalOperator.Remainder, a.ToString(), new NumericalString(b).ToString());
            NumericalString c = new NumericalString(Result);
            return c;
        }
        public static NumericalString operator %(object a, NumericalString b) {
            string Result = ProcessOperator(LocalOperator.Remainder, new NumericalString(a).ToString(), b.ToString());
            NumericalString c = new NumericalString(Result);
            return c;
        }
        public static NumericalString operator ^(NumericalString a, NumericalString b) {
            string Result = ProcessOperator(LocalOperator.Power, a.ToString(), b.ToString());
            NumericalString c = new NumericalString(Result);
            return c;
        }
        public static NumericalString operator ^(NumericalString a, object b) {
            string Result = ProcessOperator(LocalOperator.Power, a.ToString(), new NumericalString(b).ToString());
            NumericalString c = new NumericalString(Result);
            return c;
        }
        public static NumericalString operator ^(object a, NumericalString b) {
            string Result = ProcessOperator(LocalOperator.Power, new NumericalString(a).ToString(), b.ToString());
            NumericalString c = new NumericalString(Result);
            return c;
        }
        public NumericalString Negate() {
            if (numericalvalue.StartsWith("-")) {
                return new NumericalString(numericalvalue.Replace("-", ""));
            }
            return new NumericalString("-" + numericalvalue);
        }
        public NumericalString Abs() {
            return new NumericalString(numericalvalue.Replace("-", ""));
        }
        public NumericalString Floor() {
            if (numericalvalue.Contains('.')) {
                return new NumericalString(this.GetIntergralComponent());
            }
            else { return this; }
        }
        public NumericalString Ceiling() {
            if (numericalvalue.Contains('.')) {
                NumericalString A = new NumericalString(this.GetIntergralComponent());
                return A++;
            }
            else { return this; }
        }
        public NumericalString Round(int DecimalPlaces = 0) {
            if (DecimalPlaces <= -1) {
                string TempResult = numericalvalue;
                string BuildString = "";
                bool HasOverflow = true;
                bool LastHadCarry = false;

                //23.3623696564556
                //23.3623696564566
                if (TempResult.Contains('.')) {
                    for (int i = TempResult.Length - 1; i >= 0; i--) {
                        if (TempResult[i] != '.') {
                            int CharVal = 0; int.TryParse(TempResult[i].ToString(), out CharVal);
                            if (LastHadCarry == true) {
                                CharVal++;
                                LastHadCarry = false;
                            }
                            if (HasOverflow == true) {
                                if (CharVal >= 5) { LastHadCarry = true; }
                                else { HasOverflow = false; }
                            }
                            if (HasOverflow == false) {
                                int CurrentValue = CharVal % 10;
                                BuildString = CurrentValue.ToString() + BuildString;
                            }
                        }
                        else {
                            if (BuildString.Length > 0) { BuildString = "." + BuildString; }
                            HasOverflow = false;
                        }
                    }
                    if (BuildString.Length == 0) { BuildString = "0"; }
                    return new NumericalString(BuildString);
                }
                else { return this; }
            }
            return this;
        }
        #endregion
        #region Equality Comparation
        public static bool operator ==(NumericalString a, NumericalString b) {
            if (a.IsZero() && b.IsZero()) { return true; }
            string StringA = a.ToString().TrimStart('0').TrimEnd('0');
            string StringB = b.ToString().TrimStart('0').TrimEnd('0');
            if (StringA.Length == 0) { StringA = "0"; }
            if (StringB.Length == 0) { StringB = "0"; }
            return StringA == StringB;
        }
        public static bool operator !=(NumericalString a, NumericalString b) {
            if (a.IsZero() && b.IsZero()) { return false; }
            string StringA = a.ToString().TrimStart('0').TrimEnd('0');
            string StringB = b.ToString().TrimStart('0').TrimEnd('0');
            if (StringA.Length == 0) { StringA = "0"; }
            if (StringB.Length == 0) { StringB = "0"; }
            return StringA != StringB;
        }
        public static bool operator ==(NumericalString a, object b) {
            return new NumericalString(a) == b;
        }
        public static bool operator !=(NumericalString a, object b) {
            return new NumericalString(a) != b;
        }
        public static bool operator ==(object a, NumericalString b) {
            return a == new NumericalString(b);
        }
        public static bool operator !=(object a, NumericalString b) {
            return a != new NumericalString(b);
        }
        #endregion
        #region Inequality Comparation
        public static bool operator >(NumericalString a, NumericalString b) {
            bool Result = CompareTwoValues(a, b, false, false);
            return Result;
        }
        public static bool operator <(NumericalString a, NumericalString b) {
            bool Result = CompareTwoValues(a, b, true, false);
            return Result;
        }
        public static bool operator >(NumericalString a, object b) {
            bool Result = CompareTwoValues(a, new NumericalString(b), false, false);
            return Result;
        }
        public static bool operator <(NumericalString a, object b) {
            bool Result = CompareTwoValues(a, new NumericalString(b), true, false);
            return Result;
        }
        public static bool operator >(object a, NumericalString b) {
            bool Result = CompareTwoValues(new NumericalString(a), b, false, false);
            return Result;
        }
        public static bool operator <(object a, NumericalString b) {
            bool Result = CompareTwoValues(new NumericalString(a), b, true, false);
            return Result;
        }
        public static bool operator >=(NumericalString a, NumericalString b) {
            bool Result = CompareTwoValues(a, b, false, true);
            return Result;
        }
        public static bool operator <=(NumericalString a, NumericalString b) {
            bool Result = CompareTwoValues(a, b, true, true);
            return Result;
        }
        public static bool operator >=(NumericalString a, object b) {
            bool Result = CompareTwoValues(a, new NumericalString(b), false, true);
            return Result;
        }
        public static bool operator <=(NumericalString a, object b) {
            bool Result = CompareTwoValues(a, new NumericalString(b), true, true);
            return Result;
        }
        public static bool operator >=(object a, NumericalString b) {
            bool Result = CompareTwoValues(new NumericalString(a), b, false, true);
            return Result;
        }
        public static bool operator <=(object a, NumericalString b) {
            bool Result = CompareTwoValues(new NumericalString(a), b, true, true);
            return Result;
        }
        #endregion
        #region Equality Private Functions
        private static bool CompareTwoValues(NumericalString a, NumericalString b, bool LessThan, bool CompareEqual) {
            if (CompareEqual == true) {
                if (a.IsZero() && b.IsZero()) { return true; }
            }
            bool IsANegitive = a.IsNegitive();
            bool IsBNegitive = b.IsNegitive();
            if ((IsANegitive == true) && (IsBNegitive == false)) {
                return LessThan;
            }
            else if ((IsANegitive == false) && (IsBNegitive == true)) {
                return !LessThan;
            }
            bool InvertOrder = (IsANegitive & IsBNegitive);
            string TempA = a.GetIntergralComponent().Replace("-", "");
            string TempB = b.GetIntergralComponent().Replace("-", "");
            string TempC = a.GetFractionalComponent();
            string TempD = b.GetFractionalComponent();
            if (TempA.Length > TempB.Length) {
                TempB = StringHandler.PadWithCharacter(TempB, TempA.Length, '0', false);
            }
            else if (TempB.Length > TempA.Length) {
                TempA = StringHandler.PadWithCharacter(TempA, TempB.Length, '0', false);
            }
            if (TempC.Length > TempD.Length) {
                TempD = StringHandler.PadWithCharacter(TempD, TempC.Length, '0');
            }
            else if (TempD.Length > TempC.Length) {
                TempC = StringHandler.PadWithCharacter(TempC, TempD.Length, '0');
            }
            string CompA = TempA + TempC;
            string CompB = TempB + TempD;
            if (CompareEqual == true) {
                if (CompA == CompB) { return true; }
            }
            else {
                if (CompA == CompB) { return false; }
            }
            bool CompLessThan = InvertOrder == false ? LessThan : !LessThan;
            bool IsLastEqual = true;
            bool LastResult = false;
            for (int i = 0; i < CompA.Length; i++) {
                int IntA = 0; int.TryParse(CompA[i].ToString(), out IntA);
                int IntB = 0; int.TryParse(CompB[i].ToString(), out IntB);
                if (IntA != IntB) {
                    if (CompLessThan == true) {
                        if (IntA < IntB) { LastResult = true; }
                        else { break; }
                    }
                    else {
                        if (IntA > IntB) { LastResult = true; }
                        else { break; }
                    }
                }
                else { IsLastEqual = true; }
            }
            return LastResult;
        }
        #endregion
        #region Formatting
        private static string Decimalise(string Input, int Location, bool FromEnd = true) {
            string Output = "";
            bool StartsWithNegitive = Input.StartsWith("-");
            if (Location > Input.Length - 1) {
                Output = "0.";
                string FrmtInput = Input.Replace("-", "");
                for (int i = 0; i < Location - FrmtInput.Length; i++) {
                    Output += "0";
                }

                for (int i = 0; i < FrmtInput.Length; i++) {
                    Output += FrmtInput[i];
                }
                if (StartsWithNegitive == true) { Output = "-" + Output; }
                return Output;
            }
            else if (Location > 0) {
                for (int i = 0; i < Input.Length; i++) {
                    Output += Input[i];
                    if ((i == Input.Length - 1 - Location) && (FromEnd == true)) {
                        Output += ".";
                    }
                    else if ((i == Location) && (FromEnd == false)) {
                        Output += ".";
                    }
                }
                return Output;
            }
            else {
                return Input;
            }
        }
        private static string PadString(StringDecimal Input, int MaxPoints, LocalOperator Operator = LocalOperator.Any) {
            string Padded = "";
            if (MaxPoints != Input.PointLocation) {
                int diff = MaxPoints - Input.PointLocation;
                for (int i = 0; i < diff; i++) { Padded += "0"; }
            }
            if (Operator == LocalOperator.Division) {
                for (int i = 0; i < Precision; i++) { Padded += "0"; }
            }
            string Output = Input.ValueWithoutPoint + Padded;
            return Output;
        }
        #endregion
        private static string ProcessOperator(LocalOperator Operator, string A, string B) {
            //if (!A.Contains(".")) { A = A + ".0"; }
            if ((A.Length > 0) && (B.Length > 0)) {
                StringDecimal Adec = new StringDecimal(A);
                StringDecimal Bdec = new StringDecimal(B);
                int MaxPoint = Math.Max(Adec.PointLocation + Adec.Value.Length, Bdec.PointLocation + Bdec.Value.Length);
                int MinPoint = Math.Min(Adec.PointLocation + Adec.Value.Length, Bdec.PointLocation + Bdec.Value.Length);
                LocalOperator Selected = LocalOperator.Any;
                if (Operator == LocalOperator.Division) { Selected = LocalOperator.Division; }
                //else if (Operator == LocalOperator.Remainder) { Selected = LocalOperator.Division; }
                BigInteger Alng = BigInteger.Parse(PadString(Adec, MaxPoint, Selected));
                BigInteger Blng = BigInteger.Parse(PadString(Bdec, MaxPoint));
                BigInteger Clng;
                string Output = "0";
                switch (Operator) {
                    case LocalOperator.Addition:
                        Clng = BigInteger.Add(Alng, Blng);
                        Output = Decimalise(Clng.ToString(), MaxPoint);
                        break;
                    case LocalOperator.Subtraction:
                        Clng = BigInteger.Subtract(Blng, Alng);
                        Output = Decimalise(Clng.ToString(), MaxPoint);
                        break;
                    case LocalOperator.Multiplication:
                        Clng = BigInteger.Multiply(Alng, Blng);
                        Output = Decimalise(Clng.ToString(), MaxPoint * 2);
                        break;
                    case LocalOperator.Division:
                        Clng = BigInteger.Divide(Alng, Blng);
                        Output = Decimalise(Clng.ToString(), Precision);
                        break;
                    case LocalOperator.Remainder:
                        Clng = BigInteger.Remainder(Alng, Blng);
                        Output = Decimalise(Clng.ToString(), MaxPoint);
                        break;
                    case LocalOperator.Power:
                        if (B.Contains(".")) {
                            if (B.TrimStart('0').TrimEnd('0').Replace("-", "") == ".5") {
                                NumericalString Guess = new NumericalString(10);
                                NumericalString CurrentValue = new NumericalString(A);
                                for (int i = 0; i < 15; i++) {
                                    Guess = Guess - ((Guess ^ 2) - CurrentValue) / (Guess * 2);
                                }
                                if (B.StartsWith("-")) {
                                    Guess = 1 / Guess;
                                }
                                return Guess.ToString();
                            }
                            //STR_MVSSF DecimalSpilt = StringHandler.SpiltStringMutipleValues(B, '.');
                            //string SpiltOutput = ProcessOperator(LocalOperator.Power, A, DecimalSpilt.Value[0]);
                            //DecimalSpilt.Value[1] = "0." + DecimalSpilt.Value[1];
                            ////int Exp = int.Parse(B.Replace("-", ""));
                            //bool IsNegitiveExp = Bdec.IsNegitive;
                            //StringDecimal Xdec = new StringDecimal(DecimalSpilt.Value[1]);
                            //BigInteger p = BigInteger.Parse(Xdec.Numerator.Replace("-", ""));
                            //BigInteger q = BigInteger.Parse(Xdec.Denominator.Replace("-", ""));
                            //BigInteger q1 = BigInteger.Subtract(q, 1);
                            //string y_n1 = ProcessOperator(LocalOperator.Division, A.Replace("-", ""), "4");
                            ////if (ulong.Parse(p.ToString()) > 0) {
                            ////    y_n1 = A;// p.ToString();
                            ////}
                            ////Precision = 100;
                            //string xp = "1";
                            //if (A.Contains(".")) {
                            //    xp = ProcessOperator(LocalOperator.Power, A, p.ToString());
                            //}
                            //else {
                            //    xp = BigInteger.Pow(BigInteger.Parse(A), int.Parse(p.ToString())).ToString();
                            //}
                            //string Last = "";
                            //bool BreakAll = false;
                            //for (int i = 0; i < 10000; i++) {
                            //    string yq = ProcessOperator(LocalOperator.Power, y_n1.ToString(), q.ToString());
                            //    string o1 = ProcessOperator(LocalOperator.Multiplication, q1.ToString(), yq);
                            //    string n = ProcessOperator(LocalOperator.Addition, o1, xp);
                            //
                            //    string yq1 = ProcessOperator(LocalOperator.Power, y_n1.ToString(), q1.ToString());
                            //    string d = ProcessOperator(LocalOperator.Multiplication, q.ToString(), yq1);
                            //
                            //    y_n1 = ProcessOperator(LocalOperator.Division, n, d);
                            //    if (y_n1.Split('.').Length == 2) {
                            //        if (Last.Split('.').Length == 2) {
                            //            string t_a = y_n1.Split('.').Last();
                            //            string t_b = Last.Split('.').Last();
                            //            if (t_a.Length == t_b.Length) {
                            //                BreakAll = true;
                            //                for (int j = t_a.Length - 1; j >= 0; j--) {
                            //                    if (t_a[j] != t_b[j]) {
                            //                        BreakAll = false;
                            //                        break;
                            //                    }
                            //                }
                            //            }
                            //            if (BreakAll == true) { break; }
                            //        }
                            //    }
                            //    Last = y_n1;
                            //    Debug.Print(i.ToString() + " " + y_n1);
                            //}
                            //y_n1 = ProcessOperator(LocalOperator.Multiplication, SpiltOutput, y_n1);
                            ////Math.Sqrt()
                            //if (IsNegitiveExp) {
                            //    Output = ProcessOperator(LocalOperator.Division, "1", y_n1);
                            //}
                            //else { Output = y_n1; }

                            //B is a decimal
                            //BigInteger.
                            /*  y = x^(p/q)
                             *  y^q = x^p
                             *  
                             *  y_(n+1) = ((q-1)*y^(q)+x^p) /      (q*y^(q-1))
                             *             q1*yq+xp         /
                             *             o1+xp
                             *             n
                             *  y_n1 = c/d
                             */
                        }
                        else {
                            int Exp = int.Parse(B.Replace("-", ""));
                            bool IsNegitiveExp = Bdec.IsNegitive;
                            if (Adec.Denominator == "1") {
                                Clng = BigInteger.Pow(BigInteger.Parse(A), Exp);
                                if (IsNegitiveExp) {
                                    Output = ProcessOperator(LocalOperator.Division, "1", Clng.ToString());
                                }
                                else { Output = Clng.ToString(); }
                            }
                            else {
                                BigInteger FracN = BigInteger.Pow(BigInteger.Parse(Adec.Numerator), Exp);
                                BigInteger FracD = BigInteger.Pow(BigInteger.Parse(Adec.Denominator), Exp);
                                string TempCalc = ProcessOperator(LocalOperator.Division, FracN.ToString(), FracD.ToString());
                                if (IsNegitiveExp) {
                                    Output = ProcessOperator(LocalOperator.Division, "1", TempCalc);
                                }
                                else { Output = TempCalc; }
                            }
                        }
                        break;
                    default:
                        return "0";
                }
                if (Output != "0") {
                    if (Output.Contains(".")) {
                        Output = Output.TrimEnd('0');
                        if (Output.EndsWith(".")) { Output = Output.Replace(".", ""); }
                    }
                }
                if (Output.StartsWith("-.")) { Output = Output.Replace("-.", "-0."); }
                return Output;
            }
            else {
                return "0";
            }
        }
        #region Type Casting
        public override string ToString() {
            return numericalvalue;
        }
        public decimal ToDecimal() {
            try { return Convert.ToDecimal(numericalvalue); }
            catch { return 0.00m; }
        }
        public float ToFloat() {
            try { return Convert.ToSingle(numericalvalue); }
            catch { return 0.00f; }
        }
        public double ToDouble() {
            try { return Convert.ToDouble(numericalvalue); }
            catch { return 0.00f; }
        }
        public double ToInteger() {
            try { return Convert.ToInt32(numericalvalue); }
            catch { return 0; }
        }
        #endregion
        #region Numerical Components
        public string GetIntergralComponent() {
            if (numericalvalue.Contains('.')) {
                return numericalvalue.Split('.')[0];
            }
            return numericalvalue;
        }
        public string GetFractionalComponent() {
            if (numericalvalue.Contains('.')) {
                return numericalvalue.Split('.')[1];
            }
            return "0";
        }
        #endregion
        #region Numeric State
        public bool IsZero() {
            if (numericalvalue.Contains('.')) {
                for (int i = 0; i < numericalvalue.Length; i++) {
                    if (numericalvalue[i] != '.') {
                        if (numericalvalue[i] != 0) {
                            return false;
                        }
                    }
                }
                return true;
            }
            else {
                if (numericalvalue.Length == 0) { return true; }
                else if ((numericalvalue.Length == 1) && (numericalvalue == "0")) { return true; }
                else { return false; }
            }
        }
        public bool IsNegitive() {
            if (numericalvalue.Contains('-')) {
                return true;
            }
            else {
                return false;
            }
        }

        public override bool Equals(object obj) {
            return new NumericalString(obj).ToString() == this.numericalvalue;
        }
        public override int GetHashCode() {
            return -1612030630 + EqualityComparer<string>.Default.GetHashCode(numericalvalue);
        }
        #endregion

        enum LocalOperator {
            Addition = 0x0,
            Subtraction = 0x1,
            Multiplication = 0x2,
            Division = 0x03,
            Power = 0x04,
            Remainder = 0x05,
            Any = 0xFF
        }
    }
    public class NumericalStringConverter : TypeConverter {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            return destinationType == typeof(string);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            return sourceType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType == typeof(string) && value is NumericalString numericalString) {
                return numericalString.Value?.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            if (value is string stringValue) {
                var numericalString = new NumericalString();
                //if (int.TryParse(stringValue, out int intValue)) {
                //    numericalString.Value = intValue;
                //}
                numericalString.Value = value;
                return numericalString;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
    public class StringDecimal {
        public readonly string Value;
        public readonly string ValueWithoutPoint;
        public readonly int PointLocation;
        public readonly int PointLocationFromBeginning;
        public readonly string Numerator;
        public readonly string Denominator;
        public readonly bool IsNegitive;
        public StringDecimal(string Input) {
            Value = Input;
            int cnt = 0;
            bool Returns = false;
            for (int i = Input.Length - 1; i >= 0; i--) {
                if (Input[i] == '.') {
                    Returns = true;
                    break;
                }
                cnt++;
            }
            if (Returns == false) { cnt = 0; }
            ValueWithoutPoint = Input.Replace(".", "");
            PointLocation = cnt;
            if (cnt >= 0) {
                PointLocationFromBeginning = Input.Length - 1 - cnt;
            }
            else { PointLocationFromBeginning = 0; }
            if (Input.Contains(".")) {
                Numerator = ValueWithoutPoint;
                string DemOut = "1";
                for (int i = 0; i < PointLocation; i++) {
                    DemOut += "0";
                }
                Denominator = DemOut;
            }
            else {
                Denominator = "1";
            }
            IsNegitive = Input.Contains("-");
        }
    }
    public struct Quad {
        public double I;
        public decimal F;

        public Quad(double d) {
            double i = Math.Truncate(d);
            I = i;
            F = (decimal)(d - i);
        }
        public static implicit operator Quad(double a) {
            return new Quad(a);
        }
        public static explicit operator double(Quad a) {
            return a.I + (double)a.F;
        }
        public static Quad operator +(Quad a, Quad b) {
            Quad c;
            c.I = a.I + b.I;
            c.F = a.F + b.F;
            return c;
        }
        public static Quad operator *(Quad a, Quad b) {
            Quad Intermediate = new Quad(a.I * (double)b.F + (double)a.F * b.I);
            Quad c;
            c.I = (a.I * b.I) + Intermediate.I;
            c.F = (a.F * b.F) + Intermediate.F;
            return c;
        }
        public static Quad operator /(Quad a, Quad b) {
            Quad Quotant = new Quad(1 / (double)b);
            Quad Intermediate = new Quad(a.I * (double)Quotant.F + (double)a.F * Quotant.I);
            Quad c;
            c.I = (a.I * Quotant.I) + Intermediate.I;
            c.F = (a.F * Quotant.F) + Intermediate.F;
            return c;
        }
        public void Normalise() {
            if (Math.Abs(F) < 1m)
                return;
            decimal i = Math.Truncate(F);
            F -= i;
            I += (double)i;
        }
        public int CompareTo(Quad other) {
            int c = I.CompareTo(other.I);
            if (c != 0)
                return c;
            return F.CompareTo(other.F);
        }
        public static bool operator >(Quad a, Quad b) {
            return a.CompareTo(b) == +1;
        }
        public static bool operator <(Quad a, Quad b) {
            return a.CompareTo(b) == -1;
        }
        public bool Equals(Quad other) {
            return
                I.Equals(other.I) &&
                F.Equals(other.F);
        }
        public override string ToString() {
            string Dec = F.ToString("G15");
            int j = -1;
            for (int i = Dec.Length - 1; i >= 0; i--) {
                if (Dec[i] != '0') { j = i + 1; break; }
            }
            if (j != -1) {
                Dec = Dec.Remove(j, Dec.Length - j);
            }
            string Int = "";
            string Hold = I.ToString("0");
            if (Dec.Split('.').Length == 2) {

                if (Dec.Split('.')[0].Contains('-')) {
                    if (Hold.Contains('-')) {

                    }
                    else {
                        Int = "-";
                    }

                }
                Dec = Dec.Split('.')[1];
            }
            Int += Hold + "." + Dec;
            return Int;
        }
    }

    public class BooleanVariable {
        public string Name;
        private bool Val = false;
        public BooleanVariable(string name, bool value) {
            Name = name;
            Value = value;
        }
        public object Value {
            get {
                return Val;
            }
            set {
                if (value.GetType() == typeof(bool)) {
                    Val = (bool)value;
                }
                else if (value.GetType() == typeof(string)) {
                    string Temp = value.ToString().ToLower().Replace(" ", "").Replace(Constants.Tab.ToString(), "");
                    if (Temp == "false") { Val = false; }
                    else if (Temp == "true") { Val = true; }
                    else if (Temp == "0") { Val = false; }
                    else if (Temp == "1") { Val = true; }
                }
                else if (MathHandler.IsNumericalDataType(value)) {
                    long Temp = (long)value;
                    if (Temp <= 0) { Val = false; }
                    else if (Temp >= 1) { Val = true; }
                }
            }
        }
    }
}
