using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;

namespace Handlers {
    public class PipeHandler {
        public static NamedPipeServerStream PipeServer;
        public delegate void TransmitHandler(object Object);
        public static event TransmitHandler TransmitEvent;
        public static void Transmit(string Pipe, string Message, bool WaitForConnection = false) {
            using (PipeServer = new NamedPipeServerStream(Pipe, PipeDirection.Out)) {
                StreamWriter PipeWriter = new StreamWriter(PipeServer);
                if (WaitForConnection == true) { PipeServer.WaitForConnection(); }
                PipeWriter.WriteLine(Message);
                PipeWriter.Flush();
                TransmitEvent?.Invoke(null);
            }
        }


        public delegate void ReceiveHandler(object Object, PipeArgs ReceiveArgs);
        public static event ReceiveHandler DataInEvent;


        public static NamedPipeClientStream PipeClient;
        public static System.Threading.Thread PipeThread;
        public static string Name;
        public static bool Enable = true;
        public static void PipeConnection(string name) {
            Name = name;
            PipeThread = new Thread(PipeListen) {
                IsBackground = true
            };
            PipeThread.Start();
        }

        private static void PipeListen() {
            while (Enable) {
                using (PipeClient = new NamedPipeClientStream(".", Name, PipeDirection.In)) {
                    if (!PipeClient.IsConnected) {
                        PipeClient.Connect();
                    }
                    using (StreamReader PipeReader = new StreamReader(PipeClient)) {
                        while (PipeReader.Peek() > -1) {
                            PipeArgs Pe = new PipeArgs();

                            Debug.Print(PipeReader.ReadLine());
                            DataInEvent?.Invoke(null, Pe);
                        }
                    }
                }
            }
        }
    }
    public class PipeConnection {
        public delegate void ReceiveHandler(object Object, PipeArgs ReceiveArgs);
        public event ReceiveHandler DataInEvent;


        public NamedPipeClientStream PipeClient;
        public System.Threading.Thread PipeThread;
        public string Name;
        public bool Enable = true;
        public PipeConnection(string name) {
            Name = name;
            PipeThread = new Thread(PipeListen) {
                IsBackground = true
            };
            PipeThread.Start();
        }

        public void PipeListen() {
            while (Enable) {
                using (PipeClient = new NamedPipeClientStream(".", Name, PipeDirection.In)) {
                    if (!PipeClient.IsConnected) {
                        PipeClient.Connect();
                    }
                    using (StreamReader PipeReader = new StreamReader(PipeClient)) {
                        while (PipeReader.Peek() > -1) {
                            PipeArgs Pe = new PipeArgs();

                            Debug.Print(PipeReader.ReadLine());
                            DataInEvent?.Invoke(null, Pe);
                        }
                    }
                }
            }
        }

    }
    public class PipeArgs : EventArgs {
        public string Value { get; set; }
    }
}
