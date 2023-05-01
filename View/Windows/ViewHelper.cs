using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using VievModelLib;
using ViewLib;
using ViewModel;
using ViewModelLib.Commands;
using static Views.Windows.OpenSaveViewHelper;

namespace Views.Windows
{
    public class ViewHelper
    {
        public static RelayCommand AddUserCommand { get; } = new RelayCommand<FrameworkElement>(AddUserDialog);

        public static RelayCommand OpenListUserCommand { get; } = new RelayCommand<FrameworkElement>(OpenListUserDialogAsync);
        public static RelayCommand SaveListUserCommand { get; } =
            new RelayCommand<FrameworkElement>(SaveListUserDialogAsync, CanSaveListUserCommandExecute);

        private static bool CanSaveListUserCommandExecute(FrameworkElement element)
        {
            MainViewModel? vm = element.FindData<MainViewModel>();
            if (vm is not null && vm.Users is not null) return vm.Users.Count > 0;
            return false;
        }
        public static RoutedEventHandler AddUserDialogHandler { get; } = (s, _) => AddUserDialog((FrameworkElement)s);
        public static RoutedEventHandler OpenListUserHandler { get; } = (s, _) => OpenListUserDialogAsync((FrameworkElement)s);
        public static RoutedEventHandler SaveListUserHandler { get; } = (s, _) => SaveListUserDialogAsync((FrameworkElement)s);


        private static void AddUserDialog(FrameworkElement element)
        {
            Window currWin = Window.GetWindow(element);
            MainViewModel? vm = element.FindData<MainViewModel>();
            if (currWin is not null && vm is not null)
            {
                AddUzer _addUzer = new AddUzer()
                {
                    DataContext = vm.AddUserVM,
                    Owner = currWin,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                _addUzer.ShowDialog();
            }
        }

        private static async void OpenListUserDialogAsync(FrameworkElement element)
        {
            ISaveOpen? Ivm = element.FindData<ISaveOpen>();
            if (Ivm is not null)
                await Ivm.OpenListUserAsync(OpenDial());
        }
        private static async void SaveListUserDialogAsync(FrameworkElement element)
        {
            ISaveOpen? Ivm = element.FindData<ISaveOpen>();
            if (Ivm is not null)
                await Ivm.SaveListUserAsync(SaveDial());
        }
    }
    [MarkupExtensionReturnType(typeof(NameMembers))]
    public class ViewExtension : MarkupExtension
    {
        private static readonly Binding selfBinding = new() { RelativeSource = RelativeSource.Self };

        public enum NameMembers
        {
            Add,
            Open,
            Save
        }
        public NameMembers Name { get; set; }

        public ViewExtension() { }
        public ViewExtension(NameMembers name)
        {
            Name = name;
        }



        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget service = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget))!;
            FrameworkElement element = (FrameworkElement)service.TargetObject;
            if (element is ICommandSource cs)
            {
                var descriptor = DependencyPropertyDescriptor.FromName(
                    nameof(ICommandSource.CommandParameter),
                    element.GetType(),
                    element.GetType());
                element.SetBinding(descriptor.DependencyProperty, selfBinding);
            }
            if (service.TargetProperty is DependencyProperty dp)
            {
                if (dp.PropertyType.IsAssignableFrom(typeof(RelayCommand)))
                {
                    return Name switch
                    {
                        NameMembers.Open => ViewHelper.OpenListUserCommand,
                        NameMembers.Save => ViewHelper.SaveListUserCommand,
                        NameMembers.Add => ViewHelper.AddUserCommand,
                        _ => throw new NotImplementedException()
                    };
                }
            }

            else if (service.TargetProperty is EventInfo ei)
            {
                if (ei.EventHandlerType == typeof(RoutedEventHandler))
                {
                    return Name switch
                    {
                        NameMembers.Open => ViewHelper.OpenListUserHandler,
                        NameMembers.Save => ViewHelper.SaveListUserHandler,
                        NameMembers.Add => ViewHelper.AddUserDialogHandler,
                        _ => throw new NotImplementedException()
                    };
                }

            }
            throw new NotImplementedException();
        }
    }
}
