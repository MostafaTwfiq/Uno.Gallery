﻿using Microsoft.Extensions.Logging;
using ShowMeTheXAML;
using System;
using System.Linq;
using System.Reflection;
using Uno.Gallery.Controls;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Uno.Gallery
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	sealed partial class App : Application
	{
		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			ConfigureFilters(global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory);
			ConfigureXamlDisplay();

			this.InitializeComponent();
			this.Suspending += OnSuspending;
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="e">Details about the launch request and process.</param>
		protected override void OnLaunched(LaunchActivatedEventArgs e)
		{
#if WINDOWS_UWP
			ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(320, 568)); // (size of the iPhone SE)
#endif

			var window = Windows.UI.Xaml.Window.Current;
			if (!(window.Content is Shell))
			{
				window.Content = BuildShell();
			}

			// Ensure the current window is active
			window.Activate();
		}

		/// <summary>
		/// Invoked when application execution is being suspended.  Application state is saved
		/// without knowing whether the application will be terminated or resumed with the contents
		/// of memory still intact.
		/// </summary>
		/// <param name="sender">The source of the suspend request.</param>
		/// <param name="e">Details about the suspend request.</param>
		private void OnSuspending(object sender, SuspendingEventArgs e)
		{
			var deferral = e.SuspendingOperation.GetDeferral();
			//TODO: Save application state and stop any background activity
			deferral.Complete();
		}

		private Shell BuildShell()
		{
			var shell = new Shell();
			var nv = shell.NavigationView;
			AddNavigationItems(nv);
			SetupSettingButton(nv);

			// navigation + setting handler
			nv.ItemInvoked += OnNavigationItemInvoked;

			return shell;
		}

		private void OnNavigationItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs e)
		{
			if (!e.IsSettingsInvoked)
			{
				if (e.InvokedItemContainer.DataContext is Sample sample)
				{
					var page = (Page)Activator.CreateInstance(sample.ViewType);
					page.DataContext = sample;

					sender.Content = page;
				}
			}
			else
			{
#if WINDOWS_UWP
				// Set theme for window root.
				if (Window.Current.Content is FrameworkElement frameworkElement)
				{
					if (frameworkElement.ActualTheme == ElementTheme.Dark)
					{
						frameworkElement.RequestedTheme = ElementTheme.Light;
					}
					else
					{
						frameworkElement.RequestedTheme = ElementTheme.Dark;
					}
				}
#endif
			}
		}

		private void AddNavigationItems(NavigationView nv)
		{
			var categories = Assembly.GetExecutingAssembly().DefinedTypes
				.Where(x => x.Namespace?.StartsWith("Uno.Gallery") == true)
				.Select(x => new { TypeInfo = x, SamplePageAttribute = x.GetCustomAttribute<SamplePageAttribute>() })
				.Where(x => x.SamplePageAttribute != null)
				.Select(x => new Sample(x.SamplePageAttribute, x.TypeInfo.AsType()))
				.OrderByDescending(x => x.SortOrder.HasValue)
				.ThenBy(x => x.SortOrder)
				.ThenBy(x => x.Title)
				.GroupBy(x => x.Category);

			foreach (var category in categories.OrderBy(x => x.Key))
			{
				if (category.Key != SampleCategory.None)
				{
					nv.MenuItems.Add(new NavigationViewItemHeader
					{
						Content = category.Key.GetDescription() ?? category.Key.ToString(),
					});
				}

				foreach (var sample in category)
				{
					nv.MenuItems.Add(new NavigationViewItem
					{
						Content = sample.Title,
						DataContext = sample
					});
				}
			}
		}

		private void SetupSettingButton(NavigationView nv)
		{
#if WINDOWS_UWP
			nv.IsSettingsVisible = true;
			nv.Loaded += OnNavigationViewLoaded;

			void OnNavigationViewLoaded(object sender, RoutedEventArgs e)
			{
				if (nv.SettingsItem is NavigationViewItem item)
				{
					item.Content = "Toggle Light/Dark theme";
					nv.Loaded -= OnNavigationViewLoaded;
				}
			}
#else
			nv.IsSettingsVisible = false;
#endif
		}

		/// <summary>
		/// Configures global logging
		/// </summary>
		/// <param name="factory"></param>
		static void ConfigureFilters(ILoggerFactory factory)
		{
			factory
				.WithFilter(new FilterLoggerSettings
					{
						{ "Uno", LogLevel.Warning },
						{ "Windows", LogLevel.Warning },

						// Debug JS interop
						// { "Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug },

						// Generic Xaml events
						// { "Windows.UI.Xaml", LogLevel.Debug },
						// { "Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug },
						// { "Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug },
						// { "Windows.UI.Xaml.UIElement", LogLevel.Debug },

						// Layouter specific messages
						// { "Windows.UI.Xaml.Controls", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.Panel", LogLevel.Debug },
						// { "Windows.Storage", LogLevel.Debug },

						// Binding related messages
						// { "Windows.UI.Xaml.Data", LogLevel.Debug },

						// DependencyObject memory references tracking
						// { "ReferenceHolder", LogLevel.Debug },

						// ListView-related messages
						// { "Windows.UI.Xaml.Controls.ListViewBase", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.ListView", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.GridView", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.VirtualizingPanelLayout", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.NativeListViewBase", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.ListViewBaseSource", LogLevel.Debug }, //iOS
						// { "Windows.UI.Xaml.Controls.ListViewBaseInternalContainer", LogLevel.Debug }, //iOS
						// { "Windows.UI.Xaml.Controls.NativeListViewBaseAdapter", LogLevel.Debug }, //Android
						// { "Windows.UI.Xaml.Controls.BufferViewCache", LogLevel.Debug }, //Android
						// { "Windows.UI.Xaml.Controls.VirtualizingPanelGenerator", LogLevel.Debug }, //WASM
					}
				)
#if DEBUG
				.AddConsole(LogLevel.Debug);
#else
				.AddConsole(LogLevel.Information);
#endif
		}

		static void ConfigureXamlDisplay()
		{
			XamlDisplay.Init();
		}
	}
}
