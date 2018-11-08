Imports System.Runtime.InteropServices
Module GoSdkWrapper
    Public Class Constants
        Public Const GODLLPATH As String = "GoSdk.dll"
        Public Const KAPIDLLPATH As String = "kApi.dll"
    End Class
    Public Structure address
        Public version As Char
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=16)> _
        Public IPaddress() As Byte
    End Structure

    Public Structure GoMeasurementData
        Public value As Double
        Public decision As SByte
    End Structure

    Public Structure GoStamp
        Public frameIndex As UInt64
        Public timestamp As UInt64
        Public encoder As Int64
        Public encoderAtZ As Int64
        Public reserved As UInt64
    End Structure

    Public Structure GoPoints
        Public x As Int16
        Public y As Int16
    End Structure

    Public Structure ProfilePoints
        Public x As Double
        Public z As Double
        Public intensity As Byte
    End Structure

    Public Structure GoProfile
        Public points() As ProfilePoints
        Public encoder As Long
        Public timestamp As ULong
    End Structure



    Public Class DataContext
        Public xResolution As Double
        Public zResolution As Double
        Public xOffset As Double
        Public zOffset As Double
        Public serialNumber As UInteger
    End Class

    Enum kStatus
        kERROR_STATE = -1000                                                ' Invalid state.
        kERROR_NOT_FOUND = -999                                             ' Item is not found.
        kERROR_COMMAND = -998                                               ' Command not recognized.
        kERROR_PARAMETER = -997                                             ' Parameter is invalid.
        kERROR_UNIMPLEMENTED = -996                                         ' Feature not implemented.
        kERROR_HANDLE = -995                                                ' Handle is invalid.
        kERROR_MEMORY = -994                                                ' Out of memory.
        kERROR_TIMEOUT = -993                                               ' Action timed out.
        kERROR_INCOMPLETE = -992                                            ' Buffer not large enough for data.
        kERROR_STREAM = -991                                                ' Error in stream.
        kERROR_CLOSED = -990                                                ' Resource is no longer avaiable. 
        kERROR_VERSION = -989                                               ' Invalid version number.
        kERROR_ABORT = -988                                                 ' Operation aborted.
        kERROR_ALREADY_EXISTS = -987                                        ' Conflicts with existing item.
        kERROR_NETWORK = -986                                               ' Network setup/resource error.
        kERROR_HEAP = -985                                                  ' Heap error (leak/double-free).
        kERROR_FORMAT = -984                                                ' Data parsing/formatting error. 
        kERROR_READ_ONLY = -983                                             ' Object is read-only (cannot be written).
        kERROR_WRITE_ONLY = -982                                            ' Object is write-only (cannot be read). 
        kERROR_BUSY = -981                                                  ' Agent is busy (cannot service request).
        kERROR_CONFLICT = -980                                              ' State conflicts with another object.
        kERROR_OS = -979                                                    ' Generic error reported by underlying OS.
        kERROR_DEVICE = -978                                                ' Hardware device error.
        kERROR_FULL = -977                                                  ' Resource is already fully utilized.
        kERROR_IN_PROGRESS = -976                                           ' Operation is in progress, but not yet complete.
        kERROR = 0                                                          ' General error. 
        kOK = 1                                                             ' Operation successful. 
    End Enum

    Enum GoDataMessageTypes
        GO_DATA_MESSAGE_TYPE_UNKNOWN = -1
        GO_DATA_MESSAGE_TYPE_STAMP = 0
        GO_DATA_MESSAGE_TYPE_HEALTH = 1
        GO_DATA_MESSAGE_TYPE_VIDEO = 2
        GO_DATA_MESSAGE_TYPE_RANGE = 3
        GO_DATA_MESSAGE_TYPE_RANGE_INTENSITY = 4
        GO_DATA_MESSAGE_TYPE_PROFILE = 5
        GO_DATA_MESSAGE_TYPE_PROFILE_INTENSITY = 6
        GO_DATA_MESSAGE_TYPE_RESAMPLED_PROFILE = 7
        GO_DATA_MESSAGE_TYPE_SURFACE = 8
        GO_DATA_MESSAGE_TYPE_SURFACE_INTENSITY = 9
        GO_DATA_MESSAGE_TYPE_MEASUREMENT = 10
        GO_DATA_MESSAGE_TYPE_ALIGNMENT = 11
        GO_DATA_MESSAGE_TYPE_EXPOSURE_CAL = 12
    End Enum

    Enum GoToolType
        GO_TOOL_UNKNOWN = -1
        GO_TOOL_RANGE_POSITION = 0
        GO_TOOL_RANGE_THICKNESS = 1
        GO_TOOL_PROFILE_AREA = 2
        GO_TOOL_PROFILE_CIRCLE = 3
        GO_TOOL_PROFILE_DIMENSION = 4
        GO_TOOL_PROFILE_GROOVE = 5
        GO_TOOL_PROFILE_INTERSECT = 6
        GO_TOOL_PROFILE_LINE = 7
        GO_TOOL_PROFILE_PANEL = 8
        GO_TOOL_PROFILE_POSITION = 9
        GO_TOOL_PROFILE_STRIP = 10
        GO_TOOL_SURFACE_BOUNDING_BOX = 11
        GO_TOOL_SURFACE_ELLIPSE = 12
        GO_TOOL_SURFACE_HOLE = 13
        GO_TOOL_SURFACE_OPENING = 14
        GO_TOOL_SURFACE_PLANE = 15
        GO_TOOL_SURFACE_POSITION = 16
        GO_TOOL_SURFACE_STUD = 17
        GO_TOOL_SURFACE_VOLUME = 18
        GO_TOOL_SCRIPT = 19
    End Enum

    Enum GoRole
        GO_ROLE_MAIN = 0                                                    ' Sensor is operating as a main sensor
        GO_ROLE_BUDDY = 1                                                   ' Sensor is operating as a buddy sensor
    End Enum

    Enum GoTrigger
        GO_TRIGGER_TIME
        GO_TRIGGER_ENCODER
        GO_TRIGGER_INPUT
        GO_TRIGGER_SOFTWARE
    End Enum


    Delegate Function onDataType(ByVal ctx As DataContext, ByVal sys As IntPtr, ByVal data As IntPtr) As Integer

    ' use DLL import to access GoSdkd.dll
    ' Note: to import the release version of the GoSdk and kApi, use GoSdk.dll and kApi.dll. GoSdkd.dll and kApid.dll are debug versions of the DLL
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSdk_Construct(ByRef assembly As IntPtr) As kStatus
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSystem_Construct(ByRef system As IntPtr, ByVal allocator As IntPtr) As kStatus
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSystem_FindSensorByIpAddress(ByVal system As IntPtr, ByVal addressPointer As IntPtr, ByRef sensor As IntPtr) As kStatus
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSystem_EnableData(ByVal system As IntPtr, ByVal enable As Boolean) As kStatus
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSystem_Connect(ByVal system As IntPtr) As kStatus
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSensor_Connect(ByVal sensor As IntPtr) As kStatus
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSystem_ReceiveData(ByVal system As IntPtr, ByRef data As IntPtr, ByVal timeout As UInt64) As kStatus
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSystem_SetDataHandler(ByVal system As IntPtr, ByVal callback As onDataType, ByVal context As DataContext) As kStatus
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSystem_Start(ByVal system As IntPtr) As kStatus
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSystem_Stop(ByVal system As IntPtr) As kStatus
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoDataSet_Count(ByVal dataset As IntPtr) As UInt32
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoDataSet_At(ByVal dataset As IntPtr, ByVal index As UInt32) As IntPtr
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoStampMsg_Count(ByVal msg As IntPtr) As UInt32
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoStampMsg_At(ByVal msg As IntPtr, ByVal index As UInt32) As IntPtr
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoProfileMsg_Count(ByVal msg As IntPtr) As UInt32
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoProfileMsg_At(ByVal msg As IntPtr, ByVal index As UInt32) As IntPtr
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoProfileMsg_Width(ByVal msg As IntPtr) As UInt32
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoProfileMsg_XResolution(ByVal msg As IntPtr) As UInt32
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoProfileMsg_ZResolution(ByVal msg As IntPtr) As UInt32
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoProfileMsg_XOffset(ByVal msg As IntPtr) As Int32
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoProfileMsg_ZOffset(ByVal msg As IntPtr) As Int32
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoResampledProfileMsg_At(ByVal msg As IntPtr, ByVal index As UInt32) As IntPtr
    End Function
    <DllImport(Constants.GODLLPATH)>
    Public Function GoResampledProfileMsg_Width(ByVal msg As IntPtr) As UInt32
    End Function

    'll
    <DllImport(Constants.GODLLPATH)>
    Public Function GoProfileIntensityMsg_At(ByVal msg As IntPtr, ByVal index As UInt32) As IntPtr
    End Function
    <DllImport(Constants.GODLLPATH)>
    Public Function GoProfileIntensityMsg_Width(ByVal msg As IntPtr) As UInt32
    End Function

    <DllImport(Constants.GODLLPATH)> _
    Public Function GoResampledProfileMsg_XResolution(ByVal msg As IntPtr) As UInt32
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoResampledProfileMsg_ZResolution(ByVal msg As IntPtr) As UInt32
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoResampledProfileMsg_XOffset(ByVal msg As IntPtr) As Int32
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoResampledProfileMsg_ZOffset(ByVal msg As IntPtr) As Int32
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSensor_Flush(ByVal sensor As IntPtr) As kStatus
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSensor_Setup(ByVal sensor As IntPtr) As IntPtr
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSensor_Tools(ByVal sensor As IntPtr) As IntPtr
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoTools_AddTool(ByVal sensor As IntPtr, ByVal type As GoToolType) As IntPtr
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoProfilePosition_ZMeasurement(ByVal tool As IntPtr) As IntPtr
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoMeasurement_Enable(ByVal measurement As IntPtr, ByVal enable As Boolean) As Int32
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoMeasurement_SetId(ByVal measurement As IntPtr, ByVal id As UInt32) As Int32
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoProfilePosition_Feature(ByVal tool As IntPtr) As IntPtr
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoProfileFeature_SetType(ByVal feature As IntPtr, ByVal featuretype As Int32) As Int32
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoProfileFeature_Region(ByVal feature As IntPtr) As IntPtr
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoProfileRegion_SetX(ByVal region As IntPtr, ByVal x As Double) As Int32
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoProfileRegion_SetZ(ByVal region As IntPtr, ByVal z As Double) As Int32
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoProfileRegion_SetHeight(ByVal region As IntPtr, ByVal height As Double) As Int32
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoProfileRegion_SetWidth(ByVal region As IntPtr, ByVal width As Double) As Int32
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSetup_TransformedDataRegionX(ByVal setup As IntPtr, ByVal role As GoRole) As Double
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSetup_TransformedDataRegionZ(ByVal setup As IntPtr, ByVal role As GoRole) As Double
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSetup_TransformedDataRegionHeight(ByVal setup As IntPtr, ByVal role As GoRole) As Double
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSetup_SetEncoderSpacing(ByVal setup As IntPtr, ByVal period As Double) As kStatus
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSetup_SetTriggerSource(ByVal setup As IntPtr, ByVal source As GoTrigger) As kStatus
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSetup_TransformedDataRegionWidth(ByVal setup As IntPtr, ByVal role As GoRole) As Double
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSetup_SetExposure(ByVal setup As IntPtr, ByVal role As GoRole, ByVal exposure As Double) As kStatus
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSensor_Output(ByVal sensor As IntPtr) As IntPtr
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoOutput_Ethernet(ByVal output As IntPtr) As IntPtr
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoEthernet_ClearAllSources(ByVal ethernet As IntPtr) As Int32
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoEthernet_AddSource(ByVal ethernet As IntPtr, ByVal GoOutputSource As Int32, ByVal sourceId As UInt32) As Int32
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoDataMsg_Type(ByVal msg As IntPtr) As GoDataMessageTypes
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoMeasurementMsg_Count(ByVal msg As IntPtr) As UInt32
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoMeasurementMsg_Id(ByVal msg As IntPtr) As UInt16
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoMeasurementMsg_At(ByVal msg As IntPtr, ByVal index As UInt32) As IntPtr
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoDestroy(ByVal obj As IntPtr) As kStatus
    End Function
    <DllImport(Constants.KAPIDLLPATH)> _
    Public Function kIpAddress_Parse(ByVal addressPointer As IntPtr, ByVal text As String) As kStatus
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSensor_UploadFile(ByVal sensor As IntPtr, ByVal sourcePath As String, ByVal destName As String) As kStatus
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSensor_DownloadFile(ByVal sensor As IntPtr, ByVal sourceName As String, ByVal destPath As String) As kStatus
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSensor_PlaybackSeek(ByVal sensor As IntPtr, ByVal position As Integer) As kStatus
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoSensor_PlaybackPosition(ByVal sensor As IntPtr, ByRef position As Integer, ByRef count As Integer) As kStatus
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoTools_ToolCount(ByVal tools As IntPtr) As UInt32
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoTools_ClearTools(ByVal tools As IntPtr) As kStatus
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoTools_ToolAt(ByVal tools As IntPtr, ByVal index As Integer) As IntPtr
    End Function
    <DllImport(Constants.GODLLPATH)> _
    Public Function GoTool_Name(ByVal tool As IntPtr, ByRef name As String, ByVal capacity As Integer) As kStatus
    End Function
End Module
