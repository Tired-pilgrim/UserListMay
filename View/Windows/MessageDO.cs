using JointLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using VievModelLib;

namespace Views.Windows
{
    public class MessageDO : DependencyObject
    {
        private MessageDO() { }
        public static readonly MessageDO Instance = new MessageDO();

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
                //Оператор ?? возвращает левый операнд, если этот операнд не равен null.
                //Иначе возвращается правый операнд. 
            });

        public static void Register() => Register(DialogsService.Default);

        public static void Register(DialogsService service)
        {
            service.Register(new Action<Error>(ShowErrorDialog));
            service.Register(new Action<Info>(MessageShow));
        }
        private static void ShowErrorDialog(Error message)
        {
            _ = Instance.Dispatcher.BeginInvoke(() => MessageBox.Show(message.error, "Список служащих"));
        }
        private static void MessageShow(Info message)
        {
            _ = Instance.Dispatcher.BeginInvoke(() =>
            {
                Instance.InfoText = message.info;
                elements.RemoveAll(elm => !elm.TryGetTarget(out _));

                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i].TryGetTarget(out UIElement? element))
                    {
                        InfoReceivedArgs args = new InfoReceivedArgs(InfoReceivedEvent, element, message);
                        element.RaiseEvent(args);
                        break;
                    }
                }
            });
        }

        // Register a custom routed event using the bubble routing strategy.
        public static readonly RoutedEvent InfoReceivedEvent = EventManager.RegisterRoutedEvent(
            nameof(InfoReceivedEvent)[0..^5], RoutingStrategy.Bubble, typeof(InfoReceivedHandler), typeof(MessageDO));

        // Provide an add handler accessor method for the Clean event.
        public static void AddInfoReceivedHandler(DependencyObject dependencyObject, RoutedEventHandler handler)
        {
            if (dependencyObject is not UIElement uiElement)
                return;

            uiElement.AddHandler(InfoReceivedEvent, handler);
        }

        // Provide a remove handler accessor method for the Clean event.
        public static void RemoveInfoReceivedHandler(DependencyObject dependencyObject, RoutedEventHandler handler)
        {
            if (dependencyObject is not UIElement uiElement)
                return;

            uiElement.RemoveHandler(InfoReceivedEvent, handler);
        }
        public static bool GetNeedRaiseInfoReceived(UIElement obj)
        {
            return (bool)obj.GetValue(NeedRaiseInfoReceivedProperty);
        }

        public static void SetNeedRaiseInfoReceived(UIElement obj, bool value)
        {
            obj.SetValue(NeedRaiseInfoReceivedProperty, value);
        }

        // Using a DependencyProperty as the backing store for NeedRaiseMessageReceived.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NeedRaiseInfoReceivedProperty =
            DependencyProperty.RegisterAttached(
                nameof(GetNeedRaiseInfoReceived)[3..],
                typeof(bool),
                typeof(MessageDO),
                new PropertyMetadata(false, OnNeedRaiseMessageReceivedChanged));

        private static void OnNeedRaiseMessageReceivedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not UIElement element)
                throw new NotImplementedException("Только для UIElement");
            //TryGetTarget Пытается извлечь целевой объект, на который ссылается текущий объект
            //Возвращает False, если целевой объект не был извлечен; 
            elements.RemoveAll(elm => !elm.TryGetTarget(out _));//см. предикат
            // Здесь, удаляет все элементы, которые не удалось извлечь - пустые ссылки
            /* Элд Хасп
             * Когда view:MessageDO.NeedRaiseInfoReceived="True" нужно обойти список объектов и проверить,
             * есть ли в нём этот объект. Перед циклом из списка удаляются все пустые ссылки.
             * В итерации цикла происходит попытка извлечения объекта 
             * и потом его сравнение с тем к которому нужно прикрепить.
             * Если такой попадётся, то цикл будет прерван и, следовательно, i не достигнет длины списка.
             * Поэтому после цикла по сравнению i с длиной списка можно определить найден был элемент или нет. 
             * Если не найден - значит надо его добавить в список.
             */
            if (Equals(e.NewValue, true))
            {
                int i = 0;
                for (; i < elements.Count; i++)
                {
                    if (elements[i].TryGetTarget(out UIElement? elm) && elm == element)
                        break;
                }
                if (i >= elements.Count)
                {
                    elements.Add(new(element));
                }
            }
            else
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i].TryGetTarget(out UIElement? elm) && elm == element)
                    {
                        elements.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// список объектов к которым прикреплено событие RouteedEvent 
        /// </summary>
        private static readonly List<WeakReference<UIElement>> elements = new List<WeakReference<UIElement>>();
    }


    public delegate void InfoReceivedHandler(object sender, InfoReceivedArgs e);
    //Насколько понимаю, в классе наследнике добавляется новый параметр - наш  тип данных
    public class InfoReceivedArgs : RoutedEventArgs
    {
        public Info Info { get; }
        public InfoReceivedArgs(RoutedEvent routedEvent, object source, Info info)
            : base(routedEvent, source)
        {
            Info = info;
        }
    }

}