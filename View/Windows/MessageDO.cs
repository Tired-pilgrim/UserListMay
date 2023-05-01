using System;
using System.Windows;
using System.Windows.Media.Animation;
using VievModelLib;
using JointLib;

namespace Views.Windows
{
    public class MessageDO : DependencyObject
    {
        private MessageDO() { }
        public static readonly MessageDO Instance = new MessageDO();

        public double InfoOpacity
        {
            get { return (double)GetValue(InfoOpacityProperty); }
            set { SetValue(InfoOpacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Opacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InfoOpacityProperty =
            DependencyProperty.Register(nameof(InfoOpacity), typeof(double), typeof(MessageDO), new PropertyMetadata(0.0));

        public string InfoText
        {
            get { return (string)GetValue(InfoTextProperty); }
            set { SetValue(InfoTextProperty, value); }
        }
        // Using a DependencyProperty as the backing store for InfoText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InfoTextProperty =
            DependencyProperty.Register(nameof(InfoText), typeof(string), typeof(MessageDO), new PropertyMetadata(string.Empty)
            {
                CoerceValueCallback = (d, e) => e ?? string.Empty
            });

        public static void Register() => Register(DialogsService.Default);

        public static void Register(DialogsService service)
        {
            service.Register(new Action<Error>(ShowErrorDialog));
            service.Register(new Action<Info>(MessageShow));
        }
        private static void ShowErrorDialog(Error message)
        {
            MessageBox.Show(message.error, "Список служащих");
        }
        private static readonly Storyboard ShowAnimation;
        static MessageDO()
        {
            DoubleAnimationUsingKeyFrames doubleAnimation = new DoubleAnimationUsingKeyFrames();
            doubleAnimation.KeyFrames.Add(new DiscreteDoubleKeyFrame(0, KeyTime.FromPercent(0)));
            doubleAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(1, KeyTime.FromPercent(0.07)));
            doubleAnimation.KeyFrames.Add(new DiscreteDoubleKeyFrame(1, KeyTime.FromPercent(0.8)));
            doubleAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0, KeyTime.FromPercent(1)));
            doubleAnimation.Duration = TimeSpan.FromSeconds(5);

            Storyboard.SetTarget(doubleAnimation, Instance);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(InfoOpacityProperty));

            ShowAnimation = new Storyboard();
            ShowAnimation.Children.Add(doubleAnimation);
        }

        private static void MessageShow(Info message)
        {
            _ = Instance.Dispatcher.BeginInvoke(() =>
            {
                Instance.InfoText = message.info;
                ShowAnimation.Begin();
            });
        }
    }
}
