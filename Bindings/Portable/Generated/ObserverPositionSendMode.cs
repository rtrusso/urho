// WARNING - AUTOGENERATED - DO NOT EDIT
// 
// Generated using `sharpie urho`
// 
// ObserverPositionSendMode.cs
// 
// Copyright 2015 Xamarin Inc. All rights reserved.

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Urho.Urho2D;
using Urho.Gui;
using Urho.Resources;
using Urho.IO;
using Urho.Navigation;
using Urho.Network;

namespace Urho.Network
{
	/// <summary>
	/// Send modes for observer position/rotation. Activated by the client setting either position or rotation.
	/// </summary>
	public enum ObserverPositionSendMode
	{
		None = 0,
		Position,
		PositionRotation
	}
}
