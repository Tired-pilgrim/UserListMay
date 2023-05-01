using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Data;

namespace Views.Windows
{
    /// <summary>Предоставляет прокси <see cref="DependencyObject"/> с одним свойством и 
    /// событием уведомляющем о его изменении.</summary>
    /// <typeparam name="TValue">Тип свойства <see cref="Value"/>.</typeparam>
    public class ValueProxy<TValue> : Freezable, INotifyPropertyChanged
    {
        // <summary>Свойство для задания внешних привязок.</summary>
        public TValue Value
        {
            get => (TValue)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        // Свойство для более быстрого доступа к значению ValueProperty из любого потока.
        protected TValue? ProtectedValue { get; private set; }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(TValue),
                typeof(ValueProxy<TValue>),
                new PropertyMetadata(null, (d, e) => ((ValueProxy<TValue>)d).ProtectedValue = (TValue)e.NewValue));
        /// <summary>Привязка по умолчанию - пустой экземпляр <see cref="Binding()"/>. 
        /// </summary>
        private static readonly Binding DefaultBinding = new();

        /// <summary>Создаёт экземпляр <see cref="ValueProxy{T}"/>
        /// со 100мс вызовом <see cref="OnTick(TValue)"/>.</summary>
       // <summary>Создаёт экземпляр <see cref="ValueProxy{T}"/>
        /// с указанным интервалом вызова <see cref="OnTick(TValue)"/>.</summary>
        /// <summary>Создаёт экземпляр <see cref="ValueProxy{T}"/>
        /// со 100мс вызовом <see cref="OnTick(TValue)"/>.</summary>
        public ValueProxy()
            : this(100)
        { }
        public ValueProxy(int tickInterval)
        {
            SetValueBinding(DefaultBinding);
            timer = new Timer(OnTick, this, tickInterval, tickInterval);
        }
        public void SetValueBinding(BindingBase binding)
        {
            if (binding != null)
                BindingOperations.SetBinding(this, ValueProperty, binding);
            else
                BindingOperations.ClearBinding(this, ValueProperty);
        }
        private static void OnTick(object? state)
        {
            ValueProxy<TValue> proxy = (ValueProxy<TValue>)(state ?? throw new ArgumentNullException(nameof(state)));
            proxy.OnTick(proxy.ProtectedValue);
        }
        /// <summary>Периодически вызываемый асинхронный метод, который можно использовать
        /// для проверки объектов и и их членов не имеющих уведомления об изменении.</summary>
        /// <param name="value">Значение свойства <see cref="Value"/>.</param>
        protected virtual void OnTick(TValue? value)
        { }

#pragma warning disable IDE0052 // Удалить непрочитанные закрытые члены
        private readonly Timer timer;
#pragma warning restore IDE0052 
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            ValueChanged?.Invoke(this, e);
        }

        /// <summary>Событие, возникающее при изменении значения любого <see cref="DependencyProperty"/>.</summary>
        public event EventHandler<DependencyPropertyChangedEventArgs>? ValueChanged;

        /// <summary>Возвращает <see langword="true"/>, если значение свойства <see cref="Value"/> не задано.</summary>
        public bool IsUnsetValue => Equals(ReadLocalValue(ValueProperty), DependencyProperty.UnsetValue);

        /// <summary>Очистка всех <see cref="DependencyProperty"/> этого <see cref="ProxyDO"/>.</summary>
        public void Reset()
        {
            LocalValueEnumerator locallySetProperties = GetLocalValueEnumerator();
            while (locallySetProperties.MoveNext())
            {
                DependencyProperty propertyToClear = locallySetProperties.Current.Property;
                if (!propertyToClear.ReadOnly)
                {
                    ClearValue(propertyToClear);
                }
            }
        }

        /// <summary><see langword="true"/> если свойству задана Привязка.</summary>
        public bool IsValueBinding => BindingOperations.GetBindingExpressionBase(this, ValueProperty) != null;

        /// <summary><see langword="true"/> если свойству задана привязка
        /// и она в состоянии <see cref="BindingStatus.Active"/>.</summary>
        public bool IsActiveValueBinding
        {
            get
            {
                BindingExpressionBase exp = BindingOperations.GetBindingExpressionBase(this, ValueProperty);
                if (exp == null)
                {
                    return false;
                }

                BindingStatus status = exp.Status;
                return status == BindingStatus.Active;
            }
        }


        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc cref="INotifyPropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>Защищённый метод для создания события <see cref="PropertyChanged"/> с расширенным аргументом <see cref="ExtendedPropertyChangedEventArgs"/>.</summary>
        /// <param name="propertyName">Имя изменившегося свойства. 
        /// Если значение не задано, то используется имя метода в котором был вызов.</param>
        /// <param name="oldValue">Старое (ло изменения) значение свойства.</param>
        /// <param name="newValue">Новое (текущее) значение свойства.</param>
        protected void RaisePropertyChanged(object? oldValue, object? newValue, [CallerMemberName] string? propertyName = null)
        {
            ExtendedPropertyChangedEventArgs args = new(oldValue, newValue, propertyName!);
            RaisePropertyChanged(args);
        }

        /// <summary>Защищённый метод для создания события <see cref="PropertyChanged"/>.</summary>
        /// <param name="propertyName">Имя изменившегося свойства. 
        /// Если значение не задано, то используется имя метода в котором был вызов.</param>
        protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChangedEventArgs args = string.IsNullOrEmpty(propertyName) ? allProperties : new(propertyName);
            RaisePropertyChanged(args);
        }
        /// <summary>Защищённый метод для создания события <see cref="PropertyChanged"/>.</summary>
        /// <param name="args">Аргумент события <see cref="PropertyChanged"/>.</param>
        protected void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            if (args == null)
            {
                PropertyChanged?.Invoke(this, allProperties);
            }
            else
            {
                PropertyChanged?.Invoke(this, args);
                if (args is ExtendedPropertyChangedEventArgs extArgs)
                {
                    OnPropertyChanged(extArgs);
                }
            }
        }
        private static readonly PropertyChangedEventArgs allProperties = new(string.Empty);
        /// <summary>Защищённый виртуальный метод, который переопреляется в производных
        /// классах для обработки изменения свойства.</summary>
        /// <param name="args">Аргументы изменения свойства.</param>
        protected virtual void OnPropertyChanged(ExtendedPropertyChangedEventArgs args)
        { }


        /// <summary>Защищённый метод для присвоения значения полю и
        /// создания события <see cref="PropertyChanged"/>.</summary>
        /// <typeparam name="TProperty">Тип поля и присваиваемого значения.</typeparam>
        /// <param name="propertyField">Ссылка на поле.</param>
        /// <param name="newValue">Присваиваемое значение.</param>
        /// <param name="propertyName">Имя изменившегося свойства. 
        /// Если значение не задано, то используется имя метода в котором был вызов.</param>
        /// <returns>Возвращает <see langword="true"/>, если значение изменилось и
        /// было поднято событие <see cref="PropertyChanged"/>.</returns>
        /// <remarks>Метод предназначен для использования в сеттере свойства.<br/>
        /// Сравнение нового значения со значением поля производится методом <see cref="object.Equals(object, object)"/>.<br/>
        /// Если присваиваемое значение не эквивалентно значению поля, то оно присваивается полю.<br/>
        /// После присвоения создаётся событие <see cref="PropertyChanged"/> вызовом
        /// метода <see cref="RaisePropertyChanged(string)"/>
        /// с передачей ему параметра <paramref name="propertyName"/>.<br/>
        /// После создания события вызывается метод <see cref="OnPropertyChanged(in string, in object, in object)"/>.</remarks>
        protected bool Set<TProperty>(ref TProperty propertyField, TProperty newValue, [CallerMemberName] string? propertyName = null)
        {
            bool isEquals = Equals(propertyField, newValue);
            if (!isEquals)
            {
                TProperty oldValue = propertyField;
                propertyField = newValue;
                RaisePropertyChanged(oldValue, newValue, propertyName);
            }

            return !isEquals;
        }
        protected class ExtendedPropertyChangedEventArgs : PropertyChangedEventArgs
        {
            public object? OldValue { get; }
            public object? NewValue { get; }

            public ExtendedPropertyChangedEventArgs(object? oldValue, object? newValue, string propertyName)
                : base(!string.IsNullOrWhiteSpace(propertyName) ? propertyName : throw new ArgumentNullException(nameof(propertyName)))
            {
                OldValue = oldValue;
                NewValue = newValue;
            }
        }
        
    }
    /// <inheritdoc cref="ValueProxy{T}"/>
    public class ValueProxy : ValueProxy<object>
    {

    }
}
