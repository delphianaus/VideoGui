using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoGui;
using VideoGui.Models;
using VideoGui.Models.delegates;
using Windows.Foundation.Diagnostics;
using Windows.Media.Protection.PlayReady;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

public class FtpServer : IDisposable
{
    private TcpListener _listener;
    private string _rootDirectory;
    databasehook<object> ModuleCallBack = null;
    private System.Threading.Timer _startTimer;
    private bool disposedValue;
    private List<TcpClient> _clients = new List<TcpClient>();

    private List<TcpListener> _listeners;
    private List<IPAddress> _ipAddresses;
    public FtpServer(int port, string rootDirectory, databasehook<object> _Modulecallback, List<IPAddress> ipAddresses)
    {
        try
        {
            _ipAddresses = ipAddresses;
            _listeners = new List<TcpListener>();
            foreach (var ipAddress in _ipAddresses)
            {
                _listeners.Add(new TcpListener(ipAddress, port));
            }
            _rootDirectory = rootDirectory;
            ModuleCallBack = _Modulecallback;
            _startTimer = new System.Threading.Timer(StartFtpServer, null, 5000, Timeout.Infinite);
        }
        catch (Exception ex)
        {
            ex.LogWrite($"Error starting FTP server - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
        }
    }

    private void StartFtpServer(object state)
    {
        try
        {
            foreach (var listener in _listeners)
            {
                listener.Start();
            }

            Task.Run(() =>
            {
                while (true)
                {
                    foreach (var listener in _listeners)
                    {
                        if (listener.Pending())
                        {
                            TcpClient client = listener.AcceptTcpClient();
                            _clients.Add(client);
                            Thread thread = new Thread(new ParameterizedThreadStart(HandleClient));
                            thread.Start(client);
                        }
                    }
                }
            });
        }
        catch (Exception ex)
        {
            ex.LogWrite($"Error starting FTP server - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
        }
    }

    private void HandleClient(object client)
    {
        try
        {
            TcpClient tcpClient = (TcpClient)client;
            FtpSession session = new FtpSession(tcpClient, _rootDirectory, ModuleCallBack);
            try
            {
                session.HandleCommands();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error handling FTP client - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
            finally
            {
                tcpClient.Close();
            }
        }
        catch (Exception ex)
        {
            ex.LogWrite($"Error handling FTP client - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposedValue) return;

        if (disposing)
        {
            _clients.ForEach(c => c.Dispose());
        }

        disposedValue = true;
    }

    ~FtpServer()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

