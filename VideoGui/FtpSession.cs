using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VideoGui.Models;
using VideoGui.Models.delegates;

namespace VideoGui
{
    public class FtpSession
    {
        private TcpClient _client;
        private string _rootDirectory;
        private string _currentDirectory;
        private string _username;
        private int _bufferSize = 1024; // default buffer size to 1024 bytes (1KB)
        private string _password;
        databasehook<object> Invoker = null;
        private bool _loggedIn = false;
        private bool AuthenticatedUser = false;
        private bool AuthenticatedPassword = false;
        private string _restartFile;
        private long _restartPosition;
        private string _renameFrom;
        private TcpClient _dataClient;
        private TcpListener _dataListener;
        private int _umask;

        public FtpSession(TcpClient client, string rootDirectory, databasehook<object> _Invoker)
        {
            try
            {
                _client = client;
                _rootDirectory = rootDirectory;
                _currentDirectory = rootDirectory;
                Invoker = _Invoker;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error creating FTP session - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        public void HandleCommands()
        {
            try
            {
                NetworkStream stream = _client.GetStream();
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);

                writer.WriteLine("220 Welcome to FTP server");
                writer.Flush();

                while (true)
                {
                    string command = reader.ReadLine();
                    if (command == null)
                        break;
                    /*ESPV, FEAT , MLSD , MLST, PBSZ */
                    string[] parts = command.Split(' ');
                    string cmd = parts[0].ToUpper();
                    switch (cmd)
                    {
                        case "ESPV":
                            HandleEspv(parts);
                            break;
                        case "FEAT":
                            HandleFeat();
                            break;
                        case "MLSD":
                            HandleMlsd(parts[1]);
                            break;
                        case "MLST":
                            HandleMlst(parts[1]);
                            break;
                        case "PBSZ":
                            HandlePbsz(parts[1]);
                            break;
                        case "PASV":
                            HandlePasv();
                            break;
                        case "NOOP":
                            HandleNoop();
                            break;
                        case "TYPE":
                            HandleType(parts[1]);
                            break;
                        case "RMD":
                            HandleRmdAsync(parts[1]);
                            break;
                        case "SIZE":
                            HandleSIZEAsync(parts[1]);
                            break;
                        case "NLST":
                            HandleNLSTAsync(parts[1]);
                            break;
                        case "MKD":
                            HandleMkdAsync(parts[1]);
                            break;
                        case "ABOR":
                            HandleAbort();
                            break;
                        case "DELE":
                            HandleDeleAsync(parts[1]);
                            break;
                        case "MDTM":
                            HandleMDTMAsync(parts[1]);
                            break;
                        case "USER":
                            HandleUser(parts[1]);
                            break;
                        case "PASS":
                            HandlePass(parts[1]);
                            break;
                        case "CWD":
                            HandleCwd(parts[1]);
                            break;
                        case "LIST":
                            HandleList();
                            break;
                        case "RETR":
                            HandleRetrAsync(parts[1]);
                            break;
                        case "REST":
                            HandleRestAsync(parts[1]);
                            break;
                        case "STOR":
                            HandleStorAsync(parts[1]);
                            break;
                        case "PWD":
                            HandlePwd();
                            break;
                        case "QUIT":
                            HandleQuit();
                            break;
                        case "APPE":
                            HandleAppendAsync(parts[1]);
                            break;
                        case "RNFR":
                            HandleRnfrAsync(parts[1]);
                            break;
                        case "RNTO":
                            HandleRntoAsync(parts[1]);
                            break;
                        case "PORT":
                            HandlePort(parts[1]);
                            break;
                        default:
                            writer.WriteLine("500 Unknown command");
                            writer.Flush();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP commands - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void HandlePbsz(string v)
        {
            try
            {
                if (_loggedIn)
                {
                    if (string.IsNullOrEmpty(v))
                    {
                        throw new ArgumentException("PBSZ command requires a buffer size.");
                    }
                    int bufferSize;
                    if (int.TryParse(v, out bufferSize))
                    {
                        _bufferSize = bufferSize;
                        _client.GetStream().Write(Encoding.ASCII.GetBytes($"200 PBSZ command successful.\r\n"));
                    }
                    else
                    {
                        _client.GetStream().Write(Encoding.ASCII.GetBytes($"500 Invalid buffer size.\r\n"));
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP PBSZ - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void HandleMlst(string path)
        {
            try
            {
                if (_loggedIn)
                {
                    string[] files = Directory.GetFiles(path);
                    string[] directories = Directory.GetDirectories(path);

                    _client.GetStream().Write(Encoding.ASCII.GetBytes("250-MLST list follows:\r\n"));

                    foreach (string file in files)
                    {
                        string fileType = "file";
                        string fileSize = new FileInfo(file).Length.ToString();
                        string fileModifyTime = File.GetLastWriteTime(file).ToString("yyyyMMddHHmmss");
                        string filePermissions = GetFilePermissions(file);

                        _client.GetStream().Write(Encoding.ASCII.GetBytes($" type={fileType};modify={fileModifyTime};size={fileSize};perm={filePermissions}; {Path.GetFileName(file)}\r\n"));
                    }

                    foreach (string directory in directories)
                    {
                        string directoryType = "dir";
                        string directoryModifyTime = Directory.GetLastWriteTime(directory).ToString("yyyyMMddHHmmss");
                        string directoryPermissions = GetDirectoryPermissions(directory);

                        _client.GetStream().Write(Encoding.ASCII.GetBytes($" type={directoryType};modify={directoryModifyTime};perm={directoryPermissions}; {Path.GetFileName(directory)}\r\n"));
                    }

                    _client.GetStream().Write(Encoding.ASCII.GetBytes("250 End MLST list\r\n"));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP MLST - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private string GetDirectoryPermissions(string directoryPath)
        {
            try
            {
                var directoryInfo = new DirectoryInfo(directoryPath);
                var directorySecurity = directoryInfo.GetAccessControl();
                System.Security.AccessControl.AuthorizationRuleCollection accessRules = directorySecurity.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));

                var permissions = new StringBuilder();

                foreach (System.Security.AccessControl.FileSystemAccessRule rule in accessRules)
                {
                    if (rule.AccessControlType == System.Security.AccessControl.AccessControlType.Allow)
                    {
                        if ((rule.FileSystemRights & System.Security.AccessControl.FileSystemRights.Read) == System.Security.AccessControl.FileSystemRights.Read)
                            permissions.Append("r");
                        if ((rule.FileSystemRights & System.Security.AccessControl.FileSystemRights.Write) == System.Security.AccessControl.FileSystemRights.Write)
                            permissions.Append("w");
                        if ((rule.FileSystemRights & System.Security.AccessControl.FileSystemRights.ExecuteFile) == System.Security.AccessControl.FileSystemRights.ExecuteFile)
                            permissions.Append("x");
                        if ((rule.FileSystemRights & System.Security.AccessControl.FileSystemRights.Delete) == System.Security.AccessControl.FileSystemRights.Delete)
                            permissions.Append("d");
                    }
                }

                return permissions.ToString();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error getting directory permissions - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return "";
            }
        }

        private string GetFilePermissions(string file)
        {
            try
            {
                var fileInfo = new FileInfo(file);
                return fileInfo.GetAccessControl().GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount)).ToString();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error getting file permissions - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return string.Empty;
            }
        }

        private void HandleMlsd(string v)
        {
            try
            {
                if (_loggedIn)
                {
                    string directoryPath = Path.Combine(_currentDirectory, v);
                    if (Directory.Exists(directoryPath))
                    {
                        string[] files = Directory.GetFiles(directoryPath);
                        string[] directories = Directory.GetDirectories(directoryPath);
                        _client.GetStream().Write(Encoding.ASCII.GetBytes($"250-MLSD list follows\r\n"));
                        foreach (string file in files)
                        {
                            _client.GetStream().Write(Encoding.ASCII.GetBytes($" type=file;modify={File.GetLastWriteTime(file).ToString("yyyyMMddHHmmss")};size={new FileInfo(file).Length}; {Path.GetFileName(file)}\r\n"));
                        }
                        foreach (string directory in directories)
                        {
                            _client.GetStream().Write(Encoding.ASCII.GetBytes($" type=dir;modify={Directory.GetLastWriteTime(directory).ToString("yyyyMMddHHmmss")}; {Path.GetFileName(directory)}\r\n"));
                        }
                        _client.GetStream().Write(Encoding.ASCII.GetBytes($"250 End MLSD list\r\n"));
                    }
                    else
                    {
                        _client.GetStream().Write(Encoding.ASCII.GetBytes($"550 Directory does not exist.\r\n"));
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP MLSD - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void HandleFeat()
        {
            try
            {
                string[] features = new string[]
                {
        "MLST type*;size*;modify*;",
        "MLSD",
        "UTF8"
                };

                string response = "211-Features supported:\r\n";
                foreach (string feature in features)
                {
                    response += feature + "\r\n";
                }
                response += "211 End\r\n";

                _client.GetStream().Write(Encoding.ASCII.GetBytes(response));
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP FEAT - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void HandleEspv(string[] parts)
        {
            try
            {
                if (_loggedIn)
                {
                    if (parts.Length < 2)
                    {
                        throw new ArgumentException("Invalid ESPV command format.");
                    }
                    string espvCommand = parts[1].ToUpper();
                    switch (espvCommand)
                    {

                        case "ALL":
                            HandleEspvAll();
                            break;
                        case "HELP":
                            HandleEspvHelp();
                            break;
                        case "OPTS":
                            HandleEspvOpts(parts);
                            break;
                        default:
                            _client.GetStream().Write(Encoding.ASCII.GetBytes($"500 Unknown ESPV command.\r\n"));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP ESPV - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void HandleEspvOpts(string[] parts)
        {
            try
            {
                if (_loggedIn)
                {
                    if (parts.Length < 2)
                    {
                        throw new ArgumentException("Invalid ESPV OPTS command format.");
                    }
                    string espvOptsCommand = parts[1].ToUpper();
                    switch (espvOptsCommand)
                    {
                        //TODO other ESPV OPTS commands
                        case "MLSX":
                            // Implement MLSx command logic here
                            _client.GetStream().Write(Encoding.ASCII.GetBytes($"200 MLSx command successful.\r\n"));
                            break;
                        default:
                            _client.GetStream().Write(Encoding.ASCII.GetBytes($"500 Unknown ESPV OPTS command.\r\n"));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP ESPV OPTS - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void HandleEspvHelp()
        {
            _client.GetStream().Write(Encoding.ASCII.GetBytes($"214-ESPV help\r\n"));
            _client.GetStream().Write(Encoding.ASCII.GetBytes($" The ESPV command is used to specify the ESPV protocol version.\r\n"));
            _client.GetStream().Write(Encoding.ASCII.GetBytes($"214 End of ESPV help\r\n"));
        }

        private void HandleEspvAll()
        {
            try
            {
                if (_loggedIn)
                {
                    _client.GetStream().Write(Encoding.ASCII.GetBytes($"200 ESPV ALL command successful.\r\n"));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP ESPV ALL - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private bool IsPortInUse(int port)
        {
            try
            {
                using (var listener = new TcpListener(IPAddress.Any, port))
                {
                    listener.Start();
                    listener.Stop();
                    return false;
                }
            }
            catch (SocketException)
            {
                return true;
            }
        }
        private void HandlePasv()
        {
            try
            {
                if (_loggedIn)
                {
                    int port;
                    do
                    {
                        port = 1024 + new Random().Next(64511);
                    } while (IsPortInUse(port));


                    IPEndPoint localEndPoint = (IPEndPoint)_client.Client.LocalEndPoint;
                    string localIP = localEndPoint.Address.ToString().Split('.')[0];
                    string pasvMessage = $"227 Entering Passive Mode ({localIP},{port / 256},{port % 256})\r\n";
                    _client.GetStream().Write(Encoding.ASCII.GetBytes(pasvMessage));
                    _dataListener = new TcpListener(IPAddress.Parse(localIP), port);
                    _dataListener.Start();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP PASV - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void HandleNoop()
        {
            try
            {
                if (_loggedIn)
                {
                    _client.GetStream().Write(Encoding.ASCII.GetBytes("200 OK\r\n"));
                }
                else
                {
                    _client.GetStream().Write(Encoding.ASCII.GetBytes("530 Not logged in\r\n"));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP NOOP - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void HandleType(string v)
        {
            try
            {
                if (_loggedIn)
                {
                    string type = v.ToUpper();
                    switch (type)
                    {
                        case "I":
                            _client.GetStream().Write(Encoding.ASCII.GetBytes($"200 Type set to I.\r\n"));
                            break;
                        case "A":
                            _client.GetStream().Write(Encoding.ASCII.GetBytes($"200 Type set to A.\r\n"));
                            break;
                        default:
                            _client.GetStream().Write(Encoding.ASCII.GetBytes($"504 Unknown TYPE.\r\n"));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP TYPE - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }


        private void HandleUmask(string[] parts)
        {
            try
            {
                if (_loggedIn)
                {
                    if (parts.Length < 2)
                    {
                        throw new ArgumentException("Invalid UMASK command format.");
                    }
                    string umaskValue = parts[1];
                    int umask;
                    if (!int.TryParse(umaskValue, out umask))
                    {
                        throw new ArgumentException("Invalid UMASK value.");
                    }
                    // Set the umask value
                    _umask = umask;
                    _client.GetStream().Write(Encoding.ASCII.GetBytes($"200 UMASK set to {umask}.\r\n"));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP UMASK - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void HandleChmod(string[] parts)
        {
            try
            {
                if (_loggedIn)
                {
                    if (parts.Length < 3)
                    {
                        throw new ArgumentException("Invalid CHMOD command format.");
                    }
                    string filename = parts[2];
                    string filePath = Path.Combine(_currentDirectory, filename);
                    if (!File.Exists(filePath))
                    {
                        throw new FileNotFoundException($"File '{filename}' does not exist.");
                    }
                    string mode = parts[1];
                    if (!int.TryParse(mode, out int fileMode))
                    {
                        throw new ArgumentException("Invalid file mode.");
                    }
                    File.SetAttributes(filePath, (FileAttributes)fileMode);
                    _client.GetStream().Write(Encoding.ASCII.GetBytes($"200 File mode changed successfully.\r\n"));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP CHMOD - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private Task HandleRmdAsync(string v)
        {
            return Task.Run(() =>
            {
                if (_loggedIn)
                {
                    string directoryPath = Path.Combine(_currentDirectory, v);
                    if (Directory.Exists(directoryPath))
                    {
                        Directory.Delete(directoryPath);
                        _client.GetStream().Write(Encoding.ASCII.GetBytes($"250 Directory deleted successfully.\r\n"));
                    }
                    else
                    {
                        _client.GetStream().Write(Encoding.ASCII.GetBytes($"550 Directory does not exist.\r\n"));
                    }
                }
            });
        }

        private Task HandleMkdAsync(string v)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (_loggedIn)
                    {
                        string directoryPath = Path.Combine(_currentDirectory, v);
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                            _client.GetStream().Write(Encoding.ASCII.GetBytes($"250 Directory created successfully.\r\n"));
                        }
                        else
                        {
                            _client.GetStream().Write(Encoding.ASCII.GetBytes($"550 Directory already exists.\r\n"));
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.LogWrite($"Error handling FTP MKD - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                }
            });
        }

        private async Task HandleNLSTAsync(string path)
        {
            try
            {
                if (_loggedIn)
                {
                    string[] files = await Task.Run(() => Directory.GetFiles(path));
                    string[] directories = await Task.Run(() => Directory.GetDirectories(path));

                    using (StreamWriter writer = new StreamWriter(_client.GetStream()))
                    {
                        foreach (string file in files)
                        {
                            writer.WriteLine(Path.GetFileName(file));
                        }
                        foreach (string dir in directories)
                        {
                            writer.WriteLine(Path.GetFileName(dir) + "/");
                        }
                        writer.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP NLST - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private async Task HandleSIZEAsync(string filename)
        {
            try
            {
                string filePath = Path.Combine(_currentDirectory, filename);
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"File '{filename}' does not exist.");
                }
                long fileSize = await Task.Run(() => new FileInfo(filePath).Length);
                _client.GetStream().Write(Encoding.ASCII.GetBytes($"213 {fileSize}\r\n"));
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP SIZE - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        private async Task HandleMDTMAsync(string filename)
        {
            try
            {
                if (_loggedIn)
                {
                    string filePath = Path.Combine(_currentDirectory, filename);
                    if (File.Exists(filePath))
                    {
                        DateTime modificationTime = File.GetLastWriteTime(filePath);
                        string modificationTimeStr = modificationTime.ToString("yyyyMMddHHmmss");
                        await _client.GetStream().WriteAsync(Encoding.ASCII.GetBytes($"213 {modificationTimeStr}\r\n"));
                    }
                    else
                    {
                        await _client.GetStream().WriteAsync(Encoding.ASCII.GetBytes($"550 File does not exist.\r\n"));
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP MDTM - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }


        private Task HandleDeleAsync(string filename)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (_loggedIn)
                    {
                        string filePath = Path.Combine(_currentDirectory, filename);
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                            _client.GetStream().Write(Encoding.ASCII.GetBytes($"250 File deleted successfully.\r\n"));
                        }
                        else
                        {
                            _client.GetStream().Write(Encoding.ASCII.GetBytes($"550 File does not exist.\r\n"));
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.LogWrite($"Error handling FTP DELE - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                }
            });
        }

        private void HandleAbort()
        {
            try
            {
                _client.GetStream().Write(Encoding.ASCII.GetBytes("226 Abort successful.\r\n"));
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP ABOR - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void HandlePort(string v)
        {
            try
            {
                if (_loggedIn)
                {
                    string[] parts = v.Split(',');
                    if (parts.Length != 6)
                    {
                        throw new ArgumentException("Invalid PORT command format.");
                    }
                    int[] portParts = new int[6];
                    for (int i = 0; i < 6; i++)
                    {
                        if (!int.TryParse(parts[i], out portParts[i]))
                        {
                            throw new ArgumentException("Invalid PORT command format.");
                        }
                    }
                    int port = (portParts[4] << 8) + portParts[5];
                    _client.GetStream().Write(Encoding.ASCII.GetBytes($"200 PORT command successful.\r\n"));
                    IPAddress address = new IPAddress(new byte[] { (byte)portParts[0], (byte)portParts[1],
                    (byte)portParts[2], (byte)portParts[3] });
                    IPEndPoint endPoint = new IPEndPoint(address, port);
                    _dataClient = new TcpClient();
                    _dataClient.Connect(endPoint);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP PORT - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void HandlePwd()
        {
            try
            {
                if (_loggedIn)
                {
                    string currentDirectory = _currentDirectory;
                    _client.GetStream().Write(Encoding.ASCII.GetBytes($"257 \"{currentDirectory}\" is current directory.\r\n"));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP PWD - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private Task HandleRnfrAsync(string filename)
        {
            return Task.Run(() =>
            {
                try
                {
                    _renameFrom = filename;
                    _client.GetStream().Write(Encoding.ASCII.GetBytes($"350 File name noted as \"{filename}\".\r\n"));
                }
                catch (Exception ex)
                {
                    ex.LogWrite($"Error handling FTP RNFR - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                }
            });
        }


        private Task HandleRntoAsync(string filename)
        {
            return Task.Run(() =>
            {
                try
                {
                    // TODO RNTO FTP command
                    if (_loggedIn)
                    {
                        string oldFilename = _renameFrom;
                        if (string.IsNullOrEmpty(oldFilename))
                        {
                            throw new ArgumentException("RNFR command must be sent before RNTO.");
                        }
                        string newFilename = Path.Combine(_currentDirectory, filename);
                        if (File.Exists(newFilename))
                        {
                            throw new IOException($"File '{newFilename}' already exists.");
                        }
                        File.Move(Path.Combine(_currentDirectory, oldFilename), newFilename);
                        _renameFrom = null;
                        _client.GetStream().Write(Encoding.ASCII.GetBytes($"250 File renamed successfully.\r\n"));
                    }
                }
                catch (Exception ex)
                {
                    ex.LogWrite($"Error handling FTP RNTO - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                }
            });
        }

        private Task HandleRestAsync(string filename)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (_loggedIn)
                    {
                        if (string.IsNullOrEmpty(filename))
                        {
                            throw new ArgumentException("Filename is required.");
                        }
                        long restartPosition = 0;
                        if (long.TryParse(filename, out restartPosition))
                        {
                            _restartPosition = restartPosition;
                            _restartFile = null;
                        }
                        else
                        {
                            _restartFile = filename;
                            _restartPosition = 0;
                        }
                        _client.GetStream().Write(Encoding.ASCII.GetBytes($"350 Restarting at {_restartPosition}. Send STORE or RETRIEVE to transfer file.\r\n"));
                    }
                }
                catch (Exception ex)
                {
                    ex.LogWrite($"Error handling FTP REST - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                }
            });
        }

        private Task HandleAppendAsync(string filename)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_loggedIn)
                    {
                        string filePath = Path.Combine(_currentDirectory, filename);
                        if (!File.Exists(filePath))
                        {
                            throw new FileNotFoundException($"File '{filename}' does not exist.");
                        }
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write))
                        {
                            using (StreamWriter writer = new StreamWriter(fileStream))
                            {
                                NetworkStream stream = _client.GetStream();
                                byte[] buffer = new byte[_bufferSize];
                                int bytesRead;
                                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    await writer.BaseStream.WriteAsync(buffer, 0, bytesRead);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.LogWrite($"Error handling FTP append - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                }
            });
        }

        private void HandleUser(string username)
        {
            try
            {
                _username = username;
                var auth = new CustomParams_Authorize(username, true);
                Invoker?.Invoke(this, auth);
                _username = (!auth.Authorized) ? username : "Unauthorized";
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP user - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void HandlePass(string password)
        {
            try
            {
                if (_password != "Unauthorized")
                {
                    var auth = new CustomParams_Authorize(password, false);
                    Invoker?.Invoke(this, auth);
                    _password = (!auth.Authorized) ? password : "Unauthorized";
                }
                else
                {
                    _password = "Unauthorized";
                }
                _loggedIn = (_username != "Unauthorized" && _password != "Unauthorized");
                if (_loggedIn)
                {
                    var bd = new CustomParams_GetBaseDirectory(_username);
                    Invoker?.Invoke(this, bd);
                    if (bd.found)
                    {
                        _rootDirectory = bd.basedir;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP password - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void HandleCwd(string directory)
        {
            try
            {
                if (_loggedIn)
                {
                    string newPath = Path.Combine(_rootDirectory, directory);
                    if (!Directory.Exists(newPath))
                    {
                        throw new DirectoryNotFoundException($"Directory '{newPath}' does not exist.");
                    }
                    _currentDirectory = newPath;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP directory - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void HandleList()
        {
            try
            {
                if (_loggedIn)
                {
                    StreamWriter writer = new StreamWriter(_client.GetStream());
                    string[] files = Directory.GetFiles(_currentDirectory);
                    foreach (string file in files)
                    {
                        try
                        {
                            string fileName = Path.GetFileName(file);
                            writer.WriteLine($"- {fileName}");
                        }
                        catch (Exception ex)
                        {
                            ex.LogWrite($"Error listing FTP files - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                        }
                    }
                    try
                    {
                        writer.Flush();
                    }

                    catch (Exception ex)
                    {
                        ex.LogWrite($"Error flushing FTP writer - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP list - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private Task HandleRetrAsync(string filename)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!_loggedIn)
                    {
                        string filePath = Path.Combine(_currentDirectory, filename);
                        if (!File.Exists(filePath))
                        {
                            throw new FileNotFoundException($"File '{filename}' does not exist.");
                        }
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            using (StreamWriter writer = new StreamWriter(_client.GetStream()))
                            {
                                writer.Write(new StreamReader(fileStream).ReadToEnd());
                                writer.Flush();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.LogWrite($"Error handling FTP retrieve - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                }
            });
        }

        private Task HandleStorAsync(string filename)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!_loggedIn)
                    {
                        string filePath = Path.Combine(_currentDirectory, filename);
                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            NetworkStream stream = _client.GetStream();
                            byte[] buffer = new byte[_bufferSize]; // use the global buffer size
                            int bytesRead;
                            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                writer.BaseStream.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.LogWrite($"Error handling FTP store - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                }
            });
        }

        private void HandleQuit()
        {
            try
            {
                _client.Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP quit - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
    }
}
