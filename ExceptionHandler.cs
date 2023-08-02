// '======================================
// EXCEPTION HANDLER
// ONE DESKTOP COMPONENTS 
// JULIAN HIRNIAK
// COPYRIGHT (C) 2014-2018 J.J.HIRNIAK
// '======================================
// Imports System.Runtime.CompilerServices
// Imports System.Threading
// Imports System.IO
// Imports System.Text
// Imports System.Data
// Imports System.Data.SqlClient
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
//using Metrics;
using System.Runtime.InteropServices;
//using Microsoft.VisualBasic.CompilerServices;

namespace Handlers {
    public class ExceptionHandler {
        public static event MessageEventHandler TriggeredMessageEvent;
        public delegate void MessageEventHandler(object sender, ErrorMessage e);
        public static List<ErrorMessage> ErrorLog = new List<ErrorMessage>();
        public static List<string> Messages = new List<string>();
        public static List<Command> Commands = new List<Command>();
        public static string C_Year = "2020";
        public static string C_Production = Application.CompanyName + " Software";
        private const int VER_PLATFORM_WIN32s = 0;
        private const int VER_PLATFORM_WIN32_WINDOWS = 1;
        private const int VER_PLATFORM_WIN32_NT = 2;
        private const string Value = ")";
        [DllImport("user32.dll")]
        private static extern long LockWorkStation();
        [DllImport("kernel32"
       , EntryPoint = "GetVersionExA"
      )]
        public static extern long GetVersionEx(OSVERSIONINFO lpVersionInformation);
        //struct RGB_WINVER {
        //    public long PlatformID;
        //    public string VersionName;
        //    public string VersionNo;
        //    public string ServicePack;
        //    public string BuildNo;
        //}
        public struct OSVERSIONINFO {
            public long OSVSize;         // size, in bytes, of this data structure
            public long dwVerMajor;         // ie NT 3.51, dwVerMajor = 3; NT 4.0, dwVerMajor = 4.
            public long dwVerMinor;         // ie NT 3.51, dwVerMinor = 51; NT 4.0, dwVerMinor= 0.
            public long dwBuildNumber;         // NT: build number of the OS
            public long PlatformID;         // Identifies the operating system platform.
            public string szCSDVersion; // * 128 'NT: string, such as "Service Pack 3"
        }
        public static void OS_Version() {
        }
        public static void LoadCommands() {
            var C1 = new Command();
            {
                var withBlock = C1;
                withBlock.Cmd = Getapp_abbrevshort();
                withBlock.Type = SXCommandType.C_Namespace;
                withBlock.Parent = "";
            }
            Commands.Add(C1);
            var C2 = new Command();
            {
                var withBlock1 = C2;
                withBlock1.Cmd = "Namespace";
                withBlock1.Type = SXCommandType.C_Property;
                withBlock1.Parent = "";
            }
            Commands.Add(C2);
            var C3 = new Command();
            {
                var withBlock2 = C3;
                withBlock2.Cmd = "Close";
                withBlock2.Type = SXCommandType.C_Event;
                withBlock2.Parent = "";
            }
            Commands.Add(C3);
            var C4 = new Command();
            {
                var withBlock3 = C4;
                withBlock3.Cmd = "Window";
                withBlock3.Type = SXCommandType.C_Class;
                withBlock3.Parent = "";
            }
            Commands.Add(C4);
            //Assembly assmb = Assembly.GetEntryAssembly();
            //Type[] Types = assmb.GetTypes()
            //foreach(Type deftype in Types) {
            //    if(deftype.BaseType == null) {
            //        continue;
            //    }
            //    if(deftype.BaseType.FullName == "System.Windows.Form.Form") {
            //        deftype.Name
            //    }
            //}

            //foreach (var formprop in //global::Metrics.My.MyProject.Forms.GetType().GetProperties()) {
            //    string lname = (string)formprop.Name;
            //    lname = lname.ToLower();
            //    if ((lname ?? "") == "form 1" | (lname ?? "") == "meloadscreen" | (lname ?? "") == "mainform" | (lname ?? "") == "cmd") {
            //    }
            //    else {
            //        var CWIN = new Command();
            //        {
            //            var withBlock4 = CWIN;
            //            withBlock4.Cmd = lname;
            //            withBlock4.Type = SXCommandType.C_Property;
            //            withBlock4.Parent = Commands[3].Cmd;
            //        }
            //        Commands.Add(CWIN);
            //    }
            //}
        }
        public static string Getapp_abbrevshort(bool UseAssemblyName = true, string ApplicationName = "") {
            string final = "";
            string fullval = Assembly.GetEntryAssembly().GetName().Name;
            if (UseAssemblyName == false) {
                fullval = ApplicationName;
            }
            var splitval = fullval.Split(' ');
            var cnt = fullval.Split(' ');
            if (cnt.Count() == 1) {
                string v1 = splitval[0].ToUpper();
                if (v1.Length >= 3) {
                    final = v1[0].ToString() + v1[2].ToString();
                    return final;
                }
                else
                    return "OD6";
            }
            else if (cnt.Count() >= 2) {
                string v1 = splitval[0].ToUpper();
                string v2 = splitval[1].ToUpper();
                final = v1[0].ToString() + v2[0].ToString();
                return final;
            }
            else
                return "OD6";
        }
        public static string Getapp_abbrev(bool UseAssemblyName = true, string ApplicationName = "") {
            string final = "";
            string fullval = Assembly.GetEntryAssembly().GetName().Name;
            if (UseAssemblyName == false) {
                fullval = ApplicationName;
            }
            var splitval = fullval.Split(' ');
            var cnt = fullval.Split(' ');
            if (cnt.Count() == 1) {
                string v1 = splitval[0].ToUpper();
                if (v1.Length >= 3) {
                    final = v1[0].ToString() + v1[1].ToString() + v1[2].ToString();
                    return final;
                }
                else
                    return "OD6";
            }
            else if (cnt.Count() == 2) {
                string v1 = splitval[0].ToUpper();
                string v2 = splitval[1].ToUpper();
                final = v1[0].ToString() + v2[0].ToString() + v2[v2.Length - 1].ToString();
                return final;
            }
            else if (cnt.Count() >= 3) {
                string v1 = splitval[0].ToUpper();
                string v2 = splitval[1].ToUpper();
                string v3 = splitval[2].ToUpper();
                final = v1[0].ToString() + v2[0].ToString() + v3[0].ToString();
                return final;
            }
            else
                return "OD6";
        }
        public static string Copyright_Info(Style_Copyright Style, bool ShowPeriod) {
            if (Style == (int)Style_Copyright.LongCopyright) {
                string cout = "Copyright © " + C_Year + " " + Application.CompanyName + ". All Rights Reserved";
                string dout = "";
                if (ShowPeriod == true) {
                    dout = cout + ".";
                    return dout;
                }
                else {
                    dout = cout;
                    return dout;
                }
            }
            else if ((int)Style == (int)Style_Copyright.LongCopyrightWithoutReserves) {
                string cout = "Copyright © " + C_Year + " " + Application.CompanyName;
                string dout = "";
                if (ShowPeriod == true) {
                    dout = cout + ".";
                    return dout;
                }
                else {
                    dout = cout;
                    return dout;
                }
            }
            else if ((int)Style == (int)Style_Copyright.ShortCopyright) {
                string cout = "© " + C_Year + " " + Application.CompanyName + ". All Rights Reserved";
                string dout = "";
                if (ShowPeriod == true) {
                    dout = cout + ".";
                    return dout;
                }
                else {
                    dout = cout;
                    return dout;
                }
            }
            else if ((int)Style == (int)Style_Copyright.ShortCopyrightWithoutReserves) {
                string cout = "© " + C_Year + " " + Application.CompanyName;
                string dout = "";
                if (ShowPeriod == true) {
                    dout = cout + ".";
                    return dout;
                }
                else {
                    dout = cout;
                    return dout;
                }
            }
            else
                return null;
        }
        public static void PostMessage(ErrorType MessageSeverity, string Message, string Application, string Source, string Code, object Invoker = null, bool AppendToLog = false) {
            ErrorMessage ErrMsg = new ErrorMessage(MessageSeverity, Message, Application, Source, Code);
            if (AppendToLog == true) {
                ErrorLog.Add(ErrMsg);
            }
            TriggeredMessageEvent?.Invoke(Invoker, ErrMsg);
        }
        public static void Print(string msg) {
            // Print_Status(msg)
            //global::Metrics.My.MyProject.MyForms.logs.ConsoleInterface1.Print(msg);
            //if (global::Metrics.My.MyProject.MyForms.logs.Visible == true)
            //    global::Metrics.My.MyProject.MyForms.logs.ConsoleInterface1.Invalidate();
        }
        public static void Print_Status(string Status) {
            try {
                //if (global::Metrics.My.MyProject.MyForms.MainForm.Visible == true)
                //    global::Metrics.My.MyProject.MyForms.MainForm.Status.Message = Status;

                Messages.Add(Status);
                //if (global::Metrics.My.MyProject.MyForms.logs.Visible == true)
                //    global::Metrics.My.MyProject.MyForms.logs.ConsoleInterface1.Print(Status);
            }
            catch {
            }
        }
        public static void Print_OnThread(string Status) {
            try {
                Messages.Add(Status);
            }
            catch {
            }
        }
        public static void Print_Status_Logs(string Status) {
            try {
                //global::Metrics.My.MyProject.MyForms.MainForm.Status.Message = Status;
                Messages.Add(Status);
                //global::Metrics.My.MyProject.MyForms.logs.ConsoleInterface1.Print(Status);
            }
            catch {
            }
        }
        public static void Print_Error(string msg, bool display, ErrorType ErrorType = ErrorType.M_Notification) {
            try {
                string ver_string = Getapp_abbrev() + Assembly.GetEntryAssembly().GetName().Version.Major.ToString() + "." + Assembly.GetEntryAssembly().GetName().Version.Minor.ToString();
                if (display == true) {
                    Clear_Set();
                    //global::Metrics.My.MyProject.MyForms.MainForm.Status.Message = ver_string + "_" + msg;
                    //global::Metrics.My.MyProject.MyForms.err.Tag = ver_string + "_" + msg;
                    Messages.Add(msg);
                    //global::Metrics.My.MyProject.MyForms.err.Show();
                    //if (global::Metrics.My.MyProject.MyForms.logs.Visible == true)
                    //    global::Metrics.My.MyProject.MyForms.logs.ConsoleInterface1.Print(msg);
                }
                else {
                    Clear_Set();
                    //global::Metrics.My.MyProject.MyForms.MainForm.Status.Message = ver_string + "_" + msg;
                    Messages.Add(msg);
                    //if (global::Metrics.My.MyProject.MyForms.logs.Visible == true)
                    //    global::Metrics.My.MyProject.MyForms.logs.ConsoleInterface1.Print(msg);
                }
            }
            catch {
            }
        }
        public static void Clear_Set() {
            try {
                //global::Metrics.My.MyProject.MyForms.MainForm.Status.Message = "";
            }
            catch {
            }
        }
        public static string Msg_out(string tpe) {
            if ((tpe ?? "") == "system.appdomainunloadedexception")
                return "APP_DOMAINUNLOAD:";
            else if ((tpe ?? "") == "system.argumentexception")
                return "APP_ARGS:";
            else if ((tpe ?? "") == "system.arithmeticexception")
                return "APP_ARITHMETIC:";
            else if ((tpe ?? "") == "system.arraytypemismatchexception")
                return "APP_ARRAY_FAULT:";
            else if ((tpe ?? "") == "system.badimageformatexception")
                return "IO_BADIMAGE:";
            else if ((tpe ?? "") == "system.cannotunloadappdomainexception")
                return "APP_DOMAINUNLOAD:";
            else if ((tpe ?? "") == "system.componentmodel.design.serialization")
                return "COM_SERIALD:";
            else if ((tpe ?? "") == "system.codedomserializerexception")
                return "COM_SERIAL_DOM:";
            else if ((tpe ?? "") == "system.componentmodel.licenseexception")
                return "COM_LICENSE:";
            else if ((tpe ?? "") == "system.componentmodel.warningexception")
                return "COM_WARN:";
            else if ((tpe ?? "") == "system.configuration.configurationexception")
                return "SET_CONFIG:";
            else if ((tpe ?? "") == "system.configuration.install.installexception")
                return "SET_CONFIG_INSTALL:";
            else if ((tpe ?? "") == "system.contextmarshalexception")
                return "SYS_CONTEXT:";
            else if ((tpe ?? "") == "system.data.dataexception")
                return "DBR_DATA_FAIL:";
            else if ((tpe ?? "") == "system.data.dbconcurrencyexception")
                return "DBR_DATA_CONCURRENT:";
            else if ((tpe ?? "") == "system.data.sqlclient.sqlexception")
                return "DBR_DATA_SQLCLIENT:";
            else if ((tpe ?? "") == "system.data.sqltypes.sqltypeexception")
                return "DBR_DATA_SQL_TYPES:";
            else if ((tpe ?? "") == "system.drawing.printing.invalidprinterexception")
                return "APP_DISPLAY_FAULT:";
            else if ((tpe ?? "") == "system.enterpriseservices.registrationexception")
                return "APP_ESERV_REG:";
            else if ((tpe ?? "") == "system.enterpriseservices.serviced")
                return "APP_ESERV_SEV:";
            else if ((tpe ?? "") == "system.componentexception")
                return "APP_COM:";
            else if ((tpe ?? "") == "system.executionengineexception")
                return "APP_EXECUTE:";
            else if ((tpe ?? "") == "system.formatexception")
                return "APP_INVAILD_FORMAT:";
            else if ((tpe ?? "") == "system.indexoutofrangeexception")
                return "IO_OUT_OF_RANGE:";
            else if ((tpe ?? "") == "system.invalidcastexception")
                return "IO_CAST_FAIL:";
            else if ((tpe ?? "") == "system.invalidoperationexception")
                return "IO_INVAILDOPER:";
            else if ((tpe ?? "") == "system.invalidprogramexception")
                return "IO_OUT_OF_RANGE:";
            else if ((tpe ?? "") == "system.io.internalbufferoverflowexception")
                return "IO_BUFFEROF:";
            else if ((tpe ?? "") == "system.io.ioexception")
                return "IO_READ_WRITEIO:";
            else if ((tpe ?? "") == "system.management.managementexception")
                return "SYS_MANAGE:";
            else if ((tpe ?? "") == "system.memberaccessexception")
                return "APP_MEMBERACCESS:";
            else if ((tpe ?? "") == "system.multicastnotsupportedexception")
                return "APP_MULTICAST:";
            else if ((tpe ?? "") == "system.notimplementedexception")
                return "APP_NOTIMPLEMENTED:";
            else if ((tpe ?? "") == "system.notsupportedexception")
                return "APP_NOTSUPPORTED:";
            else if ((tpe ?? "") == "system.nullreferenceexception")
                return "APP_NULL_CHAIN:";
            else if ((tpe ?? "") == "system.outofmemoryexception")
                return "IO_MEMORY_ALLOC:";
            else if ((tpe ?? "") == "system.rankexception")
                return "APP_RANK:";
            else if ((tpe ?? "") == "system.reflection.ambiguousmatch")
                return "APP_REFLECTION_HALT:";
            else if ((tpe ?? "") == "system.exception")
                return "APP_EXCEPTION:";
            else if ((tpe ?? "") == "system.reflection.reflectiontype")
                return "APP_REFLECTION_HALT:";
            else if ((tpe ?? "") == "system.loadexception")
                return "APP_LAUNCH:";
            else if ((tpe ?? "") == "system.resources.missingmanifest")
                return "HALT:";
            else if ((tpe ?? "") == "system.resourceexception")
                return "HALT:";
            else if ((tpe ?? "") == "system.runtime.interopservices")
                return "HALT:";
            else if ((tpe ?? "") == "system.stackoverflowexception")
                return "APP_STACKIO:";
            else if ((tpe ?? "") == "system.externalexception")
                return "APP_EXTERN:";
            else if ((tpe ?? "") == "system.overflowexception")
                return "APP_OVRFLOW:";
            else
                return "APP_ERROR:";
        }
        public static void ThrownException(string type, string messge, bool Showmsg, ErrorType ErrorType = ErrorType.M_Notification) {
            string tpe = type.ToLower();
            string tme = String.Format("{0:dd}/{0:MM} {0:HH}:{0:mm}:{0:ss}", DateTime.Now);
            string hdr = Msg_out(tpe);
            string msg = hdr + " (" + tme + ")  -> " + messge;
            Print_Error(msg, Showmsg, ErrorType);
        }
        private static void PrintErrorMessage(string msg) {
            Print_Status(msg);
            Messages.Add(msg);
            //global::Metrics.My.MyProject.MyForms.logs.ConsoleInterface1.Print(msg, global::Metrics.Types.InvaildCommand);
        }
        public static void Command_Memory(string CommandString) {
            if ((CommandString.ToLower() ?? "") == "clr" | (CommandString.ToLower() ?? "") == "clear")
                GC.Collect();
            else if ((CommandString.ToLower() ?? "") == "collectcount" | (CommandString.ToLower() ?? "") == "colcnt" | (CommandString.ToLower() ?? "") == "colcnt") {
            }
        }

        public static void Command_SX(string CommandString) {
            if ((CommandString.ToLower() ?? "") == "list") {
                Print("SX Variable Table");
                Print("---------------------------");
                Print(Handlers.StringHandler.STR_ASF("Name", 25) + Handlers.StringHandler.STR_ASF("Type", 5) + StringHandler.Space(5) + "Value");
                Print(" ");
                for (int i = 0, loopTo = Handlers.DocumentHandler.VAR.Count - 1; i <= loopTo; i++) {
                    string outp = Handlers.StringHandler.STR_ASF(Handlers.DocumentHandler.VAR[i].Name, 25) + Handlers.StringHandler.STR_ASF(Handlers.DocumentHandler.VAR[i].DataType.ToString(), 5);
                    outp += Handlers.DocumentHandler.VAR[i].Value.ToString();
                    Print(outp);
                }
            }
            else if ((CommandString.ToLower() ?? "") == "parms") {
                Print("SX Parameter Table");
                Print("---------------------------");
                for (int i = 0, loopTo1 = Handlers.DocumentHandler.PARM.Count - 1; i <= loopTo1; i++) {
                    string title = Handlers.StringHandler.STR_ASF("Name: " + Handlers.DocumentHandler.PARM[i].Name, 25) + "Members: " + Handlers.DocumentHandler.PARM[i].VALUES.Count.ToString();
                    Print(title);
                    Print(StringHandler.Space(title.Length).Replace(" ", "+"));
                    if (Handlers.DocumentHandler.PARM[i].VALUES.Count > 0) {
                        Print("   " + Handlers.StringHandler.STR_ASF("Name", 25) + Handlers.StringHandler.STR_ASF("Type", 5) + "Value");
                        Print(" ");
                        for (int j = 0, loopTo2 = Handlers.DocumentHandler.PARM[i].VALUES.Count - 1; j <= loopTo2; j++) {
                            string outp = "   " + Handlers.StringHandler.STR_ASF(Handlers.DocumentHandler.PARM[i].VALUES[j].Name, 25) + Handlers.StringHandler.STR_ASF(Handlers.DocumentHandler.PARM[i].VALUES[j].DataType.ToString(), 5);
                            outp += Handlers.DocumentHandler.PARM[i].VALUES[j].Value.ToString();
                            Print(outp);
                        }
                    }
                    Print(" ");
                }
            }
        }


        private static bool CompareCommand(string Command, string CommandShort) {
            if (Command.ToLower().StartsWith(CommandShort.ToLower() + "("))
                return true;
            else
                return false;
        }
        private static string FormatCommand(string Command, string CommandShort) {
            string CMSHT = CommandShort.ToLower() + "(";
            if (Command.ToLower().StartsWith(CMSHT)) {
                string CMDFORM = Command.Remove(0, CMSHT.Length);
                if (CMDFORM.EndsWith(")")) {
                    string CMXFORM = CMDFORM.Remove(CMDFORM.Length - 1, 1);
                    return CMXFORM;
                }
                else
                    return CMDFORM;
            }
            else
                return " ";
        }
        public enum Style_Copyright {
            /// <summary>
            /// Long Copyright text
            /// </summary>
            LongCopyright = 0,
            /// <summary>
            /// Long Copyright text without reserves
            /// </summary>
            LongCopyrightWithoutReserves = 1,
            /// <summary>
            /// Short Copyright text
            /// </summary>
            ShortCopyright = 2,
            /// <summary>
            /// Short Copyright text without reserves
            /// </summary>
            ShortCopyrightWithoutReserves = 3
        }
    }
    public enum ErrorType {
        M_Notification = 0,
        M_Warning = 1,
        M_Error = 2,
        M_CriticalError = 3
    }
    public enum SXCommandType {
        C_Class = 0,
        C_Namespace = 1,
        C_Property = 2,
        C_Event = 3,
        C_Routine = 4
    }
    public class Command {
        public string Cmd;
        public SXCommandType Type;
        public string Parent;
        public int ID;
    }
    public class ErrorMessage : EventArgs {
        ErrorType severity;
        public ErrorType Severity { get => severity; }
        string message = "";
        public string Message { get => message; set => message = value; }
        string errorCode = "";
        public string ErrorCode { get => errorCode; }
        string source = "";
        public string Source { get => source; set => source = value; }
        string applicationame = "";
        public string ApplicationName { get => applicationame; }
        DateTime time;
        public DateTime Time { get => time; }
        public ErrorMessage(ErrorType MessageSeverity, string Message, string Application, string Source, string Code) {
            severity = MessageSeverity;
            message = Message;
            applicationame = Application;
            source = Source;
            errorCode = Code;
            time = DateTime.Now;
        }
        public override string ToString() {
            string BuildString = "";
            if (applicationame != "") { BuildString += applicationame; }
            if (BuildString != "") {
                if (source != "") { BuildString += ", " + source; }
            }
            else {
                if (source != "") { BuildString += source; }
            }
            if (BuildString != "") { BuildString += ": "; }
            BuildString += String.Format("{0:dd}/{0:MM} {0:HH}:{0:mm}:{0:ss}", time);
            if (errorCode != "") { BuildString += ", " + errorCode; }
            BuildString += ": " + message;
            return BuildString;
        }
    }
}

