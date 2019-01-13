namespace CodeArt.Media.Converter
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Web;

    public class FFMpegConverter : IDisposable
    {
        private Process FFMpegProcess;
        private static object globalObj = new object();

        public event EventHandler<ConvertProgressEventArgs> ConvertProgress;

        public event EventHandler<FFMpegLogEventArgs> LogReceived;

        public FFMpegConverter()
        {
            this.FFMpegProcessPriority = ProcessPriorityClass.Normal;
            this.LogLevel = "info";
            this.FFMpegToolPath = string.Format("{0}_Media\\", AppDomain.CurrentDomain.BaseDirectory);
            if (string.IsNullOrEmpty(this.FFMpegToolPath))
            {
                this.FFMpegToolPath = Path.GetDirectoryName(typeof(FFMpegConverter).Assembly.Location);
            }
            this.FFMpegExeName = "ffmpeg.exe";
        }

        public void Abort()
        {
            this.EnsureFFMpegProcessStopped();
        }

        private string CommandArgParameter(string arg)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('"');
            builder.Append(arg);
            builder.Append('"');
            return builder.ToString();
        }

        protected string ComposeFFMpegCommandLineArgs(string inputFile, string inputFormat, string outputFile, string outputFormat, ConvertSettings settings)
        {
            //return "-y -loglevel info -f dshow -i audio=\"麦克风 (Realtek High Definition Audio)\" -f gdigrab  -draw_mouse 1 -i \"desktop\" -ar 44100 -r 30 -s hd1080  -qscale 15 udp://127.0.0.1:7777";
            //return "-y -f dshow -i audio=\"您听到的声音 (Sound Blaster Recon3Di)\" -f gdigrab -draw_mouse 1 -video_size 1920x1080 -i desktop -vcodec h264 " + this.CommandArgParameter(outputFile);

            StringBuilder builder = new StringBuilder();

            if (!string.IsNullOrEmpty(settings.AudioDevice))
            {
                builder.AppendFormat("-f dshow -i audio=\"{0}\"", settings.AudioDevice);
            }

            if (settings.AppendSilentAudioStream)
            {
                builder.Append(" -f lavfi -i aevalsrc=0 ");
            }
            if (settings.Seek.HasValue)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, " -ss {0}", new object[] { settings.Seek });
            }
            if (inputFormat != null)
            {
                builder.Append(" -f " + inputFormat);
            }
            if (settings.CustomInputArgs != null)
            {
                builder.AppendFormat(" {0} ", settings.CustomInputArgs);
            }
            StringBuilder outputArgs = new StringBuilder();
            this.ComposeFFMpegOutputArgs(outputArgs, outputFormat, settings);
            if (settings.AppendSilentAudioStream)
            {
                outputArgs.Append(" -shortest ");
            }
            return string.Format("-y -loglevel {4} {0} -i {1} {2} {3}", new object[] { builder.ToString(), this.CommandArgParameter(inputFile), outputArgs.ToString(), this.CommandArgParameter(outputFile), this.LogLevel });
        }

        protected void ComposeFFMpegOutputArgs(StringBuilder outputArgs, string outputFormat, OutputSettings settings)
        {
            if (settings != null)
            {
                if (settings.MaxDuration.HasValue)
                {
                    outputArgs.AppendFormat(CultureInfo.InvariantCulture, " -t {0}", new object[] { settings.MaxDuration });
                }
                if (outputFormat != null)
                {
                    outputArgs.AppendFormat(" -f {0} ", outputFormat);
                }
                if (settings.AudioSampleRate.HasValue)
                {
                    outputArgs.AppendFormat(" -ar {0}", settings.AudioSampleRate);
                }
                if (settings.AudioCodec != null)
                {
                    outputArgs.AppendFormat(" -acodec {0}", settings.AudioCodec);
                }
                if (settings.VideoFrameCount.HasValue)
                {
                    outputArgs.AppendFormat(" -vframes {0}", settings.VideoFrameCount);
                }
                if (settings.VideoFrameRate.HasValue)
                {
                    outputArgs.AppendFormat(" -r {0}", settings.VideoFrameRate);
                }
                if (settings.VideoCodec != null)
                {
                    outputArgs.AppendFormat(" -vcodec {0}", settings.VideoCodec);
                }
                if (settings.VideoFrameSize != null)
                {
                    outputArgs.AppendFormat(" -s {0}", settings.VideoFrameSize);
                }
                if (settings.CustomOutputArgs != null)
                {
                    outputArgs.AppendFormat(" {0} ", settings.CustomOutputArgs);
                }
            }
        }

        public void ConcatMedia(string[] inputFiles, string outputFile, string outputFormat, ConcatSettings settings)
        {
            this.EnsureFFMpegLibs();
            string fFMpegExePath = this.GetFFMpegExePath();
            if (!File.Exists(fFMpegExePath))
            {
                throw new FileNotFoundException("Cannot find ffmpeg tool: " + fFMpegExePath);
            }
            StringBuilder builder = new StringBuilder();
            foreach (string str2 in inputFiles)
            {
                if (!File.Exists(str2))
                {
                    throw new FileNotFoundException("Cannot find input video file: " + str2);
                }
                builder.AppendFormat(" -i {0} ", this.CommandArgParameter(str2));
            }
            StringBuilder outputArgs = new StringBuilder();
            this.ComposeFFMpegOutputArgs(outputArgs, outputFormat, settings);
            outputArgs.Append(" -filter_complex \"");
            outputArgs.AppendFormat("concat=n={0}", inputFiles.Length);
            if (settings.ConcatVideoStream)
            {
                outputArgs.Append(":v=1");
            }
            if (settings.ConcatAudioStream)
            {
                outputArgs.Append(":a=1");
            }
            if (settings.ConcatVideoStream)
            {
                outputArgs.Append(" [v]");
            }
            if (settings.ConcatAudioStream)
            {
                outputArgs.Append(" [a]");
            }
            outputArgs.Append("\" ");
            if (settings.ConcatVideoStream)
            {
                outputArgs.Append(" -map \"[v]\" ");
            }
            if (settings.ConcatAudioStream)
            {
                outputArgs.Append(" -map \"[a]\" ");
            }
            string arguments = string.Format("-y -loglevel {3} {0} {1} {2}", new object[] { builder.ToString(), outputArgs, this.CommandArgParameter(outputFile), this.LogLevel });
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(fFMpegExePath, arguments) {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(this.FFMpegToolPath),
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                this.InitStartInfo(startInfo);
                if (this.FFMpegProcess != null)
                {
                    throw new InvalidOperationException("FFMpeg process is already started");
                }
                this.FFMpegProcess = Process.Start(startInfo);
                if (this.FFMpegProcessPriority != ProcessPriorityClass.Normal)
                {
                    this.FFMpegProcess.PriorityClass = this.FFMpegProcessPriority;
                }
                string lastErrorLine = string.Empty;
                FFMpegProgress ffmpegProgress = new FFMpegProgress(new Action<ConvertProgressEventArgs>(this.OnConvertProgress), this.ConvertProgress != null);
                if (settings != null)
                {
                    ffmpegProgress.MaxDuration = settings.MaxDuration;
                }
                this.FFMpegProcess.ErrorDataReceived += delegate (object o, DataReceivedEventArgs args) {
                    if (args.Data != null)
                    {
                        lastErrorLine = args.Data;
                        ffmpegProgress.ParseLine(args.Data);
                        this.FFMpegLogHandler(args.Data);
                    }
                };
                this.FFMpegProcess.OutputDataReceived += delegate (object o, DataReceivedEventArgs args) {
                };
                this.FFMpegProcess.BeginOutputReadLine();
                this.FFMpegProcess.BeginErrorReadLine();
                this.WaitFFMpegProcessForExit();
                if (this.FFMpegProcess.ExitCode != 0)
                {
                    throw new FFMpegException(this.FFMpegProcess.ExitCode, lastErrorLine);
                }
                this.FFMpegProcess.Close();
                this.FFMpegProcess = null;
                ffmpegProgress.Complete();
            }
            catch (Exception)
            {
                this.EnsureFFMpegProcessStopped();
                throw;
            }
        }

        public ConvertLiveMediaTask ConvertLiveMedia(string inputFormat, Stream outputStream, string outputFormat, ConvertSettings settings)
        {
            return this.ConvertLiveMedia((Stream) null, inputFormat, outputStream, outputFormat, settings);
        }

        public ConvertLiveMediaTask ConvertLiveMedia(Stream inputStream, string inputFormat, Stream outputStream, string outputFormat, ConvertSettings settings)
        {
            this.EnsureFFMpegLibs();
            string toolArgs = this.ComposeFFMpegCommandLineArgs("-", inputFormat, "-", outputFormat, settings);
            return this.CreateLiveMediaTask(toolArgs, inputStream, outputStream, settings);
        }

        public ConvertLiveMediaTask ConvertLiveMedia(Stream inputStream, string inputFormat, string outputFile, string outputFormat, ConvertSettings settings)
        {
            this.EnsureFFMpegLibs();
            string toolArgs = this.ComposeFFMpegCommandLineArgs("-", inputFormat, outputFile, outputFormat, settings);
            return this.CreateLiveMediaTask(toolArgs, inputStream, null, settings);
        }

        public ConvertLiveMediaTask ConvertLiveMedia(string inputSource, string inputFormat, Stream outputStream, string outputFormat, ConvertSettings settings)
        {
            this.EnsureFFMpegLibs();
            string toolArgs = this.ComposeFFMpegCommandLineArgs(inputSource, inputFormat, "-", outputFormat, settings);
            return this.CreateLiveMediaTask(toolArgs, null, outputStream, settings);
        }

        internal void ConvertMedia(Media input, Media output, ConvertSettings settings)
        {
            this.EnsureFFMpegLibs();
            string filename = input.Filename;
            if (filename == null)
            {
                filename = Path.GetTempFileName();
                using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    this.CopyStream(input.DataStream, stream, 0x40000);
                }
            }
            string path = output.Filename;
            if (path == null)
            {
                path = Path.GetTempFileName();
            }
            if (((output.Format == "flv") || (Path.GetExtension(path).ToLower() == ".flv")) && !settings.AudioSampleRate.HasValue)
            {
                settings.AudioSampleRate = 0xac44;
            }
            try
            {
                string fFMpegExePath = this.GetFFMpegExePath();
                if (!File.Exists(fFMpegExePath))
                {
                    throw new FileNotFoundException("Cannot find ffmpeg tool: " + fFMpegExePath);
                }
                string arguments = this.ComposeFFMpegCommandLineArgs(filename, input.Format, path, output.Format, settings);
                ProcessStartInfo startInfo = new ProcessStartInfo(fFMpegExePath, arguments) {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = Path.GetDirectoryName(this.FFMpegToolPath),
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                this.InitStartInfo(startInfo);
                if (this.FFMpegProcess != null)
                {
                    throw new InvalidOperationException("FFMpeg process is already started");
                }
                this.FFMpegProcess = Process.Start(startInfo);
                if (this.FFMpegProcessPriority != ProcessPriorityClass.Normal)
                {
                    this.FFMpegProcess.PriorityClass = this.FFMpegProcessPriority;
                }
                string lastErrorLine = string.Empty;
                FFMpegProgress ffmpegProgress = new FFMpegProgress(new Action<ConvertProgressEventArgs>(this.OnConvertProgress), this.ConvertProgress != null);
                if (settings != null)
                {
                    ffmpegProgress.Seek = settings.Seek;
                    ffmpegProgress.MaxDuration = settings.MaxDuration;
                }
                this.FFMpegProcess.ErrorDataReceived += delegate (object o, DataReceivedEventArgs args) {
                    if (args.Data != null)
                    {
                        lastErrorLine = args.Data;
                        ffmpegProgress.ParseLine(args.Data);
                        this.FFMpegLogHandler(args.Data);
                    }
                };
                this.FFMpegProcess.OutputDataReceived += delegate (object o, DataReceivedEventArgs args) {
                };
                this.FFMpegProcess.BeginOutputReadLine();
                this.FFMpegProcess.BeginErrorReadLine();
                this.WaitFFMpegProcessForExit();
                if (this.FFMpegProcess.ExitCode != 0)
                {
                    throw new FFMpegException(this.FFMpegProcess.ExitCode, lastErrorLine);
                }
                this.FFMpegProcess.Close();
                this.FFMpegProcess = null;
                ffmpegProgress.Complete();
                if (output.Filename == null)
                {
                    using (FileStream stream2 = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        this.CopyStream(stream2, output.DataStream, 0x40000);
                    }
                }
            }
            catch (Exception)
            {
                this.EnsureFFMpegProcessStopped();
                throw;
            }
            finally
            {
                if (((filename != null) && (input.Filename == null)) && File.Exists(filename))
                {
                    File.Delete(filename);
                }
                if (((path != null) && (output.Filename == null)) && File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        public void ConvertMedia(string inputFile, Stream outputStream, string outputFormat)
        {
            this.ConvertMedia(inputFile, null, outputStream, outputFormat, null);
        }

        public void ConvertMedia(string inputFile, string outputFile, string outputFormat)
        {
            this.ConvertMedia(inputFile, null, outputFile, outputFormat, null);
        }

        public void ConvertMedia(FFMpegInput[] inputs, string output, string outputFormat, OutputSettings settings)
        {
            if ((inputs == null) || (inputs.Length == 0))
            {
                throw new ArgumentException("At least one ffmpeg input should be specified");
            }
            FFMpegInput input = inputs[inputs.Length - 1];
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < (inputs.Length - 1); i++)
            {
                FFMpegInput input2 = inputs[i];
                if (input2.Format != null)
                {
                    builder.Append(" -f " + input2.Format);
                }
                if (input2.CustomInputArgs != null)
                {
                    builder.AppendFormat(" {0} ", input2.CustomInputArgs);
                }
                builder.AppendFormat(" -i {0} ", this.CommandArgParameter(input2.Input));
            }
            ConvertSettings outputSettings = new ConvertSettings();
            settings.CopyTo(outputSettings);
            outputSettings.CustomInputArgs = builder.ToString() + input.CustomInputArgs;
            this.ConvertMedia(input.Input, input.Format, output, outputFormat, outputSettings);
        }

        public void ConvertMedia(string inputFile, string inputFormat, Stream outputStream, string outputFormat, ConvertSettings settings)
        {
            if (inputFile == null)
            {
                throw new ArgumentNullException("inputFile");
            }
            if ((File.Exists(inputFile) && string.IsNullOrEmpty(Path.GetExtension(inputFile))) && (inputFormat == null))
            {
                throw new Exception("Input format is required for file without extension");
            }
            if (outputFormat == null)
            {
                throw new ArgumentNullException("outputFormat");
            }
            Media input = new Media {
                Filename = inputFile,
                Format = inputFormat
            };
            Media output = new Media {
                DataStream = outputStream,
                Format = outputFormat
            };
            this.ConvertMedia(input, output, settings ?? new ConvertSettings());
        }

        public void ConvertMedia(string inputFile, string inputFormat, string outputFile, string outputFormat, ConvertSettings settings)
        {
            if (inputFile == null)
            {
                throw new ArgumentNullException("inputFile");
            }
            if (outputFile == null)
            {
                throw new ArgumentNullException("outputFile");
            }
            if ((File.Exists(inputFile) && string.IsNullOrEmpty(Path.GetExtension(inputFile))) && (inputFormat == null))
            {
                throw new Exception("Input format is required for file without extension");
            }
            if (string.IsNullOrEmpty(Path.GetExtension(outputFile)) && (outputFormat == null))
            {
                throw new Exception("Output format is required for file without extension");
            }
            Media input = new Media {
                Filename = inputFile,
                Format = inputFormat
            };
            Media output = new Media {
                Filename = outputFile,
                Format = outputFormat
            };
            this.ConvertMedia(input, output, settings ?? new ConvertSettings());
        }

        private void CopyStream(Stream inputStream, Stream outputStream, int bufSize)
        {
            int num;
            byte[] buffer = new byte[bufSize];
            while ((num = inputStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                outputStream.Write(buffer, 0, num);
            }
        }

        private ConvertLiveMediaTask CreateLiveMediaTask(string toolArgs, Stream inputStream, Stream outputStream, ConvertSettings settings)
        {
            FFMpegProgress progress = new FFMpegProgress(new Action<ConvertProgressEventArgs>(this.OnConvertProgress), this.ConvertProgress != null);
            if (settings != null)
            {
                progress.Seek = settings.Seek;
                progress.MaxDuration = settings.MaxDuration;
            }
            return new ConvertLiveMediaTask(this, toolArgs, inputStream, outputStream, progress);
        }

        private void EnsureFFMpegLibs()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string[] manifestResourceNames = executingAssembly.GetManifestResourceNames();
            string str = "NReco.VideoConverter.FFMpeg.";
            foreach (string str2 in manifestResourceNames)
            {
                if (str2.StartsWith(str))
                {
                    string path = str2.Substring(str.Length);
                    string str4 = Path.Combine(this.FFMpegToolPath, Path.GetFileNameWithoutExtension(path));
                    lock (globalObj)
                    {
                        if (!File.Exists(str4) || (File.GetLastWriteTime(str4) <= File.GetLastWriteTime(executingAssembly.Location)))
                        {
                            using (GZipStream stream2 = new GZipStream(executingAssembly.GetManifestResourceStream(str2), CompressionMode.Decompress, false))
                            {
                                using (FileStream stream3 = new FileStream(str4, FileMode.Create, FileAccess.Write, FileShare.None))
                                {
                                    int num;
                                    byte[] buffer = new byte[0x10000];
                                    while ((num = stream2.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        stream3.Write(buffer, 0, num);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void EnsureFFMpegProcessStopped()
        {
            if ((this.FFMpegProcess != null) && !this.FFMpegProcess.HasExited)
            {
                try
                {
                    this.FFMpegProcess.Kill();
                    this.FFMpegProcess = null;
                }
                catch (Exception)
                {
                }
            }
        }

        public void ExtractFFmpeg()
        {
            this.EnsureFFMpegLibs();
        }

        internal void FFMpegLogHandler(string line)
        {
            if (this.LogReceived != null)
            {
                this.LogReceived(this, new FFMpegLogEventArgs(line));
            }
        }

        internal string GetFFMpegExePath()
        {
            return Path.Combine(this.FFMpegToolPath, this.FFMpegExeName);
        }

        public void GetVideoThumbnail(string inputFile, Stream outputJpegStream)
        {
            this.GetVideoThumbnail(inputFile, outputJpegStream, null);
        }

        public void GetVideoThumbnail(string inputFile, string outputFile)
        {
            this.GetVideoThumbnail(inputFile, outputFile, null);
        }

        public void GetVideoThumbnail(string inputFile, Stream outputJpegStream, float? frameTime)
        {
            Media input = new Media {
                Filename = inputFile
            };
            Media output = new Media {
                DataStream = outputJpegStream,
                Format = "mjpeg"
            };
            ConvertSettings settings = new ConvertSettings {
                VideoFrameCount = 1,
                Seek = frameTime,
                MaxDuration = 1f
            };
            this.ConvertMedia(input, output, settings);
        }

        public void GetVideoThumbnail(string inputFile, string outputFile, float? frameTime)
        {
            Media input = new Media {
                Filename = inputFile
            };
            Media output = new Media {
                Filename = outputFile,
                Format = "mjpeg"
            };
            ConvertSettings settings = new ConvertSettings {
                VideoFrameCount = 1,
                Seek = frameTime,
                MaxDuration = 1f
            };
            this.ConvertMedia(input, output, settings);
        }

        internal void InitStartInfo(ProcessStartInfo startInfo)
        {
            if (this.FFMpegProcessUser != null)
            {
                if (this.FFMpegProcessUser.Domain != null)
                {
                    startInfo.Domain = this.FFMpegProcessUser.Domain;
                }
                if (this.FFMpegProcessUser.UserName != null)
                {
                    startInfo.UserName = this.FFMpegProcessUser.UserName;
                }
                if (this.FFMpegProcessUser.Password != null)
                {
                    startInfo.Password = this.FFMpegProcessUser.Password;
                }
            }
        }

        public void Invoke(string ffmpegArgs)
        {
            this.EnsureFFMpegLibs();
            try
            {
                string fFMpegExePath = this.GetFFMpegExePath();
                if (!File.Exists(fFMpegExePath))
                {
                    throw new FileNotFoundException("Cannot find ffmpeg tool: " + fFMpegExePath);
                }
                ProcessStartInfo startInfo = new ProcessStartInfo(fFMpegExePath, ffmpegArgs) {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = Path.GetDirectoryName(this.FFMpegToolPath),
                    RedirectStandardInput = true,
                    RedirectStandardOutput = false,
                    RedirectStandardError = true
                };
                this.InitStartInfo(startInfo);
                if (this.FFMpegProcess != null)
                {
                    throw new InvalidOperationException("FFMpeg process is already started");
                }
                this.FFMpegProcess = Process.Start(startInfo);
                if (this.FFMpegProcessPriority != ProcessPriorityClass.Normal)
                {
                    this.FFMpegProcess.PriorityClass = this.FFMpegProcessPriority;
                }
                string lastErrorLine = string.Empty;
                this.FFMpegProcess.ErrorDataReceived += delegate (object o, DataReceivedEventArgs args) {
                    if (args.Data != null)
                    {
                        lastErrorLine = args.Data;
                        this.FFMpegLogHandler(args.Data);
                    }
                };
                this.FFMpegProcess.BeginErrorReadLine();
                this.WaitFFMpegProcessForExit();
                if (this.FFMpegProcess.ExitCode != 0)
                {
                    throw new FFMpegException(this.FFMpegProcess.ExitCode, lastErrorLine);
                }
                this.FFMpegProcess.Close();
                this.FFMpegProcess = null;
            }
            catch (Exception)
            {
                this.EnsureFFMpegProcessStopped();
                throw;
            }
        }

        internal void OnConvertProgress(ConvertProgressEventArgs args)
        {
            if (this.ConvertProgress != null)
            {
                this.ConvertProgress(this, args);
            }
        }

        public bool Stop()
        {
            if (((this.FFMpegProcess != null) && !this.FFMpegProcess.HasExited) && this.FFMpegProcess.StartInfo.RedirectStandardInput)
            {
                this.FFMpegProcess.StandardInput.WriteLine("q\n");
                this.FFMpegProcess.StandardInput.Close();
                this.WaitFFMpegProcessForExit();
                return true;
            }
            return false;
        }

        protected void WaitFFMpegProcessForExit()
        {
            if (this.FFMpegProcess == null)
            {
                throw new FFMpegException(-1, "FFMpeg process was aborted");
            }
            if (!this.FFMpegProcess.HasExited)
            {
                int milliseconds = this.ExecutionTimeout.HasValue ? ((int) this.ExecutionTimeout.Value.TotalMilliseconds) : 0x7fffffff;
                if (!this.FFMpegProcess.WaitForExit(milliseconds))
                {
                    this.EnsureFFMpegProcessStopped();
                    throw new FFMpegException(-2, string.Format("FFMpeg process exceeded execution timeout ({0}) and was aborted", this.ExecutionTimeout));
                }
            }
        }

        public void Dispose()
        {
            Process[] processes = Process.GetProcessesByName("ffmpeg");
            foreach (Process p in processes)
            {
                p.Kill();
                p.Close();
            }
        }

        public TimeSpan? ExecutionTimeout { get; set; }

        public string FFMpegExeName { get; set; }

        public ProcessPriorityClass FFMpegProcessPriority { get; set; }

        public FFMpegUserCredential FFMpegProcessUser { get; set; }

        public string FFMpegToolPath { get; set; }

        public string LogLevel { get; set; }
    }
}

