<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.ColorDialog1 = New System.Windows.Forms.ColorDialog()
        Me.btnConnect = New System.Windows.Forms.Button()
        Me.btnDisconnect = New System.Windows.Forms.Button()
        Me.btnStart = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnGetHalconImage = New System.Windows.Forms.Button()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.btnLoadData = New System.Windows.Forms.Button()
        Me.btnCloseLaser = New System.Windows.Forms.Button()
        Me.btnOpenLaser = New System.Windows.Forms.Button()
        Me.btnResetSensor = New System.Windows.Forms.Button()
        Me.btnSaveImage = New System.Windows.Forms.Button()
        Me.btnReloadSetting = New System.Windows.Forms.Button()
        Me.checkHieght = New System.Windows.Forms.RadioButton()
        Me.checkIntensity = New System.Windows.Forms.RadioButton()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnConnect
        '
        Me.btnConnect.Location = New System.Drawing.Point(835, 27)
        Me.btnConnect.Margin = New System.Windows.Forms.Padding(2)
        Me.btnConnect.Name = "btnConnect"
        Me.btnConnect.Size = New System.Drawing.Size(103, 25)
        Me.btnConnect.TabIndex = 1
        Me.btnConnect.Text = "连接系统"
        Me.btnConnect.UseVisualStyleBackColor = True
        '
        'btnDisconnect
        '
        Me.btnDisconnect.Location = New System.Drawing.Point(835, 56)
        Me.btnDisconnect.Margin = New System.Windows.Forms.Padding(2)
        Me.btnDisconnect.Name = "btnDisconnect"
        Me.btnDisconnect.Size = New System.Drawing.Size(103, 25)
        Me.btnDisconnect.TabIndex = 2
        Me.btnDisconnect.Text = "断开系统"
        Me.btnDisconnect.UseVisualStyleBackColor = True
        '
        'btnStart
        '
        Me.btnStart.Location = New System.Drawing.Point(835, 85)
        Me.btnStart.Margin = New System.Windows.Forms.Padding(2)
        Me.btnStart.Name = "btnStart"
        Me.btnStart.Size = New System.Drawing.Size(103, 25)
        Me.btnStart.TabIndex = 3
        Me.btnStart.Text = "开始扫描"
        Me.btnStart.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(835, 114)
        Me.btnClose.Margin = New System.Windows.Forms.Padding(2)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(103, 25)
        Me.btnClose.TabIndex = 4
        Me.btnClose.Text = "结束扫描"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'btnGetHalconImage
        '
        Me.btnGetHalconImage.Location = New System.Drawing.Point(835, 143)
        Me.btnGetHalconImage.Margin = New System.Windows.Forms.Padding(2)
        Me.btnGetHalconImage.Name = "btnGetHalconImage"
        Me.btnGetHalconImage.Size = New System.Drawing.Size(103, 25)
        Me.btnGetHalconImage.TabIndex = 5
        Me.btnGetHalconImage.Text = "生成图片"
        Me.btnGetHalconImage.UseVisualStyleBackColor = True
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.Black
        Me.PictureBox1.Location = New System.Drawing.Point(12, 27)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(800, 600)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 9
        Me.PictureBox1.TabStop = False
        '
        'btnLoadData
        '
        Me.btnLoadData.Location = New System.Drawing.Point(835, 203)
        Me.btnLoadData.Margin = New System.Windows.Forms.Padding(2)
        Me.btnLoadData.Name = "btnLoadData"
        Me.btnLoadData.Size = New System.Drawing.Size(103, 25)
        Me.btnLoadData.TabIndex = 10
        Me.btnLoadData.Text = "加载数据"
        Me.btnLoadData.UseVisualStyleBackColor = True
        '
        'btnCloseLaser
        '
        Me.btnCloseLaser.Location = New System.Drawing.Point(835, 261)
        Me.btnCloseLaser.Margin = New System.Windows.Forms.Padding(2)
        Me.btnCloseLaser.Name = "btnCloseLaser"
        Me.btnCloseLaser.Size = New System.Drawing.Size(103, 25)
        Me.btnCloseLaser.TabIndex = 12
        Me.btnCloseLaser.Text = "关闭激光"
        Me.btnCloseLaser.UseVisualStyleBackColor = True
        '
        'btnOpenLaser
        '
        Me.btnOpenLaser.Location = New System.Drawing.Point(835, 232)
        Me.btnOpenLaser.Margin = New System.Windows.Forms.Padding(2)
        Me.btnOpenLaser.Name = "btnOpenLaser"
        Me.btnOpenLaser.Size = New System.Drawing.Size(103, 25)
        Me.btnOpenLaser.TabIndex = 11
        Me.btnOpenLaser.Text = "打开激光"
        Me.btnOpenLaser.UseVisualStyleBackColor = True
        '
        'btnResetSensor
        '
        Me.btnResetSensor.Location = New System.Drawing.Point(835, 290)
        Me.btnResetSensor.Margin = New System.Windows.Forms.Padding(2)
        Me.btnResetSensor.Name = "btnResetSensor"
        Me.btnResetSensor.Size = New System.Drawing.Size(103, 25)
        Me.btnResetSensor.TabIndex = 13
        Me.btnResetSensor.Text = "重置激光"
        Me.btnResetSensor.UseVisualStyleBackColor = True
        '
        'btnSaveImage
        '
        Me.btnSaveImage.Location = New System.Drawing.Point(835, 172)
        Me.btnSaveImage.Margin = New System.Windows.Forms.Padding(2)
        Me.btnSaveImage.Name = "btnSaveImage"
        Me.btnSaveImage.Size = New System.Drawing.Size(103, 25)
        Me.btnSaveImage.TabIndex = 14
        Me.btnSaveImage.Text = "保存图片"
        Me.btnSaveImage.UseVisualStyleBackColor = True
        '
        'btnReloadSetting
        '
        Me.btnReloadSetting.Location = New System.Drawing.Point(835, 319)
        Me.btnReloadSetting.Margin = New System.Windows.Forms.Padding(2)
        Me.btnReloadSetting.Name = "btnReloadSetting"
        Me.btnReloadSetting.Size = New System.Drawing.Size(103, 25)
        Me.btnReloadSetting.TabIndex = 15
        Me.btnReloadSetting.Text = "重加配置"
        Me.btnReloadSetting.UseVisualStyleBackColor = True
        '
        'checkHieght
        '
        Me.checkHieght.AutoSize = True
        Me.checkHieght.Checked = True
        Me.checkHieght.Location = New System.Drawing.Point(638, 645)
        Me.checkHieght.Name = "checkHieght"
        Me.checkHieght.Size = New System.Drawing.Size(59, 16)
        Me.checkHieght.TabIndex = 16
        Me.checkHieght.TabStop = True
        Me.checkHieght.Text = "高度图"
        Me.checkHieght.UseVisualStyleBackColor = True
        '
        'checkIntensity
        '
        Me.checkIntensity.AutoSize = True
        Me.checkIntensity.Location = New System.Drawing.Point(754, 645)
        Me.checkIntensity.Name = "checkIntensity"
        Me.checkIntensity.Size = New System.Drawing.Size(59, 16)
        Me.checkIntensity.TabIndex = 17
        Me.checkIntensity.TabStop = True
        Me.checkIntensity.Text = "亮度图"
        Me.checkIntensity.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(987, 730)
        Me.Controls.Add(Me.checkIntensity)
        Me.Controls.Add(Me.checkHieght)
        Me.Controls.Add(Me.btnReloadSetting)
        Me.Controls.Add(Me.btnSaveImage)
        Me.Controls.Add(Me.btnResetSensor)
        Me.Controls.Add(Me.btnCloseLaser)
        Me.Controls.Add(Me.btnOpenLaser)
        Me.Controls.Add(Me.btnLoadData)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.btnGetHalconImage)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.btnStart)
        Me.Controls.Add(Me.btnDisconnect)
        Me.Controls.Add(Me.btnConnect)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Form1"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents ColorDialog1 As System.Windows.Forms.ColorDialog
    Friend WithEvents btnConnect As System.Windows.Forms.Button
    Friend WithEvents btnDisconnect As System.Windows.Forms.Button
    Friend WithEvents btnStart As System.Windows.Forms.Button
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents btnGetHalconImage As System.Windows.Forms.Button
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents btnLoadData As System.Windows.Forms.Button
    Friend WithEvents btnCloseLaser As System.Windows.Forms.Button
    Friend WithEvents btnOpenLaser As System.Windows.Forms.Button
    Friend WithEvents btnResetSensor As System.Windows.Forms.Button
    Friend WithEvents btnSaveImage As System.Windows.Forms.Button
    Friend WithEvents btnReloadSetting As System.Windows.Forms.Button
    Friend WithEvents checkHieght As RadioButton
    Friend WithEvents checkIntensity As RadioButton
End Class
