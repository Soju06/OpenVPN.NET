# OpenVPN.Net

.Net Framework 4.7.2용 OpenVPN Management

### 현재 개발단계입니다.

## 사용 방법

다음은 nordvpn 예제 입니다.

```csharp
if (!OpenVPN.IsOpenVPNInstalled) {
    Console.WriteLine("OpenVPN이 설치되어있지 않습니다!");
    Console.ReadLine();
    Environment.Exit(0);
}
Application.ApplicationExit += Application_ApplicationExit;
Console.CancelKeyPress += Console_CancelKeyPress;
Console.WriteLine("OpenVPN");
var Manager = new OpenVPNManager(new OpenVPNConfig(File.ReadAllText(@"D:\Users\soju_\Downloads\jp585.nordvpn.com.udp1194.ovpn")));
Console.WriteLine("OpenVPN opened.");
Console.WriteLine("state: {0}", Manager.State(true));
Console.WriteLine("login..");
Console.WriteLine(Manager.Username(""));
Console.WriteLine(Manager.Password(""));
Console.WriteLine("pid: {0}", Manager.Pid());
Console.WriteLine(Manager.Help());
Console.WriteLine(Manager.Log());
while (true) {
    Console.Write("tx>");
    try {
        var cmd = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(cmd)) continue;
        Console.WriteLine("rx>\n" + Manager.SendCommand(cmd));
    } catch (OpenVPNManagementCommandErrorException ex) {
        Console.WriteLine("rx ERROR>\n{0}", ex.Message);
    }
}

static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
    e.Cancel = true;
    Exit();
}
static void Application_ApplicationExit(object sender, EventArgs e) => Exit();
static void Exit() {
    Console.WriteLine("종료중..");
    Manager?.Dispose();
    Environment.Exit(0);
}
```

OpenVPNManager 및 OpenVPN Daemon을 종료하는 방법

```csharp
Manager.Close();
```

OpenVPNManager는 재활용할 수 없으며, Close와 Dispose는 동일한 역할을 수행합니다.