namespace CodeArt.Media.Converter
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    public class ConvertLiveMediaTask
    {
        private Thread CopyFromStdOutThread;
        private Thread CopyToStdInThread;
        private FFMpegConverter FFMpegConv;
        private Process FFMpegProcess;
        private FFMpegProgress ffmpegProgress;
        private string FFMpegToolArgs;
        private Stream Input;
        private string lastErrorLine;
        private Exception lastStreamException;
        private Stream Output;
        public EventHandler OutputDataReceived;
        private long WriteBytesCount;

        internal ConvertLiveMediaTask(FFMpegConverter ffmpegConv, string ffMpegArgs, Stream inputStream, Stream outputStream, FFMpegProgress progress)
        {
            this.Input = inputStream;
            this.Output = outputStream;
            this.FFMpegConv = ffmpegConv;
            this.FFMpegToolArgs = ffMpegArgs;
            this.ffmpegProgress = progress;
        }

        public void Abort()
        {
            if (this.CopyToStdInThread != null)
            {
                this.CopyToStdInThread = null;
            }
            if (this.CopyFromStdOutThread != null)
            {
                this.CopyFromStdOutThread = null;
            }
            try
            {
                this.FFMpegProcess.Kill();
            }
            catch (InvalidOperationException)
            {
            }
        }

        protected void CopyFromStdOut()
        {
            int num;
            byte[] buffer = new byte[0x10000];
            Thread copyFromStdOutThread = this.CopyFromStdOutThread;
            Stream baseStream = this.FFMpegProcess.StandardOutput.BaseStream;
        Label_0023:
            if (!object.ReferenceEquals(copyFromStdOutThread, this.CopyFromStdOutThread))
            {
                return;
            }
            try
            {
                num = baseStream.Read(buffer, 0, buffer.Length);
            }
            catch (Exception exception)
            {
                this.OnStreamError(exception, true);
                return;
            }
            if (num <= 0)
            {
                Thread.Sleep(30);
                goto Label_0023;
            }
            if (!object.ReferenceEquals(copyFromStdOutThread, this.CopyFromStdOutThread))
            {
                return;
            }
            try
            {
                this.Output.Write(buffer, 0, num);
                this.Output.Flush();
            }
            catch (Exception exception2)
            {
                this.OnStreamError(exception2, false);
                return;
            }
            if (this.OutputDataReceived != null)
            {
                this.OutputDataReceived(this, EventArgs.Empty);
            }
            goto Label_0023;
        }

        protected void CopyToStdIn()
        {
            int num;
            byte[] buffer = new byte[0x10000];
            Thread copyToStdInThread = this.CopyToStdInThread;
            Process fFMpegProcess = this.FFMpegProcess;
            Stream baseStream = this.FFMpegProcess.StandardInput.BaseStream;
        Label_002A:
            try
            {
                num = this.Input.Read(buffer, 0, buffer.Length);
            }
            catch (Exception exception)
            {
                this.OnStreamError(exception, false);
                return;
            }
            if (num > 0)
            {
                if (((this.FFMpegProcess == null) || !object.ReferenceEquals(copyToStdInThread, this.CopyToStdInThread)) || !object.ReferenceEquals(fFMpegProcess, this.FFMpegProcess))
                {
                    return;
                }
                try
                {
                    baseStream.Write(buffer, 0, num);
                    baseStream.Flush();
                    goto Label_002A;
                }
                catch (Exception exception2)
                {
                    this.OnStreamError(exception2, true);
                    return;
                }
            }
            this.FFMpegProcess.StandardInput.Close();
        }

        private void OnStreamError(Exception ex, bool isStdinStdout)
        {
            if (!(ex is IOException) || !isStdinStdout)
            {
                this.lastStreamException = ex;
                this.Abort();
            }
        }

        public void Start()
        {
            this.lastStreamException = null;
            string fFMpegExePath = this.FFMpegConv.GetFFMpegExePath();
            if (!File.Exists(fFMpegExePath))
            {
                throw new FileNotFoundException("Cannot find ffmpeg tool: " + fFMpegExePath);
            }
            ProcessStartInfo startInfo = new ProcessStartInfo(fFMpegExePath, "-stdin " + this.FFMpegToolArgs) {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(fFMpegExePath),
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.Default
            };
            this.FFMpegConv.InitStartInfo(startInfo);
            this.FFMpegProcess = Process.Start(startInfo);
            if (this.FFMpegConv.FFMpegProcessPriority != ProcessPriorityClass.Normal)
            {
                this.FFMpegProcess.PriorityClass = this.FFMpegConv.FFMpegProcessPriority;
            }
            this.lastErrorLine = null;
            this.ffmpegProgress.Reset();
            this.FFMpegProcess.ErrorDataReceived += delegate (object o, DataReceivedEventArgs args) {
                if (args.Data != null)
                {
                    this.lastErrorLine = args.Data;
                    this.ffmpegProgress.ParseLine(args.Data);
                    this.FFMpegConv.FFMpegLogHandler(args.Data);
                }
            };
            this.FFMpegProcess.BeginErrorReadLine();
            if (this.Input != null)
            {
                this.CopyToStdInThread = new Thread(new ThreadStart(this.CopyToStdIn));
                this.CopyToStdInThread.Start();
            }
            else
            {
                this.CopyToStdInThread = null;
            }
            if (this.Output != null)
            {
                this.CopyFromStdOutThread = new Thread(new ThreadStart(this.CopyFromStdOut));
                this.CopyFromStdOutThread.Start();
            }
            else
            {
                this.CopyFromStdOutThread = null;
            }
        }

        public void Stop()
        {
            this.Stop(false);
        }

        public void Stop(bool forceFFMpegQuit)
        {
            if (this.CopyToStdInThread != null)
            {
                this.CopyToStdInThread = null;
            }
            if (forceFFMpegQuit)
            {
                if ((this.Input == null) && (this.WriteBytesCount == 0L))
                {
                    this.FFMpegProcess.StandardInput.WriteLine("q\n");
                    this.FFMpegProcess.StandardInput.Close();
                }
                else
                {
                    this.Abort();
                }
            }
            else
            {
                this.FFMpegProcess.StandardInput.BaseStream.Close();
            }
            this.Wait();
        }

        public void Wait()
        {
            this.FFMpegProcess.WaitForExit(0x7fffffff);
            if (this.CopyToStdInThread != null)
            {
                this.CopyToStdInThread = null;
            }
            if (this.CopyFromStdOutThread != null)
            {
                this.CopyFromStdOutThread = null;
            }
            if (this.FFMpegProcess.ExitCode != 0)
            {
                throw new FFMpegException(this.FFMpegProcess.ExitCode, this.lastErrorLine ?? "Unknown error");
            }
            if (this.lastStreamException != null)
            {
                throw new IOException(this.lastStreamException.Message, this.lastStreamException);
            }
            this.FFMpegProcess.Close();
            this.ffmpegProgress.Complete();
        }

        public void Write(byte[] buf, int offset, int count)
        {
            if (this.FFMpegProcess.HasExited)
            {
                if (this.FFMpegProcess.ExitCode != 0)
                {
                    throw new FFMpegException(this.FFMpegProcess.ExitCode, string.IsNullOrEmpty(this.lastErrorLine) ? "FFMpeg process has exited" : this.lastErrorLine);
                }
                throw new FFMpegException(-1, "FFMpeg process has exited");
            }
            this.FFMpegProcess.StandardInput.BaseStream.Write(buf, offset, count);
            this.FFMpegProcess.StandardInput.BaseStream.Flush();
            this.WriteBytesCount += count;
        }

        internal class StreamOperationContext
        {
            private bool isInput;
            private bool isRead;

            internal StreamOperationContext(Stream stream, bool isInput, bool isRead)
            {
                this.TargetStream = stream;
                this.isInput = isInput;
                this.isRead = isRead;
            }

            public bool IsInput
            {
                get
                {
                    return this.isInput;
                }
            }

            public bool IsOutput
            {
                get
                {
                    return !this.isInput;
                }
            }

            public bool Read
            {
                get
                {
                    return this.isRead;
                }
            }

            public Stream TargetStream { get; private set; }

            public bool Write
            {
                get
                {
                    return !this.isRead;
                }
            }
        }
    }
}

