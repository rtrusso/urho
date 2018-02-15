﻿using System;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Urho.UWP
{
	public class UrhoSurface : SwapChainPanel
	{
		static bool paused;
		bool stop;
		bool inited;
		bool firstFrameRendered;
		TaskCompletionSource<bool> loadedTaskSource;
		Application activeApp;

		public UrhoSurface()
		{
			Opacity = 0;
			loadedTaskSource = new TaskCompletionSource<bool>();
			Loaded += (s, e) => loadedTaskSource.TrySetResult(true);
			Unloaded += UrhoSurface_Unloaded;
			SizeChanged += UrhoSurface_SizeChanged;
		}

		void UrhoSurface_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
		{
			if (!inited)
				return;

			Sdl.SendWindowEvent(SdlWindowEvent.SDL_WINDOWEVENT_RESIZED, DipsToPixels(this.ActualWidth), DipsToPixels(this.ActualHeight));
		}

		//see WINRT_DIPS_TO_PHYSICAL_PIXELS in SDL_winrtvideo_cpp.h:L90
		static int DipsToPixels(double dips)
		{
			var dpi = Windows.Graphics.Display.DisplayInformation.GetForCurrentView().LogicalDpi;
			return (int) (0.5f + (float)dips * dpi / 96.0f);
		}

		void UrhoSurface_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			if (Application.HasCurrent && Application.Current == activeApp)
				Application.StopCurrent().Wait();
		}

		public Task WaitLoadedAsync() => loadedTaskSource.Task;

		public TGame Run<TGame>(ApplicationOptions options = null) where TGame : Urho.Application
		{
			return (TGame) Run(typeof(TGame), options);
		}
		
		public Application Run(Type appType, ApplicationOptions options = null)
		{
			Opacity = 0;
			Application.StopCurrent().Wait();
			stop = false;
			paused = false;
			inited = false;
			firstFrameRendered = false;
			Sdl.InitUwp();
			var app = activeApp = Application.CreateInstance(appType, options);
			app.Run();
			inited = true;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			ThreadPool.RunAsync(async _ =>
				{
					await Task.Delay(16);//skip first frame
					while (stopRenderingTask == null)
					{
						if (!paused && !app.IsExiting)
						{
							app.Engine.RunFrame();
							if (!firstFrameRendered)
							{
								firstFrameRendered = true;
								Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Opacity = 1);
							}
						}
					}
					stopRenderingTask.TrySetResult(true);
					stopRenderingTask = null;
				});
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			return app;
		}

		internal static TaskCompletionSource<bool> stopRenderingTask;
		internal static Task StopRendering()
		{
			stopRenderingTask = new TaskCompletionSource<bool>();
			return stopRenderingTask.Task;
		}

		public static void Pause()
		{
			paused = true;
		}

		public static void Resume()
		{
			paused = false;
		}
	}
}
