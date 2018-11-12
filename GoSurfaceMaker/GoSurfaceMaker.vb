Imports System.Threading
Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Xml.Serialization
Imports HalconDotNet
Imports commonLib
Imports Lmi3d.GoSdk.Messages

Public Class GoSurfaceMaker

    Private mSet As GoSurfaceMakerSetting 'get this setting when initializing this class
    'Private mProfilesBuffer As List(Of Profile)
    'll
    Private mIntensityProfileBuffer As List(Of IntensityByte)

    Private mProfilesBuffer As List(Of ProfileByte)
    Private Const invalid As Integer = -32768
    Private isBuffering As Boolean
    Private mAuthorize As New Authorize
    Private isLicenseValid As Boolean


    'gocator pointers
    Private go_api As IntPtr = IntPtr.Zero
    Private go_system As IntPtr = IntPtr.Zero
    Private go_sensor As IntPtr = IntPtr.Zero
    Private go_setup As IntPtr = IntPtr.Zero

    Private context As DataContext = New DataContext()
    Private myOnData As New onDataType(AddressOf onData)

    'debug
    Private mSw As New Stopwatch

    <Serializable()> Public Structure ProfilePoint
        Public x As Single
        Public z As Single
    End Structure
    <Serializable()> Public Structure Profile
        Public points() As ProfilePoint
        Public encoder As Long
    End Structure
    <Serializable()> Public Structure ProfileByte
        'X values are 0,1,2,3,4,5....
        'Z values use 8 bit representation
        'Purpose, save the data spacing and also the processing time in converting to image
        Public zBytes() As Byte
        Public encoder As Long
    End Structure

    'll
    <Serializable()> Public Structure IntensityByte
        'X values are 0,1,2,3,4,5....
        'Z values use 8 bit representation
        'Purpose, save the data spacing and also the processing time in converting to image
        Public intensity() As Byte
        Public encoder As Long
    End Structure

    <Serializable()> Public Class GoSurfaceMakerSetting
        Public gocatorIP As String
        Public imageWidth As Integer 'pixel
        Public imageHeight As Integer 'pixel
        Public imageResolution As Single
        Public encoderResolution As Single 'mm/tick
        Public isFillingRows As Boolean
        Public isFlipingX As Boolean

        Public bufferMaxLength As Integer

        Public isUpsideDownImage1 As Boolean
        Public isUpsideDownImage2 As Boolean
        Public isUpsideDownImage3 As Boolean
        Public isUpsideDownImage4 As Boolean
        Public isUpsideDownImage5 As Boolean
        Public isUpsideDownImage6 As Boolean

        Public ignoreBeginProfilesNb As Integer '忽略开始的几条轮廓

        'Gocator FOV
        Public FOV_X_Start As Single
        Public FOV_X_Range As Single
        Public FOV_Z_Start As Single
        Public FOV_Z_Range As Single

        Public ImageForIntensity As Boolean

        'Base and glue track segmentation
        Public isUseSegmentation As Boolean
        Public useThreadNb As Integer
        Public SegHeightShift As Integer '
        Public SegShiftUp As Integer
        Public SegShiftDown As Integer

        Public SegBaseMeasurementRegion1 As SegBaseMeasurementRegion
        Public SegBaseMeasurementRegion2 As SegBaseMeasurementRegion
        Public SegBaseMeasurementRegion3 As SegBaseMeasurementRegion
        Public SegBaseMeasurementRegion4 As SegBaseMeasurementRegion
        Public SegBaseMeasurementRegion5 As SegBaseMeasurementRegion
        Public SegBaseMeasurementRegion6 As SegBaseMeasurementRegion

        Public isUseSegmentationMask As Boolean
        Public MaskForIntensity As Boolean
        Public SegBaseMaskRegion1 As SegBaseMaskRegion
        Public SegBaseMaskRegion2 As SegBaseMaskRegion
        Public SegBaseMaskRegion3 As SegBaseMaskRegion
        Public SegBaseMaskRegion4 As SegBaseMaskRegion
        Public SegBaseMaskRegion5 As SegBaseMaskRegion
        Public SegBaseMaskRegion6 As SegBaseMaskRegion

        'Top and Glue track segmentation
        Public isUseSegmentationTopTrack As Boolean
        Public SegTopTrackHeightShift As Integer
        Public SegTopTrackShiftUp As Integer
        Public SegTopTrackShiftDown As Integer
        Public SegTopTrackRegion1 As SegTopTrackRegion
        Public SegTopTrackRegion2 As SegTopTrackRegion
        Public SegTopTrackRegion3 As SegTopTrackRegion
        Public SegTopTrackRegion4 As SegTopTrackRegion
        Public SegTopTrackRegion5 As SegTopTrackRegion
        Public SegTopTrackRegion6 As SegTopTrackRegion
        Public SegTopTrackRegion7 As SegTopTrackRegion
        Public SegTopTrackRegion8 As SegTopTrackRegion
        Public SegTopTrackRegion9 As SegTopTrackRegion
        Public SegTopTrackRegion10 As SegTopTrackRegion
        Public SegTopTrackRegion11 As SegTopTrackRegion
        Public SegTopTrackRegion12 As SegTopTrackRegion
        Public SegTopTrackRegion13 As SegTopTrackRegion
        Public SegTopTrackRegion14 As SegTopTrackRegion


        'Edge Smoothing
        Public isUseEdgeSmoothing As Boolean '需要同时启用Segmentation
        Public EdgeSmoothingRegion1 As EdgeSmoothingRegion
        Public EdgeSmoothingRegion2 As EdgeSmoothingRegion
        Public EdgeSmoothingRegion3 As EdgeSmoothingRegion
        Public EdgeSmoothingRegion4 As EdgeSmoothingRegion
        Public EdgeSmoothingRegion5 As EdgeSmoothingRegion
        Public EdgeSmoothingRegion6 As EdgeSmoothingRegion
        Public EdgeSmoothingRegion7 As EdgeSmoothingRegion
        Public EdgeSmoothingRegion8 As EdgeSmoothingRegion


        'Support Points Offset
        Public isUseSupportPointsOffset As Boolean
        Public SupportPoints1 As SupportPoints
        Public SupportPoints2 As SupportPoints
        Public SupportPoints3 As SupportPoints
        Public SupportPoints4 As SupportPoints

        'save data
        Public isSaveData As Boolean
        Public DataSaveNb As Integer
        Public DataSavePath As String
        Public DataAutoSaveFolder As String
        Public DataNameMaxCount As Integer '文件名后缀计数



        Public Sub New()
            gocatorIP = "192.168.1.10"
            imageWidth = 3200 'pixel
            imageHeight = 16000 'pixel
            imageResolution = 0.01 'mm
            encoderResolution = 0.001
            isFillingRows = True
            isFlipingX = True


            bufferMaxLength = 20000

            'Gocator FOV
            FOV_X_Start = -16
            FOV_X_Range = 32
            FOV_Z_Start = -12.5
            FOV_Z_Range = 25

            'Base and glue track segmentation
            SegBaseMeasurementRegion1 = New SegBaseMeasurementRegion
            SegBaseMeasurementRegion2 = New SegBaseMeasurementRegion
            SegBaseMeasurementRegion3 = New SegBaseMeasurementRegion
            SegBaseMeasurementRegion4 = New SegBaseMeasurementRegion
            SegBaseMeasurementRegion5 = New SegBaseMeasurementRegion
            SegBaseMeasurementRegion6 = New SegBaseMeasurementRegion

            'Top and Glue track segmentation
            SegTopTrackHeightShift = -12
            SegTopTrackShiftUp = 150
            SegTopTrackShiftDown = 150
            SegTopTrackRegion1 = New SegTopTrackRegion
            SegTopTrackRegion2 = New SegTopTrackRegion
            SegTopTrackRegion3 = New SegTopTrackRegion
            SegTopTrackRegion4 = New SegTopTrackRegion
            SegTopTrackRegion5 = New SegTopTrackRegion
            SegTopTrackRegion6 = New SegTopTrackRegion
            SegTopTrackRegion7 = New SegTopTrackRegion
            SegTopTrackRegion8 = New SegTopTrackRegion
            SegTopTrackRegion9 = New SegTopTrackRegion
            SegTopTrackRegion10 = New SegTopTrackRegion
            SegTopTrackRegion11 = New SegTopTrackRegion
            SegTopTrackRegion12 = New SegTopTrackRegion
            SegTopTrackRegion13 = New SegTopTrackRegion
            SegTopTrackRegion14 = New SegTopTrackRegion



            'Segmentation Mask Regions
            SegBaseMaskRegion1 = New SegBaseMaskRegion
            SegBaseMaskRegion2 = New SegBaseMaskRegion
            SegBaseMaskRegion3 = New SegBaseMaskRegion
            SegBaseMaskRegion4 = New SegBaseMaskRegion
            SegBaseMaskRegion5 = New SegBaseMaskRegion
            SegBaseMaskRegion6 = New SegBaseMaskRegion

            'Edge Smoothing Regions
            EdgeSmoothingRegion1 = New EdgeSmoothingRegion
            EdgeSmoothingRegion2 = New EdgeSmoothingRegion
            EdgeSmoothingRegion3 = New EdgeSmoothingRegion
            EdgeSmoothingRegion4 = New EdgeSmoothingRegion
            EdgeSmoothingRegion5 = New EdgeSmoothingRegion
            EdgeSmoothingRegion6 = New EdgeSmoothingRegion
            EdgeSmoothingRegion7 = New EdgeSmoothingRegion
            EdgeSmoothingRegion8 = New EdgeSmoothingRegion


            'Support Points Offset
            SupportPoints1 = New SupportPoints
            SupportPoints2 = New SupportPoints
            SupportPoints3 = New SupportPoints
            SupportPoints4 = New SupportPoints


            'save data
            isSaveData = True
            DataSaveNb = 100
            DataSavePath = Application.StartupPath & "\Data_Auto_Saved\"
            DataAutoSaveFolder = "Data_Auto_Saved"
            DataNameMaxCount = 3
        End Sub
    End Class

    Public Class SupportPoints
        Public enabled As Boolean
        Public runID As Integer
        Public side As Integer
        Public length As Integer
        Public searchWidth As Integer
        Public edgeStepThreshold As Integer
        Public offset As Integer
        Public points As List(Of Point)

        Public Sub New()
            length = 100 'pixel, total length
            searchWidth = 100 'pixel
            offset = 10 'pixel
            points = New List(Of Point)
        End Sub
    End Class
    <Serializable()> Public Class SegBaseMeasurementRegion
        Public startRow As Integer 'row pixel
        Public startColumn As Integer 'column pixel
        Public NbRegions As Integer
        Public MeasurementWidth As Integer
        Public MeasurementLength As Integer
    End Class

    <Serializable()> Public Class SegBaseMaskRegion
        Public runID As Integer '在第几副图像
        Public startRow As Integer 'row pixel
        Public startColumn As Integer 'column pixel
        Public width As Integer
        Public length As Integer
    End Class

    <Serializable()> Public Class SegTopTrackRegion
        Public enabled As Boolean
        Public runID As Integer '在第几副图像
        Public side As Integer '1-左；2-右；3-上；4；下
        Public startRow As Integer 'row pixel
        Public startColumn As Integer 'column pixel
        Public width As Integer
        Public length As Integer
        Public outlierThreshold As Integer '异常点阀值：到均值的距离' value = 0 then disabled
        Public averagingSize As Integer '用相邻的多少个点算平均值
        '’Public segmentationWidth As Integer '找到顶点以后，以顶点为中心，在该宽度内应用分割
        Public segmentationExtendUp As Integer '用最上面（左边）的分割高度，往上（左）延长分割区域
        Public segmentationExtendDown As Integer '用最下面（右边）的分割高度，往下（右）延长分割区域


        Public Sub New()
            enabled = False
            averagingSize = 200
            'segmentationWidth = 200
        End Sub

    End Class

    <Serializable()> Public Class EdgeSmoothingRegion
        Public enabled As Boolean
        Public runID As Integer '在第几副图像
        Public side As Integer '1-左；2-右；3-上；4；下
        Public startRow As Integer 'row pixel
        Public startColumn As Integer 'column pixel
        Public width As Integer
        Public length As Integer
        'Public edgeSearchDirection As Integer '1-up 上; 2-down下; 3-left左; 4-right右
        Public edgeStepThreshold As Integer '边缘点梯度差
        Public outlierThreshold As Integer '异常点阀值：到均值的距离' value = 0 then disabled
        Public smoothingSize As Integer '用相邻的多少个点算平均值
        ' Public smoothingSizeUsePercentage As Integer '用多少百分比的数据来计算均值
        Public edgeFillingWidth As Integer '找到边缘点以后，左边在width宽度内填充0，右边在width宽度内填充255

        Public Sub New()
            enabled = False
            edgeStepThreshold = 50
            smoothingSize = 20
            edgeFillingWidth = 20

        End Sub

    End Class
    Public Sub New()
        If mAuthorize.IsLicenseValid("license.dat", 1, 1, New List(Of String)) Then
            If Not File.Exists(settingFile) Then
                mSet = New GoSurfaceMakerSetting
                SaveSetting()
            End If
            LoadSetting()
            isLicenseValid = True
        Else
            MsgBox("License Invalid", MsgBoxStyle.Critical)
        End If
    End Sub
    Public Function connect(ByRef errMsg As String) As Boolean
        'initialize and connect to the gocator
        Try

            'refresh setting
            If Not isLicenseValid Then
                MsgBox("License Invalid", MsgBoxStyle.Critical)
                Return False
            End If
            LoadSetting()
            SaveSetting()
            mCurrentDataNameCountNb = 0
            '''''''''''''''''''''''''''

            Dim status As kStatus
            Dim addr As New address
            Dim addrPtr As IntPtr = IntPtr.Zero

            status = GoSdk_Construct(go_api)
            If status <> 1 Then
                errMsg = String.Format("GoSdk_Construct Error:{0}", status)
                Return False
            End If

            status = GoSystem_Construct(go_system, IntPtr.Zero)
            If status <> 1 Then
                errMsg = String.Format("GoSystem_Construct Error:{0}", status)
                Return False
            End If

            addrPtr = Marshal.AllocHGlobal(Marshal.SizeOf(addr))
            Marshal.StructureToPtr(addr, addrPtr, False)

            status = kIpAddress_Parse(addrPtr, mSet.gocatorIP)
            If status <> 1 Then
                errMsg = String.Format("kIpAddress_Parse Error:{0}", status)
                Return False
            End If

            status = GoSystem_FindSensorByIpAddress(go_system, addrPtr, go_sensor) '通过IP地址获取gocator指针
            If status <> 1 Then
                errMsg = String.Format("GoSystem_FindSensorByIpAddress Error:{0}", status)
                Return False
            End If

            status = GoSystem_Connect(go_system) '连接gocator
            If status <> 1 Then
                errMsg = String.Format("GoSensor_Connect Error:{0}", status)
                Return False
            End If

            go_setup = GoSensor_Setup(go_sensor) '获取Gocaotr配置指针


            status = GoSystem_EnableData(go_system, True) '启用数据通道
            If status <> 1 Then
                errMsg = String.Format("GoSystem_EnableData Error:{0}", status)
                Return False
            End If

            status = GoSystem_SetDataHandler(go_system, myOnData, context) '设置回调函数
            If status <> 1 Then
                Console.WriteLine("GoSystem_SetDataHandler Error:{0}", status)
                Return False
            End If


            Return True
        Catch ex As Exception
            errMsg = ex.Message
            Return False
        End Try
    End Function '连接系统
    Public Function disconnect(ByRef errMsg As String) As Boolean
        'disconnect the gocator and close the buffering process
        Try
            Dim status As kStatus

            'If Not close(errMsg) Then Return False 'close the buffering process

            status = GoSystem_Stop(go_system)
            If status <> 1 Then
                Console.WriteLine("GoSystem_Stop Error:{0}", status)
                Return False
            End If

            Dim context As DataContext = New DataContext()
            status = GoSystem_SetDataHandler(go_system, Nothing, context)
            If status <> 1 Then
                errMsg = String.Format("GoSystem_SetDataHandler Error:{0}", status)
                Return False
            End If

            status = GoDestroy(go_system)
            If status <> 1 Then
                Console.WriteLine("GoDestroy Error:{0}", status)
                Return False
            End If

            status = GoDestroy(go_api)
            If status <> 1 Then
                Console.WriteLine("GoDestroy Error:{0}", status)
                Return False
            End If
            Return True
        Catch ex As Exception
            errMsg = ex.Message
            Return False
        End Try
    End Function '断开系统
    Public Function start(Optional ByRef errMsg As String = "") As Boolean
        'start the sensor and start the buffering process
        Try
            'reset buffering
            If mProfilesBuffer IsNot Nothing Then
                mProfilesBuffer.Clear()
                mProfilesBuffer = Nothing
            End If
            mProfilesBuffer = New List(Of ProfileByte)

            'll
            mIntensityProfileBuffer = New List(Of IntensityByte)

            ''''''''''''''''

            Dim status As kStatus
            status = GoSystem_Start(go_system)
            If status <> 1 Then
                Debug.Print("GoSystem_Start Error:{0}", status)
                Return False
            End If

            Return True
        Catch ex As Exception
            errMsg = ex.Message
            Return False
        End Try
    End Function '开始扫描，开始接受轮廓线
    Public Function close(Optional ByRef errMsg As String = "") As Boolean
        'stop the sensor and stop the buffering process
        Try
            Dim status As kStatus = GoSystem_Stop(go_system)
            If status <> 1 Then
                errMsg = String.Format("GoSystem_Stop Error:{0}", status)
                Return False
            End If
            Return True
        Catch ex As Exception
            errMsg = ex.Message
            Return False
        End Try
    End Function '停止扫描
    Public Function setEncoderSpacing(ByRef space As Double, Optional isRestartSensor As Boolean = True) As Boolean
        'set the triggering spacing of the sensor' in mm
        Try
            mSw.Reset() : mSw.Start()
            Dim status As kStatus

            status = GoSystem_Stop(go_system) 'stop the sensor first

            status = GoSetup_SetEncoderSpacing(go_setup, space)
            If status <> 1 Then
                Debug.Print("GoSetup_SetEncoderSpacing Error:{0}", status)
                Return False
            End If
            status = GoSensor_Flush(go_sensor)
            If status <> 1 Then
                Debug.Print("GoSensor_Flush Error:{0}", status)
                Return False
            End If

            If isRestartSensor Then
                status = GoSystem_Start(go_system) 'restart
            End If

            mSw.Stop()
            Debug.Print("Set encoder spacing spent: " & mSw.ElapsedMilliseconds)

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function '设置编码器触发间隔
    Public Function openLaser() As Boolean
        'set the triggering by time
        'start sensor
        Try
            mSw.Reset() : mSw.Start()
            Dim status As kStatus

            status = GoSystem_Stop(go_system) 'stop the sensor first

            status = GoSetup_SetTriggerSource(go_setup, GoTrigger.GO_TRIGGER_TIME)
            If status <> 1 Then
                Debug.Print("GoSetup_SetTriggerSource Error:{0}", status)
                Return False
            End If
            status = GoSensor_Flush(go_sensor)
            If status <> 1 Then
                Debug.Print("GoSensor_Flush Error:{0}", status)
                Return False
            End If

            status = GoSystem_Start(go_system) 'restart

            mSw.Stop()
            Debug.Print("openLaser spent: " & mSw.ElapsedMilliseconds)

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function '打开激光（方便调试时看激光位置）
    Public Function closeLaser() As Boolean
        'set the triggering back to encoder
        'stop sensor
        Try
            mSw.Reset() : mSw.Start()
            Dim status As kStatus

            status = GoSystem_Stop(go_system) 'stop the sensor first

            status = GoSetup_SetTriggerSource(go_setup, GoTrigger.GO_TRIGGER_ENCODER)
            If status <> 1 Then
                Debug.Print("GoSetup_SetTriggerSource Error:{0}", status)
                Return False
            End If
            status = GoSensor_Flush(go_sensor)
            If status <> 1 Then
                Debug.Print("GoSensor_Flush Error:{0}", status)
                Return False
            End If

            mSw.Stop()
            Debug.Print("closeLaser spent: " & mSw.ElapsedMilliseconds)

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function '关闭激光
    Public Function reset() As Boolean
        mProfilesBuffer = New List(Of ProfileByte)
        mCurrentDataNameCountNb = 0
        closeLaser()
        Return True
    End Function
    'Public Function getHalconImage(ByRef image As HObject, Optional RunID As Integer = 0, Optional ByRef imgArray() As Byte = Nothing, Optional ByRef imgWidth As Integer = 0, Optional ByRef imgHeight As Integer = 0) As Boolean
    '    Dim width As Integer = mSet.imageWidth
    '    Dim height As Integer = mSet.imageHeight 'mSet,解析xml
    '    Dim imageResolution As Single = mSet.imageResolution 'mm/row pixel
    '    Dim encoderResolution As Single = mSet.encoderResolution 'mm/tick
    '    Dim X_Start As Single = mSet.FOV_X_Start
    '    Dim X_Range As Single = mSet.FOV_X_Range
    '    Dim Z_Start As Single = mSet.FOV_Z_Start
    '    Dim Z_Range As Single = mSet.FOV_Z_Range
    '    Dim Z_Resolution As Double = 255 / Z_Range

    '    Try
    '        If mProfilesBuffer Is Nothing Then Return False
    '        If mProfilesBuffer.Count <= mSet.ignoreBeginProfilesNb Then Return False

    '        Dim beginIndex As Integer = mSet.ignoreBeginProfilesNb


    '        Dim imageArray(height - 1, width - 1) As Byte
    '        'll
    '        Dim imageIntensityArray(height - 1, width - 1) As Byte

    '        Dim lastRowIndex As Integer = 0
    '        Dim bufferCount As Integer = mProfilesBuffer.Count

    '        mSw.Reset() : mSw.Start() '计时

    '        'save data first
    '        If mSet.isSaveData Then '保存数据setting.xml中改为true
    '            mCurrentDataNameCountNb = RunID
    '            Dim thrdStart As New Threading.Thread(AddressOf SaveData)
    '            thrdStart.Start()
    '        End If

    '        'check if is upsidedown image 检查是否让图像上下倒置
    '        Dim isUpsideDownImage As Boolean = False '根据xml中的设置是否颠倒图像
    '        Select Case RunID
    '            Case 1
    '                isUpsideDownImage = mSet.isUpsideDownImage1
    '            Case 2
    '                isUpsideDownImage = mSet.isUpsideDownImage2
    '            Case 3
    '                isUpsideDownImage = mSet.isUpsideDownImage3
    '            Case 4
    '                isUpsideDownImage = mSet.isUpsideDownImage4
    '            Case 5
    '                isUpsideDownImage = mSet.isUpsideDownImage5
    '            Case 6
    '                isUpsideDownImage = mSet.isUpsideDownImage6
    '            Case Else
    '                MsgBox("允许6张图")
    '                Return False
    '        End Select
    '        Dim startIndex As Integer = beginIndex
    '        Dim endIndex As Integer = bufferCount - 1
    '        Dim stepSize As Integer = 1
    '        If isUpsideDownImage Then '如果需要颠倒，首尾坐标互换
    '            startIndex = bufferCount - 1
    '            endIndex = beginIndex
    '            stepSize = -1
    '        End If
    '        Dim lastEncoder As Long = mProfilesBuffer(startIndex).encoder '获取起始轮廓脉冲值
    '        '''''''''''''''''''''''''''''''''''''''''''''''''''''


    '        For i As Integer = startIndex To endIndex Step stepSize
    '            '算出当前是第几行--》根据脉冲
    '            Dim curtRowIndex As Integer = CInt(lastRowIndex + Math.Abs((mProfilesBuffer(i).encoder - lastEncoder)) * encoderResolution / imageResolution)
    '            lastEncoder = mProfilesBuffer(i).encoder

    '            If curtRowIndex >= height Then Exit For 'exceed the image height

    '            Dim pointsLength As Integer = mProfilesBuffer(i).zBytes.Length '一条轮廓z轴点的个数
    '            Dim points = mProfilesBuffer(i).zBytes
    '            'll
    '            Dim mIntensityPoints = mIntensityProfileBuffer(i).intensity


    '            'filling rows by profiles
    '            Dim XStartIndex As Integer = 0
    '            Dim XEndIndex As Integer = pointsLength - 1
    '            Dim XStepSize As Integer = 1
    '            If mSet.isFlipingX Then '是否反向
    '                XStartIndex = pointsLength - 1
    '                XEndIndex = 0
    '                XStepSize = -1
    '            End If
    '            Dim curtColIndex As Integer = 0
    '            For j As Integer = XStartIndex To XEndIndex Step XStepSize '将轮廓中的点收集在imageArray数组中
    '                'assume one point per pixel
    '                If curtColIndex >= width Then 'exceed the image width
    '                    Exit For
    '                End If
    '                imageArray(curtRowIndex, curtColIndex) = points(j) '将高度信息赋给每一行每一列（改成亮度）
    '                'll
    '                imageIntensityArray(curtRowIndex, curtColIndex) = mIntensityPoints(j)

    '                curtColIndex += 1
    '            Next

    '            '如果当前行与上一行的间距大于1 补一行（补亮度）
    '            If curtRowIndex - lastRowIndex > 1 And mSet.isFillingRows Then ' fill the rows for big spacing rows
    '                calcFillingRows(imageArray, lastRowIndex, curtRowIndex)

    '                'll
    '                calcFillingRows(imageIntensityArray, lastRowIndex, curtRowIndex)
    '            End If
    '            lastRowIndex = curtRowIndex
    '        Next

    '        'Apply Base-Track Segmentation 底板与点胶台分割
    '        If mSet.isUseSegmentation And Not mSet.isUseSegmentationTopTrack And RunID <> 0 Then
    '            Dim segRegion As New SegBaseMeasurementRegion
    '            Select Case RunID
    '                Case 1
    '                    segRegion = mSet.SegBaseMeasurementRegion1
    '                Case 2
    '                    segRegion = mSet.SegBaseMeasurementRegion2
    '                Case 3
    '                    segRegion = mSet.SegBaseMeasurementRegion3
    '                Case 4
    '                    segRegion = mSet.SegBaseMeasurementRegion4
    '                Case 5
    '                    segRegion = mSet.SegBaseMeasurementRegion5
    '                Case 6
    '                    segRegion = mSet.SegBaseMeasurementRegion6
    '                Case 7
    '                    MsgBox("允许6张图")
    '                    Return False
    '            End Select
    '            '原版
    '            'If Not SegBaseTrack(RunID, imageArray, segRegion) Then
    '            '    Debug.Print("No Segmentation Applied: The segmentation base regions includes invalid base measurements.")
    '            'End If

    '            'll
    '            If Not SegBaseTrack(RunID, imageArray, imageIntensityArray, segRegion) Then
    '                Debug.Print("No Segmentation Applied: The segmentation base regions includes invalid base measurements.")
    '            End If
    '        End If
    '        ''''''''''''''''''''''''''''''''''''''

    '        If mSet.isUseSegmentationTopTrack And RunID <> 0 Then

    '        End If

    '        'Appy Top-Track Segmentation 外立面顶点与点胶台分割
    '        Dim mSw3 As New Stopwatch : mSw3.Start()
    '        If mSet.isUseSegmentation And mSet.isUseSegmentationTopTrack And RunID <> 0 Then 'Only enabled when using segmentation
    '            '原版
    '            'applyTopTrackSegmentation(imageArray, RunID, mSet.SegTopTrackRegion1) '搜索区域内大于 最高点向下偏移12的点变为255，其它点变为0
    '            'applyTopTrackSegmentation(imageArray, RunID, mSet.SegTopTrackRegion2)
    '            'applyTopTrackSegmentation(imageArray, RunID, mSet.SegTopTrackRegion3)
    '            'applyTopTrackSegmentation(imageArray, RunID, mSet.SegTopTrackRegion4)
    '            'applyTopTrackSegmentation(imageArray, RunID, mSet.SegTopTrackRegion5)
    '            'applyTopTrackSegmentation(imageArray, RunID, mSet.SegTopTrackRegion6)
    '            'applyTopTrackSegmentation(imageArray, RunID, mSet.SegTopTrackRegion7)
    '            'applyTopTrackSegmentation(imageArray, RunID, mSet.SegTopTrackRegion8)
    '            'applyTopTrackSegmentation(imageArray, RunID, mSet.SegTopTrackRegion9)
    '            'applyTopTrackSegmentation(imageArray, RunID, mSet.SegTopTrackRegion10)

    '            'll
    '            Select Case RunID
    '                Case 1
    '                    applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion1) '搜索区域内大于 最高点向下偏移12的点变为255，其它点变为0
    '                    applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion2)
    '                    applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion3)
    '                Case 2
    '                    applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion4)
    '                    applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion5)
    '                Case 3
    '                    applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion6)
    '                    applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion7)
    '                Case 4
    '                    applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion8)
    '                    applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion9)
    '                Case 5
    '                    applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion10)
    '                    applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion11)
    '                Case 6
    '                    applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion12)
    '                    applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion13)
    '                    applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion14)
    '            End Select
    '        End If
    '        mSw3.Stop()
    '        Debug.Print("Top-Track Segmentation spent: " & mSw3.ElapsedMilliseconds)
    '        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''


    '        'Appy Edge Smoothing
    '        Dim mSw4 As New Stopwatch : mSw4.Start()
    '        If mSet.isUseEdgeSmoothing And mSet.isUseSegmentation Then 'Only enabled when using segmentation
    '            applyEdgeSmoothing(imageArray, RunID, mSet.EdgeSmoothingRegion1)
    '            applyEdgeSmoothing(imageArray, RunID, mSet.EdgeSmoothingRegion2)
    '            applyEdgeSmoothing(imageArray, RunID, mSet.EdgeSmoothingRegion3)
    '            applyEdgeSmoothing(imageArray, RunID, mSet.EdgeSmoothingRegion4)
    '            applyEdgeSmoothing(imageArray, RunID, mSet.EdgeSmoothingRegion5)
    '            applyEdgeSmoothing(imageArray, RunID, mSet.EdgeSmoothingRegion6)
    '            applyEdgeSmoothing(imageArray, RunID, mSet.EdgeSmoothingRegion7)
    '            applyEdgeSmoothing(imageArray, RunID, mSet.EdgeSmoothingRegion8)
    '        End If
    '        mSw4.Stop()
    '        Debug.Print("Edge Smoothing spent: " & mSw4.ElapsedMilliseconds)
    '        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''


    '        'write to Halcon Image
    '        'convert imageArray to Halcon Image
    '        Dim mSw2 As New Stopwatch : mSw2.Start()
    '        HOperatorSet.GenImageConst(image, "byte", width, height)
    '        Dim imgPtr As IntPtr
    '        HOperatorSet.GetImagePointer1(image, imgPtr, New Integer, New Integer, New Integer)

    '        Dim mArrayByte(width * height - 1) As Byte
    '        System.Buffer.BlockCopy(imageArray, 0, mArrayByte, 0, width * height)
    '        Marshal.Copy(mArrayByte, 0, imgPtr, width * height)
    '        'mArrayByte = Nothing
    '        mSw2.Stop()
    '        Debug.Print("Halcon Image conversion spent: " & mSw2.ElapsedMilliseconds)

    '        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '        'output the array
    '        imgArray = mArrayByte
    '        imgWidth = width
    '        imgHeight = height
    '        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '        mSw.Stop()
    '        Debug.Print("getHalconImage spent: " & mSw.ElapsedMilliseconds)

    '        Return True
    '    Catch ex As Exception
    '        MsgBox(ex.Message)
    '        Return False
    '    Finally
    '        GC.Collect()
    '    End Try
    'End Function
    Public Function getHalconImage(ByRef image As HObject, Optional RunID As Integer = 0, Optional ByRef ImgArray() As Byte = Nothing, Optional ByRef imgWidth As Integer = 0, Optional ByRef imgHeight As Integer = 0) As Boolean
        Dim width As Integer = mSet.imageWidth
        Dim height As Integer = mSet.imageHeight 'mSet,解析xml
        Dim imageResolution As Single = mSet.imageResolution 'mm/row pixel
        Dim encoderResolution As Single = mSet.encoderResolution 'mm/tick
        Dim X_Start As Single = mSet.FOV_X_Start
        Dim X_Range As Single = mSet.FOV_X_Range
        Dim Z_Start As Single = mSet.FOV_Z_Start
        Dim Z_Range As Single = mSet.FOV_Z_Range
        Dim Z_Resolution As Double = 255 / Z_Range

        Try
            If mProfilesBuffer Is Nothing Then Return False
            If mProfilesBuffer.Count <= mSet.ignoreBeginProfilesNb Then Return False

            Dim beginIndex As Integer = mSet.ignoreBeginProfilesNb


            Dim imageArray(height - 1, width - 1) As Byte
            Dim imageSegmentArray(height - 1, width - 1) As Byte
            'll
            Dim imageIntensityArray(height - 1, width - 1) As Byte

            Dim lastRowIndex As Integer = 0
            Dim bufferCount As Integer = mProfilesBuffer.Count

            mSw.Reset() : mSw.Start() '计时

            'save data first
            If mSet.isSaveData Then '保存数据setting.xml中改为true
                mCurrentDataNameCountNb = RunID
                Dim thrdStart As New Threading.Thread(AddressOf SaveData)
                thrdStart.Start()
            End If

            'check if is upsidedown image 检查是否让图像上下倒置
            Dim isUpsideDownImage As Boolean = False '根据xml中的设置是否颠倒图像
            Select Case RunID
                Case 1
                    isUpsideDownImage = mSet.isUpsideDownImage1
                Case 2
                    isUpsideDownImage = mSet.isUpsideDownImage2
                Case 3
                    isUpsideDownImage = mSet.isUpsideDownImage3
                Case 4
                    isUpsideDownImage = mSet.isUpsideDownImage4
                Case 5
                    isUpsideDownImage = mSet.isUpsideDownImage5
                Case 6
                    isUpsideDownImage = mSet.isUpsideDownImage6
            End Select
            Dim startIndex As Integer = beginIndex
            Dim endIndex As Integer = bufferCount - 1
            Dim stepSize As Integer = 1
            If isUpsideDownImage Then '如果需要颠倒，首尾坐标互换
                startIndex = bufferCount - 1
                endIndex = beginIndex
                stepSize = -1
            End If
            Dim lastEncoder As Long = mProfilesBuffer(startIndex).encoder '获取起始轮廓脉冲值
            '''''''''''''''''''''''''''''''''''''''''''''''''''''


            For i As Integer = startIndex To endIndex Step stepSize
                '算出当前是第几行--》根据脉冲
                Dim curtRowIndex As Integer = CInt(lastRowIndex + Math.Abs((mProfilesBuffer(i).encoder - lastEncoder)) * encoderResolution / imageResolution)
                lastEncoder = mProfilesBuffer(i).encoder

                If curtRowIndex >= height Then Exit For 'exceed the image height

                Dim pointsLength As Integer = mProfilesBuffer(i).zBytes.Length '一条轮廓z轴点的个数
                Dim points = mProfilesBuffer(i).zBytes
                'll
                Dim mIntensityPoints = mIntensityProfileBuffer(i).intensity


                'filling rows by profiles
                Dim XStartIndex As Integer = 0
                Dim XEndIndex As Integer = pointsLength - 1
                Dim XStepSize As Integer = 1
                If mSet.isFlipingX Then '是否反向
                    XStartIndex = pointsLength - 1
                    XEndIndex = 0
                    XStepSize = -1
                End If
                Dim curtColIndex As Integer = 0
                For j As Integer = XStartIndex To XEndIndex Step XStepSize '将轮廓中的点收集在imageArray数组中
                    'assume one point per pixel
                    If curtColIndex >= width Then 'exceed the image width
                        Exit For
                    End If
                    imageArray(curtRowIndex, curtColIndex) = points(j) '将高度信息赋给每一行每一列（改成亮度）
                    'll
                    imageIntensityArray(curtRowIndex, curtColIndex) = mIntensityPoints(j)

                    curtColIndex += 1
                Next

                '如果当前行与上一行的间距大于1 补一行（补亮度）
                If curtRowIndex - lastRowIndex > 1 And mSet.isFillingRows Then ' fill the rows for big spacing rows
                    calcFillingRows(imageArray, lastRowIndex, curtRowIndex)

                    'll
                    calcFillingRows(imageIntensityArray, lastRowIndex, curtRowIndex)
                End If
                lastRowIndex = curtRowIndex
            Next

            If (Not mSet.isUseSegmentationMask) And (Not mSet.ImageForIntensity) Then
                HOperatorSet.GenImageConst(image, "byte", width, height)
                Dim imgPtr As IntPtr
                HOperatorSet.GetImagePointer1(image, imgPtr, New Integer, New Integer, New Integer)
                Dim mArrayByte(width * height - 1) As Byte
                System.Buffer.BlockCopy(imageArray, 0, mArrayByte, 0, width * height)
                Marshal.Copy(mArrayByte, 0, imgPtr, width * height)
                ImgArray = mArrayByte
            End If

            If (Not mSet.isUseSegmentationMask) And mSet.ImageForIntensity Then
                HOperatorSet.GenImageConst(image, "byte", width, height) 'll
                Dim imgIntensityPtr As IntPtr 'll
                HOperatorSet.GetImagePointer1(image, imgIntensityPtr, New Integer, New Integer, New Integer) 'll
                Dim mIntensityByte(width * height - 1) As Byte 'll
                System.Buffer.BlockCopy(imageIntensityArray, 0, mIntensityByte, 0, width * height) 'll
                Marshal.Copy(mIntensityByte, 0, imgIntensityPtr, width * height)
                ImgArray = mIntensityByte
            End If




            'Apply Base-Track Segmentation 底板与点胶台分割
            If mSet.isUseSegmentation And Not mSet.isUseSegmentationTopTrack And RunID <> 0 Then
                Dim segRegion As New SegBaseMeasurementRegion
                Select Case RunID
                    Case 1
                        segRegion = mSet.SegBaseMeasurementRegion1
                    Case 2
                        segRegion = mSet.SegBaseMeasurementRegion2
                    Case 3
                        segRegion = mSet.SegBaseMeasurementRegion3
                    Case 4
                        segRegion = mSet.SegBaseMeasurementRegion4
                    Case 5
                        segRegion = mSet.SegBaseMeasurementRegion5
                    Case 6
                        segRegion = mSet.SegBaseMeasurementRegion6
                    Case Else
                        MsgBox("允许6张图")
                        Return False
                End Select
                '原版
                'If Not SegBaseTrack(RunID, imageArray, segRegion) Then
                '    Debug.Print("No Segmentation Applied: The segmentation base regions includes invalid base measurements.")
                'End If

                'll
                If mSet.isUseSegmentationMask And (Not mSet.MaskForIntensity) Then '如果mask区域需要覆盖高度图，亮度数组直接指向高度数组
                    imageIntensityArray = imageArray
                End If
                If Not SegBaseTrack(RunID, imageArray, imageIntensityArray, segRegion) Then
                    Debug.Print("No Segmentation Applied: The segmentation base regions includes invalid base measurements.")
                End If
            End If
            ''''''''''''''''''''''''''''''''''''''

            If mSet.isUseSegmentationTopTrack And RunID <> 0 Then

            End If

            'Appy Top-Track Segmentation 外立面顶点与点胶台分割
            Dim mSw3 As New Stopwatch : mSw3.Start()
            If mSet.isUseSegmentation And mSet.isUseSegmentationTopTrack And RunID <> 0 Then 'Only enabled when using segmentation
                '原版
                'applyTopTrackSegmentation(imageArray, RunID, mSet.SegTopTrackRegion1) '搜索区域内大于 最高点向下偏移12的点变为255，其它点变为0
                'applyTopTrackSegmentation(imageArray, RunID, mSet.SegTopTrackRegion2)
                'applyTopTrackSegmentation(imageArray, RunID, mSet.SegTopTrackRegion3)
                'applyTopTrackSegmentation(imageArray, RunID, mSet.SegTopTrackRegion4)
                'applyTopTrackSegmentation(imageArray, RunID, mSet.SegTopTrackRegion5)
                'applyTopTrackSegmentation(imageArray, RunID, mSet.SegTopTrackRegion6)
                'applyTopTrackSegmentation(imageArray, RunID, mSet.SegTopTrackRegion7)
                'applyTopTrackSegmentation(imageArray, RunID, mSet.SegTopTrackRegion8)
                'applyTopTrackSegmentation(imageArray, RunID, mSet.SegTopTrackRegion9)
                'applyTopTrackSegmentation(imageArray, RunID, mSet.SegTopTrackRegion10)

                'll
                If mSet.isUseSegmentationMask And (Not mSet.MaskForIntensity) Then '如果mask区域需要覆盖高度图，亮度数组直接指向高度数组
                    imageIntensityArray = imageArray
                End If
                Select Case RunID
                    Case 1
                        applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion1) '搜索区域内大于 最高点向下偏移12的点变为255，其它点变为0
                        applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion2)
                        applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion3)
                    Case 2
                        applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion4)
                        applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion5)
                    Case 3
                        applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion6)
                        applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion7)
                    Case 4
                        applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion8)
                        applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion9)
                    Case 5
                        applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion10)
                        applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion11)
                    Case 6
                        applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion12)
                        applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion13)
                        applyTopTrackSegmentation(imageIntensityArray, imageArray, RunID, mSet.SegTopTrackRegion14)
                End Select
            End If
            mSw3.Stop()
            Debug.Print("Top-Track Segmentation spent: " & mSw3.ElapsedMilliseconds)
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''


            'Appy Edge Smoothing
            Dim mSw4 As New Stopwatch : mSw4.Start()
            If mSet.isUseEdgeSmoothing And mSet.isUseSegmentation Then 'Only enabled when using segmentation 
                'task
                applyEdgeSmoothing(imageArray, RunID, mSet.EdgeSmoothingRegion1)
                applyEdgeSmoothing(imageArray, RunID, mSet.EdgeSmoothingRegion2)
                applyEdgeSmoothing(imageArray, RunID, mSet.EdgeSmoothingRegion3)
                applyEdgeSmoothing(imageArray, RunID, mSet.EdgeSmoothingRegion4)
                applyEdgeSmoothing(imageArray, RunID, mSet.EdgeSmoothingRegion5)
                applyEdgeSmoothing(imageArray, RunID, mSet.EdgeSmoothingRegion6)
                applyEdgeSmoothing(imageArray, RunID, mSet.EdgeSmoothingRegion7)
                applyEdgeSmoothing(imageArray, RunID, mSet.EdgeSmoothingRegion8)
            End If
            mSw4.Stop()
            Debug.Print("Edge Smoothing spent: " & mSw4.ElapsedMilliseconds)
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''


            'write to Halcon Image
            'convert imageArray to Halcon Image
            Dim mSw2 As New Stopwatch : mSw2.Start()

            If mSet.isUseSegmentation And True Then
                HOperatorSet.GenImageConst(image, "byte", width, height)
                Dim segmentImgPtr As IntPtr
                HOperatorSet.GetImagePointer1(image, segmentImgPtr, New Integer, New Integer, New Integer)
                Dim mSegmentArrayByte(width * height - 1) As Byte
                System.Buffer.BlockCopy(imageArray, 0, mSegmentArrayByte, 0, width * height)
                Marshal.Copy(mSegmentArrayByte, 0, segmentImgPtr, width * height)
                ImgArray = mSegmentArrayByte
            End If



            'mArrayByte = Nothing
            mSw2.Stop()
            Debug.Print("Halcon Image conversion spent: " & mSw2.ElapsedMilliseconds)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            'output the array
            imgWidth = width
            imgHeight = height
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            mSw.Stop()
            Debug.Print("getHalconImage spent: " & mSw.ElapsedMilliseconds)

            Return True
        Catch ex As Exception
            MsgBox(ex.Message)
            Return False
        Finally
            GC.Collect()
        End Try
    End Function



    Private Sub calcFillingRows(ByRef imageArray(,) As Byte, ByVal lastRowIndex As Integer, ByVal curtRowIndex As Integer)
        Dim width As Integer = UBound(imageArray, 2) + 1
        Dim nbFillingRows As Integer = curtRowIndex - lastRowIndex - 1
        Dim rowAve(width - 1) As Byte 'average of 2 rows
        For i As Integer = 0 To width - 1
            Dim tmp As Single = imageArray(curtRowIndex, i)
            tmp += imageArray(lastRowIndex, i)
            rowAve(i) = CByte(tmp / 2)
        Next
        For i As Integer = 1 To nbFillingRows
            System.Buffer.BlockCopy(rowAve, 0, imageArray, (lastRowIndex + i) * width, width)
        Next
    End Sub
    Private Sub calcFillingRowsGradient(ByRef imageArray(,) As Byte, ByVal lastRowIndex As Integer, ByVal curtRowIndex As Integer)
        Dim width As Integer = UBound(imageArray, 2) + 1
        Dim nbFillingRows As Integer = curtRowIndex - lastRowIndex - 1
        Dim rowDiffIncrements(width - 1) As Single
        For i As Integer = 0 To width - 1
            Dim d As Single = CInt(imageArray(curtRowIndex, i)) - CInt(imageArray(lastRowIndex, i))
            rowDiffIncrements(i) = d / (nbFillingRows + 1)
        Next
        For i As Integer = 1 To nbFillingRows
            For j As Integer = 0 To width - 1
                imageArray(lastRowIndex + i, j) = imageArray(lastRowIndex, j) + i * rowDiffIncrements(j)
            Next
        Next
    End Sub

#Region "Receiving Profiles Async and Buffering"
    Private Function onData(ByVal ctx As DataContext, ByVal sys As IntPtr, ByVal data As IntPtr) As Integer
        'async receive profiles from gocator and store it in buffer
        Try
            'll 
            Dim profileIntensityMsg As IntPtr = IntPtr.Zero

            Dim mIntensityProfile As IntensityByte

            Dim dataObj As IntPtr = IntPtr.Zero
            Dim stampMsg As IntPtr = IntPtr.Zero
            Dim profileMsg As IntPtr = IntPtr.Zero
            Dim stampPtr As IntPtr = IntPtr.Zero
            Dim stamp As New GoStamp
            Dim context As DataContext = ctx

            ' each result can have multiple data items
            Dim itemCount = GoDataSet_Count(data)
            Dim mProfile As New ProfileByte

            ' loop through all items in result message
            For i As Integer = 0 To itemCount - 1
                dataObj = GoDataSet_At(data, i)
                If GoDataMsg_Type(dataObj) = GoDataMessageTypes.GO_DATA_MESSAGE_TYPE_STAMP Then
                    stampMsg = dataObj
                    stampPtr = GoStampMsg_At(stampMsg, 0)
                    stamp = Marshal.PtrToStructure(stampPtr, GetType(GoStamp))
                    mProfile.encoder = stamp.encoder
                    mIntensityProfile.encoder = stamp.encoder
                End If

                ' retrieve resampeld profile data
                If GoDataMsg_Type(dataObj) = GoDataMessageTypes.GO_DATA_MESSAGE_TYPE_RESAMPLED_PROFILE Then
                    profileMsg = dataObj
                    context.xResolution = GoProfileMsg_XResolution(profileMsg) / 1000000
                    context.zResolution = GoProfileMsg_ZResolution(profileMsg) / 1000000
                    context.xOffset = GoProfileMsg_XOffset(profileMsg) / 1000
                    context.zOffset = GoProfileMsg_ZOffset(profileMsg) / 1000

                    Dim profilePointCount = GoResampledProfileMsg_Width(profileMsg)
                    Dim points(profilePointCount - 1) As Short
                    Dim profileBuffer(profilePointCount - 1) As ProfilePoint
                    Dim pointsPtr As IntPtr = GoResampledProfileMsg_At(profileMsg, 0)
                    Marshal.Copy(pointsPtr, points, 0, points.Length)

                    Dim profileZBytes(profilePointCount - 1) As Byte
                    Dim z_byte_resolution As Double = 255 / mSet.FOV_Z_Range

                    For arrayIndex As Integer = 0 To profilePointCount - 1
                        Dim x_real = context.xOffset + context.xResolution * arrayIndex
                        Dim z_real = IIf(points(arrayIndex) = invalid, invalid, context.zOffset + context.zResolution * points(arrayIndex))
                        'convert z value into 8-bit value
                        Dim z_byte As Double = IIf(z_real = invalid, 0, (z_real - mSet.FOV_Z_Start) * z_byte_resolution)
                        If z_byte < 0 Then
                            z_byte = 0
                        ElseIf z_byte > 255 Then
                            z_byte = 255
                        End If
                        profileZBytes(arrayIndex) = CByte(z_byte)
                    Next
                    mProfile.zBytes = profileZBytes
                    'Exit For
                End If
                '亮度信息
                If GoDataMsg_Type(dataObj) = GoDataMessageTypes.GO_DATA_MESSAGE_TYPE_PROFILE_INTENSITY Then
                    'Dim profileIntensityMsg As GoProfileIntensityMsg
                    profileIntensityMsg = dataObj



                    Dim profileIntensityPointCount = GoProfileIntensityMsg_Width(profileIntensityMsg)
                    Dim intensityPoints(profileIntensityPointCount - 1) As Byte
                    Dim intensityPtr = GoProfileIntensityMsg_At(profileIntensityMsg, 0)


                    Marshal.Copy(intensityPtr, intensityPoints, 0, intensityPoints.Length)

                    mIntensityProfile.intensity = intensityPoints

                    'Dim profilePointCount = GoResampledProfileMsg_Width(profileMsg)
                    'Dim points(profilePointCount - 1) As Short
                    'Dim profileBuffer(profilePointCount - 1) As ProfilePoint
                    'Dim pointsPtr As IntPtr = GoResampledProfileMsg_At(profileMsg, 0)
                    'Marshal.Copy(pointsPtr, points, 0, points.Length)

                    Exit For
                End If


            Next
            If mProfilesBuffer.Count < mSet.bufferMaxLength Then
                mProfilesBuffer.Add(mProfile)
                'll
                mIntensityProfileBuffer.Add(mIntensityProfile)

                If mProfilesBuffer.Count Mod 1000 = 0 Then
                    Debug.Print(mProfilesBuffer.Count & ": encoder " & mProfile.encoder)
                End If
            End If

            Return kStatus.kOK
        Catch ex As Exception
            MsgBox("扫描异常")
            Return kStatus.kERROR
        Finally
            GoDestroy(data)
        End Try
        Return kStatus.kOK
    End Function

#End Region

#Region "Save/Load files"

    Private settingFile = "setting.xml"
    Friend Function LoadSetting() As Boolean
        Try
            Dim serializer As New XmlSerializer(GetType(GoSurfaceMakerSetting))
            If File.Exists(settingFile) Then
                Using file = System.IO.File.OpenRead(settingFile)
                    mSet = DirectCast(serializer.Deserialize(file), GoSurfaceMakerSetting)
                    'debug''''''''''''''''''''
                    If mSet.SupportPoints1.points.Count = 0 Then
                        mSet.SupportPoints1.points.Add(New Point(0, 0))
                    End If
                    If mSet.SupportPoints2.points.Count = 0 Then
                        mSet.SupportPoints2.points.Add(New Point(0, 0))
                    End If
                    If mSet.SupportPoints3.points.Count = 0 Then
                        mSet.SupportPoints3.points.Add(New Point(0, 0))
                    End If
                    If mSet.SupportPoints4.points.Count = 0 Then
                        mSet.SupportPoints4.points.Add(New Point(0, 0))
                    End If
                    ''''''''''''''''''''''''''
                    Return True
                End Using
            End If
            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function
    Friend Function SaveSetting() As Boolean
        Try
            Dim serializer As New XmlSerializer(GetType(GoSurfaceMakerSetting))
            If File.Exists(settingFile) Then
                File.Delete(settingFile)
            End If
            Using file As System.IO.FileStream = System.IO.File.Open(settingFile, IO.FileMode.OpenOrCreate, IO.FileAccess.Write)
                serializer.Serialize(file, mSet)
            End Using
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private mCurrentDataNameCountNb As Integer
    Friend Function LoadData(ByVal fileName As String) As Boolean
        If fileName = "" Then
            Dim fd As New OpenFileDialog
            fd.Filter = "Data file (*.dat)|*.dat"
            If fd.ShowDialog = DialogResult.OK Then
                fileName = fd.FileName
            End If
        End If
        If File.Exists(fileName) Then
            Dim fs As Stream = New FileStream(fileName, FileMode.Open)
            Dim bf As BinaryFormatter = New BinaryFormatter()
            mProfilesBuffer = CType(bf.Deserialize(fs), List(Of ProfileByte))
            fs.Close()
            Return True
        End If
        Return False
    End Function
    Friend Function SaveData(fileName As String) As Boolean 'data auto saved
        If mProfilesBuffer IsNot Nothing Then
            If My.Computer.FileSystem.FileExists(fileName) = True Then
                My.Computer.FileSystem.DeleteFile(fileName)
            End If
            Dim fs As Stream = New FileStream(fileName, FileMode.Create)
            Dim bf As BinaryFormatter = New BinaryFormatter()
            bf.Serialize(fs, mProfilesBuffer)
            fs.Close()
            Return True
        End If
        Return False
    End Function
    Private Sub SaveData()
        If mSet.DataSaveNb > 0 Then
            Dim fileName As String = String.Format("{0}\{1}\{2}_{3}.dat", Application.StartupPath, mSet.DataAutoSaveFolder, Now.Ticks, mCurrentDataNameCountNb)
            Dim folder As String = Path.GetDirectoryName(fileName)
            If Not Directory.Exists(folder) Then
                Directory.CreateDirectory(folder)
            End If
            Dim files() = Directory.GetFiles(folder)
            Dim nb = files.Count
            If files.Count >= mSet.DataSaveNb Then
                File.Delete(files(0)) 'remove one
            End If
            SaveData(fileName)
        End If
    End Sub

#End Region

#Region "Base and glue track segmentation"


#Region "Multi-Thread Segmentation"

    Private mImgArray(,) As Byte
    Private mSegRegion As SegBaseMeasurementRegion
    Private mBaseHeights() As Single
    Private Function SegBaseTrack(runID As Integer, ByRef imgArray(,) As Byte, ByRef imgIntensityArray(,) As Byte, segRegion As SegBaseMeasurementRegion) As Boolean

        Dim sw As New Stopwatch : sw.Start()

        Dim NbRegion As Integer = segRegion.NbRegions
        Dim BaseHeights(NbRegion - 1) As Single
        Dim imgLength As Integer = UBound(imgArray, 1) + 1
        Dim imgWidth As Integer = UBound(imgArray, 2) + 1


        Dim p1 As New Point(segRegion.startColumn, segRegion.startRow) 'top left point
        Dim p2 As New Point(segRegion.startColumn + segRegion.MeasurementWidth, segRegion.startRow + segRegion.MeasurementLength * segRegion.NbRegions)
        Dim Heights(NbRegion - 1) As List(Of Integer)
        For i As Integer = 0 To NbRegion - 1
            Heights(i) = New List(Of Integer)
        Next

        'make sure the region is within the image
        If p1.X < 0 Then p1.X = 0
        If p1.X > imgWidth - 1 Then p1.X = imgWidth - 1
        If p1.Y < 0 Then p1.Y = 0
        If p1.Y > imgLength - 1 Then p1.Y = imgLength - 1

        If p2.X < 0 Then p2.X = 0
        If p2.X > imgWidth - 1 Then p2.X = imgWidth - 1
        If p2.Y < 0 Then p2.Y = 0
        If p2.Y > imgLength - 1 Then p2.Y = imgLength - 1

        For i As Integer = p1.Y To p2.Y - 1
            Dim curtRegionIdx As Integer = Math.Floor((i - p1.Y) / segRegion.MeasurementLength)
            For j As Integer = p1.X To p2.X
                If imgArray(i, j) <> 0 Then
                    Heights(curtRegionIdx).Add(imgArray(i, j))
                End If
            Next
        Next
        'get the base heights as the average pixel value within its region
        For i As Integer = 0 To NbRegion - 1
            If Heights(i).Count = 0 Then
                Return False
            End If
            BaseHeights(i) = Heights(i).Average
        Next
        ''''''''''''''''''''''''''''''''''''''''''''

        'make a local copy of image array for multi-thread processiing
        ReDim mImgArray(imgLength - 1, imgWidth - 1)
        System.Buffer.BlockCopy(imgArray, 0, mImgArray, 0, imgWidth * imgLength)
        '''''''''''''''''

        'Multi-Thread: Shift up and down
        mSegRegion = segRegion
        mBaseHeights = BaseHeights

        Dim nbThread As Integer = IIf(mSet.useThreadNb > 1, mSet.useThreadNb, 1)
        Dim threads As New List(Of Thread)
        Dim imgLengthPerThread As Integer = Math.Floor(imgLength / nbThread)
        For i As Integer = 0 To nbThread - 1
            Dim idxVal As New indexval
            idxVal.IdxStart = i * imgLengthPerThread
            idxVal.IdxEnd = (i + 1) * imgLengthPerThread - 1
            Dim thread = New Threading.Thread(AddressOf applySegmentation) '大于baseHeight+ mSet.SegHeightShift为255，小于的为0
            threads.Add(thread)
            thread.Start(idxVal)
        Next
        While True
            Dim isAllFinished As Boolean = True
            For i As Integer = 0 To threads.Count - 1
                If threads(i).IsAlive Then
                    isAllFinished = False
                    Exit For
                End If
            Next
            If isAllFinished Then
                Exit While
            End If
            Thread.Sleep(10)
        End While

        'Appy masks
        If mSet.isUseSegmentationMask Then
            '原版
            'applySegmentationMaskRegion(runID, imgArray, mImgArray, mSet.SegBaseMaskRegion1)
            'applySegmentationMaskRegion(runID, imgArray, mImgArray, mSet.SegBaseMaskRegion2)
            'applySegmentationMaskRegion(runID, imgArray, mImgArray, mSet.SegBaseMaskRegion3)

            'll
            Select Case runID
                Case 1
                    applySegmentationMaskRegion(runID, imgIntensityArray, mImgArray, mSet.SegBaseMaskRegion1)
                Case 2
                    applySegmentationMaskRegion(runID, imgIntensityArray, mImgArray, mSet.SegBaseMaskRegion2)
                Case 3
                    applySegmentationMaskRegion(runID, imgIntensityArray, mImgArray, mSet.SegBaseMaskRegion3)
                Case 4
                    applySegmentationMaskRegion(runID, imgIntensityArray, mImgArray, mSet.SegBaseMaskRegion4)
                Case 5
                    applySegmentationMaskRegion(runID, imgIntensityArray, mImgArray, mSet.SegBaseMaskRegion5)
                Case 6
                    applySegmentationMaskRegion(runID, imgIntensityArray, mImgArray, mSet.SegBaseMaskRegion6)
            End Select
        End If
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        sw.Stop()
        Debug.Print("SegBaseTrack spent: " & sw.ElapsedMilliseconds)


        imgArray = mImgArray
        Return True
    End Function

    Private Sub applySegmentation(idxval As indexval)
        'appy segmentation for imgArray from startRow to endRow by segHeight
        Dim imgWidth As Integer = UBound(mImgArray, 2)
        Dim NbRegion As Integer = mBaseHeights.Count

        For i As Integer = idxval.IdxStart To idxval.IdxEnd
            Dim curRegionIdx As Integer = Math.Floor((i - mSegRegion.startRow) / mSegRegion.MeasurementLength)
            If curRegionIdx < 0 Then curRegionIdx = 0
            If curRegionIdx > NbRegion - 1 Then curRegionIdx = NbRegion - 1
            Dim segHeight As Integer = mBaseHeights(curRegionIdx) + mSet.SegHeightShift
            For j As Integer = 0 To imgWidth
                Dim tmp As Integer
                If mImgArray(i, j) > segHeight Then
                    tmp = mImgArray(i, j) + mSet.SegShiftUp
                Else
                    tmp = mImgArray(i, j) - mSet.SegShiftDown
                End If
                If tmp < 0 Then tmp = 0
                If tmp > 255 Then tmp = 255
                mImgArray(i, j) = tmp
            Next
        Next
    End Sub
    Private Structure indexval
        Public IdxStart As Integer
        Public IdxEnd As Integer
    End Structure
    Private Sub applySegmentationMaskRegion(runID As Integer, originalImage(,) As Byte, segmentedImage(,) As Byte, mask As SegBaseMaskRegion)
        If runID = mask.runID Then
            Dim startRow As Integer = mask.startRow
            Dim endRow As Integer = mask.startRow + mask.length - 1
            Dim startColumn As Integer = mask.startColumn
            Dim endColumn As Integer = mask.startColumn + mask.width - 1

            Dim imgLength As Integer = UBound(originalImage, 1) + 1
            Dim imgWidth As Integer = UBound(originalImage, 2) + 1

            'mask sure the mask region is within the image
            If endRow >= imgLength Then endRow = imgLength - 1
            If endColumn >= imgWidth Then endColumn = imgWidth - 1

            'copy pixels in MaskRegion from originalImage to segmentedImage
            For i As Integer = startRow To endRow
                For j As Integer = startColumn To endColumn
                    segmentedImage(i, j) = originalImage(i, j)
                Next
            Next
        End If
    End Sub


#End Region

#End Region

#Region "Top and Glue track segmentation"
    'similar to "Edge Smoothing" 与边缘滤波类似，只是把找边缘改为找顶点，边缘填充改为分割

    Private Sub applyTopTrackSegmentation(ByRef imgIntensityArray(,) As Byte, ByRef imageArray(,) As Byte, runID As Integer, region As SegTopTrackRegion)

        If Not region.enabled Then Exit Sub
        If Not runID = region.runID Then Exit Sub

        '1, find the top points one by one 
        Dim topPts As New List(Of Integer)
        topPts = segTopTrackTopPointsExtraction(imageArray, region) '通过画搜索区域找到最高点
        If topPts.Count = 0 Then Exit Sub

        '2, apply smoothing to the edge points
        profileSmoothingAverage(topPts, region.averagingSize, region.outlierThreshold)

        '3, apply segmentation in the region base on the heights
        Dim heights As New List(Of Integer)
        For i As Integer = 0 To topPts.Count - 1
            heights.Add(topPts(i) + mSet.SegTopTrackHeightShift) '将最高点下移12
        Next
        Dim p1 As New Point(region.startRow, region.startColumn) '搜索区域矩形的起始坐标
        Dim p2 As New Point(region.startRow + region.length - 1, region.startColumn + region.width - 1) '结束坐标
        segTopTrackFilling(imageArray, region.side, p1, p2, heights) '搜索区域内大于 最高点下移12的点变255 ，其它点变0
        ''extend up and down
        segTopTrackExtendFilling(imageArray, region.side, p1, p2, region.segmentationExtendUp, heights) '延伸区域内大于 最高点下移12的点变255 ，其它点变0
        segTopTrackExtendFilling(imageArray, region.side, p1, p2, -region.segmentationExtendDown, heights)

        'll
        If mSet.isUseSegmentationMask Then
            Select Case runID
                Case 1
                    applySegmentationMaskRegion(runID, imgIntensityArray, imageArray, mSet.SegBaseMaskRegion1)
                Case 2
                    applySegmentationMaskRegion(runID, imgIntensityArray, imageArray, mSet.SegBaseMaskRegion2)
                Case 3
                    applySegmentationMaskRegion(runID, imgIntensityArray, imageArray, mSet.SegBaseMaskRegion3)
                Case 4
                    applySegmentationMaskRegion(runID, imgIntensityArray, imageArray, mSet.SegBaseMaskRegion4)
                Case 5
                    applySegmentationMaskRegion(runID, imgIntensityArray, imageArray, mSet.SegBaseMaskRegion5)
                Case 6
                    applySegmentationMaskRegion(runID, imgIntensityArray, imageArray, mSet.SegBaseMaskRegion6)
            End Select
        End If

    End Sub

    Private Function segTopTrackTopPointsExtraction(imageArray(,) As Byte, region As SegTopTrackRegion) As List(Of Integer)
        'get the top point from the region

        Dim profile As New List(Of Integer)
        Dim side As Integer = region.side '区域的边
        Dim startRow As Integer = region.startRow
        Dim startColumn As Integer = region.startColumn
        Dim width As Integer = region.width
        Dim length As Integer = region.length

        If width > 1 And length > 1 Then
            Select Case side
                Case 1, 2
                    For i As Integer = 0 To length - 1 'for each row  画搜索区域
                        Dim maxPixel As Integer = -1
                        For j As Integer = 0 To width - 1
                            Dim curPixel As Byte = imageArray(startRow + i, startColumn + j)
                            If curPixel <> 0 And curPixel > maxPixel Then
                                maxPixel = curPixel '找到最高点
                            End If
                        Next
                        profile.Add(maxPixel)  '收集最高点
                    Next
                Case 3, 4
                    For i As Integer = 0 To width - 1 'for each column
                        Dim maxPixel As Integer = -1
                        For j As Integer = 0 To length - 1
                            Dim curPixel As Byte = imageArray(startRow + j, startColumn + i)
                            If curPixel <> 0 And curPixel > maxPixel Then
                                maxPixel = curPixel
                            End If
                        Next
                        profile.Add(maxPixel)
                    Next
            End Select
        End If
        Return profile


    End Function

    Private Sub segTopTrackFilling(ByRef imageArray(,) As Byte, side As Integer, p1 As Point, p2 As Point, heights As List(Of Integer))
        'p1: region left-top point
        'p2: region right-bottom point
        'heights: segementation heights
        'segment pixels in region(p1,p2) by heights; 根据heights里面的高度值来分割区域中的像素点

        'make sure p1,p2 is within the image
        Dim imgWidth As Integer = UBound(imageArray, 2)
        Dim imgLength As Integer = UBound(imageArray, 1)
        p1.X = valueWithinRegion(p1.X, 0, imgLength)
        p1.Y = valueWithinRegion(p1.Y, 0, imgWidth)
        p2.X = valueWithinRegion(p2.X, 0, imgLength)
        p2.Y = valueWithinRegion(p2.Y, 0, imgWidth)
        ''''''''''''''''''''''''''''''''''''''''''''''''''

        Select Case side
            Case 1, 2  '搜索区域内大于 最高点下移12的点变255 ，其它点变0
                Dim rowCnt As Integer = p2.X - p1.X + 1
                If rowCnt <> heights.Count Then Exit Sub
                For i As Integer = p1.X To p2.X
                    For j As Integer = p1.Y To p2.Y
                        Dim h As Integer = heights(i - p1.X)
                        If h <> -1 Then
                            If imageArray(i, j) > h Then
                                Dim val As Integer = valueWithinRegion(imageArray(i, j) + mSet.SegShiftUp, 0, 255)
                                imageArray(i, j) = val
                            Else
                                Dim val As Integer = valueWithinRegion(imageArray(i, j) - mSet.SegShiftDown, 0, 255)
                                imageArray(i, j) = val
                            End If
                        Else 'invalid height
                            imageArray(i, j) = 0 'make the row invalid
                        End If
                    Next
                Next
            Case 3, 4
                Dim colCnt As Integer = p2.Y - p1.Y + 1
                If colCnt <> heights.Count Then Exit Sub
                For i As Integer = p1.Y To p2.Y
                    For j As Integer = p1.X To p2.X
                        Dim h As Integer = heights(i - p1.Y)
                        If h <> -1 Then
                            If imageArray(j, i) > h Then
                                Dim val As Integer = valueWithinRegion(imageArray(j, i) + mSet.SegShiftUp, 0, 255)
                                imageArray(j, i) = val
                            Else
                                Dim val As Integer = valueWithinRegion(imageArray(j, i) - mSet.SegShiftDown, 0, 255)
                                imageArray(j, i) = val
                            End If
                        Else 'invalid height
                            imageArray(j, i) = 0 'make the column invalid
                        End If
                    Next
                Next
        End Select

    End Sub

    Private Sub segTopTrackExtendFilling(ByRef imageArray(,) As Byte, side As Integer, p1 As Point, p2 As Point, extend As Integer, heights As List(Of Integer))
        'extend > 0 => extend up
        'extend < 0 => extend down
        If extend = 0 Then Exit Sub
        Dim q1, q2 As New Point
        Dim cnt As Integer = Math.Abs(extend)
        Dim hts As New List(Of Integer)

        If extend > 0 Then ' extend up
            For i As Integer = 0 To cnt - 1 '用最上面的高点进行分割
                hts.Add(heights(0))
            Next
            Select Case side
                Case 1, 2
                    q1 = New Point(p1.X - cnt, p1.Y) 'extend region
                    q2 = New Point(p1.X - 1, p2.Y)
                Case 3, 4
                    q1 = New Point(p1.X, p1.Y - cnt)
                    q2 = New Point(p2.X, p1.Y - 1)
            End Select
        Else 'extend down
            For i As Integer = 0 To cnt - 1 '用最下面的高点进行分割
                hts.Add(heights(heights.Count - 1))
            Next
            Select Case side
                Case 1, 2
                    q1 = New Point(p2.X + 1, p1.Y) 'extend region
                    q2 = New Point(p2.X + cnt, p2.Y)
                Case 3, 4
                    q1 = New Point(p1.X, p2.Y + 1)
                    q2 = New Point(p2.X, p2.Y + cnt)
            End Select
        End If

        segTopTrackFilling(imageArray, side, q1, q2, hts)


    End Sub

#End Region

#Region "Edge Smoothing"

    Private Sub applyEdgeSmoothing(ByRef imageArray(,) As Byte, runID As Integer, region As EdgeSmoothingRegion)

        If Not region.enabled Then Exit Sub
        If Not runID = region.runID Then Exit Sub

        '1, find the edge points one by one 
        Dim edgePts As New List(Of Point)
        edgePts = edgeExtraction(imageArray, region) '一条轮廓中，后一个点比前一个点高100,此处为边缘
        If edgePts.Count = 0 Then Exit Sub

        '2, apply smoothing to the edge points
        Dim profile As New List(Of Integer)
        Select Case region.side
            Case 1, 2
                'get profile in column
                For i As Integer = 0 To edgePts.Count - 1
                    profile.Add(edgePts(i).Y)
                Next

                '对于每一个点p，取它周围n=size个点[p-n,p+n](如果是无效点，则不计)，计算平均值作为该点的值
                profileSmoothingAverage(profile, region.smoothingSize, region.outlierThreshold)

                'update the edge value from smoothing  将平滑后的点应用在edgePts上
                For i As Integer = 0 To edgePts.Count - 1
                    edgePts(i) = New Point(edgePts(i).X, profile(i))
                Next
            Case 3, 4
                'get profile in row
                For i As Integer = 0 To edgePts.Count - 1
                    profile.Add(edgePts(i).X)
                Next
                profileSmoothingAverage(profile, region.smoothingSize, region.outlierThreshold)
                'update the edge value from smoothing
                For i As Integer = 0 To edgePts.Count - 1
                    edgePts(i) = New Point(profile(i), edgePts(i).Y)
                Next
        End Select

        '3, filling the image with edge points within its filling width (0 on left, 255 on right)
        profileSmoothingFilling(imageArray, region.side, edgePts, region.edgeFillingWidth)

    End Sub

    Private Function edgeExtraction(imageArray(,) As Byte, region As EdgeSmoothingRegion) As List(Of Point)

        Dim profile As New List(Of Point)
        Dim side As Integer = region.side
        Dim startRow As Integer = region.startRow
        Dim startColumn As Integer = region.startColumn
        Dim width As Integer = region.width
        Dim length As Integer = region.length

        If width > 1 And length > 1 Then
            Select Case side
                Case 1 'left
                    startColumn += width 'change the start point to right-top
                    For i As Integer = 0 To length - 1 'for each row    一条轮廓中，后一个点比前一个点高100,此处为边缘
                        Dim isEdgePtFound As Boolean
                        Dim lastPixel As Byte = imageArray(startRow + i, startColumn - 0)
                        For j As Integer = 1 To width - 1
                            Dim curtPixel As Byte = imageArray(startRow + i, startColumn - j)
                            Dim diff As Integer = CInt(curtPixel) - CInt(lastPixel)
                            If diff > region.edgeStepThreshold Then
                                profile.Add(New Point(startRow + i, startColumn - j))
                                isEdgePtFound = True
                                Exit For
                            Else
                                lastPixel = curtPixel
                            End If
                        Next
                        If Not isEdgePtFound Then
                            profile.Add(New Point(startRow + i, -1)) 'invalid edge points
                        End If
                    Next
                Case 2 'right
                    For i As Integer = 0 To length - 1 'for each row
                        Dim isEdgePtFound As Boolean
                        Dim lastPixel As Byte = imageArray(startRow + i, startColumn + 0)
                        For j As Integer = 1 To width - 1
                            Dim curtPixel As Byte = imageArray(startRow + i, startColumn + j)
                            Dim diff As Integer = CInt(curtPixel) - CInt(lastPixel)
                            If diff > region.edgeStepThreshold Then
                                profile.Add(New Point(startRow + i, startColumn + j))
                                isEdgePtFound = True
                                Exit For
                            Else
                                lastPixel = curtPixel
                            End If
                        Next
                        If Not isEdgePtFound Then
                            profile.Add(New Point(startRow + i, -1)) 'invalid edge points
                        End If
                    Next
                Case 3 'top
                    startRow += length 'change the start point to left-bottom
                    For i As Integer = 0 To width - 1 'for each column
                        Dim isEdgePtFound As Boolean
                        Dim lastPixel As Byte = imageArray(startRow - 0, startColumn + i)
                        For j As Integer = 1 To length - 1
                            Dim curtPixel As Byte = imageArray(startRow - j, startColumn + i)
                            Dim diff As Integer = CInt(curtPixel) - CInt(lastPixel)
                            If diff > region.edgeStepThreshold Then
                                profile.Add(New Point(startRow - j, startColumn + i))
                                isEdgePtFound = True
                                Exit For
                            Else
                                lastPixel = curtPixel
                            End If
                        Next
                        If Not isEdgePtFound Then
                            profile.Add(New Point(-1, startColumn + i)) 'invalid edge points
                        End If
                    Next
                Case 4 'bottom
                    For i As Integer = 0 To width - 1 'for each column
                        Dim isEdgePtFound As Boolean
                        Dim lastPixel As Byte = imageArray(startRow + 0, startColumn + i)
                        For j As Integer = 1 To length - 1
                            Dim curtPixel As Byte = imageArray(startRow + j, startColumn + i)
                            Dim diff As Integer = CInt(curtPixel) - CInt(lastPixel)
                            If diff > region.edgeStepThreshold Then
                                profile.Add(New Point(startRow + j, startColumn + i))
                                isEdgePtFound = True
                                Exit For
                            Else
                                lastPixel = curtPixel
                            End If
                        Next
                        If Not isEdgePtFound Then
                            profile.Add(New Point(-1, startColumn + i)) 'invalid edge points
                        End If
                    Next
            End Select

        End If

        Return profile


    End Function

    Private Sub profileSmoothingAverage(ByRef profile As List(Of Integer), size As Integer, outlierThreshold As Integer)
        '对于每一个点p，取它周围n=size个点[p-n,p+n](如果是无效点，则不计)，计算平均值作为该点的值

        Dim n As Integer = profile.Count
        Dim regionSize As Integer = 2 * size + 1 ’取点个数



        Dim smoothRegionQueue As New Queue(Of Integer) 'queue of current region profile points for P
        Dim smoothRegionSum As New Double 'sum of valid current region profile points
        Dim smoothRegionCnt As New Integer 'nb of valid pts
        Dim smoothProfile As New List(Of Integer) 'smoothed profile

        '点数 < 平均区域单边长度；计算平均值并赋值给所有的点 
        profileSmoothingRemoveProfileOutlier(profile, outlierThreshold) '去除离均值超过outlierThreashold的点，如果outlierThreashold=0，不执行
        If n <= size + 1 Then
            For i As Integer = 0 To n - 1
                If profile(i) <> -1 Then 'not invalid
                    smoothRegionSum += profile(i)
                    smoothRegionCnt += 1
                End If
            Next
            If smoothRegionCnt > 0 Then
                Dim ave As Integer = CInt(smoothRegionSum / smoothRegionCnt)
                For i As Integer = 0 To n - 1
                    profile(i) = ave
                Next
            End If
            Exit Sub
        End If

        '点数 > 平均区域单边长度
        'fill the queue with data [0,size-1]
        'remove outlier before enque
        Dim profileNew = profile.GetRange(0, size) '将profile 0-200的值赋给profileNew
        profileSmoothingRemoveProfileOutlier(profileNew, outlierThreshold)

        For i As Integer = 0 To size - 1
            smoothRegionQueue.Enqueue(profileNew(i))
            If profile(i) <> -1 Then 'not invalid
                smoothRegionSum += profile(i)   'profileNew!=-1时将值 累加在smoothRegionsum
                smoothRegionCnt += 1
            End If
        Next
        'update queue and the profile points
        For i As Integer = 0 To n - 1
            Dim lastIdx As Integer = i + size
            If lastIdx < n Then
                Dim dataIn As Integer = profile(lastIdx)
                Dim dataOut As Integer = -1

                'remove outlier for dataIn > outlierThreshold
                If outlierThreshold > 0 Then
                    Dim curAve As Double = smoothRegionSum / smoothRegionCnt
                    If Math.Abs(dataIn - curAve) > outlierThreshold Then
                        dataIn = -1
                    End If
                End If
                '''''''''''''''''''''''''''''''''''''''''''''
                smoothRegionQueue.Enqueue(dataIn)
                If smoothRegionQueue.Count > regionSize Then
                    dataOut = smoothRegionQueue.Dequeue
                End If
                If dataIn <> -1 Then 'not invalid
                    smoothRegionSum += dataIn
                    smoothRegionCnt += 1
                End If
                If dataOut <> -1 Then 'not invalid
                    smoothRegionSum -= dataOut
                    smoothRegionCnt -= 1
                End If
            Else '在轮廓的尾部，Only Dequeue
                Dim dataOut = smoothRegionQueue.Dequeue
                If dataOut <> -1 Then 'not invalid
                    smoothRegionSum -= dataOut
                    smoothRegionCnt -= 1
                End If
            End If
            If smoothRegionCnt > 0 Then
                '当前点的值为平均区域[p-n,p+n]的均值
                smoothProfile.Add(CInt(smoothRegionSum / smoothRegionCnt))
            Else
                smoothProfile.Add(-1)
            End If
        Next

        profile = smoothProfile

    End Sub

    Private Sub profileSmoothingRemoveProfileOutlier(ByRef profile As List(Of Integer), outlierThreshold As Integer)
        'profile取均值，去除所有离均值超过outlierThreshold 的点（设置值为-1）
        If outlierThreshold > 0 Then
            Dim sum As Double
            Dim cnt As Integer
            Dim ave As Double

            For i As Integer = 0 To profile.Count - 1
                If profile(i) <> -1 Then
                    sum += profile(i)
                    cnt += 1
                End If
            Next

            If cnt = 0 Then Exit Sub
            ave = sum / cnt

            For i As Integer = 0 To profile.Count - 1
                If profile(i) <> -1 And Math.Abs(profile(i) - ave) > outlierThreshold Then
                    profile(i) = -1
                End If
            Next
        End If
    End Sub

    Private Sub profileSmoothingFilling(ByRef imageArray(,) As Byte, side As Integer, profile As List(Of Point), fillingWidth As Integer)

        If fillingWidth > 0 Then

            Dim imageWidth As Integer = UBound(imageArray, 2)
            Dim imageLength As Integer = UBound(imageArray, 1)

            For i As Integer = 0 To profile.Count - 1
                If profile(i).X <> -1 And profile(i).Y <> -1 Then 'not invalid
                    imageArray(profile(i).X, profile(i).Y) = 255 'profile itself
                End If
                Select Case side
                    Case 1 'left
                        If profile(i).X <> -1 And profile(i).Y <> -1 Then 'not invalid
                            For j As Integer = 1 To fillingWidth
                                imageArray(profile(i).X, profile(i).Y - j) = 255
                                imageArray(profile(i).X, profile(i).Y + j) = 0
                            Next
                        End If
                    Case 2 'right
                        If profile(i).X <> -1 And profile(i).Y <> -1 Then 'not invalid
                            For j As Integer = 1 To fillingWidth
                                imageArray(profile(i).X, profile(i).Y - j) = 0
                                imageArray(profile(i).X, profile(i).Y + j) = 255
                            Next
                        End If
                    Case 3 'top
                        If profile(i).X <> -1 And profile(i).Y <> -1 Then 'not invalid
                            For j As Integer = 1 To fillingWidth
                                imageArray(profile(i).X - j, profile(i).Y) = 255
                                imageArray(profile(i).X + j, profile(i).Y) = 0
                            Next
                        End If
                    Case 4 'bottom
                        If profile(i).X <> -1 And profile(i).Y <> -1 Then 'not invalid
                            For j As Integer = 1 To fillingWidth
                                imageArray(profile(i).X - j, profile(i).Y) = 0
                                imageArray(profile(i).X + j, profile(i).Y) = 255
                            Next
                        End If
                End Select
            Next
        End If
    End Sub

#End Region

#Region "Support Points Offset" 'NOT USED
    Private Sub applySupportPointsOffset(ByRef imageArray(,) As Byte, support As SupportPoints)
        'only consider for left and right now
        Dim imageWidth As Integer = UBound(imageArray, 2)
        Dim imageLength As Integer = UBound(imageArray, 1)
        Dim points = support.points
        For i As Integer = 0 To points.Count - 1
            Dim region As New EdgeSmoothingRegion
            With region
                .enabled = True
                .runID = support.runID
                .side = support.side
                .startColumn = valueWithinRegion(points(i).X - CInt(support.searchWidth / 2), 0, imageWidth)
                .startRow = valueWithinRegion(points(i).Y - CInt(support.length / 2), 0, imageLength)
                .width = support.searchWidth
                .length = support.length
                .edgeStepThreshold = support.edgeStepThreshold
            End With
            'find the edge points one by one
            Dim edgePts As New List(Of Point)
            edgePts = edgeExtraction(imageArray, region)
            'filling the image with edge points within its filling width (55 on right)
            supportPointsFilling(imageArray, support.side, edgePts, support.offset)
        Next


    End Sub

    Private Sub supportPointsFilling(ByRef imageArray(,) As Byte, side As Integer, profile As List(Of Point), fillingWidth As Integer)

        If fillingWidth > 0 Then

            Dim imageWidth As Integer = UBound(imageArray, 2)
            Dim imageLength As Integer = UBound(imageArray, 1)

            For i As Integer = 0 To profile.Count - 1
                If profile(i).X <> -1 And profile(i).Y <> -1 Then 'not invalid
                    imageArray(profile(i).X, profile(i).Y) = 255 'profile itself
                End If
                Select Case side
                    Case 1 'left
                        If profile(i).X <> -1 And profile(i).Y <> -1 Then 'not invalid
                            For j As Integer = 1 To fillingWidth
                                imageArray(profile(i).X, profile(i).Y + j) = 255
                            Next
                        End If
                    Case 2 'right
                        If profile(i).X <> -1 And profile(i).Y <> -1 Then 'not invalid
                            For j As Integer = 1 To fillingWidth
                                imageArray(profile(i).X, profile(i).Y - j) = 255
                            Next
                        End If
                End Select
            Next
        End If
    End Sub

    Private Function valueWithinRegion(ByVal val As Integer, lowBound As Integer, highBound As Integer) As Integer
        'make sure the value is within [lowBound,highBound]
        Dim valNew As Integer = val  ’val=p1.x  0 ,width
        If valNew < lowBound Then
            valNew = lowBound
        ElseIf valNew > highBound Then
            valNew = highBound
        End If
        Return valNew
    End Function
#End Region 'NOT USED

End Class
