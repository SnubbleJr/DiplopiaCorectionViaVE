<?xml version="1.0" encoding="UTF-8"?>
<MiddleVR>
	<Kernel LogLevel="2" LogInSimulationFolder="0" EnableCrashHandler="0" Version="1.6.1.f1" />
	<DeviceManager>
		<Driver Type="vrDriverDirectInput" />
		<Driver Type="vrDriverVRPN">
			<Tracker Address="DTrack@128.16.14.45" ChannelIndex="0" ChannelsNb="3" Name="VRPNTracker0" Right="X" Front="Y" Up="Z" NeutralPosition="0.000000,0.000000,0.000000" Scale="1" WaitForData="0" />
			<Axis Address="DTrack@128.16.14.45" ChannelIndex="0" ChannelsNb="2" Name="VRPNAxis0" />
			<Buttons Address="DTrack@128.16.14.45" ChannelIndex="0" ChannelsNb="6" Name="VRPNButtons0" />
		</Driver>
		<Wand Name="Wand0" Driver="0" Axis="0" HorizontalAxis="0" HorizontalAxisScale="1" VerticalAxis="1" VerticalAxisScale="1" AxisDeadZone="0.3" Buttons="VRPNButtons0.Buttons" Button0="0" Button1="1" Button2="2" Button3="3" Button4="4" Button5="5" />
	</DeviceManager>
	<DisplayManager Fullscreen="0" AlwaysOnTop="1" WindowBorders="0" ShowMouseCursor="0" VSync="0" GraphicsRenderer="1" AntiAliasing="0" ForceHideTaskbar="0" SaveRenderTarget="0" ChangeWorldScale="0" WorldScale="1">
		<Node3D Name="Offset" Parent="None" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="-0.100000,-0.000000,0.000000" OrientationLocal="0.000000,0.000000,0.000000,1.000000" />
		<Node3D Name="Screens" Parent="Offset" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="0.100000,0.000000,1.100000" OrientationLocal="0.000000,0.000000,0.000000,1.000000" />
		<Screen Name="FrontScreen" Parent="Screens" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="0.000000,1.480000,0.000000" OrientationLocal="0.000000,0.000000,0.000000,1.000000" Width="2.98" Height="2.2" />
		<Screen Name="LeftScreen" Parent="Screens" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="-1.490000,0.000000,0.000000" OrientationLocal="0.000000,0.000000,-0.707107,0.707107" Width="2.96" Height="2.2" />
		<Screen Name="RightScreen" Parent="Screens" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="1.490000,0.000000,0.000000" OrientationLocal="0.000000,0.000000,0.707107,0.707107" Width="2.96" Height="2.2" />
		<Screen Name="FloorScreen" Parent="Screens" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="0.000000,0.050000,-1.100000" OrientationLocal="0.000000,0.707107,-0.707107,0.000000" Width="2.98" Height="2.86" />
		<Node3D Name="HeadNode" Tag="Head" Parent="Offset" Tracker="VRPNTracker0.Tracker0" IsFiltered="0" Filter="0" UseTrackerX="1" UseTrackerY="1" UseTrackerZ="1" UseTrackerYaw="1" UseTrackerPitch="1" UseTrackerRoll="1" />
		<Node3D Name="HeadOffset" Parent="HeadNode" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="0.000000,0.000000,0.000000" OrientationLocal="0.500000,0.500000,0.500000,0.500000" />
		<CameraStereo Name="FloorCameraStereo" Parent="HeadOffset" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="0.000000,0.000000,0.000000" VerticalFOV="60" Near="0.1" Far="1000" Screen="FloorScreen" ScreenDistance="1" UseViewportAspectRatio="0" AspectRatio="1.04196" InterEyeDistance="0.063" LinkConvergence="1" />
		<CameraStereo Name="FrontCameraStereo" Parent="HeadOffset" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="0.000000,0.000000,0.000000" VerticalFOV="60" Near="0.1" Far="1000" Screen="FrontScreen" ScreenDistance="1" UseViewportAspectRatio="0" AspectRatio="1.35455" InterEyeDistance="0.063" LinkConvergence="1" />
		<CameraStereo Name="LeftCameraStereo" Parent="HeadOffset" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="0.000000,0.000000,0.000000" VerticalFOV="60" Near="0.1" Far="1000" Screen="LeftScreen" ScreenDistance="1" UseViewportAspectRatio="0" AspectRatio="1.34545" InterEyeDistance="0.063" LinkConvergence="1" />
		<CameraStereo Name="RightCameraStereo" Parent="HeadOffset" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="0.000000,0.000000,0.000000" VerticalFOV="60" Near="0.1" Far="1000" Screen="RightScreen" ScreenDistance="1" UseViewportAspectRatio="0" AspectRatio="1.34545" InterEyeDistance="0.063" LinkConvergence="1" />
		<Node3D Name="Buttons" Parent="Offset" Tracker="VRPNTracker0.Tracker1" IsFiltered="0" Filter="0" UseTrackerX="1" UseTrackerY="1" UseTrackerZ="1" UseTrackerYaw="1" UseTrackerPitch="1" UseTrackerRoll="1" />
		<Node3D Name="HandRotOffset" Parent="Offset" Tracker="VRPNTracker0.Tracker1" IsFiltered="0" Filter="0" UseTrackerX="1" UseTrackerY="1" UseTrackerZ="1" UseTrackerYaw="1" UseTrackerPitch="1" UseTrackerRoll="1" />
		<Node3D Name="HandNode" Tag="Hand" Parent="HandRotOffset" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="0.000000,0.000000,0.000000" OrientationLocal="0.707107,0.000000,0.000000,0.707107" />
		<Node3D Name="VRSystemCenterNode" Tag="VRSystemCenter" Parent="None" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="0.000000,0.000000,0.000000" OrientationLocal="0.000000,0.000000,0.000000,1.000000" />
		<Node3D Name="CenterNode" Parent="VRSystemCenterNode" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="0.000000,0.000000,0.000000" OrientationLocal="0.000000,0.000000,0.000000,1.000000" />
		<Viewport Name="FrontViewport" Left="1400" Top="0" Width="1400" Height="1050" Camera="FrontCameraStereo" Stereo="1" StereoMode="0" CompressSideBySide="0" StereoInvertEyes="1" OculusRiftWarping="0" UseHomography="0" />
		<Viewport Name="LeftViewport" Left="0" Top="0" Width="1400" Height="1050" Camera="LeftCameraStereo" Stereo="1" StereoMode="0" CompressSideBySide="0" StereoInvertEyes="1" OculusRiftWarping="0" UseHomography="0" />
		<Viewport Name="RightViewport" Left="2800" Top="0" Width="1400" Height="1050" Camera="RightCameraStereo" Stereo="1" StereoMode="0" CompressSideBySide="0" StereoInvertEyes="1" OculusRiftWarping="0" UseHomography="0" />
		<Viewport Name="FloorViewport" Left="4200" Top="0" Width="1100" Height="1050" Camera="FloorCameraStereo" Stereo="1" StereoMode="0" CompressSideBySide="0" StereoInvertEyes="1" OculusRiftWarping="0" UseHomography="0" />
	</DisplayManager>
	<Scripts>
		<Script Type="TrackerSimulatorMouse" Name="TrackerSimulatorMouse0.Tracker0" />
	</Scripts>
	<ClusterManager NVidiaSwapLock="0" DisableVSyncOnServer="1" ForceOpenGLConversion="0" BigBarrier="0" SimulateClusterLag="0" MultiGPUEnabled="0" ImageDistributionMaxPacketSize="8000" />
</MiddleVR>
