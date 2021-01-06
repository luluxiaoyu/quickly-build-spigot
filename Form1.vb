Imports System.IO
Imports System.Net
Public Class Form1
    Dim bt As New ProcessStartInfo("curl")
    Dim build As New ProcessStartInfo("java")
    Private _process As Process = Nothing
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Button2.Enabled = True
        '判断文件夹是否存在
        If System.IO.Directory.Exists(Application.StartupPath & "/BuildSpigot") = False Then
            System.IO.Directory.CreateDirectory(Application.StartupPath & "/BuildSpigot")
            TextBox1.Text += (vbCrLf & "创建BuildSpigot文件夹")
        End If

        '检查buildtools是否存在
        If My.Computer.FileSystem.FileExists(Application.StartupPath & "/BuildSpigot/BuildTools.jar") = False Then
            bt.Arguments = " -o BuildTools.jar https://hub.spigotmc.org/jenkins/job/BuildTools/lastSuccessfulBuild/artifact/target/BuildTools.jar"
            bt.WorkingDirectory = Application.StartupPath & "/BuildSpigot"
            Process.Start(bt)
            TextBox1.Text += (vbCrLf & "下载buildtools.jar后，请重新点击构建")
        End If
        build.WindowStyle = ProcessWindowStyle.Hidden
        build.Arguments = " -jar BuildTools.jar --rev " & ComboBox1.Text
        build.WorkingDirectory = Application.StartupPath & "/BuildSpigot"
        build.UseShellExecute = False
        build.RedirectStandardOutput = True
        build.CreateNoWindow = True
        _process = New Process()
        _process.StartInfo = build
        ' 定义接收消息的Handler
        AddHandler _process.OutputDataReceived, New DataReceivedEventHandler(AddressOf Process1_OutputDataReceived)
        _process.Start()
        ' 开始接收
        _process.BeginOutputReadLine()
    End Sub
    Private Delegate Sub AddMessageHandler(ByVal msg As String)
    Private Sub Process1_OutputDataReceived(sender As Object, e As DataReceivedEventArgs) Handles Process1.OutputDataReceived
        Dim handler As AddMessageHandler = Function(msg As String)
                                               TextBox1.AppendText(msg + Environment.NewLine)
                                               'TextBox1.Text += msg + Environment.NewLine
                                           End Function
        If Me.TextBox1.InvokeRequired Then
            Me.TextBox1.Invoke(handler, e.Data)
        End If
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        TextBox1.ScrollToCaret()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '禁用窗体最大化按钮
        Me.MaximizeBox = False
        '禁止用户用鼠标改变窗体大小
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
        Button2.Enabled = False
        My.Computer.Network.DownloadFile("https://www.nstar.xyz/data/ver.txt", Application.StartupPath & "/BuildSpigot/ver.txt", "", "", True, 1000, True)
        Dim textArr As String() = File.ReadAllLines(Application.StartupPath & "/BuildSpigot/ver.txt")
        For Each s As String In textArr
            Console.WriteLine(s)
            ComboBox1.Items.Add(s)
        Next
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        _process.CancelOutputRead()
        _process.Close()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form2.Show()
    End Sub
End Class
