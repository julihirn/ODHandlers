using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace Handlers {
    public class ConversionHandler {
        public enum Interval {
            Nanosecond = 0x00,
            Microsecond = 0x01,
            Millisecond = 0x02,
            Centisecond = 0x03,
            Decisecond = 0x04,
            Second = 0x05,
            Decasecond = 0x06,
            Hectosecond = 0x07,
            Kilosecond = 0x08,
            Megasecond = 0x09,
            Gigasecond = 0x0A,
            Minute = 0x0B,
            Hour = 0x0C,
            Day = 0x0D,
            Week = 0x0E,
            Month = 0x0F,
            Quarter = 0x10,
            Year = 0x11,
            Decade = 0x12,
            Century = 0x13,
            Millennium = 0x14
        }
        public enum TimeInterval {
            Millisecond = 0x01,
            Second = 0x02,
            Minute = 0x03,
            Hour = 0x04
        }
        public enum TimeFormat {
            /// <summary>HH:MM</summary>
            Time = 0,
            /// <summary>HH:MM:SS</summary>
            TimeAndSeconds = 1,
            /// <summary>HH:MM:SS.FFF</summary>
            TimeAndMilliseconds = 2,
            /// <summary>HH:MM:SS.FFF</summary>
            TimeAndDays = 3
            /// <summary>D:HH:MM:SS</summary>
        }
        public enum Prefix {
            Quecto = -12,
            Ronto = -11,
            Yocto = -10,
            Zepto = -9,
            Atto = -8,
            Femto = -7,
            Pico = -6,
            Nano = -5,
            Micro = -4,
            Milli = -3,
            Centi = -2,
            Deci = -1,
            None = 0,
            Deca = 1,
            Hecto = 2,
            Kilo = 3,
            Mega = 4,
            Giga = 5,
            Tera = 6,
            Peta = 7,
            Exa = 8,
            Zetta = 9,
            Yotta = 10,
            Ronna = 11,
            Quetta = 12
        }
        public enum UnitSystem {
            SI = 0x00,
            SI_Accepted = 0x01,
            SI_Other = 0x04,
            Imperial = 0x05,
            Other = 0x10
        }
        public static List<UnitType> UnitTypes = new List<UnitType>();
        static ConversionHandler() {
            //UnitTypes.Add(new UnitType("Length",new Unit("Metre","base","m",null,UnitSystem.SI),))
            //LengthUnits:
            //Units.Add(new Unit("Meter", "m", "Length", UnitSystem.SI));
            //  Units.Add(new Unit(""))

        }

        public static double PrefixConversion(Prefix FirstPrefix, Prefix SecondPrefix) {
            int Power = (int)FirstPrefix + (int)SecondPrefix;
            return Math.Pow(10, Power);
        }
        public static int PrefixToInt(Prefix prefix) {
            switch (prefix) {
                case Prefix.Quetta: return 30;
                case Prefix.Ronna: return 27;
                case Prefix.Yotta: return 24;
                case Prefix.Zetta: return 21;
                case Prefix.Exa: return 18;
                case Prefix.Peta: return 15;
                case Prefix.Tera: return 12;
                case Prefix.Giga: return 9;
                case Prefix.Mega: return 6;
                case Prefix.Kilo: return 3;
                case Prefix.Hecto: return 2;
                case Prefix.Deca: return 1;
                case Prefix.None: return 0;
                case Prefix.Deci: return -1;
                case Prefix.Centi: return -2;
                case Prefix.Milli: return -3;
                case Prefix.Micro: return -6;
                case Prefix.Nano: return -9;
                case Prefix.Pico: return -12;
                case Prefix.Femto: return -15;
                case Prefix.Atto: return -18;
                case Prefix.Zepto: return -21;
                case Prefix.Yocto: return -24;
                case Prefix.Ronto: return -27;
                case Prefix.Quecto: return -30;
                default: return 0;
            }
        }
        public static string PrefixToSymbol(Prefix prefix) {
            switch (prefix) {
                case Prefix.Quetta: return "Q";
                case Prefix.Ronna: return "R";
                case Prefix.Yotta: return "Y";
                case Prefix.Zetta: return "Z";
                case Prefix.Exa: return "E";
                case Prefix.Peta: return "P";
                case Prefix.Tera: return "T";
                case Prefix.Giga: return "G";
                case Prefix.Mega: return "M";
                case Prefix.Kilo: return "k";
                case Prefix.Hecto: return "h";
                case Prefix.Deca: return "da";
                case Prefix.None: return "";
                case Prefix.Deci: return "d";
                case Prefix.Centi: return "c";
                case Prefix.Milli: return "m";
                case Prefix.Micro: return "μ";
                case Prefix.Nano: return "n";
                case Prefix.Pico: return "p";
                case Prefix.Femto: return "f";
                case Prefix.Atto: return "a";
                case Prefix.Zepto: return "z";
                case Prefix.Yocto: return "y";
                case Prefix.Ronto: return "r";
                case Prefix.Quecto: return "q";
                default: return "";
            }
        }
        public static bool BittoBoolean(byte Input, int Bit) {
            if ((Bit >= 0) && (Bit < 8)) {
                byte shift = (byte)(0x80 >> (7 - Bit));
                if ((Input & shift) == shift) { return true; }
                else { return false; }
            }
            else { return false; }
        }
        public static int BittoInteger(byte Input, int Bit) {
            if ((Bit >= 0) && (Bit < 8)) {
                byte shift = (byte)(0x80 >> (7 - Bit));
                if ((Input & shift) == shift) { return 0x01; }
                else { return 0x00; }
            }
            else { return 0x00; }
        }
        public static string BytetoBinaryString(byte Input) {
            string append_output = "";
            for (int i = 0; i < 8; i++) {
                byte shift = (byte)(0x80 >> i);
                if ((Input & shift) == shift) {
                    append_output += "1";
                }
                else { append_output += "0"; }
            }
            return append_output;
        }
        public static bool IntToBool(int Input, BooleanConditioning Conditioning = BooleanConditioning.PositivesTrueNegativesFalse) {
            switch (Conditioning) {
                case BooleanConditioning.Simple:
                    if (Input == 1) { return true; }
                    else { return false; }
                case BooleanConditioning.PositivesTrueNegativesFalse:
                    if (Input >= 1) { return true; }
                    else { return false; }
                case BooleanConditioning.PositivesFalseNegativesTrue:
                    if (Input >= 1) { return false; }
                    else { return true; }
                default:
                    return false;
            }
        }
        public static string BoolToString(bool Input, bool UseIntegerOutput = true) {
            if (UseIntegerOutput == true) {
                if (Input == true) { return "1"; }
                else { return "0"; }
            }
            else {
                if (Input == true) { return "true"; }
                else { return "false"; }
            }
        }
        public static bool StringToBool(string Input) {
            string FormattedInput = Input.Replace(Constants.Tab.ToString(), "").Replace(" ", "").ToLower();
            bool NegateOutput = false;
            bool CurrentValue = false;
            if ((FormattedInput.StartsWith("~")) || (FormattedInput.StartsWith("−")) || (FormattedInput.StartsWith("-"))) {
                NegateOutput = true;
                FormattedInput = FormattedInput.Remove(0, 1);
            }
            if (Input == "true") { CurrentValue = true; }
            else if (Input == "1") { CurrentValue = true; }
            else if (Input == "false") { CurrentValue = false; }
            else if (Input == "0") { CurrentValue = false; }
            if (NegateOutput == true) {
                return !CurrentValue;
            }
            else {
                return CurrentValue;
            }
        }
        public static TimeSpan DateDifference(DateTime Start, DateTime End, bool AllowNegatives = false) {
            DateTime OfficalStart;
            DateTime OfficalEnd;
            if (AllowNegatives == false) {
                if (Start.Ticks > End.Ticks) { OfficalStart = End; OfficalEnd = Start; }
                else { OfficalStart = Start; OfficalEnd = End; }
            }
            else {
                OfficalStart = Start;
                OfficalEnd = End;
            }
            long tickdifference = OfficalEnd.Ticks - OfficalStart.Ticks;
            TimeSpan Ts = new TimeSpan(tickdifference);
            return Ts;
        }
        public static long DateIntervalDifferenceLong(DateTime Start, DateTime End, Interval ConversionInterval) {
            DateTime OfficalStart;
            DateTime OfficalEnd;
            if (Start.Ticks > End.Ticks) { OfficalStart = End; OfficalEnd = Start; }
            else { OfficalStart = Start; OfficalEnd = End; }
            long tickdifference = OfficalEnd.Ticks - OfficalStart.Ticks;
            TimeSpan Ts = new TimeSpan(tickdifference);
            long output = 0;
            switch (ConversionInterval) {
                case Interval.Nanosecond:
                    output = tickdifference * 100; break;
                case Interval.Microsecond:
                    output = (long)((double)tickdifference / (double)10); break;
                case Interval.Millisecond:                      //1 MS
                    output = (long)Ts.TotalMilliseconds; break;
                case Interval.Centisecond:                      //1 CS = 10 MS
                    output = (long)(Ts.TotalMilliseconds / (double)10); break;
                case Interval.Decisecond:                       //1 DS = 100 MS
                    output = (long)(Ts.TotalMilliseconds / (double)100); break;
                case Interval.Second:                           //1 S
                    output = (long)Ts.TotalSeconds; break;
                case Interval.Decasecond:                       //1 DAS = 10 S
                    output = (long)(Ts.TotalSeconds / (double)10); break;
                case Interval.Hectosecond:                      //1 HS = 100 S
                    output = (long)(Ts.TotalSeconds / (double)100); break;
                case Interval.Kilosecond:                       //1 KS = 1000 S
                    output = (long)(Ts.TotalSeconds / (double)1000); break;
                case Interval.Megasecond:                       //1 MS = 1000000 S
                    output = (long)(Ts.TotalSeconds / (double)1000000); break;
                case Interval.Gigasecond:                       //1 GS = 1000000000 S
                    output = (long)(Ts.TotalSeconds / (double)1000000000); break;
                case Interval.Minute:
                    output = (long)Ts.TotalMinutes; break;
                case Interval.Hour:
                    output = (long)Ts.TotalHours; break;
                case Interval.Day:
                    output = (long)Ts.TotalDays; break;
                case Interval.Week:                             //1 WK = 7 DY
                    output = (long)(Ts.TotalDays / (double)7); break;
                case Interval.Month:                            //1 MTH = 30 DY
                    output = (long)(Ts.TotalDays / (double)30); break;
                case Interval.Quarter:                          //1 QTR = 91.25 DY
                    output = (long)(Ts.TotalDays / (double)91.25); break;
                case Interval.Year:                             //1 YR = 365 DY
                    output = (long)(Ts.TotalDays / (double)365); break;
                case Interval.Decade:
                    output = (long)(Ts.TotalDays / (double)3650); break;
                case Interval.Century:
                    output = (long)(Ts.TotalDays / (double)36500); break;
                case Interval.Millennium:
                    output = (long)(Ts.TotalDays / (double)365000); break;
                default:
                    return 0;
            }
            return output;
        }
        public static double DateIntervalDifference(DateTime Start, DateTime End, Interval ConversionInterval, bool AllowNegatives = false) {
            DateTime OfficalStart;
            DateTime OfficalEnd;
            if (AllowNegatives == false) {
                if (Start.Ticks > End.Ticks) { OfficalStart = End; OfficalEnd = Start; }
                else { OfficalStart = Start; OfficalEnd = End; }
            }
            else {
                OfficalStart = Start;
                OfficalEnd = End;
            }
            long tickdifference = OfficalEnd.Ticks - OfficalStart.Ticks;
            TimeSpan Ts = new TimeSpan(tickdifference);
            double output = 0;
            switch (ConversionInterval) {
                case Interval.Nanosecond:
                    output = tickdifference * 100; break;
                case Interval.Microsecond:
                    output = (double)tickdifference / (double)10; break;
                case Interval.Millisecond:                      //1 MS
                    output = Ts.TotalMilliseconds; break;
                case Interval.Centisecond:                      //1 CS = 10 MS
                    output = Ts.TotalMilliseconds / (double)10; break;
                case Interval.Decisecond:                       //1 DS = 100 MS
                    output = Ts.TotalMilliseconds / (double)100; break;
                case Interval.Second:                           //1 S
                    output = Ts.TotalSeconds; break;
                case Interval.Decasecond:                       //1 DAS = 10 S
                    output = Ts.TotalSeconds / (double)10; break;
                case Interval.Hectosecond:                      //1 HS = 100 S
                    output = Ts.TotalSeconds / (double)100; break;
                case Interval.Kilosecond:                       //1 KS = 1000 S
                    output = Ts.TotalSeconds / (double)1000; break;
                case Interval.Megasecond:                       //1 MS = 1000000 S
                    output = Ts.TotalSeconds / (double)1000000; break;
                case Interval.Gigasecond:                       //1 GS = 1000000000 S
                    output = Ts.TotalSeconds / (double)1000000000; break;
                case Interval.Minute:
                    output = Ts.TotalMinutes; break;
                case Interval.Hour:
                    output = Ts.TotalHours; break;
                case Interval.Day:
                    output = Ts.TotalDays; break;
                case Interval.Week:                             //1 WK = 7 DY
                    output = Ts.TotalDays / (double)7; break;
                case Interval.Month:                            //1 MTH = 30 DY
                    output = Ts.TotalDays / (double)30; break;
                case Interval.Quarter:                          //1 QTR = 91.25 DY
                    output = Ts.TotalDays / (double)91.25; break;
                case Interval.Year:                             //1 YR = 365 DY
                    output = Ts.TotalDays / (double)365; break;
                case Interval.Decade:
                    output = Ts.TotalDays / (double)3650; break;
                case Interval.Century:
                    output = Ts.TotalDays / (double)36500; break;
                case Interval.Millennium:
                    output = Ts.TotalDays / (double)365000; break;
                default:
                    return 0;
            }
            return output;
        }
        public static bool IsNumber(char value) {
            if (((int)value - 0x30 >= 0) && ((int)value - 0x30 <= 9)) {
                return true;
            }
            else {
                return false;
            }
        }
        public static bool IsNumeric(string value) {
            int dec_cnt = 0;
            string trimmed = value.Trim(' ');
            bool res = false;
            for (int i = 0; i < trimmed.Length; i++) {
                if (((int)trimmed[i] - 0x30 >= 0) && ((int)trimmed[i] - 0x30 <= 9)) {
                    res = true;
                }
                else {
                    if (((trimmed[i] == '-') || (trimmed[i] == '−')) && (i == 0)) {
                        res = true;
                    }
                    else if (trimmed[i] == '.') {
                        dec_cnt++;
                        if (dec_cnt > 1) {
                            res = false;
                            break;
                        }
                    }
                    else {
                        res = false;
                        break;
                    }
                }
            }
            return res;
        }
        public static bool IsBoolean(string value, bool AllowNegatations = true, bool AllowIntegers = true) {
            if (value == null) { return false; }
            string FormattedInput = value.Replace(" ", "").ToLower();
            if (AllowNegatations == true) {
                if ((FormattedInput.StartsWith("~")) || (FormattedInput.StartsWith("−")) || (FormattedInput.StartsWith("-"))) {
                    FormattedInput = FormattedInput.Remove(0, 1);

                }
                if (FormattedInput == "true") { return true; }
                else if (FormattedInput == "false") { return true; }
                else if (FormattedInput == "1") { return AllowIntegers; }
                else if (FormattedInput == "0") { return AllowIntegers; }
                else { return false; }
            }
            else {
                if (FormattedInput == "true") { return true; }
                else if (FormattedInput == "false") { return true; }
                else if (FormattedInput == "1") { return AllowIntegers; }
                else if (FormattedInput == "0") { return AllowIntegers; }
                else { return false; }
            }
        }
        public static List<string> GetListofPrefixes() {
            return Prefix.GetNames(typeof(string)).ToList();
        }
        public static string DecimaltoTimeString(decimal Input, TimeInterval InputInterval, TimeFormat OutputFormat) {
            int ms = 0;
            int s = 0;
            int m = 0;
            int h = 0;
            int d = 0;
            if (Input >= 0) {
                if (InputInterval == TimeInterval.Millisecond) {
                    ms = (int)System.Math.Floor(Input % 1000);
                    s = (int)(System.Math.Floor(Input / (decimal)1000) % 60);

                    decimal hold = System.Math.Floor(Input / (decimal)60000);
                    m = (int)hold % 60;
                    h = (int)System.Math.Floor(hold / (decimal)60);
                }
                else if (InputInterval == TimeInterval.Second) {
                    s = (int)System.Math.Floor(Input % 60);
                    int thold = (int)System.Math.Floor(Input);
                    ms = (int)((Input - thold) * 1000);
                    decimal hold = System.Math.Floor(Input / (decimal)60);
                    m = (int)hold % 60;
                    h = (int)System.Math.Floor(hold / (decimal)60);
                }
                else if (InputInterval == TimeInterval.Minute) {
                    m = (int)System.Math.Floor(Input % 60);
                    int thold = (int)System.Math.Floor(Input);
                    decimal hold = (Input - thold) * 60;
                    int shold = (int)System.Math.Floor(hold);
                    s = shold % 60;
                    ms = (int)System.Math.Floor((hold - shold) * 1000);
                    h = (int)System.Math.Floor(Input / (decimal)60);
                }
                else if (InputInterval == TimeInterval.Hour) {
                    h = (int)System.Math.Floor(Input);
                    decimal hold = (Input - h) * 60;
                    m = (int)System.Math.Floor(hold);
                    decimal hold2 = (hold - m) * 60;
                    s = (int)System.Math.Floor(hold2) % 60;
                    ms = (int)System.Math.Floor(hold2 / (decimal)1000);
                }
            }
            if (OutputFormat == TimeFormat.Time) {
                return String.Format("{0:D2}:{1:D2}", h, m);
            }
            else if (OutputFormat == TimeFormat.TimeAndSeconds) {
                return String.Format("{0:D2}:{1:D2}:{2:D2}", h, m, s);
            }
            else if (OutputFormat == TimeFormat.TimeAndMilliseconds) {
                return String.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", h, m, s, ms);
            }
            else if (OutputFormat == TimeFormat.TimeAndDays) {
                int hr = h % 24;
                d = (int)Math.Floor((double)h / (double)24);
                return String.Format("{0:D1}:{1:D2}:{2:D2}:{3:D2}", d, hr, m, s);
            }
            else {
                return Input.ToString();
            }
        }

        public enum BooleanConditioning {
            Simple = 0x00,
            PositivesTrueNegativesFalse = 0x01,
            PositivesFalseNegativesTrue = 0x02
        }
    }
    public class UnitType {
        private string name;
        public string Name { get => name; set => name = value; }
        private string symbol;
        public string Symbol {
            get { return symbol; }
            set { symbol = value; }
        }
        private string variableName;
        public string VariableName {
            get { return variableName; }
            set { variableName = value; }
        }
        public Unit GetBaseUnit {
            get { return Units[0]; }
        }
        //public Unit BaseUnit;
        public List<Unit> Units = new List<Unit>();
        public UnitType(string name, Unit baseUnit, List<Unit> units) {
            Name = name;
            if (baseUnit.IsCompositeUnit == true) {
                //Debug.Print("");
            }
            Unit BaseUnit = baseUnit;//new Unit(baseUnit.Name, baseUnit.Symbol, baseUnit.Formula, baseUnit.System, true, baseUnit.UsePrefixEquation, baseUnit.IsCompositeUnit, baseUnit.DivideComposite, baseUnit.SecondarySymbol);
            Units.Add(BaseUnit);
            Units.AddRange(units);
        }
        public NumericalString ConvertToBase(NumericalString Value, Unit UnitFrom) {
            if (Units.Count > 0) {
                return Convert(Value, UnitFrom, Units[0]);
            }
            else {
                return new NumericalString("0");
            }
        }
        public NumericalString ConvertFromBase(NumericalString Value, Unit UnitTo) {
            if (Units.Count > 0) {
                return Convert(Value, Units[0], UnitTo);
            }
            else {
                return new NumericalString("0");
            }
        }
        //private NumericalString ProcessCompositeUnit(Unit UnitInput, NumericalString Value) {
        //    if (UnitInput.IsCompositeUnit == true) {
        //        if (UnitInput.CompositeUnitAffectsPrefix == true) {
        //            if (UnitInput.DivideComposite == true) {
        //                int TempOrder = (int)UnitInput.SecondaryPrefix *(-1);
        //                NumericalString PrefixTemp = Value / MathHandler.EvaluateExpression("10^(" + ((int)TempOrder).ToString() + ")", null);
        //                return PrefixTemp;
        //            }
        //            else {
        //                NumericalString PrefixTemp = Value * MathHandler.EvaluateExpression("10^(" + ((int)UnitInput.SecondaryPrefix).ToString() + ")", null);
        //                return PrefixTemp;
        //            }
        //        }
        //        return Value;
        //    }
        //    else { return Value; }
        //}
        private int CompositePrefix(Unit UnitInput, int Value) {
            if (UnitInput.IsCompositeUnit == true) {
                if (UnitInput.CompositeUnitAffectsPrefix == true) {
                    if (UnitInput.DivideComposite == true) {
                        int TempOrder = (int)UnitInput.SecondaryPrefix * (-1);
                        int PrefixTemp = Value + TempOrder;
                        return PrefixTemp;
                    }
                    else {
                        int TempOrder = (int)UnitInput.SecondaryPrefix;
                        int PrefixTemp = Value + TempOrder;
                        return PrefixTemp;
                    }
                }
                return Value;
            }
            else { return Value; }
        }
        public NumericalString Convert(NumericalString Value, Unit UnitFrom, Unit UnitTo) {
            Unit FromUnit = UnitFrom;//FindUnit(UnitFrom);
            Unit ToUnit = UnitTo;//FindUnit(UnitTo);
            if ((FromUnit != null) || (ToUnit != null)) {
                List<MathVariable> Vars = new List<MathVariable>();
                NumericalString PrefixToExp = MathHandler.EvaluateExpression("10^(" + (-1 * CalculatePrefix(ToUnit)).ToString() + ")", null);//MathHandler.EvaluateExpression("10^(" + (-1 * ConversionHandler.PrefixToInt(ToUnit.Prefix)).ToString() + ")", null);

                if ((FromUnit.IsBase == false) && (ToUnit.IsBase == false)) {
                    //NumericalString PrefixTemp = MathHandler.EvaluateExpression("10^(" + (CalculatePrefix(FromUnit)).ToString() + ")", null) * Value;//MathHandler.EvaluateExpression("10^(" + ((int)FromUnit.Prefix).ToString() + ")", null) * Value;
                    //Vars.Add(new MathVariable("x", PrefixTemp));
                    //NumericalString ValueCurrent = MathHandler.EvaluateExpression(FromUnit.Formula.ToBaseEquation, Vars);
                    //MathHandler.ModifyMathVariable(Vars, "x", ValueCurrent);
                    //ValueCurrent = MathHandler.EvaluateExpression(ToUnit.Formula.FromBaseEquation, Vars);
                    //return ValueCurrent * PrefixToExp;//Math.Pow(10, (int)ToUnit.Prefix);
                    return Convert(Convert(Value, FromUnit, Units[0]), Units[0], UnitTo);
                }
                else if ((FromUnit.IsBase == false) && (ToUnit.IsBase == true)) {
                    //NumericalString PrefixTemp = MathHandler.EvaluateExpression("10^(" + (CalculatePrefix(FromUnit)).ToString() + ")", null) * Value;//MathHandler.EvaluateExpression("10^(" + ((int)FromUnit.Prefix).ToString() + ")", null) * Value;
                    //if (FromUnit.UsePrefixEquation == true) {
                    //    List<MathVariable> PrefixEq = new List<MathVariable>();
                    //    PrefixEq.Add(new MathVariable("x", PrefixTemp));
                    //    PrefixTemp = MathHandler.EvaluateExpression(FromUnit.Formula.PrefixEquation, PrefixEq);
                    //}
                    //Vars.Add(new MathVariable("x", PrefixTemp));
                    //NumericalString ValueCurrent = MathHandler.EvaluateExpression(FromUnit.Formula.ToBaseEquation, Vars);
                    //if (ToUnit.UsePrefixEquation == true) {
                    //    List<MathVariable> PrefixEq = new List<MathVariable>();
                    //    PrefixEq.Add(new MathVariable("x", PrefixToExp));
                    //    PrefixToExp = MathHandler.EvaluateExpression(ToUnit.Formula.PrefixEquation, PrefixEq);
                    //    return ValueCurrent * PrefixToExp;//* Math.Pow(10, (int)ToUnit.Prefix);
                    //}
                    //else {
                    //    return ValueCurrent * PrefixToExp;
                    //}
                    Vars.Add(new MathVariable("x", Value));
                    NumericalString PrefixFrom = DetermineAndScalePrefix(FromUnit, true);
                    NumericalString TempToBase = MathHandler.EvaluateExpression(FromUnit.Formula.ToBaseEquation, Vars);
                    NumericalString PrefixTo = DetermineAndScalePrefix(ToUnit, true);
                    NumericalString ValueCurrent = TempToBase * (PrefixFrom / PrefixTo);
                    return ValueCurrent;
                }
                else if ((FromUnit.IsBase == true) && (ToUnit.IsBase == false)) {
                    //NumericalString PrefixTemp = MathHandler.EvaluateExpression("10^(" + (CalculatePrefix(FromUnit)).ToString() + ")", null);//MathHandler.EvaluateExpression("10^(" + (ConversionHandler.PrefixToInt(FromUnit.Prefix)).ToString() + ")", null);
                    //if (FromUnit.UsePrefixEquation == true) {
                    //    List<MathVariable> PrefixEq = new List<MathVariable>();
                    //    PrefixEq.Add(new MathVariable("x", PrefixTemp));
                    //    PrefixTemp = MathHandler.EvaluateExpression(FromUnit.Formula.PrefixEquation, PrefixEq);
                    //}
                    //PrefixTemp = PrefixTemp * Value;
                    //Vars.Add(new MathVariable("x", PrefixTemp));
                    //NumericalString ValueCurrent = MathHandler.EvaluateExpression(ToUnit.Formula.FromBaseEquation, Vars);
                    //if (ToUnit.UsePrefixEquation == true) {
                    //    List<MathVariable> PrefixEq = new List<MathVariable>();
                    //    PrefixEq.Add(new MathVariable("x", PrefixToExp));
                    //    PrefixToExp = MathHandler.EvaluateExpression(ToUnit.Formula.PrefixEquation, PrefixEq);
                    //    return ValueCurrent * PrefixToExp;//* Math.Pow(10, (int)ToUnit.Prefix);
                    //}
                    //else {
                    //    return ValueCurrent * PrefixToExp;//* Math.Pow(10, (int)ToUnit.Prefix);
                    //}
                    Vars.Add(new MathVariable("x", Value));
                    NumericalString PrefixFrom = DetermineAndScalePrefix(FromUnit, true);
                    NumericalString TempToBase = MathHandler.EvaluateExpression(ToUnit.Formula.FromBaseEquation, Vars);
                    NumericalString PrefixTo = DetermineAndScalePrefix(ToUnit, true);
                    NumericalString ValueCurrent = TempToBase * (PrefixFrom / PrefixTo);
                    return ValueCurrent;
                }
                else {
                    NumericalString PrefixFrom = DetermineAndScalePrefix(FromUnit, true);
                    NumericalString PrefixTo = DetermineAndScalePrefix(ToUnit, true);
                    NumericalString ValueCurrent = Value * (PrefixFrom / PrefixTo);
                    return ValueCurrent;
                    //int Exponent = (ConversionHandler.PrefixToInt(FromUnit.Prefix) - ConversionHandler.PrefixToInt(ToUnit.Prefix));
                    //int Exponent2 = (ConversionHandler.PrefixToInt(FromUnit.SecondaryPrefix) - ConversionHandler.PrefixToInt(ToUnit.SecondaryPrefix));
                    //int FinalExponent = Exponent;
                    //if (ToUnit.IsCompositeUnit == true) {
                    //    if (ToUnit.CompositeUnitAffectsPrefix == true) {
                    //        if (ToUnit.DivideComposite == true) {
                    //            FinalExponent = Exponent - Exponent2;
                    //        }
                    //        else { FinalExponent = Exponent + Exponent2; }
                    //    }
                    //}
                    //NumericalString PrefixTemp = MathHandler.EvaluateExpression("10^(" + FinalExponent.ToString() + ")", null);

                    //if (FromUnit.UsePrefixEquation == true) {
                    //    List<MathVariable> PrefixEq = new List<MathVariable>();
                    //    PrefixEq.Add(new MathVariable("x", PrefixTemp));
                    //    PrefixTemp = MathHandler.EvaluateExpression(FromUnit.Formula.PrefixEquation, PrefixEq);
                    //}
                    //NumericalString ValueCurrent = Value * PrefixTemp;
                }
            }
            else {
                return new NumericalString("0");
            }
        }
        private NumericalString DetermineAndScalePrefix(Unit Input, bool UsePrefix = true) {
            NumericalString Output = new NumericalString("1");
            if (UsePrefix == true) {
                NumericalString OutputB = new NumericalString("1");
                if (Input.UsePrefixEquation == true) {
                    List<MathVariable> PrefixEq = new List<MathVariable>();
                    NumericalString TempOutput = new NumericalString("1");
                    TempOutput = MathHandler.EvaluateExpression("10^(" + ConversionHandler.PrefixToInt(Input.Prefix) + ")", null);
                    PrefixEq.Add(new MathVariable("x", TempOutput));
                    Output = MathHandler.EvaluateExpression(Input.Formula.PrefixEquation, PrefixEq);
                }
                else {
                    List<MathVariable> PrefixEq = new List<MathVariable>();
                    PrefixEq.Add(new MathVariable("x", ConversionHandler.PrefixToInt(Input.Prefix)));
                    Output = MathHandler.EvaluateExpression("10^(x)", PrefixEq);
                }
                if (Input.IsCompositeUnit == true) {
                    if (Input.UsePrefixEquationSecondary == true) {
                        List<MathVariable> PrefixEq = new List<MathVariable>();
                        NumericalString TempOutput = new NumericalString("1");
                        TempOutput = MathHandler.EvaluateExpression("10^(" + ConversionHandler.PrefixToInt(Input.SecondaryPrefix) + ")", null);
                        PrefixEq.Add(new MathVariable("x", TempOutput));
                        OutputB = MathHandler.EvaluateExpression(Input.Formula.PrefixEquationSecondary, PrefixEq);
                    }
                    else {
                        List<MathVariable> PrefixEq = new List<MathVariable>();
                        PrefixEq.Add(new MathVariable("x", ConversionHandler.PrefixToInt(Input.SecondaryPrefix)));
                        OutputB = MathHandler.EvaluateExpression("10^(x)", PrefixEq);
                    }
                    if (Input.CompositeUnitAffectsPrefix == true) {
                        if (Input.DivideComposite == true) {
                            List<MathVariable> PrefixVars = new List<MathVariable>();
                            PrefixVars.Add(new MathVariable("x", Output));
                            PrefixVars.Add(new MathVariable("y", OutputB));
                            return MathHandler.EvaluateExpression("x/y", PrefixVars);

                        }
                        else {
                            List<MathVariable> PrefixVars = new List<MathVariable>();
                            PrefixVars.Add(new MathVariable("x", Output));
                            PrefixVars.Add(new MathVariable("y", OutputB));
                            return MathHandler.EvaluateExpression("x*y", PrefixVars);
                        }
                    }
                    else { return Output; }
                }
            }
            return Output;
        }
        private int CalculatePrefix(Unit From) {
            int Exponent = ConversionHandler.PrefixToInt(From.Prefix);
            int Exponent2 = ConversionHandler.PrefixToInt(From.SecondaryPrefix);
            int FinalExponent = Exponent;
            if (From.IsCompositeUnit == true) {
                if (From.CompositeUnitAffectsPrefix == true) {
                    if (From.DivideComposite == true) {
                        FinalExponent = Exponent - Exponent2;
                    }
                    else { FinalExponent = Exponent + Exponent2; }
                }
            }
            return FinalExponent;
        }
        //public double ConvertTo(int From, int To) {
        //    //UnitX->Base->UnitY
        //    if ((From < Units.Count)&& (To < Units.Count)&&(Units.Count >= 1)) {
        //        if (From == To)
        //    }
        //}

        private Unit FindUnit(Unit Input) {
            if (Input == null) { return null; }
            else {
                foreach (Unit U in Units) {
                    if (U.Equals(Input)) {
                        return U;
                    }
                }
                return null;
            }
        }
    }
    public class Unit {

        private string name;
        public string Name { get => name; set => name = value; }
        private string symbol;
        public string Symbol { get => symbol; set => symbol = value; }
        private string secondarysymbol = "";
        public string SecondarySymbol { get => secondarysymbol; set => secondarysymbol = value; }
        private bool isCompositeUnit = false;
        public bool IsCompositeUnit {
            get { return isCompositeUnit; }
            set { isCompositeUnit = value; }
        }
        private bool compositeUnitAffectsPrefix = false;
        public bool CompositeUnitAffectsPrefix {
            get { return compositeUnitAffectsPrefix; }
            set { compositeUnitAffectsPrefix = value; }
        }
        private bool divideComposite = false;
        public bool DivideComposite {
            get { return divideComposite; }
            set { divideComposite = value; }
        }
        public UnitConversionEquation Formula;


        public Unit(string name, string symbol, UnitConversionEquation conversionequation, ConversionHandler.UnitSystem system, bool isBase = false, bool usesPrefixEquation = false) {
            Name = name;
            Symbol = symbol;
            System = system;
            if (conversionequation != null) {
                Formula = conversionequation;
            }
            else {
                Formula = new UnitConversionEquation("x", "x"); //Do nothing
            }
            isBaseHidden = isBase;
            UsePrefixEquation = usesPrefixEquation;
        }
        public Unit(string name, string symbol, UnitConversionEquation conversionequation, ConversionHandler.UnitSystem system, bool isBase = false, bool usesPrefixEquation = false, bool HasSecondarySymbol = false, bool SecondaryHasPrefix = false, bool DivideSymbol = true, string SecondarySymbol = "", bool usesPrefixEquationSecondary = false) {
            Name = name;
            Symbol = symbol;
            System = system;
            if (conversionequation != null) {
                Formula = conversionequation;
            }
            else {
                Formula = new UnitConversionEquation("x", "x"); //Do nothing
            }
            isBaseHidden = isBase;
            UsePrefixEquation = usesPrefixEquation;
            usePrefixEquationSecondary = usesPrefixEquationSecondary;
            IsCompositeUnit = HasSecondarySymbol;
            this.SecondarySymbol = SecondarySymbol;
            DivideComposite = DivideSymbol;
            CompositeUnitAffectsPrefix = SecondaryHasPrefix;

        }
        public Unit(Unit Input) {
            this.Name = Input.Name;
            this.Symbol = Input.Symbol;
            this.System = Input.System;
            this.Formula = Input.Formula;
            this.isBaseHidden = Input.IsBase;
            this.usePrefixEquation = Input.UsePrefixEquation;
            this.usePrefixEquationSecondary = Input.UsePrefixEquationSecondary;
        }
        public Unit(Unit Input, ConversionHandler.Prefix Prefix) {
            this.Name = Input.Name;
            this.Symbol = Input.Symbol;
            this.System = Input.System;
            this.Formula = Input.Formula;
            this.isBaseHidden = Input.IsBase;
            this.usePrefixEquation = Input.UsePrefixEquation;
            this.usePrefixEquationSecondary = Input.UsePrefixEquationSecondary;
            this.Prefix = Prefix;
        }
        public Unit(Unit Input, ConversionHandler.Prefix Prefix, ConversionHandler.Prefix SecondaryPrefix) {
            if (Input == null) { return; }
            this.Name = Input.Name;
            this.IsCompositeUnit = Input.IsCompositeUnit;
            this.Symbol = Input.Symbol;
            this.System = Input.System;
            //this.secondarysystem = Input.SecondarySystem;
            this.Formula = Input.Formula;
            this.isBaseHidden = Input.IsBase;
            this.usePrefixEquation = Input.UsePrefixEquation;
            this.usePrefixEquationSecondary = Input.UsePrefixEquationSecondary;
            this.Prefix = Prefix;
            this.divideComposite = Input.DivideComposite;
            this.secondaryprefix = SecondaryPrefix;
            this.compositeUnitAffectsPrefix = Input.CompositeUnitAffectsPrefix;
        }
        private ConversionHandler.UnitSystem system;
        public ConversionHandler.UnitSystem System {
            get {
                return system;
            }
            set {
                system = value;
                if ((value == ConversionHandler.UnitSystem.Imperial) || (value == ConversionHandler.UnitSystem.SI_Other)) {
                    prefix = ConversionHandler.Prefix.None;
                }
            }
        }
        private ConversionHandler.Prefix prefix = ConversionHandler.Prefix.None;
        public ConversionHandler.Prefix Prefix {
            get {
                return prefix;
            }
            set {
                if ((system == ConversionHandler.UnitSystem.Imperial) || (system == ConversionHandler.UnitSystem.SI_Other)) {
                    prefix = ConversionHandler.Prefix.None;
                }
                else {
                    prefix = value;
                }
            }

        }
        private ConversionHandler.Prefix secondaryprefix = ConversionHandler.Prefix.None;
        public ConversionHandler.Prefix SecondaryPrefix {
            get {
                return secondaryprefix;
            }
            set {
                //if ((secondarysystem == ConversionHandler.UnitSystem.Imperial) || (secondarysystem == ConversionHandler.UnitSystem.SI_Other)) {
                //    secondaryprefix = ConversionHandler.Prefix.None;
                //}
                //else {
                //    secondaryprefix = value;
                //}
                secondaryprefix = value;
            }

        }
        public string GetSymbol() {
            if (isCompositeUnit == true) {
                if (divideComposite == true) {
                    return symbol + "/" + secondarysymbol;
                }
                else { return symbol + "·" + secondarysymbol; }
            }
            else { return symbol; }
        }

        public bool IsBase { get => isBaseHidden; }
        public string UnitString { get => ConversionHandler.PrefixToSymbol(prefix) + Symbol; }
        private bool usePrefixEquation = false;
        public bool UsePrefixEquation { get => usePrefixEquation; set => usePrefixEquation = value; }
        private bool usePrefixEquationSecondary = false;
        public bool UsePrefixEquationSecondary { get => usePrefixEquationSecondary; set => usePrefixEquationSecondary = value; }

        private bool isBaseHidden = false;
        public override string ToString() {
            return Name + " (" + Symbol + ")";
        }
        public override bool Equals(object obj) {
            if (obj is Unit) {
                string NameOfOther = ((Unit)obj).Name;
                string SymbolOfOther = ((Unit)obj).Symbol;
                if ((NameOfOther == Name) && (SymbolOfOther == Symbol)) { return true; }
                else { return false; }
            }
            else { return false; }
        }
        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
    public class UnitConversionEquation {
        public string ToBaseEquation = "";
        public string FromBaseEquation = "";
        public string PrefixEquation = "";
        public string PrefixEquationSecondary = "";
        public UnitConversionEquation(string toBaseEquation, string fromBaseEquation, string prefixEquation = "", string prefixEquationSecondary = "x") {
            ToBaseEquation = toBaseEquation;
            FromBaseEquation = fromBaseEquation;
            PrefixEquation = prefixEquation;
            PrefixEquationSecondary = prefixEquationSecondary;
        }
        public string GetEquation(bool FromBase) {
            if (FromBase == true) {
                return FromBaseEquation;
            }
            else {
                return ToBaseEquation;
            }
        }
    }
}
