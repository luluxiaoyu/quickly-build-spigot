Public Class Form2
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Shell("explorer.exe https://github.com/luluxiaoyu/quickly-build-spigot")
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Shell("explorer.exe https://www.mcbbs.net/thread-1149682-1-1.html")
    End Sub

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '禁用窗体最大化按钮
        Me.MaximizeBox = False
        '禁止用户用鼠标改变窗体大小
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
    End Sub
End Class