// '======================================
// STRING HANDLER
// 
// JULIAN HIRNIAK
// COPYRIGHT (C) 2015-2020 J.J.HIRNIAK
// '======================================
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Handlers {
    public class StringHandler {
        public static string EncapsulateString(string Input) {
            return Constants.DblQuote + Input + Constants.DblQuote;
        }
        public static string STR_APP_VER(bool WithVersion) {
            char rechr = ' ';
            string CMPLIE_STR = "";
            string FINAL_STR = "";
            string fullval = Assembly.GetEntryAssembly().GetName().Name.ToUpper();
            var splitval = fullval.Split(rechr);
            var cnt = fullval.Split(rechr);
            if (cnt.Count() <= 1) {
                string SP_1 = splitval[0];
                if (splitval[0].Length >= 3)
                    CMPLIE_STR = SP_1[0].ToString() + SP_1[1].ToString() + SP_1[2].ToString();
                else
                    CMPLIE_STR = "ODA";
            }
            if (cnt.Count() == 2) {
                string SP_1 = splitval[0];
                string SP_2 = splitval[1];
                CMPLIE_STR = SP_1[0].ToString() + SP_1[SP_1.Length - 1].ToString() + SP_2[0].ToString();
            }
            if (cnt.Count() >= 3) {
                string SP_1 = splitval[0];
                string SP_2 = splitval[1];
                string SP_3 = splitval[2];
                CMPLIE_STR = SP_1[0].ToString() + SP_2[0].ToString() + SP_3[0].ToString();
            }
            if (WithVersion == true)
                FINAL_STR = CMPLIE_STR + Assembly.GetEntryAssembly().GetName().Version.Major.ToString() + "." + Assembly.GetEntryAssembly().GetName().Version.Minor.ToString();
            //FINAL_STR = CMPLIE_STR + Conversions.ToString(Metrics.My.MyProject.Application.Info.Version.Major) + "." + Conversions.ToString(Metrics.My.MyProject.Application.Info.Version.Minor);
            else
                FINAL_STR = CMPLIE_STR;
            return FINAL_STR;
        }
        public static double STR_SOS(string Text) {
            return Encoding.ASCII.GetByteCount(Text) / (double)1024;
        }
        // STRING SEPERATION
        /// <summary>
        /// A single combined string split into different parts based on the delimiter.
        /// </summary>
        /// <param name="value">This string needed to be split.</param>
        /// <param name="chr">The delimiter character.</param>
        /// <param name="pos">Position of splitted string to return.</param>
        /// <remarks></remarks>
        public static string STR_SS(string value, char chr, int pos) {
            string rechr = chr.ToString();
            string fullval = value;
            var splitval = fullval.Split(rechr.ToCharArray());
            var cnt = fullval.Split(chr);
            if (pos <= cnt.Count() - 1)
                return splitval[pos];
            else
                return null;
        }
        public static string SplitTrimString(string Value, char SpiltAtCharacter, int SpiltAtCharacterOccurance, StringSide SpiltSide) {
            int End_index = 0;
            int Spilt_CharOccurance = 0;
            for (int i = 0; i < Value.Length; i++) {

                if (Value[i] == SpiltAtCharacter) {
                    if (SpiltAtCharacterOccurance == Spilt_CharOccurance) {
                        End_index = i;
                        break;
                    }
                    Spilt_CharOccurance++;
                }
            }
            if (SpiltSide == StringSide.Left) {
                if (End_index < 0) {
                    return "";
                }
                else {
                    return Value.Substring(0, End_index);
                }
            }
            else {
                if (End_index + 1 >= Value.Length) {
                    return "";
                }
                else {
                    return Value.Substring(End_index + 1, Value.Length - End_index - 1);
                }
            }
        }
        public static string SpiltString(string Value, char SpiltAtCharacter, int SubstringIndex) {
            return STR_SS(Value, SpiltAtCharacter, SubstringIndex);
        }
        public static string MergeStrings(List<string> List, char Spilter) {
            string Buffer = "";
            for (int i = 0; i < List.Count; i++) {
                if (i != List.Count - 1) {
                    Buffer += List[i] + Spilter;
                }
                else {
                    Buffer += List[i];
                }
            }
            return Buffer;
        }
        public static List<string> SpiltStringAtCapitals(string Input) {
            List<string> List = new List<string>();
            if (Input.Length > 0) {
                List.Add("");
                for (int i = 0; i < Input.Length; i++) {
                    if (i != 0) {
                        if (char.IsUpper(Input[i])) {
                            List.Add(Input[i].ToString());
                        }
                        else {
                            List[List.Count - 1] += Input[i];
                        }
                    }
                    else {
                        List[List.Count - 1] += Input[i];
                    }
                }
            }
            return List;
        }
        public static STR_MVSSF SpiltStringMutipleValues(string Value, char SpiltAtCharacter) {
            return STR_MVSS(Value, SpiltAtCharacter);
        }
        public static string CombineSpilt(STR_MVSSF Spilt, char Spilter, int CombineFrom = 0) {
            string Output = "";
            if (CombineFrom >= Spilt.Count) { return ""; }
            for (int i = CombineFrom; i < Spilt.Count; i++) {
                Output += Spilt.Value[i];
                if (i != Spilt.Count - 1) {
                    Output += Spilter;
                }
            }
            return Output;
        }
        public static STR_MVSSF SpiltAndCombineAfter(string Value, char SpiltAtCharacter, int CombineFrom = 0) {
            STR_MVSSF SpiltTemp = STR_MVSS(Value, SpiltAtCharacter);
            string Output = "";
            if (CombineFrom >= SpiltTemp.Count) { return SpiltTemp; }
            else if (CombineFrom == 0) { return new STR_MVSSF(Value); }
            for (int i = CombineFrom; i < SpiltTemp.Count; i++) {
                Output += SpiltTemp.Value[i];
                if (i != SpiltTemp.Count - 1) {
                    Output += SpiltAtCharacter;
                }
            }
            STR_MVSSF OutputTemp = new STR_MVSSF();
            for (int i = 0; i < CombineFrom; i++) {
                OutputTemp.Value.Add(SpiltTemp.Value[i]);
            }
            OutputTemp.Value.Add(Output);
            OutputTemp.Count = CombineFrom + 1;
            return OutputTemp;
        }
        public static STR_MVSSF STR_MVSS(string value, char chr) {
            string rechr = chr.ToString();
            string fullval = value;
            var splitval = fullval.Split(rechr.ToCharArray());
            var cnt = fullval.Split(chr);
            var cp = new STR_MVSSF();
            for (int i = 0, loopTo = cnt.Count() - 1; i <= loopTo; i++)
                cp.Value.Add(splitval[i]);
            cp.Count = cnt.Count();
            return cp;
        }
        public static int CountCharacters(string Value, char CharacterToCount) {
            return STR_SCC(Value, CharacterToCount);
        }
        public static int STR_SCC(string value, char ch) {
            int cnt = 0;
            foreach (char c in value) {
                if (c == ch)
                    cnt += 1;
            }
            return cnt;
        }

        public static STR_SVIL STR_SCI(string value, char ch) {
            int cnt = 0;
            var setC = default(int);
            int ind = 0;
            var p1 = default(int);
            int p2;
            var cp = new STR_SVIL();
            foreach (char c in value) {
                if (c == ch) {
                    if (setC == 1) {
                        p2 = cnt;
                        setC = 0;
                        cp.index.Add(new StringPoint(p1, p2));
                    }
                    else {
                        p1 = cnt;
                        setC = 1;
                        if (cnt == value.Length - 1) {
                            p2 = value.Length - 1;
                            cp.index.Add(new StringPoint(p1, p2));
                        }
                    }
                    ind += 1;
                }
                else if (!(ind % 2 == 0)) {
                    if (cnt == value.Length - 1) {
                        p2 = value.Length - 1;
                        cp.index.Add(new StringPoint(p1, p2));
                    }
                }
                cnt += 1;
            }
            return cp;
        }
        public static string STR_SSRA(string value, char ch, char oldv, char newv) {
            string FINAL = value;
            foreach (var item in STR_SCI(value, ch).index) {
                string bs = STR_SSS(value, item.X, item.Y);
                string chk = bs;
                string @out = bs.Replace(oldv, newv);
                int distance = item.Y - item.X + 1;
                try {
                    FINAL = FINAL.Remove(item.X, distance).Insert(item.X, @out);
                }
                catch {
                }
            }
            return FINAL;
        }
        public static string ChangeCharacterInEncapulated(string Input, char StartEncapulator, char EndEncapulator, char OldCharacter, char NewCharacter) {
            bool InEncaps = false;
            string Output = "";
            if ((Input.Contains(StartEncapulator) && (Input.Contains(EndEncapulator)))) {
                for (int i = 0; i < Input.Length; i++) {
                    if (InEncaps == false) {
                        if (Input[i] == StartEncapulator) { InEncaps = true; Output += Input[i]; }
                    }
                    else {
                        if (Input[i] == EndEncapulator) {
                            InEncaps = false; Output += Input[i];
                        }
                        else if (Input[i] == OldCharacter) {
                            Output += NewCharacter;
                        }
                    }
                }
                return Output;
            }
            else {
                return Input;
            }
        }
        public static string ExtractStringBetweenIndices(string Input, int LowerIndex, int UpperIndex) {
            return STR_SSS(Input, LowerIndex, UpperIndex);
        }
        public static string STR_SSS(string msg, int lowerbounds, int upperbounds) {
            int lngth = msg.Length - 1;
            string bs = "";
            if (lowerbounds > lngth)
                return "";
            else
                for (int i = lowerbounds; i <= upperbounds; i++) {
                    if (i > lngth)
                        break;
                    else
                        bs += msg[i].ToString();
                }
            return bs;
        }
        public static int STR_CLO(string msg, char input) {
            int CNT = 0;
            for (int i = msg.Length - 1; i >= 0; i += -1) {
                if (msg[i] == input)
                    CNT += 1;
                else
                    break;
            }
            return CNT;
        }
        public static string RemoveFromEnd(string Input, char CharacterToTrimTill) {
            return STR_RFE(Input, CharacterToTrimTill);
        }
        public static string STR_RFE(string Input, char RemoveUntil) {
            int srt_ind = -1;
            string output = Input;
            for (int i = Input.Length - 1; i >= 0; i--) {
                if (Input[i] == RemoveUntil) {
                    srt_ind = i;
                    break;
                }
            }
            if (srt_ind != -1) {
                output = Input.Remove(srt_ind, Input.Length - srt_ind);
            }
            return output;
        }
        public static string ShortFilePath(string Input, int CutAtIndex) {
            return STR_SFN(Input, CutAtIndex);
        }
        public static string STR_SFN(string msg, int CutAt) {
            var MVS = STR_MVSS(msg, Convert.ToChar(@"\"));
            string SB = "";
            int SC = 0;
            for (int J = MVS.Count - 1; J >= 0; J += -1) {
                if (SC == CutAt) {
                    SB = @"$\" + SB;
                    break;
                }
                else if (SC == 0)
                    SB = MVS.Value[J];
                else
                    SB = MVS.Value[J] + @"\" + SB;
                SC += 1;
            }
            return SB;
        }
        // STRING REMOVE SPACES
        /// <summary>
        /// Remove spaces from input string.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <remarks></remarks>
        public static string STR_RS(string value) {
            return value.Replace(" ", "");
        }
        public static string FileAddress(string Directory, string File) {
            string append = Directory;
            if (Directory.EndsWith(@"\")) {
                append += File;
            }
            else {
                append += @"\" + File;
            }
            return append;
        }
        // STRING REMOVE BRACKETS
        /// <summary>
        /// Remove brackets from input string.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <remarks></remarks>
        [System.ComponentModel.Description("Remove Brackets")]
        public static string STR_RB(string value) {
            string invalue = value.Replace("(", "");
            invalue = value.Replace(")", "");
            return invalue;
        }
        public static string STR_RSE(string value) {
            try {
                if (value.Length > 2) {
                    string invalue = value.Remove(value.Length - 1, 1);
                    invalue = invalue.Remove(0, 1);
                    return invalue;
                }
                else
                    return "";
            }
            catch {
                return "";
            }
        }
        /// <summary>
        /// Remove quotation marks from input string.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <remarks></remarks>
        public static string STR_RQ(string value) {
            string invalue = value.Replace(Convert.ToString((char)34), "");
            invalue = value.Replace(Convert.ToString((char)34), "");
            return invalue;
        }
        /// <summary>
        /// Remove special quotation marks from input string.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <remarks></remarks>
        public static string STR_RSQ(string value) {
            string invalue = value.Replace("'", "");
            invalue = value.Replace("'", "");
            return invalue;
        }
        public static string TrimStartFromFirstOccurance(string Input, char TrimAtCharacter, bool AllowInQuotes = false) {
            if (Input.Contains(TrimAtCharacter) == true) {
                if (Input.Length > 0) {
                    int Marker = 0;
                    bool AllowTrim = false;
                    string Output = Input;
                    bool EnteredQuote = false;
                    for (int i = 0; i < Input.Length; i++) {
                        if (Input[i] == Constants.DblQuote) {
                            if (EnteredQuote == false) { EnteredQuote = true; }
                            else { EnteredQuote = false; }
                        }
                        if (AllowInQuotes == true) {
                            if (Input[i] == TrimAtCharacter) {
                                Marker = i;
                                AllowTrim = true;
                                break;
                            }
                        }
                        else {
                            if (EnteredQuote == false) {
                                if (Input[i] == TrimAtCharacter) {
                                    Marker = i;
                                    AllowTrim = true;
                                    break;
                                }

                            }
                        }

                    }
                    if (AllowTrim == true) {
                        Output = Input.Remove(0, Marker + 1);
                    }
                    return Output;
                }
                else { return Input; }
            }
            else { return Input; }
        }
        public static string STR_RSEQ(string Input, bool TrimFromFirst = false) {
            if (Input.Contains(" ")) {
                int INS = Input.IndexOf(" ");
                if (TrimFromFirst == true) {
                    if (INS > Input.Length - 1)
                        INS = 0;
                    else if (INS < 0)
                        INS = 0;
                }
                else
                    INS = 0;
                string APPENDSTRING = "";
                bool INQUOTE = false;
                for (int i = INS, loopTo = Input.Length - 1; i <= loopTo; i++) {
                    if (INQUOTE == false) {
                        if (Input[i] == (char)34)
                            INQUOTE = true;
                        if (Input[i].ToString() == " ") {
                        }
                        else
                            APPENDSTRING += Input[i].ToString();
                    }
                    else {
                        if (Input[i] == (char)34)
                            INQUOTE = false;
                        APPENDSTRING += Input[i].ToString();
                    }
                }
                return APPENDSTRING;
            }
            else
                return Input;
        }
        public static string STR_RCM(string Input, string Comment = "--") {
            if (Input.Contains(Comment) == true) {
                string HOLDSTRING = "";
                bool ONEINSTAN = false;
                for (int i = 0, loopTo = Input.Length - 1; i <= loopTo; i++) {
                    if (Input[i].ToString() == "-") {
                        if (ONEINSTAN == false)
                            ONEINSTAN = true;
                        else
                            break;
                    }
                    else {
                        HOLDSTRING += Input[i].ToString();
                        if (ONEINSTAN == true) {
                            ONEINSTAN = false;
                            HOLDSTRING += "-";
                        }
                    }
                }
                return HOLDSTRING;
            }
            else
                return Input;
        }
        public static int STR_FIOS(string Input, string SearchString, bool CaseSensitive = true, bool Ignore = false, char IgnoreCharacter = '$') {
            if (SearchString.Length == 0)
                return -1;
            else {
                if (SearchString.Length < Input.Length) {
                    bool CONTT = false;
                    int RET = -1;
                    int LEN_IN = Input.Length - 1;
                    int LEN_CM = SearchString.Length - 1;
                    for (int I = 0, loopTo = LEN_IN - LEN_CM; I <= loopTo; I++) {
                        for (int j = 0, loopTo1 = LEN_CM; j <= loopTo1; j++) {
                            string STR1 = Input[j + I].ToString();
                            string STR2 = SearchString[j].ToString();
                            char IGNO = IgnoreCharacter;
                            if (CaseSensitive == false) {
                                STR1.ToLower();
                                STR2.ToLower();
                                IGNO.ToString().ToLower();
                            }
                            if ((STR2 ?? "") == (STR1 ?? "")) {
                                if (j == 0)
                                    CONTT = true;
                            }
                            else if (Ignore == true) {
                                if (STR2 != IGNO.ToString()) {
                                    if (CONTT == true)
                                        CONTT = false;
                                }
                            }
                            else if (CONTT == true)
                                CONTT = false;
                        }
                        if (CONTT == true) {
                            RET = I;
                            break;
                        }
                    }
                    return RET;
                }
                else if (SearchString.Length == Input.Length) {
                    if ((SearchString ?? "") == (Input ?? ""))
                        return 0;
                }
                else
                    return -1;
                return 0;
            }
        }
        public static string STR_SSACL(string INPUT) {
            string STR = "";
            if (INPUT.Length > 0) {
                for (int I = 0, loopTo = INPUT.Length - 1; I <= loopTo; I++) {
                    if (char.IsUpper(INPUT[I]))
                        STR += " " + INPUT[I].ToString();
                    else
                        STR += INPUT[I].ToString();
                }
                return STR;
            }
            else
                return INPUT;
        }
        public static string NewLineTab(int TabAmount) {
            string ss = Environment.NewLine;
            if (TabAmount >= 1) {
                for (int i = 0; i < TabAmount; i++) {
                    ss += Constants.Tab;
                }
            }
            return ss;
        }
        public static string AddTabs(int TabAmount, string Input) {
            string ss = "";
            if (TabAmount >= 1) {
                for (int i = 0; i < TabAmount; i++) {
                    ss += Constants.Tab;
                }
            }
            return ss + Input;
        }
        public static string ConcatStringList(List<string> Strings, bool NewLinePerString = false) {
            string concat = "";
            try {
                if (Strings.Count >= 1) {
                    int indx = 0;
                    foreach (string s in Strings) {
                        if (NewLinePerString == true) {
                            if (indx == 0) {
                                concat = s;
                            }
                            else {
                                concat += Environment.NewLine + s;
                            }
                        }
                        else {
                            concat += s;
                        }
                        indx++;
                    }
                }
                return concat;
            }
            catch {
                return "";
            }
        }
        public static string STR_RSFS(string INPUT, char LookFor) {
            try {
                //string STR = "";
                int INDX = -1;
                if (INPUT.Length > 0) {
                    for (int I = INPUT.Length - 1; I >= 0; I += -1) {
                        if (INPUT[I] == LookFor) {
                            INDX = I;
                            break;
                        }
                    }
                    if (INDX == -1)
                        return INPUT;
                    else {
                        INPUT = INPUT.Remove(0, INDX + 1);
                        return INPUT;
                    }
                }
                else
                    return INPUT;
            }
            catch {
                return INPUT;
            }
        }
        /// <summary>
        /// Automatically formats the string to have a number of spaces at the end.
        /// </summary>
        /// <param name="msg">Input string.</param>
        /// <param name="spacer">Total length of string.</param>
        /// <remarks></remarks>
        public static string STR_ASF(string msg, int spacer) {
            string dstr = msg;
            int islA = dstr.Length;
            int limitA = spacer;
            if (limitA - islA < -1)
                return dstr + Space(1);
            else
                return dstr + Space(limitA - islA);
        }
        /// <summary>
        /// Automatic replacement: Replaces all occurances of one character with another.
        /// </summary>
        /// <param name="msg ">Input string.</param>
        /// <param name="oldv">Old character to replace.</param>
        /// <param name="newv">New character to replace with.</param>
        /// <remarks></remarks>
        public static string Space(int length) {
            int i = 0;
            string bult = "";
            if (length != 0) {
                for (i = 0; i < length; i++) {
                    bult += " ";
                }
            }
            return bult;
        }
        public static string CharacterString(int length, char CharacterToPlace) {
            int i = 0;
            string bult = "";
            if (length != 0) {
                for (i = 0; i < length; i++) {
                    bult += CharacterToPlace;
                }
            }
            return bult;
        }
        public static string PadWithSpace(string Input, int Length) {
            int Len = Input.Length;
            if (Len >= Length) {
                return Input;
            }
            else {
                return Input + Space(Length - Len);
            }
        }
        public static string PadWithCharacter(string Input, int Length, char CharacterToPlace, bool PostPad = true) {
            int Len = Input.Length;
            if (Len >= Length) {
                return Input;
            }
            else {
                if (PostPad == true) {
                    return Input + CharacterString(Length - Len, CharacterToPlace);
                }
                else {
                    return  CharacterString(Length - Len, CharacterToPlace) + Input;
                }
            }
        }
        public static string STR_AR(string msg, char oldv, char newv) {
            string vins = msg.Replace(oldv, newv);
            return vins;
        }
        public static string STR_ARB(string msg, char oldv, char newv, int lowerbounds, int upperbounds) {
            string bs = STR_SSS(msg, lowerbounds, upperbounds);
            string chk = bs;
            string @out = bs.Replace(oldv, newv);
            int distance = upperbounds - lowerbounds + 1;
            try {
                string final = msg.Remove(lowerbounds, distance).Insert(lowerbounds, @out);
                return final;
            }
            catch {
                return "";
            }
        }
        public static string STR_WSWC(string Input, char WrapChar) {
            string STR = WrapChar + Input + WrapChar;
            return STR;
        }
        public static string TrimStringFromStart(string Input, string Reference) {
            return STR_TRIMSTART(Input, Reference);
        }
        public static string STR_TRIMSTART(string Input, string Ref) {
            if (Input.Length >= 1) {
                string Build = "";
                int Cnt = 0;
                bool PostOut = false;
                if (Ref.Length >= 1) {
                    for (int i = 0, loopTo = Input.Length - 1; i <= loopTo; i++) {
                        if (PostOut == true) {
                            Build += Input[i].ToString();
                        }
                        if (Cnt < Ref.Length - 1) {
                            if (Ref.ToLower()[Cnt] == Input.ToLower()[i]) {
                                Cnt += 1;
                            }
                            else {
                                Cnt = 0;
                                Build = "";
                            }
                        }
                        else
                            PostOut = true;
                    }
                }
                else
                    Build = Input;
                return Build;
            }
            else
                return Input;
        }
        public static string ToSentenceCase(string Input) {
            return STR_TOSENT(Input);
        }
        public static string STR_TOSENT(string Input) {
            // ' Dim sentenceRegex = New Regex("(^[a-z])|[?!.:,;]\s+(.)", RegexOptions.ExplicitCapture)
            // ' Return sentenceRegex.Replace(Input.ToLower(), Function(s) s.Value.ToUpper())
            string bult = "";
            for (int i = 0; i < Input.Length; i++) {
                if (i == 0) {
                    bult += Input[i].ToString().ToUpper();
                }
                else {
                    string getlst = Input[i - 1].ToString();
                    if (getlst == " ") {
                        bult += Input[i].ToString().ToUpper();
                    }
                    else {
                        bult += Input[i].ToString();
                    }
                }
            }
            return bult;
            //return Strings.StrConv(Input, VbStrConv.ProperCase);
        }
        public static string ConvertTextToASCIIBinary(string Input) {
            return STR_TB(Input);
        }
        /// <summary>
        /// Converts from Text to Binary
        /// </summary>
        public static string STR_TB(string Binarys) {
            try {
                string Val = null;
                var Result = new StringBuilder();
                foreach (byte Character in Encoding.ASCII.GetBytes(Binarys)) {
                    Result.Append(Convert.ToString(Character, 2).PadLeft(8, '0'));
                    Result.Append(" ");
                }
                Val = Result.ToString().Substring(0, Result.ToString().Length - 1);
                return Val;
            }
            catch {
                return Binarys;
            }
        }
        public static string ConvertASCIIBinaryToText(string Input) {
            return STR_BT(Input);
        }
        /// <summary>
        /// Converts from Binary to Text
        /// </summary>
        public static string STR_BT(string Texts) {
            try {
                string Val = null;
                string Characters = Regex.Replace(Texts, "[^01]", "");
                var ByteArray = new byte[Convert.ToInt32(Characters.Length / (double)8 - 1) + 1];
                for (int Index = 0, loopTo = ByteArray.Length - 1; Index <= loopTo; Index++)
                    ByteArray[Index] = Convert.ToByte(Characters.Substring(Index * 8, 8), 2);
                Val = Encoding.ASCII.GetString(ByteArray);
                return Val;
            }
            catch {
                return Texts;
            }
        }
        public enum StringSide {
            Left = 0x01,
            Right = 0x02
        }
    }
    public class STR_MVSSF {
        public STR_MVSSF() { }
        public STR_MVSSF(string SingleValue) {
            Value.Add(SingleValue);
            Count = 1;
        }
        public List<string> Value = new List<string>();
        public int Count;
    }
    public class STR_SVIL {
        public List<StringPoint> index = new List<StringPoint>();
    }
    public class StringPoint {
        public int X;
        public int Y;
        public StringPoint(int x, int y) {
            this.X = x;
            this.Y = y;
        }
    }
}
//namespace Metrics
//{
//    static class ArrayExtension
//    {
//        public static void Add<T>(this ref T[] arr, T item)
//        {
//            if (arr != null) {
//                Array.Resize(ref arr, arr.Length + 1);
//                arr[arr.Length - 1] = item;
//            }
//            else {
//                arr = new T[1];
//                arr[0] = item;
//            }
//        }
//    }
//}
