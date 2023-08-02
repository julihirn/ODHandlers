using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Handlers.ShadowX {
    public class SXInterpreter {
        private string name;
        public string Name { get => name; }
        private string fileName;
        public string FileName { get => fileName; set => fileName = value; }
        private bool postOutput = true;
        public bool PostOutput { get => postOutput; set => postOutput = value; }

        //public List<string> Lines = new List<string>();
        public List<CodeListItem> CodeList = new List<CodeListItem>();
        public static List<ErrorListItem> ErrorList = new List<ErrorListItem>();

        public List<SXClass> Classes = new List<SXClass>();
        public string ExecutionTime(bool DisplayAsTime = true) {
            string Output = "00:00:00";
            if (IsExecuting == false) {
                if (DisplayAsTime == true) {
                    return Output;
                }
                else {
                    return "0";
                }
            }
            else {
                if (DisplayAsTime == true) {
                    return ConversionHandler.DateDifference(StartExecutionTime, DateTime.Now).ToString();
                }
                else {
                    return ConversionHandler.DateIntervalDifference(StartExecutionTime, DateTime.Now, ConversionHandler.Interval.Second).ToString();
                }
            }
        }
        private bool IsExecuting = false;
        private DateTime StartExecutionTime;
        public void Open(bool ReadProperties = false) {
            System.IO.StreamReader FileReader = null;
            try {
                FileReader = System.IO.File.OpenText(fileName);
                if (ReadProperties == false) {
                    SXHandler.Reporter(postOutput, name + "Reading file at: " + Handlers.StringHandler.STR_SFN(fileName, 3), StatusType.Information);
                }
                else {
                    SXHandler.Reporter(PostOutput, "Reading file at: " + Handlers.StringHandler.STR_SFN(fileName, 3), StatusType.Information);
                }
                //Read document and store to memory
                int LineNumber = 0;
                while (FileReader.Peek() > -1) {
                    string CurrentLine = FileReader.ReadLine();
                    CodeList.Add(new CodeListItem(CurrentLine, LineNumber));
                }
                PreProcessor();
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
            EntryMark = false; IsVaild = false;
            Classes.Clear();
            Classes.Add(new SXClass("core", 0));
            int IndexStart = -1;
            for (int i = 0; i < CodeList.Count; i++) {
                if (IsVaild == true) {
                    IndexStart = i;
                    break;
                    //if (CodeList[i].Type == LineType.Normal) {
                    //    for (int c = 0; c < CodeList[i].FormattedText.Length; c++) {
                    //
                    //    }
                    //}
                }
                else {
                    IsVaild = CheckVaild(CodeList[i], i);
                }
            }
            if (IndexStart > 0) {
                PreprocessClasses(IndexStart); if (StopAll == true) { return; }
                for(int i = 1; i < Classes.Count; i++) {

                }
            }
        }
        private void PreprocessClasses(int StartFrom) {
            int BracketDepth = 0;
            int CurrentClass = -1;
            bool InClass = false;
            for (int i = StartFrom; i < CodeList.Count; i++) {
                if (CodeList[i].Type == LineType.Normal) {
                    string Line = CodeList[i].ToString();
                    if ((Line.ToLower().StartsWith("class:")) && (Line.ToLower().EndsWith("{"))) {
                        string Name = Line.Remove(Line.Length - 1, 1).Remove(0, 6);
                        Classes.Add(new SXClass(GetName(Name, i), i));
                        CurrentClass = BracketDepth;
                        BracketDepth++;
                        if (InClass == true) {
                            StopAll = true;
                            ErrorList.Add(new ErrorListItem(StatusType.Error, "INT_CLASS_CASDEF", "Cassading or multiple definitions of classes", i, 0));
                            if (StopAll == true) { return; }
                        }
                        InClass = true;
                    }
                    else if (Line.Contains("{")) { BracketDepth++; }
                    else if (Line.Contains("}")) {
                        BracketDepth--;
                        if (CurrentClass == BracketDepth) {
                            if (Classes.Count > 0) {
                                Classes[Classes.Count - 1].EndLine = i;
                                InClass = false;
                            }
                            else {
                                StopAll = true;
                                ErrorList.Add(new ErrorListItem(StatusType.Error, "INT_CLASS_CASOUTOFRANGE", "Class runoff event occured", i, 0));
                                if (StopAll == true) { return; }
                            }
                        }
                    }
                }
                if (StopAll == true) { return; }
            }
        }
        private string GetName(string Input, int Line) {
            string Output = "";
            bool CheckVaild = true;
            Input = Input.TrimStart(' ').TrimEnd(' ').TrimStart(Constants.Tab).TrimEnd(Constants.Tab);
            for (int c = 0; c < Input.Length; c++) {
                if (CheckVaild == true) {
                    if (char.IsDigit(Input[c]) == true) {
                        StopAll = true;
                    }
                    else if (char.IsPunctuation(Input[c]) == true) {
                        StopAll = true;
                    }
                    else if (char.IsSeparator(Input[c]) == true) {
                        StopAll = true;
                    }
                    else {
                        CheckVaild = false;
                        Output += Input[c];
                    }
                }
                else {
                    if (char.IsDigit(Input[c]) == true) {
                        Output += Input[c];
                    }
                    else if (char.IsLetter(Input[c]) == true) {
                        Output += Input[c];
                    }
                    else if (Input[c] == '_') {
                        Output += Input[c];
                    }
                    else {
                        StopAll = true;
                        ErrorList.Add(new ErrorListItem(StatusType.Error, "INT_GETNAME", "Invaild name or declaration", Line, c));
                    }
                }
                if (StopAll == true) { return ""; }
            }

            if (StopAll == true) { return ""; }
            return Output;
        }
        bool StopAll = false;
        bool EntryMark = false;
        bool IsVaild = false;

        public SXInterpreter(string fileName) {
            FileName = fileName;
        }

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
    }
}
