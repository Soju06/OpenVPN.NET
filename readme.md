OpenVPN.NET

.Net 용 OpenVPN Management

## 사용 방법

다음은 [**ConfigAndAccount**](https://github.com/Soju06/OpenVPN.NET/blob/main/OpenVPN.Test/ConfigAndAccountExample.cs) 예제 입니다.

**using**

```csharp
using OpenVPNNET;
using OpenVPNNET.Manager;
```

**Example**

```csharp
static OpenVPNManager Manager;
static CancellationTokenSource CancellationToken;

public static void Example() {
    CancellationToken = new();
    if (!OpenVPNEnvironment.IsOpenVPNInstalled) {
        Console.WriteLine("OpenVPN is not installed!");
        Console.ReadLine();
        return;
    }

    Console.CancelKeyPress += Console_CancelKeyPress;
    Console.WriteLine("OpenVPN Manager - Config & Account Example");

    var Config = new OpenVPNConfig();

    while (!CancellationToken.IsCancellationRequested) {
        try {
            Program.Write("Config Path: ", ConsoleColor.Yellow);
            Config.Config = File.ReadAllText(Console.ReadLine());
            break;
        } catch (Exception ex) {
            Program.WriteLine(ex.ToString(), ConsoleColor.Red);
        }
    }

    CancellationToken?.Token.ThrowIfCancellationRequested();

    Manager = new OpenVPNManager(Config);
    Manager.Stream.StateChangedEvent += Stream_StateChangedEvent;
    Manager.Stream.PasswordEvent += Stream_PasswordEvent;
    Manager.Stream.AnyEvent += Stream_AnyEvent;
    Console.WriteLine("OpenVPN opened.");
    Login();

    void Login() {
        try {
            Task.Run(async () => {
                Program.Write("Username: ", ConsoleColor.Cyan);
                await Manager.Command.Username(Console.ReadLine(), 5, CancellationToken.Token);
                Program.Write("Password: ", ConsoleColor.Cyan);
                await Manager.Command.Password(Console.ReadLine(), 5, CancellationToken.Token);
                while (!CancellationToken.IsCancellationRequested) await Task.Delay(100, CancellationToken.Token);
            }, CancellationToken.Token).Wait();
        } catch {

        }
    }

    void Stream_PasswordEvent(OpenVPNMessageStream sender,
                              OpenVPNEventArgs<string> e) =>
        Login();

    static void Stream_AnyEvent(OpenVPNMessageStream sender, OpenVPNEventArgs e) {
        if(e.EventType != "STATE")
            Program.WriteLine($"{e.EventType}>{e.EventMessage}", ConsoleColor.Gray);
    }

    static void Stream_StateChangedEvent(OpenVPNMessageStream sender, 
                                         OpenVPNEventArgs<OpenVPNStateMessageInfo> e) {
        if(e?.EventMessage.State == MessageStateCode.CONNECTED) {
            Program.WriteLine($"Connected. IP: {e.EventMessage.IPAddress}", ConsoleColor.Green);
        } else if (e.EventMessage is not null) {
            Program.WriteLine($"Connecting.. {e.EventMessage.State}", ConsoleColor.Yellow);
        }
    }

    static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
        e.Cancel = true;
        Exit();
    }

    static void Exit() {
        Console.WriteLine("Closeing..");
        CancellationToken?.Cancel();
        CancellationToken?.Dispose();
        Manager?.Close();
        Manager?.Dispose();
    }
}
```

OpenVPNManager 및 OpenVPN Daemon을 종료하는 방법

```csharp
Manager.Close();
```

OpenVPNManager는 재활용할 수 없으며, Close와 Dispose는 동일한 역할을 수행합니다.