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
            TextBox1.Text += ("[系统]下载buildtools.jar后，请重新点击构建（黑色窗口关闭后就是下载完了）" + Environment.NewLine)
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
                                               TextBox1.AppendText("[信息]" & msg + Environment.NewLine)
                                               If InStr(1, msg, "Everything completed successfully") <> 0 Then
                                                   MsgBox("Spigot服务端构建成功", 64, "构建成功！")
                                                   ComboBox1.Enabled = True
                                                   Button1.Enabled = True
                                                   Button2.Enabled = False


                                               End If
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
        Try
            My.Computer.Network.DownloadFile("https://www.nstar.xyz/data/ver.txt", Application.StartupPath & "/BuildSpigot/ver.txt", "", "", True, 1000, True)
        Catch ex As Exception
            MsgBox("无法更新版本列表！" & vbCrLf & ex.Message, 16, "版本获取失败！")
        End Try

        Dim textArr As String() = File.ReadAllLines(Application.StartupPath & "/BuildSpigot/ver.txt")
        For Each s As String In textArr
            Console.WriteLine(s)
            ComboBox1.Items.Add(s)
        Next
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            _process.CancelOutputRead()
            _process.Close()
        Catch ex As Exception
            MsgBox(ex.Message, 16, "进程结束失败！（别点击那么多次）")
        End Try

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form2.Show()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim re = MsgBox("该操作会删除Buildtools.jar在内的所有新生成的文件！", 32 + 1, "确认删除？")
        If (re = 1) Then
            Try
                IO.Directory.Delete(Application.StartupPath & "/BuildSpigot", True)
            Catch ex As Exception
                MsgBox("无法删除文件，可能是由于文件被占用，请在重启电脑后再次尝试删除！" & vbCrLf & ex.Message, 16, "删除错误")
            End Try

        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Try
            MsgBox(Application.StartupPath & "\BuildSpigot\BuildTools.log.txt")
            Shell("notepad " & Application.StartupPath & "\BuildSpigot\BuildTools.log.txt")
        Catch ex As Exception
            MsgBox("未找到日志！你删掉了？" & vbCrLf & ex.ToString, 16, "日志找不到···")
        End Try
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Try
            IO.File.Delete(Application.StartupPath & "/BuildSpigot/BuildTools.jar")
            TextBox1.AppendText("[系统]删除旧版构建工具成功！" + Environment.NewLine)
        Catch ex As Exception
            MsgBox(ex.Message, 16, "构建Jar删除失败！")
        End Try
    End Sub


End Class
