Imports HalconDotNet
Imports System.IO
Imports System.Runtime.InteropServices

Public Class Form1


    Private mSurfaceMaker As GoSurfaceMaker

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.


        mSurfaceMaker = New GoSurfaceMaker() '初始化系统及参数
    End Sub

    Private Sub btnConnect_Click(sender As Object, e As EventArgs) Handles btnConnect.Click
        Dim errMsg As String = ""
        If Not mSurfaceMaker.connect(errMsg) Then
            MsgBox(errMsg, MsgBoxStyle.Critical)
            Exit Sub
        End If
        MsgBox("系统连接成功")
    End Sub
    Private Sub btnDisconnect_Click(sender As Object, e As EventArgs) Handles btnDisconnect.Click
        Dim errMsg As String = ""
        If Not mSurfaceMaker.disconnect(errMsg) Then
            MsgBox(errMsg, MsgBoxStyle.Critical)
            Exit Sub
        End If
        MsgBox("系统断开成功")
        Dim msw As New Stopwatch
    End Sub
    Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
        Dim thrdStart As New Threading.Thread(AddressOf startBuffering)
        thrdStart.Start()
    End Sub
    Private Sub startBuffering()
        Try
            Dim errMsg As String = ""
            If Not mSurfaceMaker.start(errMsg) Then
                MsgBox(errMsg, MsgBoxStyle.Critical)
                Exit Sub
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Dim errMsg As String = ""
        If Not mSurfaceMaker.close(errMsg) Then
            MsgBox(errMsg, MsgBoxStyle.Critical)
            Exit Sub
        End If
        MsgBox("激光关闭完成。")
    End Sub

    Private curImage As HObject

    Private Sub btnGetHalconImage_Click(sender As Object, e As EventArgs) Handles btnGetHalconImage.Click
        Dim errMsg As String = ""
        Dim image As New HObject

        Dim ImgArray(0) As Byte
        Dim imgWidth, imgHeight As Integer

        If Not mSurfaceMaker.getHalconImage(image, 5, ImgArray, imgWidth, imgHeight) Then
            MsgBox("error in getting halcon image", MsgBoxStyle.Critical)
            Exit Sub
        End If
        curImage = image
        HOperatorSet.WriteImage(curImage, "bmp", 0, "C:\Users\lgche\Desktop\tempImage\yuan30.bmp")


        PictureBox1.Image = imageArray2Bitmap(ImgArray, imgWidth, imgHeight)

        'mSurfaceMaker.getHalconImage(image, 3, imgArray, imgWidth, imgHeight)

        'If Not mSurfaceMaker.getHalconImage(image, intensityImage, 1, imgArray, intensityImgArray, imgWidth, imgHeight) Then
        '    MsgBox("error in getting halcon image", MsgBoxStyle.Critical)
        '    Exit Sub
        'End If

        'curHeightImage = image
        'curIntensityImage = intensityImage

        'HOperatorSet.WriteImage(curIntensityImage, "bmp", 0, "C:\Users\lgche\Desktop\tempImage\yuan17.bmp")

        'If checkHieght.Checked Then

        'PictureBox1.Image = imageArray2Bitmap(imgArray, imgWidth, imgHeight)
        'Else
        '    PictureBox1.Image = imageArray2Bitmap(intensityImgArray, imgWidth, imgHeight)
        'End If



        MsgBox("激光关闭完成。")
    End Sub
    Private Sub checkIntensity_CheckedChanged(sender As Object, e As EventArgs) Handles checkIntensity.CheckedChanged
        btnGetHalconImage_Click(sender, e)
    End Sub

    Private Sub btnSaveImage_Click(sender As Object, e As EventArgs) Handles btnSaveImage.Click
        Dim folder As String = Application.StartupPath & "\tmp\"
        Dim fileName As String = folder & "image.tif"
        If Not Directory.Exists(folder) Then
            Directory.CreateDirectory(folder)
        End If
        If curImage IsNot Nothing Then
            HOperatorSet.WriteImage(curImage, "tiff", 0, fileName)
            MsgBox("Image saved at: " & fileName)
        End If

    End Sub
    Private Sub btnSetSpacing1_Click(sender As Object, e As EventArgs)
        'If Not mSurfaceMaker.setEncoderSpacing(CSng(txtSpacing1.Text), False) Then
        '    MsgBox("Error in setting the encoder spacing")
        '    Exit Sub
        'End If
        'MsgBox("设置编码器分辨率完成。")
    End Sub
    Private Sub btnSetSpacing2_Click(sender As Object, e As EventArgs)
        If Not mSurfaceMaker.setEncoderSpacing(1) Then
            MsgBox("Error in setting the encoder spacing")
            Exit Sub
        End If
        MsgBox("设置编码器分辨率完成。")
    End Sub
    Private Function imageArray2Bitmap(imgArray() As Byte, width As Integer, Height As Integer) As Bitmap
        Dim img As New Bitmap(width, Height, Imaging.PixelFormat.Format8bppIndexed)
        Dim imgData = img.LockBits(New Rectangle(0, 0, width, height), Imaging.ImageLockMode.WriteOnly, Imaging.PixelFormat.Format8bppIndexed)
        Marshal.Copy(imgArray, 0, imgData.Scan0, imgArray.Length)
        img.UnlockBits(imgData)

        'get image in grey scale
        Dim palette = img.Palette
        For i As Integer = 0 To img.Palette.Entries.Length - 1
            palette.Entries(i) = Color.FromArgb(i, i, i)
        Next
        img.Palette = palette
        Return img
    End Function
    Private Sub btnLoadData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLoadData.Click
        Try
            Dim o_OpenFileDialog As OpenFileDialog = New OpenFileDialog()
            If (o_OpenFileDialog.ShowDialog() = Windows.Forms.DialogResult.OK) Then
                mSurfaceMaker.LoadData(o_OpenFileDialog.FileName)
            End If
            MsgBox("数据加载成功")


        Catch ex As Exception
            MsgBox(ex.Message)
            Return
        End Try
    End Sub
    Private Sub btnOpenLaser_Click(sender As Object, e As EventArgs) Handles btnOpenLaser.Click
        mSurfaceMaker.openLaser()
    End Sub

    Private Sub btnCloseLaser_Click(sender As Object, e As EventArgs) Handles btnCloseLaser.Click
        mSurfaceMaker.closeLaser()
    End Sub

    Private Sub btnResetSensor_Click(sender As Object, e As EventArgs) Handles btnResetSensor.Click
        mSurfaceMaker.reset()
    End Sub


    Private Sub btnReloadSetting_Click(sender As Object, e As EventArgs) Handles btnReloadSetting.Click
        mSurfaceMaker.LoadSetting()
        MsgBox("配置加载成功")
    End Sub


End Class
