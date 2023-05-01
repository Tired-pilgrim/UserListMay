using System;
using System.Collections.Generic;

namespace JointLib
{
    /// <summary>Простейший диалоговый сервис, для регистрации и получении делегатов диалогов по их типу.</summary>
    public class DialogsService
    {
        private readonly Dictionary<Type, Delegate> delegats = new Dictionary<Type, Delegate>();

        /// <summary>Регистрация делегата.</summary>
        /// <typeparam name="T">Тип делегата.</typeparam>
        /// <param name="delegate">Делегат.</param>
        /// <remarks>Если до этого был зарегистрирован другой делегат такого же типа,
        /// то он будет замещён новым делегатом.</remarks>
        public void Register<T>(T @delegate) where T : Delegate
        {
            delegats[typeof(T)] = @delegate;
        }

        /// <summary>Отмена регистрации делегатов указанного типа.</summary>
        /// <typeparam name="T">Тип делегата для которого отменяется регистрация.</typeparam>
        /// <returns><see langword="true"/>, если делегат такого типа был зарегистрирован.</returns>
        public bool Unregister<T>() where T : Delegate
        {
            return delegats.Remove(typeof(T));
        }

        /// <summary>Получение зарегистрированного делегата.</summary>
        /// <typeparam name="T">Тип делегата.</typeparam>
        /// <returns>Возвращает зарегистрированный делегат или <see langword="null"/>.</returns>
        public T Get<T>() where T : Delegate
        {
            if (delegats.TryGetValue(typeof(T), out Delegate @delegate))
                return (T)@delegate;
            return null;
        }

        /// <summary>Экземпляр сервиса "по умолчанию".</summary>
        public static DialogsService Default { get; } = new DialogsService();
    }
}
